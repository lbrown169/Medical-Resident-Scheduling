"use client";

import { config } from "../../../config";
import React, { useState, useEffect, useCallback } from "react";

import { Card } from "../../../components/ui/card";
import { Button } from "../../../components/ui/button";
import { ConfirmDialog } from "../../../components/ui/confirm-dialog";
import { toast } from "../../../lib/use-toast";
import { SubmissionViewDialog } from "./SubmissionViewDialog";
import { RotationScheduleTable } from "./RotationScheduleTable";

import {
  CalendarRange,
  Users,
  UserX,
  CalendarClock,
  Trash2,
  Save,
  Download,
  X,
  ClipboardList,
  ChevronDown,
  ChevronUp,
  AlertTriangle,
  CheckCircle2,
  Loader2,
} from "lucide-react";
import RotationForm from "./RotationForm";

// Individual responses
interface RotationPrefResponse {
  rotationPrefRequestId: string;

  resident: {
    resident_id: string;
    first_name: string;
    last_name: string;
    graduate_yr?: number;
    email?: string;
    phone_num?: string;
  };

  priorities: {
    rotationTypeId: string;
    rotationName: string;
  }[];

  alternatives: {
    rotationTypeId: string;
    rotationName: string;
  }[];

  avoids: {
    rotationTypeId: string;
    rotationName: string;
  }[];

  additionalNotes?: string;
}

// Check how many submissions exist
interface RotationPrefRequestsListResponse {
  count: number;
  rotationPrefRequests: RotationPrefResponse[];
}

// Schedule api types
interface RotationTypeResponse {
  rotationTypeId: string;
  rotationName: string;
}

interface RotationResponse {
  rotationId: string;
  scheduleId?: string;
  month: string;
  academicMonthIndex: number; // July is 0, so on and so forth
  pgyYear: number;
  rotationType: RotationTypeResponse;
}

interface ResidentScheduleResponse {
  resident: {
    resident_id: string;
    first_name: string;
    last_name: string;
  };
  rotations: RotationResponse[];
}

interface Pgy4RotationScheduleResponse {
  pgy4RotationScheduleId: string;
  residentCount: number;
  seed: number;
  year: number;
  isPublished: boolean;
  schedule: ResidentScheduleResponse[];
}

interface Pgy4RotationSchedulesListResponse {
  count: number;
  schedules: Pgy4RotationScheduleResponse[];
}

interface UnsubmittedResidentsResponse {
  message: string;
  unsubmittedResidents: { first_name: string; last_name: string }[];
}

interface RotationTypeApiResponse {
  rotationTypeId: string;
  rotationName: string;
  doesLongCall: boolean;
  doesShortCall: boolean;
  doesTrainingLongCall: boolean;
  doesTrainingShortCall: boolean;
  isChiefRotation: boolean;
  pgyYears: number[];
}

interface RotationTypesListResponse {
  count: number;
  rotationTypes: RotationTypeApiResponse[];
}

// Override types
interface Pgy4RotationScheduleOverrideSummary {
  resident: {
    resident_id: string;
    first_name: string;
    last_name: string;
  };
  overrideRotation: {
    rotationTypeId: string;
    rotationName: string;
  };
  academicMonthIndex: number;
}

interface Pgy4RotationScheduleOverrideListResponse {
  count: number;
  overrides: Pgy4RotationScheduleOverrideSummary[];
}

// Constraint Violation types

interface ConstraintErrorResponse {
  message: string;
  resident?: {
    resident_id: string;
    first_name: string;
    last_name: string;
  };
  academicMonthIndex?: number;
}

interface ConstraintViolationResponse {
  constraintViolated: string;
  errors: ConstraintErrorResponse[];
}

interface ConstraintViolationsListResponse {
  schedule: Pgy4RotationScheduleResponse;
  violations: ConstraintViolationResponse[];
}

const ACADEMIC_MONTHS = [
  "July",
  "August",
  "September",
  "October",
  "November",
  "December",
  "January",
  "February",
  "March",
  "April",
  "May",
  "June",
];

// Passed into RotationScheduleTable as colorMap
const rotationColorMap: Record<string, string> = {
  "Inpatient Psy": "#8b5cf6",
  "Psy Consults": "#f97316",
  VA: "#60a5fa",
  TMS: "#84cc16",
  NFETC: "#eab308",
  IOP: "#22c55e",
  "Community Psy": "#3b82f6",
  HPC: "#92400e",
  Addiction: "#14b8a6",
  Forensic: "#ef4444",
  CLC: "#ec4899",
  Chief: "#4b5563",
};

// Passed into RotationScheduleTable as displayNames
const rotationDisplayNames: Record<string, string> = {
  "Inpatient Psy": "Inpt Psy",
  "Psy Consults": "Consult",
  "Community Psy": "Comm",
};

interface PGY4RotationScheduleProps {
  residents: {
    id: string;
    name: string;
    email: string;
    pgyLevel: number | string;
    chiefType: string;
  }[];
}

function Modal({
  open,
  onClose,
  title,
  children,
}: {
  open: boolean;
  onClose: () => void;
  title: string;
  children: React.ReactNode;
}) {
  if (!open) return null;
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div className="bg-card p-8 rounded-xl shadow-lg max-w-2xl w-full relative">
        <button
          onClick={onClose}
          className="absolute top-4 right-4 text-xl font-bold cursor-pointer"
        >
          &times;
        </button>
        <h2 className="text-2xl font-bold mb-4">{title}</h2>
        <div className="overflow-y-auto max-h-[60vh]">{children}</div>
      </div>
    </div>
  );
}

