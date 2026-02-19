"use client";

import { config } from "../../../config";
import React, { useState, useEffect } from "react";

import { useRouter } from "next/navigation";
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

/**
 * Color scheme for rotation options based on the prototype:
 * - Intp Psy: Purple (#8b5cf6)
 * - Consult: Orange (#f97316)
 * - Addiction: Teal (#14b8a6)
 * - VA: Light Blue (#60a5fa)
 * - TMS: Lime (#84cc16)
 * - NFETC: Yellow (#eab308)
 * - IOP: Green (#22c55e)
 * - Comm: Blue (#3b82f6)
 * - HPC: Brown (#92400e)
 * - Forensic: Red (#ef4444)
 * - CLC: Pink (#ec4899)
 */

// Rotation options with their corresponding colors
const rotationOptions = [
  { value: "Intp Psy", label: "Intp Psy", color: "#8b5cf6" },
  { value: "Consult", label: "Consult", color: "#f97316" },
  { value: "VA", label: "VA", color: "#60a5fa" },
  { value: "TMS", label: "TMS", color: "#84cc16" },
  { value: "NFETC", label: "NFETC", color: "#eab308" },
  { value: "IOP", label: "IOP", color: "#22c55e" },
  { value: "Comm", label: "Comm", color: "#3b82f6" },
  { value: "HPC", label: "HPC", color: "#92400e" },
  { value: "Addiction", label: "Addiction", color: "#14b8a6" },
  { value: "Forensic", label: "Forensic", color: "#ef4444" },
  { value: "CLC", label: "CLC", color: "#ec4899" },
];

interface PGY3RotationFormProps {
  userId: string;
  userPGY: number;
}

/**
 * PGY-4 Rotation Request Form Component (Frontend Only)
 * 
 * This component displays a rotation preference form for PGY-3 residents.
 * Features:
 * - 8 priority selections (at least 4 required)
 * - 3 optional alternative selections
 * - 3 optional avoidance selections
 * - Additional notes field
 * - Validation to prevent duplicate selections in priority fields
 * - Frontend-only (no backend integration yet)
 */
