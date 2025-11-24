# Frontend Testing Guide

## Running Tests

### Available Commands

```bash
# Run all tests once
npm test

# Run tests in watch mode (re-runs on file changes)
npm run test:watch

# Run tests with coverage report
npm run test:coverage
```

## Test Files

### 1. Login Component Tests
**Location:** `src/app/__tests__/Login.test.tsx`

Tests the main login page functionality:

#### Scenario A: User Sees the Form
- Email input exists and is required
- Password input exists and is required
- Login button is present
- PSYCALL header is displayed
- Password toggle button is present

#### Scenario B: User Types Credentials and Submits
- Form fields update when user types
- Loading state appears on submit
- Fetch is called with correct credentials
- Form inputs are disabled during loading
- User data is stored in localStorage on success
- Error messages are displayed on failure
- Network errors are handled gracefully
- Password visibility can be toggled

### 2. Swap Request Component Tests
**Location:** `src/app/dashboard/components/__tests__/SwapRequest.test.tsx`

Tests the call swap request functionality:

#### Component Rendering
- All form fields are present
- Submit button is disabled when form is incomplete
- Resident dropdown shows all residents
- Shift type dropdown shows all shifts

#### User Interaction
- Submit button does not trigger when disabled
- Confirmation dialog appears when form is complete
- `handleSubmitSwap` is called on confirmation
- `handleSubmitSwap` is NOT called on cancel
- Form field state updates trigger correct callbacks

#### Swap Summary
- Summary shows when form is complete
- Summary displays correct resident name
- Summary displays correct dates and shift types

## Configuration Files

### jest.config.js
Configures Jest for Next.js environment:
- Uses `next/jest` for automatic Next.js configuration
- Sets up module name mapping for CSS and image imports
- Configures test environment as `jsdom`
- Excludes node_modules and .next from testing

### jest.setup.js
Global test setup:
- Imports `@testing-library/jest-dom` for custom matchers
- Mocks Next.js router (`useRouter`, `usePathname`, `useSearchParams`)
- Mocks Next.js Image component
- Sets up global `fetch` mock
- Mocks `window.matchMedia` for responsive tests
- Mocks `localStorage`

## Writing New Tests

### Basic Test Structure

```typescript
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import MyComponent from '../MyComponent'

describe('MyComponent', () => {
  beforeEach(() => {
    jest.clearAllMocks()
  })

  it('renders correctly', () => {
    render(<MyComponent />)
    expect(screen.getByText(/my text/i)).toBeInTheDocument()
  })

  it('handles user interaction', async () => {
    const user = userEvent.setup()
    render(<MyComponent />)

    const button = screen.getByRole('button', { name: /click me/i })
    await user.click(button)

    await waitFor(() => {
      expect(screen.getByText(/success/i)).toBeInTheDocument()
    })
  })
})
```

## Debugging Tests

### 1. View Rendered Output
```typescript
import { screen } from '@testing-library/react'

// Print current DOM
screen.debug()

// Print specific element
screen.debug(screen.getByRole('button'))
```

### 2. Find Available Queries
```typescript
// See all available queries for debugging
screen.logTestingPlaygroundURL()
```

### 3. Run Single Test
```bash
# Run only tests matching pattern
npm test -- Login.test.tsx

# Run only one test
npm test -- -t "renders the login form"
```

### 4. Update Snapshots
```bash
npm test -- -u
```

## Coverage Reports

Run tests with coverage:
```bash
npm run test:coverage
```

Coverage report will be generated in `/coverage` directory.

View HTML report:
```bash
open coverage/lcov-report/index.html
```