// Constraint Violations Panel
function ConstraintViolationsPanel({
  violations,
  loading,
}: {
  violations: ConstraintViolationResponse[];
  loading: boolean;
}) {
  const [expanded, setExpanded] = useState(false);

  const totalErrors = violations.reduce((sum, v) => sum + v.errors.length, 0);
  const hasViolations = violations.length > 0;

  // If there are issues we auto open this panel
  useEffect(() => {
    if (hasViolations) setExpanded(true);
  }, [hasViolations]);

  if (loading) {
    return (
      <div className="flex items-center gap-2 px-4 py-3 rounded-lg border border-border bg-background text-sm text-muted-foreground">
        <Loader2 className="h-4 w-4 animate-spin shrink-0" />
        <span>Checking constraint violations…</span>
      </div>
    );
  }

  if (!hasViolations) {
    return (
      <div className="flex items-center gap-2 px-4 py-3 rounded-lg border border-green-200 dark:border-green-800 bg-green-50 dark:bg-green-950/30 text-sm text-green-700 dark:text-green-400">
        <CheckCircle2 className="h-4 w-4 shrink-0" />
        <span className="font-medium">No constraint violations detected</span>
      </div>
    );
  }

  return (
    <div className="rounded-lg border border-red-200 dark:border-red-800 bg-red-50 dark:bg-red-950/20 overflow-hidden">
      {/* Header (expands and collapses so it isn't too annoying if they want to keep the violations present) */}
      <button
        onClick={() => setExpanded((prev) => !prev)}
        className="w-full flex items-center justify-between px-4 py-3 text-left hover:bg-red-100/50 dark:hover:bg-red-900/20 transition-colors cursor-pointer"
      >
        <div className="flex items-center gap-2">
          <AlertTriangle className="h-4 w-4 text-red-600 dark:text-red-400 shrink-0" />
          <span className="text-sm font-semibold text-red-700 dark:text-red-400">
            {violations.length} constraint violation
            {violations.length !== 1 ? "s" : ""}{" "}
            <span className="font-normal text-red-500 dark:text-red-500">
              ({totalErrors} error{totalErrors !== 1 ? "s" : ""} total)
            </span>
          </span>
        </div>
        {expanded ? (
          <ChevronUp className="h-4 w-4 text-red-500 shrink-0" />
        ) : (
          <ChevronDown className="h-4 w-4 text-red-500 shrink-0" />
        )}
      </button>

      {/* Expanded detail */}
      {expanded && (
        <div className="border-t border-red-200 dark:border-red-800 divide-y divide-red-100 dark:divide-red-900 max-h-72 overflow-y-auto">
          {violations.map((violation, vi) => (
            <div key={vi} className="px-4 py-3">
              {/* Constraint name */}
              <p className="text-xs font-bold uppercase tracking-wide text-red-600 dark:text-red-400 mb-2">
                {violation.constraintViolated}
              </p>
              {/* Individual errors */}
              <ul className="space-y-1.5">
                {violation.errors.map((err, ei) => (
                  <li
                    key={ei}
                    className="flex flex-col sm:flex-row sm:items-center gap-1 text-sm text-red-700 dark:text-red-300"
                  >
                    <span className="shrink-0 inline-block w-1.5 h-1.5 rounded-full bg-red-400 mt-1 sm:mt-0 self-start sm:self-auto" />
                    <span className="flex-1">{err.message}</span>
                    {/* Resident badge */}
                    {err.resident && (
                      <span className="text-xs bg-red-100 dark:bg-red-900/40 text-red-600 dark:text-red-400 rounded px-1.5 py-0.5 whitespace-nowrap shrink-0">
                        {err.resident.first_name} {err.resident.last_name}
                      </span>
                    )}
                    {/* Month badge */}
                    {err.academicMonthIndex != null && (
                      <span className="text-xs bg-red-100 dark:bg-red-900/40 text-red-600 dark:text-red-400 rounded px-1.5 py-0.5 whitespace-nowrap shrink-0">
                        {ACADEMIC_MONTHS[err.academicMonthIndex] ??
                          `Month ${err.academicMonthIndex}`}
                      </span>
                    )}
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

// Pending Changes Panel
function PendingChangesPanel({
  overrides,
  onRevertOne,
}: {
  overrides: Pgy4RotationScheduleOverrideSummary[];
  onRevertOne: (
    residentId: string,
    academicMonthIndex: number,
  ) => Promise<void>;
}) {
  const [expanded, setExpanded] = useState(true);
  const [revertingKey, setRevertingKey] = useState<string | null>(null);

  if (overrides.length === 0) return null;

  const handleRevert = async (residentId: string, monthIndex: number) => {
    const key = `${residentId}-${monthIndex}`;
    setRevertingKey(key);
    try {
      await onRevertOne(residentId, monthIndex);
    } finally {
      setRevertingKey(null);
    }
  };

  return (
    <div className="rounded-lg border border-yellow-200 dark:border-yellow-700 bg-yellow-50 dark:bg-yellow-950/20 overflow-hidden">
      <button
        onClick={() => setExpanded((prev) => !prev)}
        className="w-full flex items-center justify-between px-4 py-3 text-left hover:bg-yellow-100/50 dark:hover:bg-yellow-900/20 transition-colors cursor-pointer"
      >
        <div className="flex items-center gap-2">
          <Save className="h-4 w-4 text-yellow-600 dark:text-yellow-400 shrink-0" />
          <span className="text-sm font-semibold text-yellow-700 dark:text-yellow-400">
            {overrides.length} pending change{overrides.length !== 1 ? "s" : ""}{" "}
            <span className="font-normal text-yellow-500">
              — not yet applied
            </span>
          </span>
        </div>
        {expanded ? (
          <ChevronUp className="h-4 w-4 text-yellow-500 shrink-0" />
        ) : (
          <ChevronDown className="h-4 w-4 text-yellow-500 shrink-0" />
        )}
      </button>

      {expanded && (
        <div className="border-t border-yellow-200 dark:border-yellow-700 divide-y divide-yellow-100 dark:divide-yellow-900 max-h-60 overflow-y-auto">
          {overrides.map((o) => {
            const key = `${o.resident.resident_id}-${o.academicMonthIndex}`;
            const isReverting = revertingKey === key;
            return (
              <div
                key={key}
                className="flex items-center justify-between px-4 py-2.5 gap-3"
              >
                <div className="flex items-center gap-2 text-sm text-yellow-800 dark:text-yellow-300 flex-1 min-w-0">
                  <span className="font-medium shrink-0">
                    {o.resident.first_name} {o.resident.last_name}
                  </span>
                  <span className="text-yellow-500 shrink-0">·</span>
                  <span className="shrink-0 text-xs bg-yellow-100 dark:bg-yellow-900/40 text-yellow-700 dark:text-yellow-400 rounded px-1.5 py-0.5">
                    {ACADEMIC_MONTHS[o.academicMonthIndex] ??
                      `Month ${o.academicMonthIndex}`}
                  </span>
                  <span className="text-yellow-500 shrink-0">→</span>
                  <span className="font-medium truncate">
                    {o.overrideRotation.rotationName}
                  </span>
                </div>
                <button
                  onClick={() =>
                    handleRevert(o.resident.resident_id, o.academicMonthIndex)
                  }
                  disabled={isReverting}
                  className="shrink-0 flex items-center gap-1 text-xs px-2 py-1 rounded border border-yellow-400 dark:border-yellow-600 text-yellow-700 dark:text-yellow-400 hover:bg-yellow-200 dark:hover:bg-yellow-800/40 disabled:opacity-50 transition-colors cursor-pointer"
                >
                  {isReverting ? (
                    <Loader2 className="h-3 w-3 animate-spin" />
                  ) : (
                    <X className="h-3 w-3" />
                  )}
                  Revert
                </button>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}

const PGY4RotationSchedulePage: React.FC<PGY4RotationScheduleProps> = ({
  residents,
}) => {
  const [activeTab, setActiveTab] = useState<
    "schedule" | "submissions" | "configure"
  >("schedule");
  // Submission window state
  const [windowAvailableDate, setWindowAvailableDate] = useState<string>("");
  const [windowDueDate, setWindowDueDate] = useState<string>("");
  const [savingWindow, setSavingWindow] = useState(false);
  const currentYear = new Date().getFullYear();
  const selectedYear = currentYear;

  let deadline = null;
  if (windowDueDate) {
    deadline = new Date(windowDueDate);
  }

  // Schedule state
  const [schedules, setSchedules] = useState<Pgy4RotationScheduleResponse[]>(
    [],
  );
  const [selectedScheduleId, setSelectedScheduleId] = useState<string>("");
  const [loadingSchedules, setLoadingSchedules] = useState(false);
  const [generating, setGenerating] = useState(false);
  const [scheduleError, setScheduleError] = useState<string | null>(null);
  const [rotationTypeNames, setRotationTypeNames] = useState<
    { id: string; name: string }[]
  >([]);

  const selectedSchedule =
    schedules.find((s) => s.pgy4RotationScheduleId === selectedScheduleId) ??
    null;

  // Pending changes state

  // Whether there are any staged overrides for this schedule
  const [hasPendingOverrides, setHasPendingOverrides] = useState(false);
  const [pendingOverrides, setPendingOverrides] = useState<
    Pgy4RotationScheduleOverrideSummary[]
  >([]);
  const [applyingOverrides, setApplyingOverrides] = useState(false);
  const [discardingOverrides, setDiscardingOverrides] = useState(false);

  // Constraint violations state

  const [constraintViolations, setConstraintViolations] = useState<
    ConstraintViolationResponse[]
  >([]);
  const [loadingViolations, setLoadingViolations] = useState(false);

  const fetchConstraintErrors = useCallback(async (scheduleId: string) => {
    if (!scheduleId) return;
    try {
      setLoadingViolations(true);
      const res = await fetch(
        `${config.apiUrl}/api/pgy4-rotation-schedule/${scheduleId}/constraint-errors`,
      );
      if (!res.ok) {
        setConstraintViolations([]);
        return;
      }
      const data: ConstraintViolationsListResponse = await res.json();
      setConstraintViolations(data.violations ?? []);
    } catch (err) {
      console.error("Failed to fetch constraint errors:", err);
      setConstraintViolations([]);
    } finally {
      setLoadingViolations(false);
    }
  }, []);

  // Check for violations when the schedule changes
  const fetchScheduleWithOverrides = useCallback(async (scheduleId: string) => {
    const res = await fetch(
      `${config.apiUrl}/api/pgy4-rotation-schedule/${scheduleId}?applyOverrides=true`,
    );
    if (!res.ok) return;
    const updated: Pgy4RotationScheduleResponse = await res.json();
    setSchedules((prev) =>
      prev.map((s) =>
        s.pgy4RotationScheduleId === scheduleId
          ? { ...s, schedule: updated.schedule }
          : s,
      ),
    );
  }, []);

  /**
   * Get the raw schedule without overrides on a discard
   */
  const fetchScheduleWithoutOverrides = useCallback(
    async (scheduleId: string) => {
      const res = await fetch(
        `${config.apiUrl}/api/pgy4-rotation-schedule/${scheduleId}`,
      );
      if (!res.ok) return;
      const updated: Pgy4RotationScheduleResponse = await res.json();
      setSchedules((prev) =>
        prev.map((s) =>
          s.pgy4RotationScheduleId === scheduleId
            ? { ...s, schedule: updated.schedule }
            : s,
        ),
      );
    },
    [],
  );

  /**
   * Sync with previous sessions overrides
   */
  const syncOverrideState = useCallback(
    async (scheduleId: string) => {
      if (!scheduleId) {
        setHasPendingOverrides(false);
        return;
      }
      try {
        const res = await fetch(
          `${config.apiUrl}/api/pgy4-rotation-schedule-override/${scheduleId}`,
        );
        if (!res.ok) {
          setHasPendingOverrides(false);
          return;
        }
        const data: Pgy4RotationScheduleOverrideListResponse = await res.json();
        const hasAny = (data.count ?? 0) > 0;
        setHasPendingOverrides(hasAny);
        setPendingOverrides(data.overrides ?? []);
        if (hasAny) await fetchScheduleWithOverrides(scheduleId);
      } catch (err) {
        console.error("Failed to sync override state:", err);
        setHasPendingOverrides(false);
      }
    },
    [fetchScheduleWithOverrides],
  );

  // Check violation + override count when schedule changes
  useEffect(() => {
    if (selectedScheduleId) {
      syncOverrideState(selectedScheduleId);
      fetchConstraintErrors(selectedScheduleId);
    } else {
      setConstraintViolations([]);
      setHasPendingOverrides(false);
    }
  }, [selectedScheduleId, syncOverrideState, fetchConstraintErrors]);

  /**
   * Commit staged overrides
   */
  const handleApplyOverrides = async () => {
    if (!selectedScheduleId || !hasPendingOverrides) return;
    try {
      setApplyingOverrides(true);
      const res = await fetch(
        `${config.apiUrl}/api/pgy4-rotation-schedule-override/${selectedScheduleId}/apply-overrides`,
        { method: "PUT" },
      );
      if (!res.ok) {
        toast({
          variant: "destructive",
          title: "Error",
          description: "Failed to apply changes.",
        });
        return;
      }
      // Response is the schedule with overrides baked in, update the visible schedule
      const updated: Pgy4RotationScheduleResponse = await res.json();
      setSchedules((prev) =>
        prev.map((s) =>
          s.pgy4RotationScheduleId === selectedScheduleId
            ? { ...s, schedule: updated.schedule }
            : s,
        ),
      );
      setHasPendingOverrides(false);
      setPendingOverrides([]);
      await fetchConstraintErrors(selectedScheduleId);
      toast({
        variant: "success",
        title: "Applied",
        description: "All changes have been saved.",
      });
    } catch (err) {
      console.error("Failed to apply overrides:", err);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to apply changes.",
      });
    } finally {
      setApplyingOverrides(false);
    }
  };

  /**
   * Wipe all staged overrides on a discard
   */
  const handleDiscardOverrides = async () => {
    if (!selectedScheduleId || !hasPendingOverrides) return;
    try {
      setDiscardingOverrides(true);
      const res = await fetch(
        `${config.apiUrl}/api/pgy4-rotation-schedule-override/${selectedScheduleId}/all`,
        { method: "DELETE" },
      );
      if (!res.ok) {
        toast({
          variant: "destructive",
          title: "Error",
          description: "Failed to discard changes.",
        });
        return;
      }
      await fetchScheduleWithoutOverrides(selectedScheduleId);
      setHasPendingOverrides(false);
      setPendingOverrides([]);
      await fetchConstraintErrors(selectedScheduleId);
      toast({
        variant: "success",
        title: "Discarded",
        description: "All pending changes discarded.",
      });
    } catch (err) {
      console.error("Failed to discard overrides:", err);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to discard changes.",
      });
    } finally {
      setDiscardingOverrides(false);
    }
  };

  /**
   * Revert a staged override (just one)
   * Lets admin revert changes individually
   */
  const handleRevertSingleOverride = async (
    residentId: string,
    academicMonthIndex: number,
  ) => {
    if (!selectedScheduleId) return;
    try {
      const res = await fetch(
        `${config.apiUrl}/api/pgy4-rotation-schedule-override/${selectedScheduleId}`,
        {
          method: "DELETE",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ residentId, academicMonthIndex }),
        },
      );
      if (!res.ok) {
        toast({
          variant: "destructive",
          title: "Error",
          description: "Failed to revert change.",
        });
        return;
      }
      // Sync override list and schedule preview
      await syncOverrideState(selectedScheduleId);
      await fetchConstraintErrors(selectedScheduleId);
    } catch {
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to revert change.",
      });
    }
  };

  const [showRotationFormModal, setShowRotationFormModal] = useState(false);
  const [submissions, setSubmissions] = useState<RotationPrefResponse[]>([]);
  const [loadingSubmissions, setLoadingSubmissions] = useState(false);
  const [unsubmittedResidents, setUnsubmittedResidents] = useState<
    { first_name: string; last_name: string }[]
  >([]);

  // State for viewing a resident's submission
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [viewResident, setViewResident] = useState<RotationPrefResponse | null>(
    null,
  );

  const handleViewSubmission = (submission: RotationPrefResponse) => {
    setViewResident(submission);
    setViewDialogOpen(true);
  };

  // State for generate schedule confirm dialog
  const [generateDialogOpen, setGenerateDialogOpen] = useState(false);

  // State for submission deletion, used in handleDeleteSubmission for loading tracking
  const [deletingSubmission, setDeletingSubmission] = useState<string | null>(
    null,
  );
  const [deleteTargetId, setDeleteTargetId] = useState<string | null>(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);

  // Config Tab, setSwitchingChiefType used in handleSwitchChiefType
  const [switchingChiefType, setSwitchingChiefType] = useState<string | null>(
    null,
  );

  // Local overrides for chief types
  // "residents" is a prop so it won't rerender on its own
  const [chiefTypeOverrides, setChiefTypeOverrides] = useState<
    Record<string, string>
  >({});

  const handleSwitchChiefType = async (
    resident: { id: string; name: string; chiefType: string },
    newChiefType: string,
  ) => {
    // Don't do anything if the role hasn't actually changed
    const currentChiefType =
      chiefTypeOverrides[resident.id] ?? resident.chiefType;

    if (currentChiefType === newChiefType) return;

    const prevChiefType = currentChiefType;

    // Optimistic update
    setChiefTypeOverrides((prev) => ({
      ...prev,
      [resident.id]: newChiefType,
    }));

    setSwitchingChiefType(resident.id);
    try {
      const res = await fetch(
        `${config.apiUrl}/api/pgy4-chief/${resident.id}`,
        {
          method: "PATCH",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ ChiefType: newChiefType || "None" }),
        },
      );

      if (res.ok) {
        toast({
          variant: "success",
          title: "Chief Type Updated",
          description: `${resident.name} has been set to ${newChiefType || "None"}.`,
        });
      } else {
        const error = await res.text();
        // Rollback on failure
        setChiefTypeOverrides((prev) => ({
          ...prev,
          [resident.id]: prevChiefType,
        }));

        toast({
          variant: "destructive",
          title: "Error",
          description: error || "Failed to switch user chief type.",
        });
      }
    } catch (error) {
      console.error("Error switching user chief role:", error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to switch user chief type. Please try again.",
      });
    } finally {
      setSwitchingChiefType(null);
    }
  };
  const [formOverrideResidentId, setFormOverrideResidentId] = useState<
    string | null
  >(null);

  // Helper to map RotationPrefResponse to ResidentPreference
  // Use spread to unpack elements
  const toPreference = (s: RotationPrefResponse) => ({
    residentId: s.resident.resident_id,
    residentName: `${s.resident.first_name} ${s.resident.last_name}`,
    priorities: [
      ...s.priorities.map((p) => p.rotationName),
      ...Array(Math.max(0, 8 - s.priorities.length)).fill(""),
    ],
    alternatives: [
      ...s.alternatives.map((a) => a.rotationName),
      ...Array(Math.max(0, 3 - s.alternatives.length)).fill(""),
    ],
    avoids: [
      ...s.avoids.map((a) => a.rotationName),
      ...Array(Math.max(0, 3 - s.avoids.length)).fill(""),
    ],
    additionalNotes: s.additionalNotes ?? "",
  });

  // Extract only PGY-3 residents
  const PGY3Residents = residents.filter(
    (resident) => resident.pgyLevel === 3 || resident.pgyLevel === "3",
  );

  // Helper to convert to UTC String
  const toLocalInputValue = (utcString) => {
    if (!utcString) return "";

    const date = new Date(utcString);
    const pad = (n) => String(n).padStart(2, "0");

    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  };

  // Submission tracking
  const submittedCount = submissions.length;
  const missingCount = unsubmittedResidents.length;

  // Load schedules and rotation types on mount
  useEffect(() => {
    const loadSchedules = async () => {
      try {
        setLoadingSchedules(true);
        setScheduleError(null);
        const res = await fetch(`${config.apiUrl}/api/pgy4-rotation-schedule`);
        if (!res.ok) throw new Error("Failed to fetch schedules");
        const data: Pgy4RotationSchedulesListResponse = await res.json();
        const list = data.schedules ?? [];
        setSchedules(list);
        const published = list.find((s) => s.isPublished);
        setSelectedScheduleId(
          published?.pgy4RotationScheduleId ??
            list[0]?.pgy4RotationScheduleId ??
            "",
        );
      } catch (err) {
        console.error(err);
        setScheduleError("Failed to load schedules.");
      } finally {
        setLoadingSchedules(false);
      }
    };

    const loadRotationTypes = async () => {
      try {
        const res = await fetch(
          `${config.apiUrl}/api/rotation-types?pgyYear=4`,
        );
        if (!res.ok) throw new Error("Failed to fetch rotation types");
        const data: RotationTypesListResponse = await res.json();
        const known = Object.keys(rotationColorMap);
        setRotationTypeNames(
          data.rotationTypes
            .filter((rt) => known.includes(rt.rotationName))
            .map((rt) => ({ id: rt.rotationTypeId, name: rt.rotationName })),
        );
      } catch (err) {
        console.error("Failed to load rotation types", err);
        setRotationTypeNames(
          Object.keys(rotationColorMap).map((name) => ({ id: name, name })),
        );
      }
    };

    const loadSubmissionWindow = async () => {
      try {
        const res = await fetch(
          `${config.apiUrl}/api/rotation-request-submission-window`,
        );
        if (!res.ok) return;
        const data = await res.json();
        if (data.availableDate) {
          setWindowAvailableDate(
            new Date(data.availableDate + "Z").toISOString(),
          );
        }
        if (data.dueDate) {
          setWindowDueDate(new Date(data.dueDate + "Z").toISOString());
        }
      } catch {
        // Stay empty
      }
    };

    loadSchedules();
    loadRotationTypes();
    loadSubmissionWindow();
  }, []);

  const handleGenerate = async () => {
    try {
      setGenerating(true);
      setScheduleError(null);
      const res = await fetch(
        `${config.apiUrl}/api/pgy4-rotation-schedule/generate?count=1`,
      );
      if (!res.ok) {
        const err: UnsubmittedResidentsResponse = await res.json();
        if (err.unsubmittedResidents?.length > 0) {
          const names = err.unsubmittedResidents
            .map((r) => `${r.first_name} ${r.last_name}`)
            .join(", ");
          setScheduleError(`Missing submissions from: ${names}`);
        } else {
          setScheduleError(err.message ?? "Failed to generate schedule.");
        }
        return;
      }
      const data: Pgy4RotationSchedulesListResponse = await res.json();
      const newSchedules = data.schedules ?? [];
      setSchedules((prev) => [...prev, ...newSchedules]);
      if (newSchedules[0])
        setSelectedScheduleId(newSchedules[0].pgy4RotationScheduleId);
    } catch (err) {
      console.error(err);
      setScheduleError("An unexpected error occurred.");
    } finally {
      setGenerating(false);
    }
  };

  const handleDelete = async () => {
    if (!selectedScheduleId) return;
    try {
      setDeletingSchedule(true);
      const res = await fetch(
        `${config.apiUrl}/api/pgy4-rotation-schedule/${selectedScheduleId}`,
        { method: "DELETE" },
      );
      if (!res.ok) throw new Error("Failed to delete schedule");
      const remaining = schedules.filter(
        (s) => s.pgy4RotationScheduleId !== selectedScheduleId,
      );
      setSchedules(remaining);
      setSelectedScheduleId(remaining[0]?.pgy4RotationScheduleId ?? "");
    } catch (err) {
      console.error(err);
      setScheduleError("Failed to delete schedule.");
    } finally {
      setDeletingSchedule(false);
    }
  };

  const [publishingSchedule, setPublishingSchedule] = useState(false);
  const [deletingSchedule, setDeletingSchedule] = useState(false);

  const handlePublish = async () => {
    if (!selectedScheduleId) return;
    const isCurrentlyPublished = selectedSchedule?.isPublished ?? false;
    try {
      setPublishingSchedule(true);
      setScheduleError(null);

      // Call correct url based on published status
      const url = isCurrentlyPublished
        ? `${config.apiUrl}/api/pgy4-rotation-schedule/unpublish`
        : `${config.apiUrl}/api/pgy4-rotation-schedule/publish/${selectedScheduleId}`;

      const res = await fetch(url, { method: "PATCH" });

      if (!res.ok)
        throw new Error(
          isCurrentlyPublished
            ? "Failed to unpublish schedule"
            : "Failed to publish schedule",
        );

      setSchedules((prev) =>
        prev.map((s) => ({
          ...s,
          isPublished: !isCurrentlyPublished
            ? s.pgy4RotationScheduleId === selectedScheduleId
            : s.pgy4RotationScheduleId === selectedScheduleId
              ? false
              : s.isPublished,
        })),
      );
      toast({
        variant: "success",
        title: isCurrentlyPublished ? "Unpublished" : "Published",
        description: isCurrentlyPublished
          ? "Schedule has been unpublished."
          : "Schedule is now live.",
      });
    } catch (err) {
      console.error(err);
      setScheduleError(
        isCurrentlyPublished
          ? "Failed to unpublish schedule."
          : "Failed to publish schedule.",
      );
    } finally {
      setPublishingSchedule(false);
    }
  };

  const handleDeleteSubmission = async (submissionId: string) => {
    setDeletingSubmission(submissionId);

    try {
      const res = await fetch(
        `${config.apiUrl}/api/rotation-pref-request/${submissionId}`,
        {
          method: "DELETE",
        },
      );
      if (!res.ok) {
        const errorText = await res.text();
        throw new Error(`Failed to delete: ${res.status} ${errorText}`);
      }

      // Refresh submissions
      const data = await fetch(`${config.apiUrl}/api/rotation-pref-request`);
      if (!data.ok) throw new Error("Failed to fetch submissions");
      const json: RotationPrefRequestsListResponse = await data.json();
      setSubmissions(json.rotationPrefRequests ?? []);
      await fetchUnsubmittedResidents();

      toast({
        variant: "success",
        title: "Deleted",
        description: "Submission has been deleted.",
      });
    } catch (error) {
      console.error("Delete submission error:", error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to delete submission.",
      });
    } finally {
      setDeletingSubmission(null);
    }
  };

  const handleDeleteAllSubmissions = async () => {
    try {
      const results = await Promise.allSettled(
        submissions.map((submission) =>
          fetch(
            `${config.apiUrl}/api/rotation-pref-request/${submission.rotationPrefRequestId}`,
            {
              method: "DELETE",
            },
          ),
        ),
      );

      const failed = results.filter(
        (r) =>
          r.status === "rejected" || (r.status === "fulfilled" && !r.value.ok),
      );

      const data = await fetch(`${config.apiUrl}/api/rotation-pref-request`);
      if (!data.ok) throw new Error("Failed to fetch submissions");
      const json: RotationPrefRequestsListResponse = await data.json();
      setSubmissions(json.rotationPrefRequests ?? []);
      await fetchUnsubmittedResidents();

      if (failed.length > 0) {
        toast({
          variant: "destructive",
          title: "Partial Failure",
          description: `${failed.length} submission(s) could not be deleted.`,
        });
      } else {
        toast({
          variant: "success",
          title: "Cleared",
          description: "All submissions have been deleted.",
        });
      }
    } catch (error) {
      console.error("Clear submissions error:", error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to clear submissions.",
      });
    }
  };

  const handleRotationFormSuccess = async () => {
    const resident = PGY3Residents.find((r) => r.id === formOverrideResidentId);
    setShowRotationFormModal(false);
    setFormOverrideResidentId(null);
    const refreshRes = await fetch(
      `${config.apiUrl}/api/rotation-pref-request`,
    );
    if (!refreshRes.ok) return;
    const json: RotationPrefRequestsListResponse = await refreshRes.json();
    setSubmissions(json.rotationPrefRequests ?? []);
    await fetchUnsubmittedResidents();
    toast({
      variant: "success",
      title: "Submitted",
      description: `Rotation preference for ${resident?.name ?? "Unknown"} submitted.`,
    });
  };

  const handleSaveSubmissionWindow = async () => {
    setSavingWindow(true);
    try {
      const res = await fetch(
        `${config.apiUrl}/api/rotation-request-submission-window`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            availableDate: windowAvailableDate,
            dueDate: windowDueDate,
          }),
        },
      );
      if (!res.ok) {
        let errorMessage = "Error unknown. Try again later.";
        const err = await res.json();
        errorMessage = Object.values(err).flat().join(" ") || errorMessage;

        toast({
          variant: "destructive",
          title: "Error Saving Dates",
          description: errorMessage,
        });
        return;
      }
      toast({
        variant: "success",
        title: "Saved",
        description: "Submission window updated.",
      });
    } catch {
      toast({
        variant: "destructive",
        title: "Error Saving Dates",
        description: "Error unknown. Try again later.",
      });
    } finally {
      setSavingWindow(false);
    }
  };

  const fetchUnsubmittedResidents = useCallback(async () => {
    try {
      const res = await fetch(
        `${config.apiUrl}/api/rotation-pref-request/resident/unsubmitted`,
      );
      if (!res.ok) return;
      const data: UnsubmittedResidentsResponse = await res.json();
      setUnsubmittedResidents(data.unsubmittedResidents ?? []);
    } catch (err) {
      console.error("Failed to fetch unsubmitted residents", err);
    }
  }, []);

  useEffect(() => {
    const loadSubmissions = async () => {
      try {
        setLoadingSubmissions(true);

        const res = await fetch(`${config.apiUrl}/api/rotation-pref-request`);

        if (!res.ok) throw new Error("Failed to fetch submissions");

        const data: RotationPrefRequestsListResponse = await res.json();

        setSubmissions(data.rotationPrefRequests ?? []);
      } catch (err) {
        console.error("Failed to load submissions", err);
      } finally {
        setLoadingSubmissions(false);
      }
    };

    loadSubmissions();
    fetchUnsubmittedResidents();
  }, [fetchUnsubmittedResidents]);

  return (
    <div className="w-full pt-4 h-[calc(100vh-4rem)] flex flex-col items-center px-4 md:pl-8">
      {/* Rotation Dashboard Overview Card*/}
      <Card className="mb-8 p-6 flex flex-col gap-4 items-center justify-between bg-white dark:bg-neutral-900 shadow-lg rounded-2xl border border-gray-200 dark:border-gray-800">
        <h2 className="text-2xl font-bold flex items-center gap-2 justify-center w-full mb-2">
          <CalendarRange className="w-6 h-6 text-blue-600" />
          PGY-4 Rotation Dashboard
        </h2>
        <div className="flex flex-col md:flex-row items-center w-full justify-between gap-4">
          <div />
          <div className="flex flex-col sm:flex-row gap-4 md:gap-8 items-center">
            <div className="flex flex-col items-center">
              <div className="flex items-center gap-2 mb-1">
                <Users className="w-5 h-5 text-blue-500" />
                <span className="text-2xl font-bold text-gray-900 dark:text-white">
                  {submittedCount}
                </span>
              </div>
              <span className="text-xs text-gray-500">Submitted</span>
            </div>
            <div className="flex flex-col items-center border-t sm:border-t-0 sm:border-l border-gray-200 dark:border-gray-700 pt-4 sm:pt-0 sm:pl-8">
              <div className="flex items-center gap-2 mb-1">
                <UserX className="w-5 h-5 text-red-500" />
                <span className="text-2xl font-bold text-gray-900 dark:text-white">
                  {missingCount}
                </span>
              </div>
              <span className="text-xs text-gray-500">Missing</span>
            </div>
            <div className="flex flex-col items-center border-t sm:border-t-0 sm:border-l border-gray-200 dark:border-gray-700 pt-4 sm:pt-0 sm:pl-8">
              <div className="flex items-center gap-2 mb-1">
                <CalendarClock className="w-5 h-5 text-yellow-500" />
                <span className="whitespace-nowrap text-2xl font-bold text-gray-900 dark:text-white">
                  {deadline
                    ? `${deadline.toLocaleString("en-US", {
                        month: "short",
                        day: "numeric",
                        hour: "numeric",
                        minute: "2-digit",
                        hour12: true,
                      })}`
                    : "—"}
                </span>
              </div>
              <span className="text-xs text-gray-500">Submission Deadline</span>
            </div>
            <div className="h-6 sm:h-10 border-t sm:border-t-0 sm:border-l border-gray-200 dark:border-gray-700 mx-0 sm:mx-4 lg:mx-6 hidden sm:block" />
            <div className="flex items-center">
              <Button
                onClick={() => setGenerateDialogOpen(true)}
                disabled={generating || schedules.length >= 5}
                className="bg-blue-600 hover:bg-blue-700 text-white font-semibold px-6 py-3 rounded-xl shadow cursor-pointer"
              >
                {generating
                  ? "Generating..."
                  : `Generate ${selectedYear} - ${selectedYear + 1} Schedule`}
              </Button>
            </div>

            <ConfirmDialog
              open={generateDialogOpen}
              onOpenChange={setGenerateDialogOpen}
              loading={generating}
              title="Generate Schedule"
              message={
                "Generate a new rotation schedule? This will add a new schedule to the list."
              }
              confirmText="Generate"
              cancelText="Cancel"
              onConfirm={handleGenerate}
            />

            {/* Delete Schedule */}
            <ConfirmDialog
              triggerText={
                <>
                  <span className="flex items-center justify-center">
                    <Trash2 className="h-4 w-4 mr-2" />
                    Delete Current Schedule
                  </span>
                </>
              }
              title="Delete current schedule?"
              message="This action cannot be undone."
              confirmText="Delete"
              cancelText="Cancel"
              onConfirm={handleDelete}
              loading={deletingSchedule}
              disabled={!selectedScheduleId}
              variant="danger"
            />
          </div>
        </div>
      </Card>
      {scheduleError && (
        <div className="w-full max-w-6xl mb-4 px-4 py-2 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg text-sm text-red-700 dark:text-red-400">
          {scheduleError}
        </div>
      )}

      {/* Tab Navigation */}
      <div className="w-full max-w-6xl flex flex-col sm:flex-row gap-1 sm:gap-2 mb-4 sm:mb-6">
        <Button
          variant={activeTab === "schedule" ? "default" : "outline"}
          className={`flex-1 cursor-pointer rounded-b-none sm:rounded-br-none text-xs sm:text-sm py-1 sm:py-2 ${activeTab === "schedule" ? "shadow-md" : ""}`}
          onClick={() => setActiveTab("schedule")}
        >
          Current Schedule
        </Button>
        <Button
          variant={activeTab === "submissions" ? "default" : "outline"}
          className={`flex-1 cursor-pointer rounded-b-none text-xs sm:text-sm py-1 sm:py-2 ${activeTab === "submissions" ? "shadow-md" : ""}`}
          onClick={() => setActiveTab("submissions")}
        >
          Submissions
        </Button>
        <Button
          variant={activeTab === "configure" ? "default" : "outline"}
          className={`flex-1 cursor-pointer rounded-b-none text-xs sm:text-sm py-1 sm:py-2 ${activeTab === "configure" ? "shadow-md" : ""}`}
          onClick={() => setActiveTab("configure")}
        >
          Configure
        </Button>
      </div>

      {/* Tab Content */}
      <div className="w-full">
        {activeTab === "schedule" && (
          <Card className="p-4 sm:p-6 lg:p-8 bg-gray-50 dark:bg-neutral-900 shadow-lg rounded-2xl w-full flex flex-col gap-4 mb-6 sm:mb-8 border border-gray-200 dark:border-gray-800">
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-4 gap-2">
              <div className="flex flex-col sm:flex-row items-baseline gap-2">
                <h2 className="text-lg sm:text-xl font-bold font-color">
                  Current Schedule
                </h2>
                <p className="text-xs sm:text-sm text-gray-500">
                  Click a table cell to choose the rotation type
                </p>
              </div>
              <div className="flex gap-2 items-center">
                {schedules.length > 0 && (
                  <>
                    <select
                      className="px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-primary"
                      value={selectedScheduleId}
                      onChange={(e) => setSelectedScheduleId(e.target.value)}
                    >
                      {schedules.map((s, i) => (
                        <option
                          key={s.pgy4RotationScheduleId}
                          value={s.pgy4RotationScheduleId}
                        >
                          Schedule {i + 1} (Seed: {s.seed})
                          {s.isPublished ? " ✓ Published" : ""}
                        </option>
                      ))}
                    </select>
                    <Button
                      onClick={handlePublish}
                      disabled={publishingSchedule}
                      className={`text-white text-xs sm:text-sm px-3 py-2 cursor-pointer ${
                        selectedSchedule?.isPublished
                          ? "bg-green-600 hover:bg-red-600"
                          : "bg-blue-600 hover:bg-blue-700"
                      }`}
                    >
                      {publishingSchedule
                        ? selectedSchedule?.isPublished
                          ? "Unpublishing..."
                          : "Publishing..."
                        : selectedSchedule?.isPublished
                          ? "Unpublish"
                          : "Publish"}
                    </Button>
                  </>
                )}
              </div>
            </div>
            {loadingSchedules ? (
              <div className="py-8 text-center text-gray-500">
                Loading schedules...
              </div>
            ) : (
              <RotationScheduleTable
                schedule={selectedSchedule?.schedule ?? []}
                colorMap={rotationColorMap}
                displayNames={rotationDisplayNames}
                rotationTypes={rotationTypeNames}
                emptyMessage='No rotations found. Use the "Generate Schedule" button above to create a rotation schedule.'
                allowResidentReassignment={false}
                onRotationChange={async (
                  residentId,
                  monthIndex,
                  newRotationTypeId,
                ) => {
                  if (!selectedScheduleId) return;

                  try {
                    const res = await fetch(
                      `${config.apiUrl}/api/pgy4-rotation-schedule-override/${selectedScheduleId}`,
                      {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify({
                          residentId,
                          newRotationTypeId,
                          academicMonthIndex: monthIndex,
                        }),
                      },
                    );

                    if (!res.ok) {
                      const err = await res.json();
                      toast({
                        variant: "destructive",
                        title: "Error",
                        description:
                          err?.message ?? "Failed to stage rotation change.",
                      });
                      return;
                    }

                    setHasPendingOverrides(true);

                    // Sync override list for the pending changes panel
                    await syncOverrideState(selectedScheduleId);

                    // Re-validate — backend applies overrides before checking constraints
                    await fetchConstraintErrors(selectedScheduleId);
                  } catch {
                    toast({
                      variant: "destructive",
                      title: "Error",
                      description: "Failed to stage rotation change.",
                    });
                  }
                }}
              />
            )}

            {/* Constraint Violations Panel */}
            {!loadingSchedules && selectedSchedule && (
              <ConstraintViolationsPanel
                violations={constraintViolations}
                loading={loadingViolations}
              />
            )}

            {/* Pending Changes Panel */}
            {!loadingSchedules && selectedSchedule && (
              <PendingChangesPanel
                overrides={pendingOverrides}
                onRevertOne={handleRevertSingleOverride}
              />
            )}

            {/* Footer */}
            <div className="flex justify-between gap-2 pt-4">
              <Button
                onClick={() => setShowRotationFormModal(true)}
                disabled={showRotationFormModal == true}
                className="py-2 flex items-center justify-center gap-2 bg-blue-600 text-white hover:bg-blue-700 cursor-pointer"
              >
                <ClipboardList className="h-4 w-4" />
                Rotation Form Override
              </Button>
              <div className="flex justify-end gap-2">
                {/* Discard */}
                <Button
                  variant="outline"
                  disabled={
                    !hasPendingOverrides ||
                    discardingOverrides ||
                    applyingOverrides
                  }
                  onClick={handleDiscardOverrides}
                  className="flex items-center gap-2 px-5 py-2.5 text-sm font-medium
                            border-red-500 text-red-600
                            hover:bg-red-500 hover:text-white
                            disabled:opacity-40 cursor-pointer"
                >
                  {discardingOverrides ? (
                    <Loader2 className="h-4 w-4 animate-spin" />
                  ) : (
                    <X className="h-4 w-4" />
                  )}
                  <span>Discard</span>
                </Button>

                {/* Apply */}
                <Button
                  disabled={
                    !hasPendingOverrides ||
                    applyingOverrides ||
                    discardingOverrides
                  }
                  onClick={handleApplyOverrides}
                  className="flex items-center gap-2 px-5 py-2.5 text-sm font-medium
                            bg-blue-600 hover:bg-blue-700 text-white
                            disabled:opacity-40 cursor-pointer"
                >
                  {applyingOverrides ? (
                    <Loader2 className="h-4 w-4 animate-spin" />
                  ) : (
                    <Save className="h-4 w-4" />
                  )}
                  <span>Apply</span>
                </Button>

                {/* Export */}
                <Button
                  disabled={!selectedScheduleId}
                  onClick={() => {
                    window.open(
                      `${config.apiUrl}/api/pgy4-rotation-schedule/export/${selectedScheduleId}`,
                      "_blank",
                    );
                  }}
                  className="flex items-center gap-2 px-5 py-2.5 text-sm font-medium
                            bg-green-600 hover:bg-green-700 text-white
                            disabled:opacity-40 cursor-pointer"
                >
                  <Download className="h-4 w-4" />
                  <span>Export</span>
                </Button>
              </div>
            </div>
          </Card>
        )}

        {activeTab === "submissions" && (
          <Card className="p-4 sm:p-6 lg:p-8 bg-gray-50 dark:bg-neutral-900 shadow-lg rounded-2xl w-full flex flex-col gap-4 mb-6 sm:mb-8 border border-gray-200 dark:border-gray-800">
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-4 gap-2">
              <h2 className="text-lg sm:text-xl font-bold">Submissions</h2>
              <div className="flex gap-2">
                <ConfirmDialog
                  triggerText={
                    <>
                      <X className="h-4 w-4" />
                      <span>Clear</span>
                    </>
                  }
                  title="Clear all submissions?"
                  message="This action cannot be undone."
                  confirmText="Clear"
                  cancelText="Cancel"
                  onConfirm={handleDeleteAllSubmissions}
                  variant="danger"
                />
              </div>
            </div>
            <div className="overflow-x-auto max-h-[32rem] overflow-y-auto w-full">
              <table className="w-full divide-y divide-gray-200 dark:divide-gray-700">
                <thead className="sticky top-0 bg-gray-100 dark:bg-neutral-800">
                  <tr>
                    <th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Residents
                    </th>
                    <th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      View Submissions
                    </th>
                    <th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
                  {loadingSubmissions ? (
                    <tr>
                      <td colSpan={3} className="px-6 py-4 text-center">
                        Loading...
                      </td>
                    </tr>
                  ) : submissions.length > 0 ? (
                    submissions.map((submission) => (
                      <tr
                        key={submission.rotationPrefRequestId}
                        className="hover:bg-gray-50 dark:hover:bg-neutral-800"
                      >
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-100">
                          {submission.resident?.first_name}{" "}
                          {submission.resident?.last_name}
                        </td>
                        <td className="px-3 py-2 whitespace-nowrap text-sm font-medium">
                          <Button
                            variant="outline"
                            size="sm"
                            className="cursor-pointer"
                            onClick={() => handleViewSubmission(submission)}
                          >
                            View
                          </Button>
                        </td>

                        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                          <Button
                            variant="outline"
                            size="sm"
                            className="text-red-600 border-red-600 hover:bg-red-500 hover:text-white cursor-pointer"
                            onClick={() => {
                              setDeleteTargetId(
                                submission.rotationPrefRequestId,
                              );
                              setDeleteDialogOpen(true);
                            }}
                            disabled={
                              deletingSubmission ===
                              submission.rotationPrefRequestId
                            }
                          >
                            Delete
                          </Button>
                        </td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td
                        colSpan={3}
                        className="px-6 py-4 text-center text-gray-500 italic"
                      >
                        No submissions yet.
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          </Card>
        )}

        {activeTab === "configure" && (
          <Card className="p-4 sm:p-6 lg:p-8 bg-gray-50 dark:bg-neutral-900 shadow-lg rounded-2xl w-full flex flex-col gap-4 mb-6 sm:mb-8 border border-gray-200 dark:border-gray-800">
            <h2 className="text-lg sm:text-xl font-bold">Configure</h2>
            {/*Form Availabity Selection */}
            <div className="grid grid-cols-2 items-start sm:items-end gap-6">
              <div>
                <label className="flex items-center text-sm font-semibold gap-2 mb-2">
                  Rotation Form Opens
                </label>
                <input
                  type="datetime-local"
                  className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors"
                  value={toLocalInputValue(windowAvailableDate)}
                  onChange={(e) =>
                    setWindowAvailableDate(
                      new Date(e.target.value).toISOString(),
                    )
                  }
                />
              </div>
              <div>
                <label className="flex items-center text-sm font-semibold gap-2 mb-2">
                  Rotation Form Deadline
                </label>
                <input
                  type="datetime-local"
                  className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors"
                  value={toLocalInputValue(windowDueDate)}
                  onChange={(e) =>
                    setWindowDueDate(new Date(e.target.value).toISOString())
                  }
                  min={windowAvailableDate}
                />
              </div>
              <div className="flex flex-row gap-2">
                <Button
                  onClick={handleSaveSubmissionWindow}
                  disabled={
                    savingWindow || !windowAvailableDate || !windowDueDate
                  }
                  className="py-2 flex items-center justify-center gap-2 bg-blue-600 text-white hover:bg-blue-700 cursor-pointer"
                >
                  {savingWindow ? (
                    <Loader2 className="h-4 w-4 animate-spin" />
                  ) : (
                    <Save className="h-4 w-4" />
                  )}
                  {savingWindow ? "Saving..." : "Save Window"}
                </Button>
              </div>
            </div>

            <h2 className="text-lg sm:text-xl font-bold mb-4">
              Chief Selection
            </h2>

            <div className="overflow-x-auto max-h-96 overflow-y-auto">
              <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                <thead className="sticky top-0 bg-gray-100 dark:bg-neutral-800">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Name
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Chief Type
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
                  {PGY3Residents.length > 0 ? (
                    PGY3Residents.map((r) => (
                      <tr
                        key={r.id}
                        className="hover:bg-gray-50 dark:hover:bg-neutral-800"
                      >
                        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100">
                          {r.name}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm">
                          <select
                            value={
                              chiefTypeOverrides[r.id] ?? r.chiefType ?? ""
                            }
                            onChange={(e) =>
                              handleSwitchChiefType(r, e.target.value)
                            }
                            disabled={switchingChiefType === r.id}
                            className="px-2 py-1 border border-gray-300 dark:border-gray-600 rounded text-sm bg-white dark:bg-neutral-800 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500"
                          >
                            <option value="">None</option>
                            <option value="Admin">Admin</option>
                            <option value="Clinic">Clinic</option>
                            <option value="Education">Education</option>
                          </select>
                        </td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td
                        colSpan={2}
                        className="px-6 py-4 text-center text-gray-500 italic"
                      >
                        No PGY-3 residents found.
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          </Card>
        )}
      </div>

      {/* Modals*/}
      <Modal
        open={showRotationFormModal}
        onClose={() => setShowRotationFormModal(false)}
        title="Create Rotation Form"
      >
        <div className="grid grid-cols-2 items-start sm:items-end gap-6">
          <div className="mb-2">
            <label className="flex items-center text-sm font-semibold gap-2 mb-2">
              Resident Selection
            </label>
            <div className="relative">
              <select
                value={formOverrideResidentId ?? ""}
                onChange={(e) =>
                  setFormOverrideResidentId(e.target.value || null)
                }
                required={true}
                className="w-full px-4 py-3 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-blue-500 focus:border-blue-500 transition-colors appearance-none cursor-pointer"
              >
                <option value="">Select</option>
                {PGY3Residents.map((r) => (
                  <option key={r.id} value={r.id}>
                    {r.name}
                  </option>
                ))}
              </select>
              <div className="absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none">
                <ChevronDown className="h-4 w-4" />
              </div>
            </div>
          </div>
        </div>

        {/* Form with disabled overlay */}
        <div className="relative">
          {!formOverrideResidentId && (
            <div className="absolute inset-0 z-10 bg-background/60 backdrop-blur-[1px] rounded-lg flex items-center justify-center"></div>
          )}
          <RotationForm
            key={formOverrideResidentId ?? ""}
            userId={formOverrideResidentId ?? ""}
            userPGY={
              (PGY3Residents.find((r) => r.id === formOverrideResidentId)
                ?.pgyLevel as number) ?? 3
            }
            requiredPGY={3}
            rotationPgyYear={4}
            deadline={deadline ?? new Date()}
            ignoreDeadline={true}
            submitEndpoint="api/rotation-pref-request?adminOverride=true"
            fetchEndpoint="api/rotation-pref-request/resident"
            onSuccess={handleRotationFormSuccess}
          />
        </div>
      </Modal>

      {viewResident && (
        <SubmissionViewDialog
          open={viewDialogOpen}
          onOpenChange={setViewDialogOpen}
          residentId={viewResident.resident.resident_id}
          residentName={`${viewResident.resident.first_name} ${viewResident.resident.last_name}`}
          prefetchedData={toPreference(viewResident)}
        />
      )}

      <ConfirmDialog
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
        loading={deletingSubmission === deleteTargetId}
        title="Delete Submission"
        message="Are you sure you want to delete this submission? This action cannot be undone."
        confirmText="Delete"
        cancelText="Cancel"
        onConfirm={async () => {
          if (deleteTargetId) await handleDeleteSubmission(deleteTargetId);
        }}
      />
    </div>
  );
};

export default PGY4RotationSchedulePage;
