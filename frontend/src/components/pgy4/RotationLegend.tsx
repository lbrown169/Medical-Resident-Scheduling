/**
 * RotationLegend Component
 * 
 * Displays a color-coded legend showing all available rotation types.
 * This helps users understand the meaning of colors used in the rotation calendar.
 * 
 * USAGE EXAMPLES:
 * ```tsx
 * // Simple legend (horizontal layout)
 * <RotationLegend />
 * 
 * // Vertical layout
 * <RotationLegend layout="vertical" />
 * 
 * // With custom title
 * <RotationLegend title="Rotation Types" />
 * 
 * // Show only specific rotations
 * <RotationLegend rotationIds={['intp-psy', 'consult', 'va']} />
 * ```
 * 
 * @module components/pgy4/RotationLegend
 */

import React from 'react';
import { ROTATION_TYPES, RotationType } from '@/lib/api/pgy4-schedules';
import { RotationBadge } from './RotationBadge';

/**
 * Props for the RotationLegend component
 */
export interface RotationLegendProps {
  /**
   * Optional title to display above the legend
   * Defaults to "Rotation Types"
   */
  title?: string;
  
  /**
   * Layout orientation
   * - horizontal: Items displayed in a row (wraps on small screens)
   * - vertical: Items displayed in a column
   */
  layout?: 'horizontal' | 'vertical';
  
  /**
   * Filter to show only specific rotation types
   * If not provided, all rotation types are shown
   */
  rotationIds?: string[];
  
  /**
   * Whether to show rotation descriptions/full names
   * - true: Show full descriptions (e.g., "Inpatient Psychiatry")
   * - false: Show abbreviations only (e.g., "Intp Psy")
   */
  showDescriptions?: boolean;
  
  /**
   * Additional CSS classes for the container
   */
  className?: string;
}

/**
 * RotationLegend Component
 * 
 * Renders a visual legend of all rotation types with their corresponding colors.
 * This component is typically placed near rotation calendars or schedules to help
 * users understand the color coding system.
 * 
 * Features:
 * - Displays all 11 rotation types with their official colors
 * - Supports both horizontal and vertical layouts
 * - Can be filtered to show only specific rotations
 * - Optionally displays full rotation descriptions
 * - Responsive design that adapts to screen size
 * 
 * @param props - RotationLegendProps
 * @returns React component displaying the rotation legend
 */
export function RotationLegend({
  title = 'Rotation Types',
  layout = 'horizontal',
  rotationIds,
  showDescriptions = false,
  className = '',
}: RotationLegendProps) {
  // Filter rotations if specific IDs are provided
  const rotationsToDisplay: RotationType[] = rotationIds
    ? ROTATION_TYPES.filter(r => rotationIds.includes(r.id))
    : ROTATION_TYPES;

  // Container classes based on layout
  const containerClasses = layout === 'vertical'
    ? 'flex flex-col gap-2'
    : 'flex flex-wrap gap-2';

  return (
    <div className={`rotation-legend ${className}`}>
      {/* Legend title */}
      {title && (
        <h3 className="text-sm font-semibold text-gray-700 dark:text-gray-300 mb-3">
          {title}
        </h3>
      )}
      
      {/* Legend items */}
      <div className={containerClasses}>
        {rotationsToDisplay.map((rotation) => (
          <div
            key={rotation.id}
            className="flex items-center gap-2"
            // Data attribute for testing
            data-rotation-legend-item={rotation.id}
          >
            {/* Color indicator badge */}
            <RotationBadge
              rotationType={rotation.id}
              size="sm"
              showFullName={false}
            />
            
            {/* Optional description text */}
            {showDescriptions && rotation.description && (
              <span className="text-xs text-gray-600 dark:text-gray-400">
                {rotation.description}
              </span>
            )}
          </div>
        ))}
      </div>
      
      {/* Accessibility: Provide context for screen readers */}
      <div className="sr-only">
        Legend showing {rotationsToDisplay.length} rotation types with their color codes
      </div>
    </div>
  );
}

export default RotationLegend;
