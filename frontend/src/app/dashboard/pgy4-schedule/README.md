# PGY-4 Rotation Schedule Feature

## Overview

This feature allows PGY-4 residents to view their published monthly rotation schedule for the academic year (July - June). It displays both the individual resident's schedule and the complete team schedule in a calendar format.

## 📁 File Structure

```
frontend/
├── src/
│   ├── app/
│   │   └── dashboard/
│   │       ├── page.tsx                    # Updated: Added PGY-4 Schedule menu item
│   │       └── pgy4-schedule/
│   │           ├── page.tsx                # Main schedule page component
│   │           ├── print.css               # Print-optimized styles
│   │           └── README.md               # This file
│   ├── components/
│   │   └── pgy4/
│   │       ├── RotationCalendar.tsx        # Year-long calendar grid component
│   │       ├── RotationBadge.tsx           # Color-coded rotation badge
│   │       └── RotationLegend.tsx          # Legend showing rotation types
│   └── lib/
│       └── api/
│           └── pgy4-schedules.ts           # API service + TypeScript interfaces
```

## 🔧 Components

### 1. **PGY4SchedulePage** (`page.tsx`)
Main page component that orchestrates the entire schedule view.

**Features:**
- Fetches personal and team schedules on mount
- Displays loading states with spinner
- Shows error states with retry option
- Renders empty state when no schedule is published
- Provides print functionality
- Handles access control (PGY-4 only)

**State:**
- `mySchedule`: Current user's rotation assignments
- `teamSchedule`: All PGY-4 residents' assignments
- `loadingState`: Loading status tracking
- `error`: Error messages

### 2. **RotationCalendar** (`RotationCalendar.tsx`)
Displays a grid of rotation assignments organized by resident and month.

**Props:**
```typescript
{
  assignments: RotationAssignment[];  // Rotation data to display
  mode: 'team' | 'individual';       // Display mode
  title?: string;                     // Optional calendar title
  className?: string;                 // Additional CSS classes
  showLegend?: boolean;               // Show legend below calendar
}
```

**Modes:**
- `team`: Shows all residents (rows) × 12 months (columns)
- `individual`: Shows single resident with larger badges

### 3. **RotationBadge** (`RotationBadge.tsx`)
Renders a color-coded badge for a rotation type.

**Props:**
```typescript
{
  rotationType: string;              // Rotation ID (e.g., "intp-psy")
  size?: 'sm' | 'md' | 'lg';        // Badge size
  className?: string;                // Additional CSS classes
  showFullName?: boolean;            // Show full description vs abbreviation
}
```

