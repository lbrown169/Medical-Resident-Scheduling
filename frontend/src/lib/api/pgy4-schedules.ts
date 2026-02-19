/**
 * PGY-4 Rotation Schedule API Service
 * 
 * This module handles all API communication for PGY-4 rotation schedules.
 * It includes TypeScript interfaces that define the expected data structures
 * from the backend API, mock data for local development, and fetch functions.
 * 
 * INTEGRATION NOTES FOR BACKEND TEAM:
 * - All interfaces represent the expected JSON response format
 * - Set USE_MOCK_DATA to false once backend endpoints are available
 * - Update API_BASE_URL to match your backend server URL
 * - Ensure CORS is properly configured for cross-origin requests
 */

// =============================================================================
// CONFIGURATION
// =============================================================================

/**
 * Toggle between mock data and real API endpoints
 * Set to false when backend endpoints are ready
 */
const USE_MOCK_DATA = true;

/**
 * Base URL for API endpoints
 * Update this to match your backend server URL in production
 */
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

// =============================================================================
// TYPE DEFINITIONS
// =============================================================================

/**
 * Represents a single rotation type with display properties
 * 
 * BACKEND NOTE: These rotation types should match the PGY4RotationRequests
 * rotation options. The color values are used for UI display consistency.
 */
export interface RotationType {
  /** Unique identifier for the rotation (e.g., "intp-psy") */
  id: string;
  /** Display name for the rotation (e.g., "Intp Psy") */
  name: string;
  /** CSS color class or hex value for UI rendering */
  color: string;
  /** Optional description of the rotation */
  description?: string;
}

/**
 * Available rotation types for PGY-4 year
 * These match the rotation options from the PGY4RotationRequests form
 */
export const ROTATION_TYPES: RotationType[] = [
  { id: 'intp-psy', name: 'Intp Psy', color: '#a855f7', description: 'Inpatient Psychiatry' },
  { id: 'consult', name: 'Consult', color: '#fb923c', description: 'Consultation-Liaison Psychiatry' },
  { id: 'va', name: 'VA', color: '#60a5fa', description: 'Veterans Affairs' },
  { id: 'tms', name: 'TMS', color: '#84cc16', description: 'Transcranial Magnetic Stimulation' },
  { id: 'nfetc', name: 'NFETC', color: '#fbbf24', description: 'North Florida Evaluation and Treatment Center' },
  { id: 'iop', name: 'IOP', color: '#22c55e', description: 'Intensive Outpatient Program' },
  { id: 'comm', name: 'Comm', color: '#3b82f6', description: 'Community Psychiatry' },
  { id: 'hpc', name: 'HPC', color: '#92400e', description: 'Hospice and Palliative Care' },
  { id: 'addiction', name: 'Addiction', color: '#14b8a6', description: 'Addiction Psychiatry' },
  { id: 'forensic', name: 'Forensic', color: '#ef4444', description: 'Forensic Psychiatry' },
  { id: 'clc', name: 'CLC', color: '#8b5cf6', description: 'Child Learning Center' },
];

/**
 * Represents the months of the academic year (July - June)
 */
export type AcademicMonth = 'JUL' | 'AUG' | 'SEP' | 'OCT' | 'NOV' | 'DEC' | 
                            'JAN' | 'FEB' | 'MAR' | 'APR' | 'MAY' | 'JUN';

/**
 * Academic year months in order (July is month 1, June is month 12)
 */
export const ACADEMIC_MONTHS: AcademicMonth[] = [
  'JUL', 'AUG', 'SEP', 'OCT', 'NOV', 'DEC',
  'JAN', 'FEB', 'MAR', 'APR', 'MAY', 'JUN'
];

/**
 * A single rotation assignment for one resident in one month
 * 
 * BACKEND ENDPOINT SCHEMA:
 * This interface represents one record in the rotation assignments.
 * Each resident should have 12 assignments (one per month) for the academic year.
 */
export interface RotationAssignment {
  /** Unique identifier for this assignment */
  id: string;
  /** Resident's unique identifier (matches Residents.resident_id) */
  residentId: string;
  /** Resident's first name */
  firstName: string;
  /** Resident's last name */
  lastName: string;
  /** Academic month (JUL-JUN) */
  month: AcademicMonth;
  /** Rotation type ID (must match one from ROTATION_TYPES) */
  rotationType: string;
  /** Academic year this schedule belongs to (e.g., "2025-2026") */
  academicYear: string;
}

