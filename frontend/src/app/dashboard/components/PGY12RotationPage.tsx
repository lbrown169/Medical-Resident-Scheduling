"use client";

import React, { useState, useEffect, useCallback } from "react";
import { Card } from "../../../components/ui/card";
import { CalendarRange, Users, Copy } from "lucide-react";
import { config } from "../../../config";
import {
  RotationScheduleTable,
  ResidentScheduleEntry,
  RotationEntry,
} from "./RotationScheduleTable";

// Rotation-type color palette (PGY1/2 types from HospitalRole.cs)
const ROTATION_COLORS: Record<string, string> = {
  "Unassigned":      "#d1d5db", // gray
  "Inpatient Psy":   "#6366f1", // indigo
  "Geriatric":       "#14b8a6", // teal
  "PHPandIOP":       "#eab308", // yellow
  "Psy Consults":    "#f97316", // orange
  "Community Psy":   "#10b981", // emerald
  "CAP":             "#ec4899", // pink
  "Addiction":       "#a855f7", // purple
  "Forensic":        "#d946ef", // fuchsia
  "Float":           "#78716c", // stone
  "Neurology":       "#8b5cf6", // violet
  "ImOutpatient":    "#22c55e", // green
  "ImInpatient":     "#0ea5e9", // sky
  "NightFloat":      "#1e293b", // dark slate
  "EmergencyMed":    "#ef4444", // red
};
const FALLBACK_COLOR = "#6b7280";

function colorMapFromTypes(types: RotationTypeFromApi[]): Record<string, string> {
  const map: Record<string, string> = {};
  for (const t of types) {
    map[t.rotationName] = ROTATION_COLORS[t.rotationName] ?? FALLBACK_COLOR;
  }
  return map;
}

// API response shapes 
interface RotationFromApi {
  rotationId: string;
  residentId: string | null;
  scheduleId: string | null;
  month: string;
  academicYear: number;
  academicMonthIndex: number; 
  pgyYear: number;
  rotationType: { rotationTypeId: string; rotationName: string };
}

interface RotationTypeFromApi {
  rotationTypeId: string;
  rotationName: string;
  pgyYears: number[];
}

interface ResidentFromApi {
  resident_id: string;
  first_name: string;
  last_name: string;
  graduate_yr: number;
}

// Helpers
function toAcademicIndex(calMonth: number): number {
  return (calMonth + 6) % 12;
}

function currentAcademicYear(): number {
  const now = new Date();
  return now.getMonth() >= 6 ? now.getFullYear() : now.getFullYear() - 1;
}

function buildSchedule(
  rotations: RotationFromApi[],
  residents: ResidentFromApi[],
): ResidentScheduleEntry[] {
  const residentMap = new Map<string, ResidentFromApi>();
  for (const res of residents) residentMap.set(res.resident_id, res);

  // Group rotations by rotationId, each rotationId is one row
  const grouped = new Map<string, RotationFromApi[]>();
  for (const r of rotations) {
    if (!grouped.has(r.rotationId)) grouped.set(r.rotationId, []);
    grouped.get(r.rotationId)!.push(r);
  }

  return Array.from(grouped.entries()).map(([rotationId, rots]) => {
    const residentId = rots[0]?.residentId;
    const resident = residentId ? residentMap.get(residentId) : undefined;

    const entries: RotationEntry[] = rots.map((r) => ({
      rotationId: r.rotationId,
      scheduleId: r.scheduleId ?? undefined,
      month: r.month,
      academicMonthIndex: toAcademicIndex(r.academicMonthIndex),
      pgyYear: r.pgyYear,
      rotationType: {
        rotationTypeId: r.rotationType.rotationTypeId,
        rotationName: r.rotationType.rotationName,
      },
    }));

    return {
      resident: {
        // Use actual residentId when assigned so the dropdown shows the right selection
        resident_id: resident?.resident_id ?? `unassigned-${rotationId}`,
        first_name: resident?.first_name ?? "Unassigned",
        last_name: resident?.last_name ?? "",
      },
      rotations: entries,
    };
  });
}

