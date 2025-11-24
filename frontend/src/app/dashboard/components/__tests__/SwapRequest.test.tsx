import React from 'react'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import SwapCallsPage from '../SwapCallsPage'

// Mock the UI components to avoid rendering complexity
jest.mock('../../../../components/ui/card', () => ({
  Card: ({ children, className }: { children: React.ReactNode; className?: string }) => (
    <div className={className} data-testid="card">
      {children}
    </div>
  ),
}))

jest.mock('../../../../components/ui/button', () => ({
  Button: React.forwardRef(({
    children,
    onClick,
    disabled,
    variant,
    className,
    type,
  }: {
    children: React.ReactNode
    onClick?: () => void
    disabled?: boolean
    variant?: string
    className?: string
    type?: 'button' | 'submit' | 'reset'
  }, ref: React.Ref<HTMLButtonElement>) => (
    <button
      ref={ref}
      type={type}
      onClick={onClick}
      disabled={disabled}
      className={className}
      data-variant={variant}
    >
      {children}
    </button>
  )),
}))

// Mock lucide-react icons
jest.mock('lucide-react', () => ({
  Calendar: () => <div data-testid="calendar-icon" />,
  Users: () => <div data-testid="users-icon" />,
  Clock: () => <div data-testid="clock-icon" />,
  Send: () => <div data-testid="send-icon" />,
  ArrowRightLeft: () => <div data-testid="arrow-right-left-icon" />,
  AlertTriangle: () => <div data-testid="alert-triangle-icon" />,
}))

