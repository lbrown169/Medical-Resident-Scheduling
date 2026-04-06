"use client";

import { config } from "../../../config";
import React, { useState, useEffect, useCallback, useRef } from "react";

import { toast } from "../../../lib/use-toast";
import { Button } from "../../../components/ui/button";
import { ClipboardList } from "lucide-react";
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "../../../components/ui/alert-dialog";

interface RotationOption {
  value: string;
  label: string;
  color?: string;
}

interface RotationTypeDto {
  rotationTypeId: string;
  rotationName: string;
}

/**
 * PGY4 colors are default, pass props to override
 * Pass your own rotationColors prop to override for other PGY levels.
 */
const DEFAULT_ROTATION_COLORS: Record<string, string> = {
  "Inpatient Psy": "#8b5cf6",
  "Psy Consults": "#f97316",
  Addiction: "#14b8a6",
  VA: "#60a5fa",
  TMS: "#84cc16",
  NFETC: "#eab308",
  IOP: "#22c55e",
  "Community Psy": "#3b82f6",
  HPC: "#92400e",
  Forensic: "#ef4444",
  CLC: "#ec4899",
};

// Rotation types that should never appear in the form regardless of PGY year
const EXCLUDED_ROTATIONS = ["Chief", "Unassigned", "Sum"];

interface RotationFormProps {
  // The logged-in resident's ID
  userId: string;

  // The logged-in resident's PGY level
  userPGY: number;

  /**
   * Which PGY level is required to access this form.
   * If multiple PGYs are to access the same form, the prop would need to change.
   */
  requiredPGY: number;

  /**
   * Which PGY year to fetch rotation types for.
   * If multiple PGYs are to access the same form, the prop would need to change.
   */
  rotationPgyYear: number;

  /** Submission deadline, form closes after this date
   */
  deadline: Date;

  /** Ignore the submission deadline for the admins
   *  Admins should always have access
   */
  ignoreDeadline?: boolean;

  /**
   * Optional color map for rotation types.
   * Defaults to PGY4 colors.
   */
  rotationColors?: Record<string, string>;

  /**
   * Base API endpoint for submitting/updating preferences.
   * POST to this URL to create, PUT to ${submitEndpoint}/${existingId} to update.
   * Currently its /api/rotation-pref-request
   */
  submitEndpoint: string;

  /**
   * API endpoint to fetch the resident's existing submission.
   * Should accept the userId as the last path segment.
   * Currently its /api/rotation-pref-request/resident
   */
  fetchEndpoint: string;

  /** Optional callback fired after a successful submission */
  onSuccess?: () => void;
}

/**
 * Reusable Rotation Preference Form
 *
 * Features:
 * - 8 priority selections (at least 4 required)
 * - 3 optional alternative selections
 * - 3 optional avoidance selections
 * - Additional notes field
 * - Validation to prevent duplicate selections in priority fields
 *
 * Used by PGY3 residents to submit PGY4 rotation preferences,
 * and can be reused by other PGY levels by passing different props.
 * If the same form is to be used by multiple PGYs, the prop format would need to change.
 */