**Color Mapping:**
- Intp Psy: Purple (#a855f7)
- Consult: Orange (#fb923c)
- VA: Blue (#60a5fa)
- TMS: Lime (#84cc16)
- And 7 more rotation types...

### 4. **RotationLegend** (`RotationLegend.tsx`)
Displays a legend showing all rotation types with their colors.

**Props:**
```typescript
{
  title?: string;                    // Legend title
  layout?: 'horizontal' | 'vertical'; // Layout orientation
  rotationIds?: string[];            // Filter to specific rotations
  showDescriptions?: boolean;         // Show full rotation descriptions
  className?: string;                 // Additional CSS classes
}
```

## 🌐 API Integration

### Required Backend Endpoints

The backend team must implement these endpoints:

#### 1. **GET /api/pgy4-schedules/published**
Retrieves the published rotation schedule for all PGY-4 residents.

**Authentication:** Required (JWT token)

**Query Parameters:**
- `academicYear` (optional): Filter by academic year (e.g., "2025-2026")

**Response Format:**
```json
{
  "scheduleId": "uuid-string",
  "academicYear": "2025-2026",
  "status": "published",
  "publishedDate": "2026-01-15T10:30:00Z",
  "assignments": [
    {
      "id": "assignment-uuid",
      "residentId": "resident-uuid",
      "firstName": "John",
      "lastName": "Doe",
      "month": "JUL",
      "rotationType": "intp-psy",
      "academicYear": "2025-2026"
    },
    // ... 12 assignments per resident
  ]
}
```

**Error Responses:**
- `404`: No published schedule found
- `401`: Unauthorized (invalid/missing token)
- `403`: Forbidden (user not authorized)

#### 2. **GET /api/pgy4-schedules/my-schedule**
Retrieves the current user's rotation schedule.

**Authentication:** Required (JWT token - user ID extracted from token)

**Query Parameters:**
- `academicYear` (optional): Filter by academic year

**Response Format:**
```json
{
  "residentId": "resident-uuid",
  "firstName": "John",
  "lastName": "Doe",
  "academicYear": "2025-2026",
  "rotations": [
    {
      "id": "assignment-uuid",
      "residentId": "resident-uuid",
      "firstName": "John",
      "lastName": "Doe",
      "month": "JUL",
      "rotationType": "intp-psy",
      "academicYear": "2025-2026"
    },
    // ... 12 total rotations (one per month)
  ]
}
```

**Error Responses:**
- `404`: No schedule found for this resident
- `401`: Unauthorized
- `403`: User is not a PGY-4 resident

### Rotation Types

The `rotationType` field must match one of these IDs:

| ID | Display Name | Description |
|----|--------------|-------------|
| `intp-psy` | Intp Psy | Inpatient Psychiatry |
| `consult` | Consult | Consultation-Liaison Psychiatry |
| `va` | VA | Veterans Affairs |
| `tms` | TMS | Transcranial Magnetic Stimulation |
| `nfetc` | NFETC | North Florida Evaluation and Treatment Center |
| `iop` | IOP | Intensive Outpatient Program |
| `comm` | Comm | Community Psychiatry |
| `hpc` | HPC | Hospice and Palliative Care |
| `addiction` | Addiction | Addiction Psychiatry |
| `forensic` | Forensic | Forensic Psychiatry |
| `clc` | CLC | Child Learning Center |

### Academic Year Months

Months should use these abbreviations in order:
```
JUL, AUG, SEP, OCT, NOV, DEC, JAN, FEB, MAR, APR, MAY, JUN
```

## 🔐 Access Control

### PGY Level Calculation

The system determines PGY level from the `graduate_yr` field:

```typescript
// PGY level = Current Year - Graduate Year + 1
// Example for 2026:
// - graduate_yr: 2023 → PGY-4
// - graduate_yr: 2024 → PGY-3
// - graduate_yr: 2025 → PGY-2
```

### Menu Visibility

The "PGY-4 Schedule" menu item is only visible to residents where:
```typescript
currentUserPGY === 4
```

This is filtered in `dashboard/page.tsx`:
```typescript
const filteredMenuItems = menuItems.filter(item => {
  if (item.title === "PGY-4 Schedule") return currentUserPGY === 4;
  // ... other filters
});
```

### Page Access

The page component includes additional access control:
```typescript
if (currentUserPGY !== 4) {
  return <AccessDeniedMessage />;
}
```

## 🖨️ Print Functionality

### Features

The schedule includes optimized print styles (`print.css`):

- **Page Setup**: 0.5 inch margins, optimized for Letter/A4 paper
- **Hidden Elements**: Removes buttons, navigation, and non-essential UI
- **Font Optimization**: Serif fonts at 10pt base size for readability
- **Color Preservation**: Rotation badge colors preserved using `print-color-adjust: exact`
- **Page Breaks**: Prevents awkward breaks in calendar sections
- **Borders**: Ensures table borders are visible

### Usage

Click the "Print Schedule" button or use browser print (Ctrl+P / Cmd+P).

### Customization Options

In `print.css`, uncomment these lines to:

**Use landscape orientation:**
```css
@page {
  size: landscape;
}
```

**Remove all backgrounds to save ink:**
```css
* {
  background: white !important;
}
```

## 🧪 Development Mode

### Mock Data

By default, the feature uses mock data for development:

In `lib/api/pgy4-schedules.ts`:
```typescript
const USE_MOCK_DATA = true;  // Set to false when backend is ready
```

Mock data includes:
- 7 sample residents
- 12 months of randomized rotations per resident
- Realistic assignment structure
- 500ms simulated network delay

### Switching to Real API

1. Set `USE_MOCK_DATA = false` in `pgy4-schedules.ts`
2. Update `API_BASE_URL` to your backend server:
   ```typescript
   const API_BASE_URL = 'https://your-backend.com/api';
   ```
3. Ensure backend endpoints are deployed
4. Configure CORS on backend to allow frontend origin

## 🎨 Styling

### Tailwind Classes

The components use Tailwind CSS for styling. Key patterns:

- **Grid Layout**: `grid grid-cols-[200px_repeat(12,1fr)]`
- **Responsive**: `hidden md:block`, `print:hidden`
- **Colors**: Using hex colors for rotation badges
- **Spacing**: Consistent padding with `p-2`, `p-3`, `p-6`

### Custom CSS

Print styles are in a separate `print.css` file to keep concerns separated.

## 🐛 Error Handling

The page handles several error scenarios:

### No Published Schedule
```
Empty state displayed with:
- Calendar icon
- "No Schedule Published Yet" message
- Expected publication timeline
```

### API Errors
```
Error state displayed with:
- Alert icon
- Error message
- "Try Again" button
```

### Network Errors
```
Caught in try/catch blocks
Logged to console for debugging
User-friendly message displayed
```

## 🚀 Future Enhancements

Potential improvements for future iterations:

1. **PDF Export**: Add client-side PDF generation (html2pdf.js)
2. **Email Schedule**: Send schedule via email
3. **Calendar Integration**: Export to Google Calendar/iCal
4. **Filters**: Filter team view by specific residents
5. **Academic Year Selector**: View schedules for different years
6. **Mobile Optimization**: Improve responsive design for small screens
7. **Rotation Details**: Click rotation badge to see more details
8. **Notifications**: Alert when schedule is published

## 📝 Code Documentation Standards

All code follows these documentation standards:

### JSDoc Comments
- All exported functions have JSDoc blocks
- Props interfaces fully documented
- Parameters and return types specified

### Inline Comments
- Complex logic explained with inline comments
- Backend integration notes for API consumers
- Accessibility annotations for screen readers

### TypeScript
- Strong typing throughout
- Interfaces for all data structures
- No `any` types used

## 🤝 Integration Checklist

For the backend team integrating this feature:

- [ ] Implement `GET /api/pgy4-schedules/published` endpoint
- [ ] Implement `GET /api/pgy4-schedules/my-schedule` endpoint
- [ ] Ensure response formats match TypeScript interfaces
- [ ] Return only schedules with `status: "published"`
- [ ] Include all 12 months for each resident
- [ ] Use correct rotation type IDs
- [ ] Use academic month abbreviations (JUL-JUN)
- [ ] Configure CORS for frontend origin
- [ ] Implement JWT authentication
- [ ] Test with PGY-4 resident accounts
- [ ] Handle 404 for unpublished schedules gracefully

## 📧 Support

For questions or issues:

- **Frontend Team**: Review component JSDoc and inline comments
- **Backend Team**: Reference API Integration section above
- **Testing**: Use mock data mode first, then test with real endpoints

---

**Last Updated:** February 2026  
**Version:** 1.0.0  
**Maintained by:** Frontend Team
