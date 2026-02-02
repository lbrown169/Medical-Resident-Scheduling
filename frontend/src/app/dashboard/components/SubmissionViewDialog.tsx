"use client";

import React, { useState, useEffect } from "react";
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogFooter,
  AlertDialogCancel,
} from "@/components/ui/alert-dialog";
import { config } from "@/config";
import { ChevronDown } from "lucide-react";

// Types
interface ResidentPreference {
  residentId: string;
  residentName: string;
  priorities: string[];
  alternatives: string[];
  avoids: string[];
  additionalNotes: string;
}

interface SubmissionViewDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  residentId: string;
  residentName: string;
}

// Available rotation options for dropdowns
const ROTATION_OPTIONS = [
  "Admin",
  "Inpatient Psychiatry",
  "Consult",
  "IOP",
  "Forensic",
  "Community",
  "Addiction",
  "TMS",
  "NFETC",
  "HPC",
  "VA",
  "CLC",
];

export const SubmissionViewDialog: React.FC<SubmissionViewDialogProps> = ({
  open,
  onOpenChange,
  residentId,
  residentName,
}) => {
  const [preferences, setPreferences] = useState<ResidentPreference>({
    residentId: residentId,
    residentName: residentName,
    priorities: ["", "", "", "", "", "", "", ""],
    alternatives: ["", "", ""],
    avoids: ["", "", ""],
    additionalNotes: "",
  });
  const [isLoading, setIsLoading] = useState(false);

  // Fetch preferences when dialog opens
  useEffect(() => {
    if (open && residentId) {
      fetchPreferences();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, residentId]);

  const fetchPreferences = async () => {
    setIsLoading(true);
    try {
      // TODO: Update this endpoint to match your actual API
      const response = await fetch(
        `${config.apiUrl}/api/residents/preferences/${residentId}`
      );

      if (!response.ok) {
        throw new Error("Failed to fetch preferences");
      }

      const data = await response.json();
      setPreferences({
        residentId: data.residentId || residentId,
        residentName: data.residentName || residentName,
        priorities: data.priorities || ["", "", "", "", "", "", "", ""],
        alternatives: data.alternatives || ["", "", ""],
        avoids: data.avoids || ["", "", ""],
        additionalNotes: data.additionalNotes || "",
      });
    } catch (err) {
      console.error("Error fetching preferences:", err);
      // Mock data for development - remove in production
      setPreferences({
        residentId: residentId,
        residentName: residentName,
        priorities: ["Inpatient Psychiatry", "Consult", "IOP", "Forensic", "", "", "", ""],
        alternatives: ["VA", "TMS", ""],
        avoids: ["Addiction", "", ""],
        additionalNotes: "I would prefer to have my Inpatient rotation in the fall semester if possible.",
      });
    } finally {
      setIsLoading(false);
    }
  };

  // Field card component matching Figma exactly
const FieldCard = ({ value, label }: { value: string; label: string }) => (
  <div className="flex items-center border border-gray-200 dark:border-gray-700 rounded-xl overflow-hidden bg-white dark:bg-neutral-900">
    <label className="text-sm font-medium text-gray-800 dark:text-gray-200 px-5 py-4 w-40 shrink-0">
      {label}
    </label>
    <div className="flex-1 px-2 py-2">
      <div className="flex items-center bg-gray-100 dark:bg-neutral-800 border border-gray-300 dark:border-gray-600 rounded-lg px-4 py-2">
        <select
          className="flex-1 text-sm bg-transparent focus:outline-none text-gray-600 dark:text-gray-400 disabled:cursor-not-allowed appearance-none"
          value={value}
          disabled
        >
          <option value="">Select</option>
          {ROTATION_OPTIONS.map((opt) => (
            <option key={opt} value={opt}>
              {opt}
            </option>
          ))}
        </select>
        <ChevronDown className="w-5 h-5 text-gray-400 shrink-0" />
      </div>
    </div>
  </div>
);

  // Section header with underline
  const SectionHeader = ({ title }: { title: string }) => (
    <h3 className="text-lg font-bold text-gray-900 dark:text-gray-100 mb-4 pb-2 border-b border-gray-200 dark:border-gray-700">
      {title}
    </h3>
  );

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent className="max-w-[900px] w-[95vw] max-h-[90vh] overflow-hidden flex flex-col bg-white dark:bg-neutral-900 p-0 rounded-xl">
        {/* Header */}
        <AlertDialogHeader className="px-8 py-5 border-b border-gray-200 dark:border-gray-700">
          <AlertDialogTitle className="flex items-center gap-3 text-xl font-semibold">
            <span>Submission Form</span>
            <span className="text-base font-normal text-gray-500">{residentName}</span>
          </AlertDialogTitle>
        </AlertDialogHeader>

        {/* Content */}
        <div className="flex-1 overflow-y-auto px-8 py-6">
          {isLoading ? (
            <div className="text-center py-10 text-gray-500">Loading preferences...</div>
          ) : (
            <>
              {/* Priorities Section */}
              <div className="grid grid-cols-2 gap-x-10 gap-y-4">
                {/* Left Column - Priorities 1-4 */}
                <div className="space-y-4">
                  <FieldCard value={preferences.priorities[0]} label="First Priority" />
                  <FieldCard value={preferences.priorities[1]} label="Second Priority" />
                  <FieldCard value={preferences.priorities[2]} label="Third Priority" />
                  <FieldCard value={preferences.priorities[3]} label="Fourth Priority" />
                </div>

                {/* Right Column - Priorities 5-8 */}
                <div className="space-y-4">
                  <FieldCard value={preferences.priorities[4]} label="Fifth Priority" />
                  <FieldCard value={preferences.priorities[5]} label="Sixth Priority" />
                  <FieldCard value={preferences.priorities[6]} label="Seventh Priority" />
                  <FieldCard value={preferences.priorities[7]} label="Eighth Priority" />
                </div>
              </div>

              {/* Alternatives Section */}
              <div className="mt-8">
                <SectionHeader title="Alternatives" />
                <div className="grid grid-cols-2 gap-x-10 gap-y-4">
                  {/* Left Column */}
                  <div className="space-y-4">
                    <FieldCard value={preferences.alternatives[0]} label="Alternative 1" />
                    <FieldCard value={preferences.alternatives[2]} label="Alternative 3" />
                  </div>
                  {/* Right Column */}
                  <div className="space-y-4">
                    <FieldCard value={preferences.alternatives[1]} label="Alternative 2" />
                  </div>
                </div>
              </div>

              {/* Avoid Section */}
              <div className="mt-8">
                <SectionHeader title="Avoid" />
                <div className="grid grid-cols-2 gap-x-10 gap-y-4">
                  {/* Left Column */}
                  <div className="space-y-4">
                    <FieldCard value={preferences.avoids[0]} label="Avoid 1" />
                    <FieldCard value={preferences.avoids[2]} label="Avoid 3" />
                  </div>
                  {/* Right Column */}
                  <div className="space-y-4">
                    <FieldCard value={preferences.avoids[1]} label="Avoid 2" />
                  </div>
                </div>
              </div>

              {/* Additional Notes */}
              <div className="mt-8">
                <SectionHeader title="Additional Notes" />
                <textarea
                  className="w-full px-5 py-4 text-sm bg-gray-100 dark:bg-neutral-800 border border-gray-200 dark:border-gray-700 rounded-xl resize-none text-gray-600 dark:text-gray-400 disabled:cursor-not-allowed focus:outline-none"
                  rows={4}
                  value={preferences.additionalNotes}
                  placeholder="No additional notes provided"
                  disabled
                />
              </div>
            </>
          )}
        </div>

        {/* Footer */}
        <AlertDialogFooter className="px-8 py-5 border-t border-gray-200 dark:border-gray-700 flex justify-end">
          <AlertDialogCancel className="px-6 py-2 bg-white dark:bg-neutral-800 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-neutral-700 text-sm font-medium">
            Close
          </AlertDialogCancel>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
};

export default SubmissionViewDialog;
