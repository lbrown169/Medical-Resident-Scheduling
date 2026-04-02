"use client";

import React, { useEffect, useState } from "react";
import { config } from "../../../config";
import { RotationScheduleTable } from "../components/RotationScheduleTable";
import { Button } from "@/components/ui/button";
import { Calendar, Download, AlertCircle, Loader2 } from "lucide-react";

// Types

interface RotationTypeResponse {
  rotationTypeId: string;
  rotationName: string;
}

interface RotationResponse {
  rotationId: string;
  scheduleId?: string;
  month: string;
  academicMonthIndex: number;
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

// Color map

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
  Unassigned: "#6b7280",
};

const rotationDisplayNames: Record<string, string> = {
  "Inpatient Psy": "Inpt Psy",
  "Psy Consults": "Consult",
  "Community Psy": "Comm",
};

// Page

export default function PGY4SchedulePage() {
  const [schedule, setSchedule] = useState<Pgy4RotationScheduleResponse | null>(
    null,
  );
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadSchedule = async () => {
      try {
        setLoading(true);
        setError(null);
        const res = await fetch(
          `${config.apiUrl}/api/pgy4-rotation-schedule/published`,
        );
        if (res.status === 404) {
          setSchedule(null);
          return;
        }
        if (!res.ok) throw new Error("Failed to fetch schedule");
        const data: Pgy4RotationScheduleResponse = await res.json();
        setSchedule(data);
      } catch (err) {
        console.error(err);
        setError("Unable to load schedule. Please try again.");
      } finally {
        setLoading(false);
      }
    };

    loadSchedule();
  }, []);

  return (
    <div className="max-w-full mx-auto p-2 pl-8">
      {/* Header */}
      <div className="mb-4 flex justify-between items-start print:mb-4">
        <div>
          <h1 className="text-xl font-bold text-gray-900 dark:text-gray-100 mb-1">
            PGY-4 Rotation Schedule
          </h1>
          <p className="text-gray-600 dark:text-gray-400 text-xs">
            Published rotation assignments for the current academic year
          </p>
        </div>
        <div className="flex gap-3 print:hidden">
          <Button
            onClick={() => {
              // !! wire to export endpoint when ready
              console.log("Export schedule");
            }}
            className="py-2 flex items-center justify-center gap-2 bg-green-500 text-white hover:bg-green-600"
            disabled={loading || !!error || !schedule}
          >
            <Download className="h-4 w-4" />
            Export
          </Button>
        </div>
      </div>

      {/* Content */}
      <div>
        {loading && (
          <div className="flex flex-col items-center justify-center py-24">
            <Loader2 className="h-12 w-12 animate-spin text-blue-600 dark:text-blue-400 mb-4" />
            <p className="text-lg text-gray-600 dark:text-gray-400">
              Loading rotation schedule...
            </p>
          </div>
        )}

        {!loading && error && (
          <div className="flex flex-col items-center justify-center py-24">
            <AlertCircle className="h-12 w-12 text-red-500 mb-4" />
            <p className="text-lg font-medium text-gray-900 dark:text-gray-100 mb-2">
              Unable to Load Schedule
            </p>
            <p className="text-sm text-gray-600 dark:text-gray-400 mb-6">
              {error}
            </p>
            <Button onClick={() => window.location.reload()} variant="outline">
              Try Again
            </Button>
          </div>
        )}

        {!loading && !error && !schedule && (
          <div className="flex flex-col items-center justify-center py-24 bg-gray-50 dark:bg-gray-800 rounded-lg border-2 border-dashed border-gray-300 dark:border-gray-700">
            <Calendar className="h-16 w-16 text-gray-400 dark:text-gray-600 mb-4" />
            <p className="text-xl font-medium text-gray-900 dark:text-gray-100 mb-2">
              No Schedule Published Yet
            </p>
            <p className="text-sm text-gray-600 dark:text-gray-400 text-center max-w-md">
              The PGY-4 rotation schedule has not been published yet. You will
              be able to view it here once it becomes available.
            </p>
          </div>
        )}

        {!loading && !error && schedule && (
          <div className="bg-gray-50 dark:bg-neutral-900 border border-gray-200 dark:border-gray-800 rounded-2xl shadow-lg p-4 sm:p-6">
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-4 gap-1">
              <div>
                <h2 className="text-lg font-bold text-gray-900 dark:text-gray-100">
                  Published Schedule
                </h2>
                <p className="text-xs text-gray-500 dark:text-gray-400">
                  Academic year {schedule.year} – {schedule.year + 1}
                </p>
              </div>
            </div>
            <RotationScheduleTable
              schedule={schedule.schedule}
              colorMap={rotationColorMap}
              displayNames={rotationDisplayNames}
              rotationTypes={Object.entries(rotationColorMap).map(([name]) => ({
                id: name,
                name,
              }))}
              readOnly={true}
            />
          </div>
        )}
      </div>

      {/* Footer */}
      <div className="mt-8 text-center text-xs text-gray-500 dark:text-gray-500 print:mt-6">
        <p>
          For questions about your rotation schedule, please contact the Chief
          Residents or Program Director.
        </p>
      </div>
    </div>
  );
}
