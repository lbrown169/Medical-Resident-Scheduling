/**
 * RotationCalendar Component
 * 
 * Displays a year-long calendar view of rotation assignments for PGY-4 residents.
 * Shows a grid with months as columns (July-June) and residents as rows.
 * 
 * USAGE EXAMPLES:
 * ```tsx
 * // Display team schedule (all residents)
 * <RotationCalendar assignments={allAssignments} mode="team" />
 * 
 * // Display individual schedule (single resident)
 * <RotationCalendar assignments={myAssignments} mode="individual" />
 * 
 * // With custom title
 * <RotationCalendar 
 *   assignments={assignments} 
 *   mode="team" 
 *   title="2025-2026 PGY-4 Rotation Schedule" 
 * />
 * ```
 * 
 * @module components/pgy4/RotationCalendar
 */

import React, { useMemo } from 'react';
import {
  RotationAssignment,
  ACADEMIC_MONTHS,
  AcademicMonth,
} from '@/lib/api/pgy4-schedules';
import { RotationBadge } from './RotationBadge';

/**
 * Props for the RotationCalendar component
 */
export interface RotationCalendarProps {
  /**
   * Array of rotation assignments to display
   * Should include assignments for one or more residents across 12 months
   */
  assignments: RotationAssignment[];
  
  /**
   * Display mode
   * - team: Shows all residents in rows (for team view)
   * - individual: Shows single resident with larger badges (for personal view)
   */
  mode: 'team' | 'individual';
  
  /**
   * Optional title displayed above the calendar
   */
  title?: string;
  
  /**
   * Additional CSS classes for the container
   */
  className?: string;
  
  /**
   * Whether to show the legend below the calendar
   */
  showLegend?: boolean;
}

/**
 * Represents a resident's complete rotation schedule
 * Used internally to organize data by resident
 */
interface ResidentSchedule {
  residentId: string;
  residentName: string;
  rotations: Map<AcademicMonth, RotationAssignment>;
}

/**
 * RotationCalendar Component
 * 
 * Renders a comprehensive calendar view showing rotation assignments across
 * the academic year (July - June). The calendar adapts its display based on
 * whether it's showing a team schedule or an individual schedule.
 * 
 * Features:
 * - Responsive grid layout that works on desktop and tablet
 * - Color-coded rotation badges for easy visual scanning
 * - Supports both team view (multiple residents) and individual view
 * - Handles missing assignments gracefully
 * - Optimized rendering with useMemo for large datasets
 * 
 * Data Organization:
 * The component organizes assignments by resident, then by month, to ensure
 * efficient rendering and easy access to specific month/resident combinations.
 * 
 * @param props - RotationCalendarProps
 * @returns React component displaying the rotation calendar
 */