/**
 * Complete PGY-4 rotation schedule including metadata
 * 
 * BACKEND ENDPOINT: GET /api/pgy4-schedules/published
 * Response format:
 * {
 *   "scheduleId": "uuid",
 *   "academicYear": "2025-2026",
 *   "status": "published",
 *   "publishedDate": "2026-01-15T10:30:00Z",
 *   "assignments": [ ...RotationAssignment objects... ]
 * }
 */
export interface PGY4Schedule {
  /** Unique identifier for this schedule */
  scheduleId: string;
  /** Academic year (e.g., "2025-2026") */
  academicYear: string;
  /** Schedule status: "published", "under-review", "draft" */
  status: 'published' | 'under-review' | 'draft';
  /** ISO 8601 timestamp when schedule was published */
  publishedDate?: string;
  /** Array of all rotation assignments for all PGY-4 residents */
  assignments: RotationAssignment[];
}

/**
 * Response from the current user's schedule endpoint
 * 
 * BACKEND ENDPOINT: GET /api/pgy4-schedules/my-schedule
 * Response format:
 * {
 *   "residentId": "uuid",
 *   "firstName": "John",
 *   "lastName": "Doe",
 *   "academicYear": "2025-2026",
 *   "rotations": [ ...RotationAssignment objects for this resident only... ]
 * }
 */
export interface MyScheduleResponse {
  /** Current user's resident ID */
  residentId: string;
  /** Current user's first name */
  firstName: string;
  /** Current user's last name */
  lastName: string;
  /** Academic year */
  academicYear: string;
  /** Array of 12 rotation assignments (one per month) */
  rotations: RotationAssignment[];
}

// =============================================================================
// MOCK DATA GENERATOR
// =============================================================================

/**
 * Generates mock rotation assignments for development/testing
 * 
 * This function creates realistic fake data that matches the expected
 * backend API response format. Use this for frontend development before
 * backend endpoints are available.
 * 
 * @param residentCount - Number of PGY-4 residents to generate
 * @returns Complete PGY4Schedule with mock assignments
 */
function generateMockSchedule(residentCount: number = 7): PGY4Schedule {
  const mockResidents = [
    { id: '1', firstName: 'Itneharsi', lastName: 'S.' },
    { id: '2', firstName: 'Lawrence', lastName: 'C.' },
    { id: '3', firstName: 'Rachel', lastName: 'P.' },
    { id: '4', firstName: 'Roshni', lastName: 'P.' },
    { id: '5', firstName: 'Alexis', lastName: 'S.' },
    { id: '6', firstName: 'Carolyn', lastName: 'L.' },
    { id: '7', firstName: 'Lauran', lastName: 'M.' },
  ].slice(0, residentCount);

  const assignments: RotationAssignment[] = [];
  
  // Generate 12 months of rotations for each resident
  mockResidents.forEach((resident) => {
    // Shuffle rotation types to create varied schedules
    const shuffledRotations = [...ROTATION_TYPES]
      .sort(() => Math.random() - 0.5)
      .slice(0, 12);

    ACADEMIC_MONTHS.forEach((month, index) => {
      assignments.push({
        id: `${resident.id}-${month}`,
        residentId: resident.id,
        firstName: resident.firstName,
        lastName: resident.lastName,
        month,
        rotationType: shuffledRotations[index % shuffledRotations.length].id,
        academicYear: '2025-2026',
      });
    });
  });

  return {
    scheduleId: 'mock-schedule-2025-2026',
    academicYear: '2025-2026',
    status: 'published',
    publishedDate: '2026-01-15T10:30:00Z',
    assignments,
  };
}

/**
 * Generates mock schedule data for a specific resident
 * 
 * @param residentId - The resident's ID
 * @param firstName - The resident's first name
 * @param lastName - The resident's last name
 * @returns MyScheduleResponse with 12 monthly rotations
 */
function generateMockMySchedule(
  residentId: string,
  firstName: string,
  lastName: string
): MyScheduleResponse {
  const rotations: RotationAssignment[] = [];
  const shuffledRotations = [...ROTATION_TYPES]
    .sort(() => Math.random() - 0.5);

  ACADEMIC_MONTHS.forEach((month, index) => {
    rotations.push({
      id: `${residentId}-${month}`,
      residentId,
      firstName,
      lastName,
      month,
      rotationType: shuffledRotations[index % shuffledRotations.length].id,
      academicYear: '2025-2026',
    });
  });

  return {
    residentId,
    firstName,
    lastName,
    academicYear: '2025-2026',
    rotations,
  };
}

