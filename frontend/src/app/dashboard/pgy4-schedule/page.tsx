/**
 * PGY-4 Schedule Page
 * 
 * This page displays the published rotation schedule for PGY-4 residents.
 * It shows both the current user's personal schedule and the complete team schedule.
 * 
 * FEATURES:
 * - My Schedule: Individual view of the current user's 12-month rotations
 * - Team Schedule: Grid view of all PGY-4 residents' rotations
 * - Published status checking: Only shows schedules marked as "published"
 * - Print functionality: Optimized print layout for schedule export
 * - Empty states: Helpful messaging when no schedule is available
 * - Loading states: Skeleton UI while data is being fetched
 * - Error handling: User-friendly error messages with retry options
 * 
 * BACKEND INTEGRATION:
 * This page consumes two API endpoints (see lib/api/pgy4-schedules.ts):
 * 1. GET /api/pgy4-schedules/my-schedule - Current user's rotations
 * 2. GET /api/pgy4-schedules/published - All PGY-4 residents' rotations
 * 
 * Both endpoints should return data only if the schedule status is "published".
 * Unpublished or draft schedules should not be visible to residents.
 * 
 * @module app/dashboard/pgy4-schedule/page
 */

'use client';

import React, { useEffect, useState } from 'react';
import './print.css';
import { 
  fetchMySchedule, 
  fetchPublishedSchedule,
  MyScheduleResponse,
  PGY4Schedule,
} from '@/lib/api/pgy4-schedules';
import { RotationCalendar } from '@/components/pgy4/RotationCalendar';
import { RotationLegend } from '@/components/pgy4/RotationLegend';
import { Button } from '@/components/ui/button';
import { Calendar, Printer, AlertCircle, Loader2 } from 'lucide-react';

/**
 * Loading state type for better type safety
 */
type LoadingState = 'idle' | 'loading' | 'success' | 'error';

/**
 * PGY4SchedulePage Component
 * 
 * Main page component for displaying PGY-4 rotation schedules.
 * Handles data fetching, state management, and rendering of schedule views.
 * 
 * STATE MANAGEMENT:
 * - mySchedule: Current user's personal rotation schedule
 * - teamSchedule: All PGY-4 residents' rotation schedules
 * - loadingState: Tracks the loading status of data fetching
 * - error: Stores any error messages for display
 * 
 * LIFECYCLE:
 * 1. Component mounts
 * 2. Fetches both my schedule and team schedule in parallel
 * 3. Updates state based on fetch results
 * 4. Renders appropriate view (loading, error, or schedule data)
 * 
 * @returns React component for the PGY-4 schedule page
 */