const RotationForm: React.FC<RotationFormProps> = ({
  userId,
  userPGY,
  requiredPGY,
  rotationPgyYear,
  deadline,
  ignoreDeadline = false,
  rotationColors = DEFAULT_ROTATION_COLORS,
  submitEndpoint,
  fetchEndpoint,
  onSuccess,
}) => {
  // Rotation options from backend
  const [rotationOptions, setRotationOptions] = useState<RotationOption[]>([]);

  // Existing request id if editing
  const [existingRequestId, setExistingRequestId] = useState<string | null>(
    null,
  );

  // Priorities
  const [firstPriority, setFirstPriority] = useState<string | null>(null);
  const [secondPriority, setSecondPriority] = useState<string | null>(null);
  const [thirdPriority, setThirdPriority] = useState<string | null>(null);
  const [fourthPriority, setFourthPriority] = useState<string | null>(null);
  const [fifthPriority, setFifthPriority] = useState<string | null>(null);
  const [sixthPriority, setSixthPriority] = useState<string | null>(null);
  const [seventhPriority, setSeventhPriority] = useState<string | null>(null);
  const [eighthPriority, setEighthPriority] = useState<string | null>(null);

  // Alternates
  const [alternative1, setAlternative1] = useState<string | null>(null);
  const [alternative2, setAlternative2] = useState<string | null>(null);
  const [alternative3, setAlternative3] = useState<string | null>(null);

  // Avoids
  const [avoid1, setAvoid1] = useState<string | null>(null);
  const [avoid2, setAvoid2] = useState<string | null>(null);
  const [avoid3, setAvoid3] = useState<string | null>(null);

  // User notes
  const [additionalNotes, setAdditionalNotes] = useState("");

  // UI state
  const [loading, setLoading] = useState(false);
  const [showSuccessDialog, setShowSuccessDialog] = useState(false);

  const isDeadlinePassed = !ignoreDeadline && new Date() > deadline;

  // Keep a ref to rotationColors
  // If we don't it can infinitely refetch.
  const rotationColorsRef = useRef(rotationColors);
  useEffect(() => {
    rotationColorsRef.current = rotationColors;
  });

  // Load rotation types
  useEffect(() => {
    const loadRotationTypes = async () => {
      try {
        const res = await fetch(
          `${config.apiUrl}/api/rotation-types?pgyYear=${rotationPgyYear}`,
        );

        if (!res.ok) throw new Error();

        const data = await res.json();

        const options = data.rotationTypes
          .filter(
            (rt: RotationTypeDto) =>
              !EXCLUDED_ROTATIONS.includes(rt.rotationName),
          )
          .map((rt: RotationTypeDto) => ({
            value: rt.rotationTypeId,
            label: rt.rotationName,
            color: rotationColorsRef.current[rt.rotationName] ?? "#94a3b8",
          }));

        setRotationOptions(options);
      } catch {
        toast({
          variant: "destructive",
          title: "Error",
          description: "Could not load rotation types.",
        });
      }
    };

    loadRotationTypes();
  }, [rotationPgyYear]);

  // Define at component level, not inside useEffect
  const loadExistingRequest = useCallback(async () => {
    try {
      const res = await fetch(`${config.apiUrl}/${fetchEndpoint}/${userId}`);
      if (!res.ok) return;
      const data = await res.json();

      setExistingRequestId(data.rotationPrefRequestId);

      const priorities = data.priorities ?? [];
      const alternatives = data.alternatives ?? [];
      const avoids = data.avoids ?? [];

      setFirstPriority(priorities[0]?.rotationTypeId ?? null);
      setSecondPriority(priorities[1]?.rotationTypeId ?? null);
      setThirdPriority(priorities[2]?.rotationTypeId ?? null);
      setFourthPriority(priorities[3]?.rotationTypeId ?? null);
      setFifthPriority(priorities[4]?.rotationTypeId ?? null);
      setSixthPriority(priorities[5]?.rotationTypeId ?? null);
      setSeventhPriority(priorities[6]?.rotationTypeId ?? null);
      setEighthPriority(priorities[7]?.rotationTypeId ?? null);

      setAlternative1(alternatives[0]?.rotationTypeId ?? null);
      setAlternative2(alternatives[1]?.rotationTypeId ?? null);
      setAlternative3(alternatives[2]?.rotationTypeId ?? null);

      setAvoid1(avoids[0]?.rotationTypeId ?? null);
      setAvoid2(avoids[1]?.rotationTypeId ?? null);
      setAvoid3(avoids[2]?.rotationTypeId ?? null);

      setAdditionalNotes(data.additionalNotes ?? "");
    } catch {
      toast({
        variant: "destructive",
        title: "Error",
        description: "Could not load existing rotation request.",
      });
    }
  }, [userId, fetchEndpoint]);

  // Then just call it in useEffect
  useEffect(() => {
    loadExistingRequest();
  }, [loadExistingRequest]);

  const getPrioritiesArray = () =>
    [
      firstPriority,
      secondPriority,
      thirdPriority,
      fourthPriority,
      fifthPriority,
      sixthPriority,
      seventhPriority,
      eighthPriority,
    ].filter(Boolean) as string[];

  const validateNoDuplicates = () => {
    const arr = getPrioritiesArray();
    return new Set(arr).size === arr.length;
  };

  const validateMinFourFields = (): boolean => {
    return getPrioritiesArray().length >= 4;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validate at least 4 priorities are filled
    if (!validateMinFourFields()) {
      toast({
        variant: "destructive",
        title: "Error",
        description: "At least 4 priority fields are required.",
      });
      return;
    }

    // Validate no duplicates among priorities
    if (!validateNoDuplicates()) {
      toast({
        variant: "destructive",
        title: "Error",
        description:
          "Each rotation can only be selected once in the priority fields.",
      });
      return;
    }

    setLoading(true);

    // Try to upload users request after the validation above
    try {
      // Use the input from user
      const priorities = getPrioritiesArray();
      const alternatives = [alternative1, alternative2, alternative3].filter(
        Boolean,
      );
      const avoids = [avoid1, avoid2, avoid3].filter(Boolean);

      const payload = {
        residentId: userId,
        priorities,
        alternatives,
        avoids,
        additionalNotes,
      };

      // Basically, if the id exists we update it, otherwise we make a new one
      const url = existingRequestId
        ? `${config.apiUrl}/${submitEndpoint}/${existingRequestId}`
        : `${config.apiUrl}/${submitEndpoint}`;

      // Use the url we selected
      const method = existingRequestId ? "PUT" : "POST";

      // call the correct method, pass payload
      const res = await fetch(url, {
        method,
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!res.ok) throw new Error("Failed to submit");

      // Show if successful
      setShowSuccessDialog(true);
      onSuccess?.();
    } catch {
      toast({
        variant: "destructive",
        title: "Submission failed",
        description: "Unable to save rotation preferences.",
      });
    } finally {
      setLoading(false);
    }
  };

  /**
   * Gets available options for priority fields, excluding already selected priorities.
   * Used to reduce dropdown options to avoid duplicates.
   */
  const getAvailablePriorityOptions = (
    currentValue: string | null,
  ): typeof rotationOptions => {
    const selected = getPrioritiesArray().filter((v) => v !== currentValue);
    return rotationOptions.filter((opt) => !selected.includes(opt.value));
  };

  const getAllOptions = (): typeof rotationOptions => rotationOptions;

  // Access check
  if (userPGY !== requiredPGY) {
    return (
      <div className="max-w-4xl mx-auto">
        <div className="bg-card rounded-xl shadow-lg border border-border p-8 text-center">
          <ClipboardList className="h-16 w-16 mx-auto mb-4 text-muted-foreground" />
          <h2 className="text-2xl font-bold text-foreground mb-2">
            Access Denied
          </h2>
          <p className="text-muted-foreground">
            This page is only accessible to PGY-{requiredPGY} residents.
          </p>
        </div>
      </div>
    );
  }

  // Check if deadline has passed
  if (isDeadlinePassed) {
    return (
      <div className="max-w-4xl mx-auto">
        <div className="bg-card rounded-xl shadow-lg border border-border p-8 text-center">
          <ClipboardList className="h-16 w-16 mx-auto mb-4 text-muted-foreground" />
          <h2 className="text-2xl font-bold text-foreground mb-2">
            Submission Deadline Passed
          </h2>
          <p className="text-muted-foreground">
            The deadline for submitting rotation preferences was{" "}
            {deadline.toLocaleDateString("en-US", {
              day: "numeric",
              month: "long",
              year: "numeric",
            })}{" "}
            at{" "}
            {deadline.toLocaleTimeString("en-US", {
              hour: "numeric",
              minute: "2-digit",
              hour12: true,
              timeZoneName: "short",
            })}
            . The form is now closed.
          </p>
        </div>
      </div>
    );
  }

  // Show form
  return (
    <>
      <AlertDialog open={showSuccessDialog} onOpenChange={setShowSuccessDialog}>
        <AlertDialogContent className="max-w-md">
          <AlertDialogHeader>
            <div className="flex justify-center mb-4">
              <div className="bg-muted rounded-lg p-3">
                <ClipboardList className="h-8 w-8 text-muted-foreground" />
              </div>
            </div>
            <AlertDialogTitle className="text-center text-2xl">
              Rotation Request Submitted!
            </AlertDialogTitle>
            <AlertDialogDescription className="text-center">
              Your rotation preferences have been successfully submitted.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter className="sm:justify-center">
            <Button
              onClick={() => {
                setShowSuccessDialog(false);
                loadExistingRequest();
              }}
              className="w-full sm:w-auto"
            >
              Close
            </Button>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      <div className="max-w-4xl mx-auto">
        <div className="bg-card rounded-xl shadow-lg border border-border p-8">
          <form onSubmit={handleSubmit} className="space-y-8">
            {/* Priorities */}
            <div>
              <h2 className="text-xl font-bold text-foreground mb-4">
                Rotation Preferences
              </h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <SelectField
                  label="First Priority (required)"
                  value={firstPriority}
                  onChange={setFirstPriority}
                  options={getAvailablePriorityOptions(firstPriority)}
                  required
                />
                <SelectField
                  label="Second Priority (required)"
                  value={secondPriority}
                  onChange={setSecondPriority}
                  options={getAvailablePriorityOptions(secondPriority)}
                  required
                />
                <SelectField
                  label="Third Priority (required)"
                  value={thirdPriority}
                  onChange={setThirdPriority}
                  options={getAvailablePriorityOptions(thirdPriority)}
                  required
                />
                <SelectField
                  label="Fourth Priority (required)"
                  value={fourthPriority}
                  onChange={setFourthPriority}
                  options={getAvailablePriorityOptions(fourthPriority)}
                  required
                />
                <SelectField
                  label="Fifth Priority (optional)"
                  value={fifthPriority}
                  onChange={setFifthPriority}
                  options={getAvailablePriorityOptions(fifthPriority)}
                />
                <SelectField
                  label="Sixth Priority (optional)"
                  value={sixthPriority}
                  onChange={setSixthPriority}
                  options={getAvailablePriorityOptions(sixthPriority)}
                />
                <SelectField
                  label="Seventh Priority (optional)"
                  value={seventhPriority}
                  onChange={setSeventhPriority}
                  options={getAvailablePriorityOptions(seventhPriority)}
                />
                <SelectField
                  label="Eighth Priority (optional)"
                  value={eighthPriority}
                  onChange={setEighthPriority}
                  options={getAvailablePriorityOptions(eighthPriority)}
                />
              </div>
            </div>

            {/* Alternatives */}
            <div>
              <h2 className="text-xl font-bold text-foreground mb-4">
                Alternatives{" "}
                <span className="text-muted-foreground text-sm">
                  (optional)
                </span>
              </h2>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <SelectField
                  label="Alternative 1"
                  value={alternative1}
                  onChange={setAlternative1}
                  options={getAllOptions()}
                />
                <SelectField
                  label="Alternative 2"
                  value={alternative2}
                  onChange={setAlternative2}
                  options={getAllOptions()}
                />
                <SelectField
                  label="Alternative 3"
                  value={alternative3}
                  onChange={setAlternative3}
                  options={getAllOptions()}
                />
              </div>
            </div>

            {/* Avoid */}
            <div>
              <h2 className="text-xl font-bold text-foreground mb-4">
                Avoid{" "}
                <span className="text-muted-foreground text-sm">
                  (optional)
                </span>
              </h2>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <SelectField
                  label="Avoid 1"
                  value={avoid1}
                  onChange={setAvoid1}
                  options={getAllOptions()}
                />
                <SelectField
                  label="Avoid 2"
                  value={avoid2}
                  onChange={setAvoid2}
                  options={getAllOptions()}
                />
                <SelectField
                  label="Avoid 3"
                  value={avoid3}
                  onChange={setAvoid3}
                  options={getAllOptions()}
                />
              </div>
            </div>

            {/* Additional Notes */}
            <div>
              <h2 className="text-xl font-bold text-foreground mb-4">
                Additional Notes{" "}
                <span className="text-muted-foreground text-sm">
                  (optional)
                </span>
              </h2>
              <textarea
                value={additionalNotes}
                onChange={(e) => setAdditionalNotes(e.target.value)}
                placeholder="Please provide any scheduling conflicts, vacation plans, or special accommodation requests..."
                className="w-full px-4 py-3 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors resize-none"
                rows={5}
                maxLength={1000}
              />
              <p className="text-xs text-muted-foreground mt-1">
                {additionalNotes.length}/1000 characters
              </p>
            </div>

            {/* Submit */}
            <div className="flex justify-end gap-4">
              <Button type="submit" disabled={loading} className="px-8 py-3">
                {loading ? "Submitting..." : "Submit Preferences"}
              </Button>
            </div>
          </form>
        </div>
      </div>
    </>
  );
};

/**
 * Reusable Select Field Component
 */
interface SelectFieldProps {
  label: string;
  value: string | null;
  onChange: (value: string | null) => void;
  options: RotationOption[];
  required?: boolean;
}

const SelectField: React.FC<SelectFieldProps> = ({
  label,
  value,
  onChange,
  options,
  required = false,
}) => {
  const selectedOption = options.find((opt) => opt.value === value);

  return (
    <div>
      <label className="block text-sm font-semibold text-foreground mb-2">
        {label}
      </label>
      <div className="relative">
        <select
          value={value ?? ""}
          onChange={(e) => onChange(e.target.value || null)}
          required={required}
          className="w-full px-4 py-3 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors appearance-none cursor-pointer"
          style={
            selectedOption
              ? {
                  borderLeft: `4px solid ${selectedOption.color}`,
                  paddingLeft: "12px",
                }
              : undefined
          }
        >
          <option value="">Select</option>
          {options.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
        <div className="absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none">
          <svg
            className="h-5 w-5 text-muted-foreground"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fillRule="evenodd"
              d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z"
              clipRule="evenodd"
            />
          </svg>
        </div>
      </div>
    </div>
  );
};

export default RotationForm;