const PGY3RotationForm: React.FC<PGY3RotationFormProps> = ({ userId, userPGY }) => {
  // Form state
  const [firstPriority, setFirstPriority] = useState("");
  const [secondPriority, setSecondPriority] = useState("");
  const [thirdPriority, setThirdPriority] = useState("");
  const [fourthPriority, setFourthPriority] = useState("");
  const [fifthPriority, setFifthPriority] = useState("");
  const [sixthPriority, setSixthPriority] = useState("");
  const [seventhPriority, setSeventhPriority] = useState("");
  const [eighthPriority, setEighthPriority] = useState("");
  const [alternative1, setAlternative1] = useState("");
  const [alternative2, setAlternative2] = useState("");
  const [alternative3, setAlternative3] = useState("");
  const [avoid1, setAvoid] = useState("");
  const [avoid2, setAvoid2] = useState("");
  const [avoid3, setAvoid3] = useState("");
  const [additionalNotes, setAdditionalNotes] = useState("");

  // UI state
  const [loading, setLoading] = useState(false);
  const [showSuccessDialog, setShowSuccessDialog] = useState(false);
  const router = useRouter();

  // Deadline configuration (March 15, 2026 at 11:59 PM EST)
  const deadline = new Date("2026-03-15T23:59:00-05:00");
  const isDeadlinePassed = new Date() > deadline;

  useEffect(() => {
  
  // Get the existing residents request
  const loadExistingRequest = async () => {
    try {
      const res = await fetch(
        `${config.apiUrl}/api/PGY4RotationRequests/${userId}`
      );

      // If no prior submission
      if (!res.ok) return;

      const data = await res.json();

      setFirstPriority(data.firstPriority ?? "");
      setSecondPriority(data.secondPriority ?? "");
      setThirdPriority(data.thirdPriority ?? "");
      setFourthPriority(data.fourthPriority ?? "");
      setFifthPriority(data.fifthPriority ?? "");
      setSixthPriority(data.sixthPriority ?? "");
      setSeventhPriority(data.seventhPriority ?? "");
      setEighthPriority(data.eighthPriority ?? "");
      setAlternative1(data.alternative1 ?? "");
      setAlternative2(data.alternative2 ?? "");
      setAlternative3(data.alternative3 ?? "");
      setAvoid(data.avoid1 ?? "");
      setAvoid2(data.avoid2 ?? "");
      setAvoid3(data.avoid3 ?? "");
      setAdditionalNotes(data.additionalNotes ?? "");
    } catch {
      toast({
        variant: "destructive",
        title: "Error",
        description: "Could not load existing rotation request.",
      });
    }
  };

  loadExistingRequest();
}, [userId]);


  /**
   * Validates that all priority selections are unique (alternatives and avoid can duplicate)
   */
  const validateNoDuplicates = (): boolean => {
    const prioritySelections = [
      firstPriority,
      secondPriority,
      thirdPriority,
      fourthPriority,
      fifthPriority,
      sixthPriority,
      seventhPriority,
      eighthPriority,
    ].filter(s => s !== "");

    // Only check that priority fields don't have duplicates among themselves
    const uniquePriorities = new Set(prioritySelections);
    return uniquePriorities.size === prioritySelections.length;
  };

  /**
   * Validates that at least 4 priority fields are filled
   */
  const validateAllFieldsFilled = (): boolean => {
    const filledPriorities = [
      firstPriority,
      secondPriority,
      thirdPriority,
      fourthPriority,
      fifthPriority,
      sixthPriority,
      seventhPriority,
      eighthPriority,
    ].filter(p => p !== "").length;

    return filledPriorities >= 4;
  };

  /**
   * Handles form submission (frontend only - just validates and shows success) - edit: backend included
   */
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validate at least 4 priorities are filled
    if (!validateAllFieldsFilled()) {
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
        description: "Each rotation can only be selected once in the priority fields.",
      });
      return;
    }

    setLoading(true);

    // Try to upload users request after the validation above
    try {
      const res = await fetch(
        `${config.apiUrl}/api/PGY4RotationRequests`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            residentId: userId,
            firstPriority,
            secondPriority,
            thirdPriority,
            fourthPriority,
            fifthPriority,
            sixthPriority,
            seventhPriority,
            eighthPriority,
            alternative1,
            alternative2,
            alternative3,
            avoid1, // Changed this to avoid1 instead of avoid above to allow shorthand. Didn't seem to break anything. Just a note.
            avoid2,
            avoid3,
            additionalNotes,
          }),
        }
      );

      if (!res.ok) {
        throw new Error("Failed to submit");
      }

      // Show if successful
      setShowSuccessDialog(true);
    } catch {
      toast({
        variant: "destructive",
        title: "Submission failed",
        description: "Unable to save rotation preferences.",
      });
    } finally {
      setLoading(false);
    }

    // Log the data to console for demonstration (its also posted but for convenience for now), remove this later
    console.log("Rotation Preferences:", {
      userId,
      firstPriority,
      secondPriority,
      thirdPriority,
      fourthPriority,
      fifthPriority,
      sixthPriority,
      seventhPriority,
      eighthPriority,
      alternative1,
      alternative2,
      alternative3,
      avoid1,
      avoid2,
      avoid3,
      additionalNotes,
    });
    
  };

  /**
   * Gets available options for priority fields, excluding already selected priorities
   */
  const getAvailablePriorityOptions = (currentValue: string): typeof rotationOptions => {
    const selectedPriorities = [
      firstPriority,
      secondPriority,
      thirdPriority,
      fourthPriority,
      fifthPriority,
      sixthPriority,
      seventhPriority,
      eighthPriority,
    ].filter(v => v && v !== currentValue);

    return rotationOptions.filter(option => !selectedPriorities.includes(option.value));
  };

  /**
   * Gets all rotation options for alternatives and avoid (no filtering)
   */
  const getAllOptions = (): typeof rotationOptions => {
    return rotationOptions;
  };

  // Check if user is not PGY-3
  if (userPGY !== 3) {
    return (
      <div className="max-w-4xl mx-auto">
        <div className="bg-card rounded-xl shadow-lg border border-border p-8 text-center">
          <ClipboardList className="h-16 w-16 mx-auto mb-4 text-muted-foreground" />
          <h2 className="text-2xl font-bold text-foreground mb-2">Access Denied</h2>
          <p className="text-muted-foreground">
            This page is only accessible to PGY-3 residents.
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
          <h2 className="text-2xl font-bold text-foreground mb-2">Submission Deadline Passed</h2>
          <p className="text-muted-foreground">
            The deadline for submitting PGY-4 rotation preferences was March 15, 2026 at 11:59 PM EST.
            The form is now closed.
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
                router.push("/dashboard");
              }}
              className="w-full sm:w-auto"
            >
              🏠 Home
            </Button>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      <div className="max-w-4xl mx-auto">
        <div className="bg-card rounded-xl shadow-lg border border-border p-8">
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-3">
            <ClipboardList className="h-8 w-8 text-primary" />
            <h1 className="text-3xl font-bold text-foreground">Rotation Request Form</h1>
          </div>
        </div>

        <div className="mb-6">
          <p className="text-muted-foreground mb-2">
            Submit preferred shifts for 4th year residency. Please submit by deadline, failure to do so may result in random rotations.
          </p>
          <p className="text-muted-foreground mb-4">
            You may return at anytime prior to the deadline to make changes.
          </p>
          <p className="text-red-600 dark:text-red-400 font-semibold">
            Due: {deadline.toLocaleDateString('en-US', { day: 'numeric', month: 'long', year: 'numeric' })} at {deadline.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true, timeZoneName: 'short' })}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-8">
          {/* Rotation Preferences */}
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
                label="Fifth Priority (optional)"
                value={fifthPriority}
                onChange={setFifthPriority}
                options={getAvailablePriorityOptions(fifthPriority)}
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
                label="Sixth Priority (optional)"
                value={sixthPriority}
                onChange={setSixthPriority}
                options={getAvailablePriorityOptions(sixthPriority)}
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
                label="Seventh Priority (optional)"
                value={seventhPriority}
                onChange={setSeventhPriority}
                options={getAvailablePriorityOptions(seventhPriority)}
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
                label="Eighth Priority (optional)"
                value={eighthPriority}
                onChange={setEighthPriority}
                options={getAvailablePriorityOptions(eighthPriority)}
                required
              />
            </div>
          </div>

          {/* Alternatives */}
          <div>
            <h2 className="text-xl font-bold text-foreground mb-4">Alternatives <span className="text-muted-foreground text-sm">(optional)</span></h2>
            
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
            <h2 className="text-xl font-bold text-foreground mb-4">Avoid <span className="text-muted-foreground text-sm">(optional)</span></h2>
            
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <SelectField
                label="Avoid 1"
                value={avoid1}
                onChange={setAvoid}
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
            <h2 className="text-xl font-bold text-foreground mb-4">Additional Notes <span className="text-muted-foreground text-sm">(optional)</span></h2>
            
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

          {/* Submit Button */}
          <div className="flex justify-end gap-4">
            <Button
              type="submit"
              disabled={loading}
              className="px-8 py-3"
            >
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
  value: string;
  onChange: (value: string) => void;
  options: typeof rotationOptions;
  required?: boolean;
}

const SelectField: React.FC<SelectFieldProps> = ({ label, value, onChange, options, required = false }) => {
  const selectedOption = rotationOptions.find(opt => opt.value === value);
  
  return (
    <div>
      <label className="block text-sm font-semibold text-foreground mb-2">
        {label}
      </label>
      <div className="relative">
        <select
          value={value}
          onChange={(e) => onChange(e.target.value)}
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
          <svg className="h-5 w-5 text-muted-foreground" viewBox="0 0 20 20" fill="currentColor">
            <path fillRule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clipRule="evenodd" />
          </svg>
        </div>
      </div>
    </div>
  );
};

export default PGY3RotationForm;