export default function PGY4SchedulePage() {
  // ============================================================================
  // STATE MANAGEMENT
  // ============================================================================
  
  // User's personal schedule state
  const [mySchedule, setMySchedule] = useState<MyScheduleResponse | null>(null);
  
  // Team-wide schedule state
  const [teamSchedule, setTeamSchedule] = useState<PGY4Schedule | null>(null);
  
  // Loading state tracking
  const [loadingState, setLoadingState] = useState<LoadingState>('idle');
  
  // Error message state
  const [error, setError] = useState<string | null>(null);

  // ============================================================================
  // DATA FETCHING
  // ============================================================================

  /**
   * Fetches both personal and team schedules
   * 
   * This function is called on component mount and when the user
   * clicks the "Try Again" button after an error.
   * 
   * PARALLEL FETCHING:
   * Both API calls are made simultaneously using Promise.allSettled()
   * to improve performance. If one fails, the other can still succeed.
   * 
   * ERROR HANDLING:
   * - 404 errors: Treated as "no schedule published yet" (not a critical error)
   * - Other errors: Shown as error state with retry option
   */
  const loadSchedules = async () => {
    setLoadingState('loading');
    setError(null);

    try {
      // Fetch both schedules in parallel for better performance
      const [myScheduleResult, teamScheduleResult] = await Promise.allSettled([
        fetchMySchedule(),
        fetchPublishedSchedule(),
      ]);

      // Handle personal schedule result
      if (myScheduleResult.status === 'fulfilled') {
        setMySchedule(myScheduleResult.value);
      } else {
        console.error('Failed to fetch my schedule:', myScheduleResult.reason);
        // Don't treat 404 as a critical error - schedule might not be published yet
        if (!myScheduleResult.reason?.message?.includes('404')) {
          setError('Unable to load your personal schedule');
        }
      }

      // Handle team schedule result
      if (teamScheduleResult.status === 'fulfilled') {
        setTeamSchedule(teamScheduleResult.value);
      } else {
        console.error('Failed to fetch team schedule:', teamScheduleResult.reason);
        // Don't treat 404 as a critical error - schedule might not be published yet
        if (!teamScheduleResult.reason?.message?.includes('404')) {
          setError('Unable to load team schedule');
        }
      }

      // Update loading state
      // Success if at least one schedule was loaded
      if (myScheduleResult.status === 'fulfilled' || teamScheduleResult.status === 'fulfilled') {
        setLoadingState('success');
      } else {
        // Both failed - show error state
        setLoadingState('error');
        setError('Unable to load schedules. Please try again later.');
      }
    } catch (err) {
      console.error('Unexpected error loading schedules:', err);
      setLoadingState('error');
      setError('An unexpected error occurred. Please try again.');
    }
  };

  /**
   * Effect: Load schedules on component mount
   */
  useEffect(() => {
    loadSchedules();
  }, []); // Empty dependency array = run once on mount

  // ============================================================================
  // EVENT HANDLERS
  // ============================================================================

  /**
   * Handles the print button click
   * 
   * Uses the browser's native print dialog. The print.css stylesheet
   * (loaded separately) optimizes the layout for printing.
   */
  const handlePrint = () => {
    window.print();
  };

  /**
   * Handles retry button click after an error
   */
  const handleRetry = () => {
    loadSchedules();
  };

  // ============================================================================
  // RENDER FUNCTIONS
  // ============================================================================

  /**
   * Renders the loading state
   * Shows a spinner and loading message while data is being fetched
   */
  const renderLoading = () => (
    <div className="flex flex-col items-center justify-center py-24">
      <Loader2 className="h-12 w-12 animate-spin text-blue-600 dark:text-blue-400 mb-4" />
      <p className="text-lg text-gray-600 dark:text-gray-400">Loading rotation schedule...</p>
      <p className="text-sm text-gray-500 dark:text-gray-500 mt-2">This should only take a moment</p>
    </div>
  );

  /**
   * Renders the error state
   * Shows an error message with a retry button
   */
  const renderError = () => (
    <div className="flex flex-col items-center justify-center py-24">
      <AlertCircle className="h-12 w-12 text-red-500 mb-4" />
      <p className="text-lg font-medium text-gray-900 mb-2">Unable to Load Schedule</p>
      <p className="text-sm text-gray-600 mb-6 text-center max-w-md">
        {error || 'An error occurred while loading the rotation schedule. Please try again.'}
      </p>
      <Button onClick={handleRetry} variant="outline">
        Try Again
      </Button>
    </div>
  );

  /**
   * Renders the empty state (no schedule published yet)
   * Shows when API returns successfully but no schedule is published
   */
  const renderEmptyState = () => (
    <div className="flex flex-col items-center justify-center py-24 bg-gray-50 dark:bg-gray-800 rounded-lg border-2 border-dashed border-gray-300 dark:border-gray-700">
      <Calendar className="h-16 w-16 text-gray-400 dark:text-gray-600 mb-4" />
      <p className="text-xl font-medium text-gray-900 dark:text-gray-100 mb-2">No Schedule Published Yet</p>
      <p className="text-sm text-gray-600 dark:text-gray-400 text-center max-w-md mb-4">
        The PGY-4 rotation schedule for the academic year has not been published yet.
        You will receive a notification when it becomes available.
      </p>
      <p className="text-xs text-gray-500 dark:text-gray-500">
        Expected publication: March 2026
      </p>
    </div>
  );

  /**
   * Renders the schedule content
   * Shows both personal and team schedules with a legend
   */
  const renderSchedule = () => {
    // If no schedules are available, show empty state
    if (!mySchedule && !teamSchedule) {
      return renderEmptyState();
    }

    return (
      <div className="space-y-4">
        {/* My Schedule Section */}
        {mySchedule && mySchedule.rotations.length > 0 && (
          <section className="my-schedule-section">
            <div className="mb-2">
              <h2 className="text-lg font-bold text-gray-900 dark:text-gray-100 flex items-center gap-2">
                <Calendar className="h-4 w-4" />
                My Schedule
              </h2>
              <p className="text-[10px] text-gray-600 dark:text-gray-400 mt-0.5">
                Your rotation assignments for academic year {mySchedule.academicYear}
              </p>
            </div>
            
            <RotationCalendar
              assignments={mySchedule.rotations}
              mode="individual"
              className="mb-3"
            />
          </section>
        )}

        {/* Team Schedule Section */}
        {teamSchedule && teamSchedule.assignments.length > 0 && (
          <section className="team-schedule-section">
            <div className="mb-2">
              <h2 className="text-lg font-bold text-gray-900 dark:text-gray-100">Team Schedule</h2>
              <p className="text-[10px] text-gray-600 dark:text-gray-400 mt-0.5">
                All PGY-4 residents&apos; rotation assignments for {teamSchedule.academicYear}
              </p>
              {teamSchedule.publishedDate && (
                <p className="text-[10px] text-gray-500 dark:text-gray-500 mt-0.5">
                  Published on {new Date(teamSchedule.publishedDate).toLocaleDateString()}
                </p>
              )}
            </div>
            
            <RotationCalendar
              assignments={teamSchedule.assignments}
              mode="team"
              className="mb-3"
            />
          </section>
        )}

        {/* Legend Section */}
        <section className="legend-section mt-4 p-3 bg-gray-50 dark:bg-gray-800 rounded-lg">
          <RotationLegend
            title="Rotation Types"
            layout="horizontal"
            showDescriptions={true}
          />
        </section>
      </div>
    );
  };

  // ============================================================================
  // MAIN RENDER
  // ============================================================================

  return (
    <div className="pgy4-schedule-page max-w-full mx-auto p-2 pl-8">
      {/* Page Header */}
      <div className="mb-3 flex justify-between items-start print:mb-4">
        <div>
          <h1 className="text-xl font-bold text-gray-900 dark:text-gray-100 mb-1">
            PGY-4 Rotation Schedule
          </h1>
          <p className="text-gray-600 dark:text-gray-400 text-xs">
            View your monthly rotation assignments for the academic year
          </p>
        </div>
        
        {/* Action Buttons (hidden when printing) */}
        <div className="flex gap-3 print:hidden">
          <Button
            onClick={handlePrint}
            variant="outline"
            className="flex items-center gap-2"
            disabled={loadingState !== 'success' || (!mySchedule && !teamSchedule)}
          >
            <Printer className="h-4 w-4" />
            Print Schedule
          </Button>
        </div>
      </div>

      {/* Main Content Area */}
      <div className="schedule-content">
        {/* Conditional rendering based on loading state */}
        {loadingState === 'loading' && renderLoading()}
        {loadingState === 'error' && renderError()}
        {loadingState === 'success' && renderSchedule()}
      </div>

      {/* Footer Information (visible on print) */}
      <div className="mt-8 text-center text-xs text-gray-500 dark:text-gray-500 print:mt-6">
        <p>
          For questions about your rotation schedule, please contact the Chief Residents
          or Program Director.
        </p>
      </div>
    </div>
  );
}
