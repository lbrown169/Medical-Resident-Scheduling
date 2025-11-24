import React from 'react'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import Home from '../page'

// Mock the auth module
jest.mock('../../lib/auth', () => ({
  setAuthToken: jest.fn(),
}))

// Mock the toast hook and toaster component
jest.mock('../../lib/use-toast', () => ({
  useToast: () => ({
    toast: jest.fn(),
    toasts: [],
  }),
}))

jest.mock('../../components/ui/toaster', () => ({
  Toaster: () => <div data-testid="toaster" />,
}))

// Mock the config
jest.mock('../../config', () => ({
  config: {
    apiUrl: 'http://localhost:7000',
  },
}))

// Mock the AuthContext
jest.mock('../../context/AuthContext', () => ({
  useAuth: () => ({
    setUser: jest.fn(),
  }),
}))

describe('Login Component', () => {
  beforeEach(() => {
    // Clear all mocks before each test
    jest.clearAllMocks()
    localStorage.clear()
  })

  describe('Scenario A: User sees the form', () => {
    it('renders the login form with email and password inputs', () => {
      render(<Home />)

      // Check for the presence of email input
      const emailInput = screen.getByLabelText(/email/i)
      expect(emailInput).toBeInTheDocument()
      expect(emailInput).toHaveAttribute('type', 'email')
      expect(emailInput).toHaveAttribute('name', 'email')
      expect(emailInput).toHaveAttribute('required')

      // Check for the presence of password input
      const passwordInput = screen.getByPlaceholderText(/password/i)
      expect(passwordInput).toBeInTheDocument()
      expect(passwordInput).toHaveAttribute('type', 'password')
      expect(passwordInput).toHaveAttribute('name', 'password')
      expect(passwordInput).toHaveAttribute('required')
    })

    it('renders the login button', () => {
      render(<Home />)

      const loginButton = screen.getByRole('button', { name: /login/i })
      expect(loginButton).toBeInTheDocument()
      expect(loginButton).toHaveAttribute('type', 'submit')
    })

    it('renders the header with PSYCALL title', () => {
      render(<Home />)

      const title = screen.getByRole('heading', { name: /psycall/i })
      expect(title).toBeInTheDocument()
    })

    it('renders the show/hide password toggle button', () => {
      render(<Home />)

      const toggleButton = screen.getByLabelText(/show password/i)
      expect(toggleButton).toBeInTheDocument()
    })
  })

  describe('Scenario B: User types credentials and submits', () => {
    it('updates form fields when user types', async () => {
      const user = userEvent.setup()
      render(<Home />)

      const emailInput = screen.getByLabelText(/email/i)
      const passwordInput = screen.getByPlaceholderText(/password/i)

      // Type email
      await user.type(emailInput, 'test@example.com')
      expect(emailInput).toHaveValue('test@example.com')

      // Type password
      await user.type(passwordInput, 'password123')
      expect(passwordInput).toHaveValue('password123')
    })

    it('shows loading state and calls fetch with correct credentials on submit', async () => {
      const user = userEvent.setup()
      const mockFetch = global.fetch as jest.MockedFunction<typeof fetch>

      // Mock successful login response
      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: async () => ({
          token: 'mock-token-123',
          userType: 'resident',
          resident: {
            id: '1',
            name: 'Test User',
            email: 'test@example.com',
          },
        }),
      } as Response)

      render(<Home />)

      const emailInput = screen.getByLabelText(/email/i)
      const passwordInput = screen.getByPlaceholderText(/password/i)
      const loginButton = screen.getByRole('button', { name: /login/i })

      // Fill in the form
      await user.type(emailInput, 'test@example.com')
      await user.type(passwordInput, 'password123')

      // Submit the form
      await user.click(loginButton)

      // Verify loading state appears
      await waitFor(() => {
        expect(screen.getByText(/logging in\.\.\./i)).toBeInTheDocument()
      })

      // Verify fetch was called with correct parameters
      expect(mockFetch).toHaveBeenCalledWith(
        'http://localhost:7000/api/auth/login',
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            email: 'test@example.com',
            password: 'password123',
          }),
        }
      )
    })

    it('disables form inputs during loading state', async () => {
      const user = userEvent.setup()
      const mockFetch = global.fetch as jest.MockedFunction<typeof fetch>

      // Mock a delayed response
      mockFetch.mockImplementation(
        () =>
          new Promise((resolve) =>
            setTimeout(
              () =>
                resolve({
                  ok: true,
                  json: async () => ({
                    token: 'mock-token',
                    userType: 'resident',
                    resident: { id: '1', name: 'Test', email: 'test@example.com' },
                  }),
                } as Response),
              100
            )
          )
      )

      render(<Home />)

      const emailInput = screen.getByLabelText(/email/i)
      const passwordInput = screen.getByPlaceholderText(/password/i)
      const loginButton = screen.getByRole('button', { name: /login/i })

      await user.type(emailInput, 'test@example.com')
      await user.type(passwordInput, 'password123')
      await user.click(loginButton)

      // Check inputs are disabled during loading
      await waitFor(() => {
        expect(emailInput).toBeDisabled()
        expect(passwordInput).toBeDisabled()
        expect(loginButton).toBeDisabled()
      })
    })

    it('stores user data in localStorage on successful login', async () => {
      const user = userEvent.setup()
      const mockFetch = global.fetch as jest.MockedFunction<typeof fetch>
      const setItemSpy = jest.spyOn(Storage.prototype, 'setItem')

      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: async () => ({
          token: 'mock-token-123',
          userType: 'resident',
          resident: {
            id: '1',
            name: 'Test User',
            email: 'test@example.com',
          },
        }),
      } as Response)

      render(<Home />)

      const emailInput = screen.getByLabelText(/email/i)
      const passwordInput = screen.getByPlaceholderText(/password/i)
      const loginButton = screen.getByRole('button', { name: /login/i })

      await user.type(emailInput, 'test@example.com')
      await user.type(passwordInput, 'password123')
      await user.click(loginButton)

      await waitFor(() => {
        expect(setItemSpy).toHaveBeenCalledWith(
          'user',
          expect.stringContaining('Test User')
        )
      })

      setItemSpy.mockRestore()
    })

    it('handles login failure with error message', async () => {
      const user = userEvent.setup()
      const mockFetch = global.fetch as jest.MockedFunction<typeof fetch>

      mockFetch.mockResolvedValueOnce({
        ok: false,
        json: async () => ({
          message: 'Invalid username or password',
        }),
      } as Response)

      render(<Home />)

      const emailInput = screen.getByLabelText(/email/i)
      const passwordInput = screen.getByPlaceholderText(/password/i)
      const loginButton = screen.getByRole('button', { name: /login/i })

      await user.type(emailInput, 'wrong@example.com')
      await user.type(passwordInput, 'wrongpassword')
      await user.click(loginButton)

      // Wait for loading to finish
      await waitFor(() => {
        expect(screen.queryByText(/logging in\.\.\./i)).not.toBeInTheDocument()
      })

      // Verify fetch was called
      expect(mockFetch).toHaveBeenCalled()
    })

    it('handles network errors gracefully', async () => {
      const user = userEvent.setup()
      const mockFetch = global.fetch as jest.MockedFunction<typeof fetch>

      mockFetch.mockRejectedValueOnce(new TypeError('Failed to fetch'))

      render(<Home />)

      const emailInput = screen.getByLabelText(/email/i)
      const passwordInput = screen.getByPlaceholderText(/password/i)
      const loginButton = screen.getByRole('button', { name: /login/i })

      await user.type(emailInput, 'test@example.com')
      await user.type(passwordInput, 'password123')
      await user.click(loginButton)

      // Wait for error handling
      await waitFor(() => {
        expect(screen.queryByText(/logging in\.\.\./i)).not.toBeInTheDocument()
      })
    })

    it('toggles password visibility when show/hide button is clicked', async () => {
      const user = userEvent.setup()
      render(<Home />)

      const passwordInput = screen.getByPlaceholderText(/password/i)
      const toggleButton = screen.getByLabelText(/show password/i)

      // Initially password should be hidden
      expect(passwordInput).toHaveAttribute('type', 'password')

      // Click to show password
      await user.click(toggleButton)
      expect(passwordInput).toHaveAttribute('type', 'text')

      // Click to hide password again
      await user.click(toggleButton)
      expect(passwordInput).toHaveAttribute('type', 'password')
    })
  })

  describe('Form Validation', () => {
    it('requires email field to be filled', async () => {
      render(<Home />)

      const emailInput = screen.getByLabelText(/email/i)
      expect(emailInput).toBeRequired()
    })

    it('requires password field to be filled', async () => {
      render(<Home />)

      const passwordInput = screen.getByPlaceholderText(/password/i)
      expect(passwordInput).toBeRequired()
    })

    it('accepts valid email format', async () => {
      const user = userEvent.setup()
      render(<Home />)

      const emailInput = screen.getByLabelText(/email/i)

      await user.type(emailInput, 'valid@example.com')
      expect(emailInput).toHaveValue('valid@example.com')
    })
  })
})