// Component
export default function PGY12RotationPage() {
  const [academicYear, setAcademicYear] = useState(currentAcademicYear);
  const [activeTab, setActiveTab] = useState<1 | 2>(1);

  const [pgy1Rotations, setPgy1Rotations] = useState<RotationFromApi[]>([]);
  const [pgy2Rotations, setPgy2Rotations] = useState<RotationFromApi[]>([]);
  const [rotationTypes, setRotationTypes] = useState<RotationTypeFromApi[]>([]);
  const [pgy1Residents, setPgy1Residents] = useState<ResidentFromApi[]>([]);
  const [pgy2Residents, setPgy2Residents] = useState<ResidentFromApi[]>([]);
  const [copyableYears, setCopyableYears] = useState<number[]>([]);
  const [copyFromYear, setCopyFromYear] = useState<number | "">("");
  const [copying, setCopying] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const jsonOrThrow = async (res: Response) => {
        if (!res.ok) throw new Error(`${res.url} returned ${res.status}`);
        return res.json();
      };
      // Offset so future years show the right residents
      const offset = academicYear - currentAcademicYear();
      const pgy1GradYr = 1 - offset;
      const pgy2GradYr = 2 - offset;
      const [r1, r2, types, res1, res2, copyable] = await Promise.all([
        fetch(`${config.apiUrl}/api/rotations?pgyYear=1&academicYear=${academicYear}`).then(jsonOrThrow),
        fetch(`${config.apiUrl}/api/rotations?pgyYear=2&academicYear=${academicYear}`).then(jsonOrThrow),
        fetch(`${config.apiUrl}/api/rotations/types?pgyYear=1&pgyYear=2`).then(jsonOrThrow),
        fetch(`${config.apiUrl}/api/residents?graduate_yr=${pgy1GradYr}`).then(jsonOrThrow),
        fetch(`${config.apiUrl}/api/residents?graduate_yr=${pgy2GradYr}`).then(jsonOrThrow),
        fetch(`${config.apiUrl}/api/rotations/copyable`).then(jsonOrThrow),
      ]);
      setPgy1Rotations(Array.isArray(r1) ? r1 : []);
      setPgy2Rotations(Array.isArray(r2) ? r2 : []);
      setRotationTypes(Array.isArray(types) ? types : []);
      setPgy1Residents(Array.isArray(res1) ? res1 : []);
      setPgy2Residents(Array.isArray(res2) ? res2 : []);
      const years = (Array.isArray(copyable) ? copyable : []).sort((a: number, b: number) => b - a);
      setCopyableYears(years);
      if (years.length > 0) setCopyFromYear(years[0]);
    } catch (e) {
      setError("Failed to load rotation data.");
      console.error(e);
    } finally {
      setLoading(false);
    }
  }, [academicYear]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  // Build table props
  const colorMap = colorMapFromTypes(rotationTypes);
  const pgy1Schedule = buildSchedule(pgy1Rotations, pgy1Residents);
  const pgy2Schedule = buildSchedule(pgy2Rotations, pgy2Residents);

  const filteredTypes = rotationTypes
    .filter((t) => t.pgyYears.includes(activeTab))
    .map((t) => ({ id: t.rotationTypeId, name: t.rotationName }));

  const handleRotationChange = async (
    rowId: string,
    monthIndex: number,
    newRotationTypeId: string,
  ) => {
    const rotations = activeTab === 1 ? pgy1Rotations : pgy2Rotations;
    // rowId is residentId or "unassigned-{rotationId}" — match by either
    const match = rotations.find(
      (r) =>
        (r.residentId === rowId || `unassigned-${r.rotationId}` === rowId) &&
        toAcademicIndex(r.academicMonthIndex) === monthIndex,
    );
    if (!match) return;

    try {
      const res = await fetch(
        `${config.apiUrl}/api/rotations/${match.rotationId}/${monthIndex}`,
        {
          method: "PATCH",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ rotationTypeId: newRotationTypeId }),
        },
      );
      if (!res.ok) {
        const body = await res.json().catch(() => null);
        console.error("PATCH failed", body);
        return;
      }
      await fetchData();
    } catch (e) {
      console.error("Failed to update rotation", e);
    }
  };

  const handleResidentChange = async (rotationId: string, newResidentId: string) => {
    try {
      const isUnassign = !newResidentId;
      const res = await fetch(
        `${config.apiUrl}/api/rotations/${rotationId}/${isUnassign ? "unassign" : "assign"}`,
        {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          ...(!isUnassign && { body: JSON.stringify({ residentId: newResidentId }) }),
        },
      );
      if (!res.ok) {
        const body = await res.json().catch(() => null);
        console.error(isUnassign ? "Unassign failed" : "Assign failed", body);
        return;
      }
      await fetchData();
    } catch (e) {
      console.error("Failed to update resident assignment", e);
    }
  };

  const handleCopyRotations = async () => {
    if (!copyFromYear) return;
    setCopying(true);
    try {
      const res = await fetch(`${config.apiUrl}/api/rotations/copy`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ fromAcademicYear: copyFromYear, toAcademicYear: academicYear }),
      });
      if (!res.ok) {
        const body = await res.json().catch(() => null);
        const msg = body?.message ?? `Copy failed (${res.status})`;
        setError(msg);
        return;
      }
      await fetchData();
    } catch (e) {
      console.error("Failed to copy rotations", e);
      setError("Failed to copy rotations.");
    } finally {
      setCopying(false);
    }
  };

  const currentYearHasRotations = pgy1Rotations.length > 0 || pgy2Rotations.length > 0;

  const schedule = activeTab === 1 ? pgy1Schedule : pgy2Schedule;
  const residents = activeTab === 1 ? pgy1Residents : pgy2Residents;
  const residentList = [
    { id: "", name: "Unassigned" },
    ...residents.map((r) => ({
      id: r.resident_id,
      name: `${r.first_name} ${r.last_name.charAt(0)}.`,
    })),
  ];

  // Year selector range
  const thisAcYear = currentAcademicYear();
  const yearOptions = [thisAcYear, thisAcYear + 1];

  const pgy1SlotCount = new Set(pgy1Rotations.map((r) => r.rotationId)).size;
  const pgy2SlotCount = new Set(pgy2Rotations.map((r) => r.rotationId)).size;
  const assignedCount = new Set(
    (activeTab === 1 ? pgy1Rotations : pgy2Rotations)
      .filter((r) => r.residentId)
      .map((r) => r.rotationId),
  ).size;
  const totalSlots = activeTab === 1 ? pgy1SlotCount : pgy2SlotCount;

  return (
    <div className="w-full pt-4 flex flex-col items-center px-4">
      {/* Overview Card */}
      <Card className="mb-8 p-6 flex flex-col gap-4 items-center justify-between bg-white dark:bg-neutral-900 shadow-lg rounded-2xl border border-gray-200 dark:border-gray-800">
        <h2 className="text-2xl font-bold flex items-center gap-2 justify-center w-full mb-2">
          <CalendarRange className="w-6 h-6 text-blue-600" />
          PGY1 &amp; PGY2 Rotation Management
        </h2>

        <div className="flex flex-col sm:flex-row gap-4 md:gap-0 items-stretch">
          {/* Academic Year Selector */}
          <div className="flex flex-col items-center justify-center md:px-8">
            <div className="flex items-center gap-2 mb-1">
              <select
                className="border rounded px-2 py-1 text-sm font-semibold dark:bg-neutral-800 dark:border-gray-600"
                value={academicYear}
                onChange={(e) => setAcademicYear(Number(e.target.value))}
              >
                {yearOptions.map((y) => (
                  <option key={y} value={y}>
                    {y}–{y + 1}
                  </option>
                ))}
              </select>
            </div>
            <span className="text-xs text-gray-500">Academic Year</span>
          </div>

          {/* Divider */}
          <div className="hidden sm:block w-px bg-gray-200 dark:bg-gray-700" />

          {/* Assigned Stat */}
          <div className="flex flex-col items-center justify-center md:px-8 border-t sm:border-t-0 border-gray-200 dark:border-gray-700 pt-4 sm:pt-0">
            <div className="flex items-center gap-2 mb-1">
              <Users className="w-5 h-5 text-green-500" />
              <span className="text-2xl font-bold text-gray-900 dark:text-white">
                {assignedCount}/{totalSlots}
              </span>
            </div>
            <span className="text-xs text-gray-500">Slots Assigned</span>
          </div>

          {/* Divider */}
          <div className="hidden sm:block w-px bg-gray-200 dark:bg-gray-700" />

          {/* PGY Tabs */}
          <div className="flex flex-col items-center justify-center md:px-8 border-t sm:border-t-0 border-gray-200 dark:border-gray-700 pt-4 sm:pt-0">
            <div className="flex gap-2">
              {([1, 2] as const).map((pgy) => (
                <button
                  key={pgy}
                  onClick={() => setActiveTab(pgy)}
                  className={`px-4 py-2 rounded-lg text-sm font-semibold transition-colors cursor-pointer ${
                    activeTab === pgy
                      ? "bg-blue-500 hover:bg-blue-600 text-white shadow-md"
                      : "bg-gray-200 dark:bg-neutral-700 text-gray-700 dark:text-gray-200 hover:bg-gray-300 dark:hover:bg-neutral-600"
                  }`}
                >
                  PGY{pgy}
                </button>
              ))}
            </div>
          </div>

          {/* Divider */}
          <div className="hidden sm:block w-px bg-gray-200 dark:bg-gray-700" />

          {/* Copy Rotations */}
          <div className="flex flex-col items-center justify-center md:px-8 border-t sm:border-t-0 border-gray-200 dark:border-gray-700 pt-4 sm:pt-0">
            {currentYearHasRotations ? (
              <span className="text-xs text-gray-400 italic">Rotations exist</span>
            ) : (
              <>
                <div className="flex items-center gap-2 mb-1">
                  <select
                    className="border rounded px-2 py-1 text-xs dark:bg-neutral-800 dark:border-gray-600"
                    value={copyFromYear}
                    onChange={(e) => setCopyFromYear(Number(e.target.value))}
                    disabled={copyableYears.length === 0}
                  >
                    {copyableYears.length === 0 ? (
                      <option value="">No years</option>
                    ) : (
                      copyableYears.map((y) => (
                        <option key={y} value={y}>
                          {y}–{y + 1}
                        </option>
                      ))
                    )}
                  </select>
                  <button
                    onClick={handleCopyRotations}
                    disabled={copying || !copyFromYear || copyableYears.length === 0}
                    className="flex items-center gap-1 px-3 py-1.5 rounded-lg text-xs font-semibold bg-blue-500 hover:bg-blue-600 text-white shadow-md transition-colors cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    <Copy className="w-3.5 h-3.5" />
                    {copying ? "Copying..." : "Copy"}
                  </button>
                </div>
                <span className="text-xs text-gray-500">Copy from year</span>
              </>
            )}
          </div>
        </div>
      </Card>

      {/* Table */}
      {loading ? (
        <p className="text-gray-500 italic py-8">Loading rotations...</p>
      ) : error ? (
        <p className="text-red-500 py-8">{error}</p>
      ) : (
        <div className="w-full">
          <RotationScheduleTable
            schedule={schedule}
            colorMap={colorMap}
            rotationTypes={filteredTypes}
            readOnly={false}
            onRotationChange={handleRotationChange}
            allowResidentReassignment
            residentList={residentList}
            onResidentChange={handleResidentChange}
          />
        </div>
      )}
    </div>
  );
}