// =============================================================================
// API FUNCTIONS
// =============================================================================

/**
 * Fetches the published PGY-4 rotation schedule for all residents
 * 
 * BACKEND ENDPOINT SPECIFICATION:
 * - URL: GET /api/pgy4-schedules/published
 * - Authentication: Required (JWT token in Authorization header)
 * - Query Parameters:
 *   - academicYear (optional): Filter by specific year (e.g., "2025-2026")
 * - Response: PGY4Schedule object
 * - Error codes:
 *   - 404: No published schedule found
 *   - 401: Unauthorized (invalid/missing token)
 *   - 403: Forbidden (user not authorized to view schedules)
 * 
 * @param academicYear - Optional filter by academic year
 * @returns Promise resolving to PGY4Schedule
 * @throws Error if fetch fails or schedule not found
 */
export async function fetchPublishedSchedule(
  academicYear?: string
): Promise<PGY4Schedule> {
  // Return mock data if enabled
  if (USE_MOCK_DATA) {
    console.log('[DEV MODE] Using mock data for published schedule');
    return new Promise((resolve) => {
      setTimeout(() => resolve(generateMockSchedule()), 500); // Simulate network delay
    });
  }

  // Real API call
  try {
    const url = new URL(`${API_BASE_URL}/pgy4-schedules/published`);
    if (academicYear) {
      url.searchParams.append('academicYear', academicYear);
    }

    const token = localStorage.getItem('token');
    const response = await fetch(url.toString(), {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('No published schedule found');
      }
      throw new Error(`Failed to fetch schedule: ${response.statusText}`);
    }

    const data: PGY4Schedule = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching published schedule:', error);
    throw error;
  }
}

/**
 * Fetches the current user's PGY-4 rotation schedule
 * 
 * BACKEND ENDPOINT SPECIFICATION:
 * - URL: GET /api/pgy4-schedules/my-schedule
 * - Authentication: Required (JWT token, user ID extracted from token)
 * - Query Parameters:
 *   - academicYear (optional): Filter by specific year
 * - Response: MyScheduleResponse object
 * - Error codes:
 *   - 404: No schedule found for this resident
 *   - 401: Unauthorized
 *   - 403: User is not a PGY-4 resident
 * 
 * @param academicYear - Optional filter by academic year
 * @returns Promise resolving to MyScheduleResponse
 * @throws Error if fetch fails or user has no schedule
 */
export async function fetchMySchedule(
  academicYear?: string
): Promise<MyScheduleResponse> {
  // Return mock data if enabled
  if (USE_MOCK_DATA) {
    console.log('[DEV MODE] Using mock data for my schedule');
    return new Promise((resolve) => {
      setTimeout(() => {
        // Simulate current user data (in real app, this comes from auth context)
        resolve(generateMockMySchedule('current-user-id', 'John', 'Doe'));
      }, 500);
    });
  }

  // Real API call
  try {
    const url = new URL(`${API_BASE_URL}/pgy4-schedules/my-schedule`);
    if (academicYear) {
      url.searchParams.append('academicYear', academicYear);
    }

    const token = localStorage.getItem('token');
    const response = await fetch(url.toString(), {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('No schedule found for your account');
      }
      throw new Error(`Failed to fetch your schedule: ${response.statusText}`);
    }

    const data: MyScheduleResponse = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching my schedule:', error);
    throw error;
  }
}

/**
 * Utility function to get rotation details by ID
 * 
 * @param rotationId - The rotation type ID
 * @returns RotationType object or undefined if not found
 */
export function getRotationById(rotationId: string): RotationType | undefined {
  return ROTATION_TYPES.find(r => r.id === rotationId);
}

/**
 * Utility function to get rotation name by ID
 * 
 * @param rotationId - The rotation type ID
 * @returns Rotation display name or the ID if not found
 */
export function getRotationName(rotationId: string): string {
  return getRotationById(rotationId)?.name || rotationId;
}

/**
 * Utility function to get rotation color by ID
 * 
 * @param rotationId - The rotation type ID
 * @returns Color hex value or default gray
 */
export function getRotationColor(rotationId: string): string {
  return getRotationById(rotationId)?.color || '#6b7280';
}