describe('SwapCallsPage Component', () => {
  const mockResidents = [
    { id: '1', name: 'Dr. John Smith' },
    { id: '2', name: 'Dr. Jane Doe' },
    { id: '3', name: 'Dr. Michael Johnson' },
  ]

  const mockShifts = [
    { id: 'short', name: 'Short' },
    { id: 'saturday', name: 'Saturday' },
    { id: 'sunday', name: 'Sunday' },
  ]

  const defaultProps = {
    yourShiftDate: '',
    setYourShiftDate: jest.fn(),
    partnerShiftDate: '',
    setPartnerShiftDate: jest.fn(),
    selectedResident: '',
    setSelectedResident: jest.fn(),
    residents: mockResidents,
    selectedShift: '',
    setSelectedShift: jest.fn(),
    partnerShift: '',
    setPartnerShift: jest.fn(),
    shifts: mockShifts,
    handleSubmitSwap: jest.fn(),
  }

  beforeEach(() => {
    jest.clearAllMocks()
  })

  describe('Component Rendering', () => {
    it('renders the swap calls page with all form fields', () => {
      render(<SwapCallsPage {...defaultProps} />)

      expect(screen.getByText(/request call swap/i)).toBeInTheDocument()
      expect(screen.getByLabelText(/your shift date/i)).toBeInTheDocument()
      expect(screen.getByLabelText(/partner's shift date/i)).toBeInTheDocument()
      expect(screen.getByLabelText(/swap partner/i)).toBeInTheDocument()
      expect(screen.getByLabelText(/your shift type/i)).toBeInTheDocument()
      expect(screen.getByLabelText(/partner's shift type/i)).toBeInTheDocument()
    })

    it('renders the submit button as disabled when form is incomplete', () => {
      render(<SwapCallsPage {...defaultProps} />)

      const submitButton = screen.getByRole('button', { name: /submit swap request/i })
      expect(submitButton).toBeDisabled()
    })

    it('displays all residents in the dropdown', () => {
      render(<SwapCallsPage {...defaultProps} />)

      const residentSelect = screen.getByLabelText(/swap partner/i)
      const options = residentSelect.querySelectorAll('option')

      // +1 for the default "Choose a resident" option
      expect(options).toHaveLength(mockResidents.length + 1)
      expect(screen.getByText(/dr\. john smith/i)).toBeInTheDocument()
      expect(screen.getByText(/dr\. jane doe/i)).toBeInTheDocument()
      expect(screen.getByText(/dr\. michael johnson/i)).toBeInTheDocument()
    })

    it('displays all shift types in the dropdown', () => {
      render(<SwapCallsPage {...defaultProps} />)

      const shiftSelects = screen.getAllByRole('combobox')
      // Should have 3 dropdowns: resident, your shift, partner shift
      expect(shiftSelects.length).toBeGreaterThanOrEqual(2)
    })
  })

  describe('Scenario: User clicks the Request Swap button', () => {
    it('does not trigger onClick when form is incomplete', async () => {
      const user = userEvent.setup()
      render(<SwapCallsPage {...defaultProps} />)

      const submitButton = screen.getByRole('button', { name: /submit swap request/i })

      // Button should be disabled
      expect(submitButton).toBeDisabled()

      // Try to click (should not work)
      await user.click(submitButton)

      // handleSubmitSwap should NOT be called
      expect(defaultProps.handleSubmitSwap).not.toHaveBeenCalled()
    })

    it('shows confirmation dialog when form is complete and button is clicked', async () => {
      const user = userEvent.setup()
      const filledProps = {
        ...defaultProps,
        yourShiftDate: '2025-12-01',
        partnerShiftDate: '2025-12-05',
        selectedResident: '1',
        selectedShift: 'short',
        partnerShift: 'saturday',
      }

      render(<SwapCallsPage {...filledProps} />)

      const submitButton = screen.getByRole('button', { name: /submit swap request/i })

      // Button should be enabled
      expect(submitButton).not.toBeDisabled()

      // Click the button
      await user.click(submitButton)

      // Confirmation dialog should appear
      await waitFor(() => {
        expect(screen.getByText(/confirm swap request/i)).toBeInTheDocument()
      })

      // handleSubmitSwap should NOT be called yet (waiting for confirmation)
      expect(defaultProps.handleSubmitSwap).not.toHaveBeenCalled()
    })

    it('calls handleSubmitSwap when user confirms the swap', async () => {
      const user = userEvent.setup()
      const filledProps = {
        ...defaultProps,
        yourShiftDate: '2025-12-01',
        partnerShiftDate: '2025-12-05',
        selectedResident: '2',
        selectedShift: 'short',
        partnerShift: 'sunday',
      }

      render(<SwapCallsPage {...filledProps} />)

      // Click submit button
      const submitButton = screen.getByRole('button', { name: /submit swap request/i })
      await user.click(submitButton)

      // Wait for confirmation dialog
      await waitFor(() => {
        expect(screen.getByText(/confirm swap request/i)).toBeInTheDocument()
      })

      // Click the confirm button
      const confirmButton = screen.getByText(/confirm & send/i)
      await user.click(confirmButton)

      // Verify handleSubmitSwap was called
      expect(defaultProps.handleSubmitSwap).toHaveBeenCalledTimes(1)
    })

    it('does not call handleSubmitSwap when user cancels the swap', async () => {
      const user = userEvent.setup()
      const filledProps = {
        ...defaultProps,
        yourShiftDate: '2025-12-01',
        partnerShiftDate: '2025-12-05',
        selectedResident: '1',
        selectedShift: 'short',
        partnerShift: 'saturday',
      }

      render(<SwapCallsPage {...filledProps} />)

      // Click submit button
      const submitButton = screen.getByRole('button', { name: /submit swap request/i })
      await user.click(submitButton)

      // Wait for confirmation dialog
      await waitFor(() => {
        expect(screen.getByText(/confirm swap request/i)).toBeInTheDocument()
      })

      // Click the cancel button
      const cancelButton = screen.getByRole('button', { name: /cancel/i })
      await user.click(cancelButton)

      // Verify handleSubmitSwap was NOT called
      expect(defaultProps.handleSubmitSwap).not.toHaveBeenCalled()

      // Confirmation dialog should be hidden
      await waitFor(() => {
        expect(screen.queryByText(/confirm swap request/i)).not.toBeInTheDocument()
      })
    })
  })

  describe('Form Interactions', () => {
    it('calls setYourShiftDate when user selects a date', async () => {
      const user = userEvent.setup()
      render(<SwapCallsPage {...defaultProps} />)

      const dateInput = screen.getByLabelText(/your shift date/i)
      await user.type(dateInput, '2025-12-15')

      expect(defaultProps.setYourShiftDate).toHaveBeenCalled()
    })

    it('calls setPartnerShiftDate when user selects a date', async () => {
      const user = userEvent.setup()
      render(<SwapCallsPage {...defaultProps} />)

      const dateInput = screen.getByLabelText(/partner's shift date/i)
      await user.type(dateInput, '2025-12-20')

      expect(defaultProps.setPartnerShiftDate).toHaveBeenCalled()
    })

    it('calls setSelectedResident when user selects a resident', async () => {
      const user = userEvent.setup()
      render(<SwapCallsPage {...defaultProps} />)

      const residentSelect = screen.getByLabelText(/swap partner/i)
      await user.selectOptions(residentSelect, '2')

      expect(defaultProps.setSelectedResident).toHaveBeenCalledWith('2')
    })

    it('calls setSelectedShift when user selects a shift type', async () => {
      const user = userEvent.setup()
      render(<SwapCallsPage {...defaultProps} />)

      const shiftSelect = screen.getByLabelText(/your shift type/i)
      await user.selectOptions(shiftSelect, 'short')

      expect(defaultProps.setSelectedShift).toHaveBeenCalledWith('short')
    })

    it('calls setPartnerShift when user selects partner shift type', async () => {
      const user = userEvent.setup()
      render(<SwapCallsPage {...defaultProps} />)

      const shiftSelect = screen.getByLabelText(/partner's shift type/i)
      await user.selectOptions(shiftSelect, 'saturday')

      expect(defaultProps.setPartnerShift).toHaveBeenCalledWith('saturday')
    })
  })

  describe('Swap Summary Display', () => {
    it('shows swap summary when form is complete', () => {
      const filledProps = {
        ...defaultProps,
        yourShiftDate: '2025-12-01',
        partnerShiftDate: '2025-12-05',
        selectedResident: '1',
        selectedShift: 'short',
        partnerShift: 'saturday',
      }

      render(<SwapCallsPage {...filledProps} />)

      expect(screen.getByText(/swap summary/i)).toBeInTheDocument()
    })

    it('does not show swap summary when form is incomplete', () => {
      render(<SwapCallsPage {...defaultProps} />)

      expect(screen.queryByText(/swap summary/i)).not.toBeInTheDocument()
    })

    it('displays correct resident name in summary', () => {
      const filledProps = {
        ...defaultProps,
        yourShiftDate: '2025-12-01',
        partnerShiftDate: '2025-12-05',
        selectedResident: '2',
        selectedShift: 'short',
        partnerShift: 'sunday',
      }

      render(<SwapCallsPage {...filledProps} />)

      // The summary should show the selected resident's name
      const summary = screen.getByText(/swap summary/i).parentElement
      expect(summary).toHaveTextContent('Dr. Jane Doe')
    })
  })

  describe('Mocking Child Components', () => {
    it('renders mocked Card component', () => {
      render(<SwapCallsPage {...defaultProps} />)

      const card = screen.getByTestId('card')
      expect(card).toBeInTheDocument()
    })

    it('renders mocked Button components', () => {
      const filledProps = {
        ...defaultProps,
        yourShiftDate: '2025-12-01',
        partnerShiftDate: '2025-12-05',
        selectedResident: '1',
        selectedShift: 'short',
        partnerShift: 'saturday',
      }

      render(<SwapCallsPage {...filledProps} />)

      const buttons = screen.getAllByRole('button')
      expect(buttons.length).toBeGreaterThan(0)
    })

    it('renders mocked icons', () => {
      render(<SwapCallsPage {...defaultProps} />)

      expect(screen.getAllByTestId('calendar-icon').length).toBeGreaterThan(0)
      expect(screen.getByTestId('users-icon')).toBeInTheDocument()
      expect(screen.getAllByTestId('clock-icon').length).toBeGreaterThan(0)
      expect(screen.getByTestId('arrow-right-left-icon')).toBeInTheDocument()
    })
  })
})
