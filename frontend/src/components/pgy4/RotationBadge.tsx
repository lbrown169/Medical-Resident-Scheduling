/**
 * RotationBadge Component
 * 
 * Displays a single rotation assignment as a colored badge.
 * This is the basic building block for displaying rotation types throughout
 * the PGY-4 schedule interface.
 * 
 * USAGE EXAMPLES:
 * ```tsx
 * // Simple rotation badge
 * <RotationBadge rotationType="intp-psy" />
 * 
 * // With custom sizing
 * <RotationBadge rotationType="consult" size="lg" />
 * 
 * // With custom class names
 * <RotationBadge rotationType="va" className="my-2" />
 * ```
 * 
 * @module components/pgy4/RotationBadge
 */

import React from 'react';
import { getRotationById } from '@/lib/api/pgy4-schedules';

/**
 * Props for the RotationBadge component
 */
export interface RotationBadgeProps {
  /** 
   * The rotation type ID (e.g., "intp-psy", "consult", "va")
   * Must match a valid rotation ID from ROTATION_TYPES
   */
  rotationType: string;
  
  /** 
   * Size variant for the badge
   * - sm: Small badge (compact view)
   * - md: Medium badge (default, good for calendar cells)
   * - lg: Large badge (header or prominent display)
   */
  size?: 'sm' | 'md' | 'lg';
  
  /** 
   * Additional CSS classes to apply to the badge
   * Useful for custom spacing or layout adjustments
   */
  className?: string;
  
  /**
   * Whether to show the full rotation name or abbreviation
   * - true: Show full name (e.g., "Inpatient Psychiatry")
   * - false: Show abbreviation (e.g., "Intp Psy")
   */
  showFullName?: boolean;
}

/**
 * RotationBadge Component
 * 
 * Renders a color-coded badge representing a rotation assignment.
 * The badge color is determined by the rotation type and matches
 * the color scheme used throughout the application.
 * 
 * The component automatically handles:
 * - Color mapping from rotation type to visual style
 * - Responsive sizing based on size prop
 * - Accessible color contrast for readability
 * - Graceful fallback for unknown rotation types
 * 
 * @param props - RotationBadgeProps
 * @returns React component displaying a rotation badge
 */
export function RotationBadge({
  rotationType,
  size = 'md',
  className = '',
  showFullName = false,
}: RotationBadgeProps) {
  // Look up rotation details from the centralized rotation types list
  const rotation = getRotationById(rotationType);
  
  // Fallback handling: if rotation type is not recognized, show a neutral badge
  if (!rotation) {
    console.warn(`Unknown rotation type: ${rotationType}`);
    return (
      <span className={`inline-block px-2 py-1 text-xs bg-gray-200 text-gray-800 rounded ${className}`}>
        {rotationType}
      </span>
    );
  }

  // Determine sizing classes based on size prop
  // These classes control padding, font size, and border radius
  const sizeClasses = {
    sm: 'px-1.5 py-0.5 text-[10px] rounded',
    md: 'px-2 py-0.5 text-xs rounded',
    lg: 'px-3 py-1 text-sm rounded-md',
  };

  // Display name: use full name with description if requested, otherwise abbreviation
  const displayName = showFullName && rotation.description 
    ? rotation.description 
    : rotation.name;

  return (
    <span
      className={`inline-block font-medium text-white ${sizeClasses[size]} ${className}`}
      style={{ 
        backgroundColor: rotation.color,
        // Ensure text is readable on colored background
        color: '#ffffff',
      }}
      // Accessibility: provide full rotation name for screen readers
      aria-label={`${rotation.name} rotation${rotation.description ? `: ${rotation.description}` : ''}`}
      // Data attribute for testing and debugging
      data-rotation-type={rotationType}
    >
      {displayName}
    </span>
  );
}

export default RotationBadge;