export function RotationCalendar({
  assignments,
  mode,
  title,
  className = '',
}: RotationCalendarProps) {
  /**
   * Organize assignments by resident for efficient lookup
   * 
   * This memo creates a data structure like:
   * [
   *   {
   *     residentId: "123",
   *     residentName: "John Doe",
   *     rotations: Map { "JUL" => {...}, "AUG" => {...}, ... }
   *   },
   *   ...
   * ]
   * 
   * This allows O(1) lookup for any resident/month combination
   */
  const residentSchedules: ResidentSchedule[] = useMemo(() => {
    // Group assignments by resident ID
    const byResident = new Map<string, RotationAssignment[]>();
    
    assignments.forEach((assignment) => {
      const existing = byResident.get(assignment.residentId) || [];
      existing.push(assignment);
      byResident.set(assignment.residentId, existing);
    });

    // Convert to structured format with month map for each resident
    return Array.from(byResident.entries()).map(([residentId, residentAssignments]) => {
      // Get resident name from first assignment
      const firstAssignment = residentAssignments[0];
      const residentName = `${firstAssignment.firstName} ${firstAssignment.lastName}`;
      
      // Create month-keyed map for O(1) lookup
      const rotations = new Map<AcademicMonth, RotationAssignment>();
      residentAssignments.forEach((assignment) => {
        rotations.set(assignment.month, assignment);
      });

      return {
        residentId,
        residentName,
        rotations,
      };
    });
  }, [assignments]);

  /**
   * Renders a single cell in the calendar grid
   * 
   * @param schedule - The resident's full schedule
   * @param month - The specific month to render
   * @returns React element for the cell
   */
  const renderCell = (schedule: ResidentSchedule, month: AcademicMonth) => {
    const assignment = schedule.rotations.get(month);
    
    if (!assignment) {
      // Empty cell if no assignment for this month
      return (
        <div 
          className="py-1 text-center text-[10px] text-gray-400 dark:text-gray-600 border-r border-gray-200 dark:border-gray-700"
          aria-label={`No rotation assigned for ${month}`}
        >
          —
        </div>
      );
    }

    return (
      <div 
        className="py-1 flex items-center justify-center border-r border-gray-200 dark:border-gray-700"
        // Data attributes for testing and debugging
        data-resident-id={schedule.residentId}
        data-month={month}
        data-rotation={assignment.rotationType}
      >
        <RotationBadge
          rotationType={assignment.rotationType}
          size={mode === 'individual' ? 'md' : 'sm'}
        />
      </div>
    );
  };

  // Show empty state if no assignments
  if (assignments.length === 0) {
    return (
      <div className={`rotation-calendar-empty ${className}`}>
        <div className="text-center py-12 text-gray-500 dark:text-gray-400">
          <p className="text-lg font-medium">No rotation schedule available</p>
          <p className="text-sm mt-2">Assignments will appear here once published</p>
        </div>
      </div>
    );
  }

  return (
    <div className={`rotation-calendar ${className}`}>
      {/* Calendar title */}
      {title && (
        <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100 mb-4">
          {title}
        </h2>
      )}

      {/* Calendar grid container */}
      <div className="border border-gray-300 dark:border-gray-700 rounded-lg">
        <div className="w-full">
          {/* Header row with month names */}
          <div className="grid grid-cols-[100px_repeat(12,minmax(0,1fr))] bg-gray-100 dark:bg-gray-800 border-b-2 border-gray-300 dark:border-gray-700">
            {/* First column: "RESIDENTS" label */}
            <div className="p-1 font-semibold text-[10px] text-gray-700 dark:text-gray-300 border-r-2 border-gray-300 dark:border-gray-700">
              {mode === 'team' ? 'RESIDENTS' : 'MONTHS'}
            </div>
            
            {/* Month columns */}
            {ACADEMIC_MONTHS.map((month) => (
              <div
                key={month}
                className="p-1 text-center font-semibold text-[10px] text-gray-700 dark:text-gray-300 border-r border-gray-200 dark:border-gray-700 last:border-r-0"
              >
                {month}
              </div>
            ))}
          </div>

          {/* Resident rows */}
          {residentSchedules.map((schedule, index) => (
            <div
              key={schedule.residentId}
              className={`grid grid-cols-[100px_repeat(12,minmax(0,1fr))] ${
                index % 2 === 0 ? 'bg-white dark:bg-gray-900' : 'bg-gray-50 dark:bg-gray-800'
              } border-b border-gray-200 dark:border-gray-700 last:border-b-0`}
            >
              {/* Resident name column */}
              <div className="p-1 font-medium text-[10px] text-gray-900 dark:text-gray-100 border-r-2 border-gray-300 dark:border-gray-700 flex items-center truncate">
                {schedule.residentName}
              </div>
              
              {/* Rotation cells for each month */}
              {ACADEMIC_MONTHS.map((month) => (
                <React.Fragment key={month}>
                  {renderCell(schedule, month)}
                </React.Fragment>
              ))}
            </div>
          ))}
        </div>
      </div>

      {/* Accessibility: Screen reader summary */}
      <div className="sr-only">
        Rotation calendar showing {residentSchedules.length} resident(s) 
        across {ACADEMIC_MONTHS.length} months from July to June
      </div>
    </div>
  );
}

export default RotationCalendar;
