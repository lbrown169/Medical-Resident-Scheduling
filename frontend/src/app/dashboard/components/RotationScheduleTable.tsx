"use client";
import React, { useState, useRef, useEffect, useLayoutEffect, useMemo } from "react";
import { createPortal } from "react-dom";
import { Button } from "../../../components/ui/button";

export interface RotationTypeEntry {
  rotationTypeId: string;
  rotationName: string;
}

export interface RotationEntry {
  rotationId: string;
  scheduleId?: string;
  month: string;
  academicMonthIndex: number;
  pgyYear: number;
  rotationType: RotationTypeEntry;
}

export interface ResidentScheduleEntry {
  resident: {
    resident_id: string;
    first_name: string;
    last_name: string;
  };
  rotations: RotationEntry[];
}


export interface RotationScheduleTableProps {
  schedule: ResidentScheduleEntry[];
  colorMap: Record<string, string>;
  displayNames?: Record<string, string>;
		/** List of allowed rotation types with their IDs and names for the dropdown */
  rotationTypes: { id: string; name: string }[];
		/** If true, rotation pills are non-interactive colored labels instead of dropdowns */
  readOnly?: boolean;
  allowResidentReassignment?: boolean;
		/** Fires with the rotation type ID (not name) when a rotation is changed */
  onRotationChange?: (residentId: string, monthIndex: number, newRotationTypeId: string) => void;
		
		/**
	 * List of all residents to populate the reassignment dropdown.
	 * Only used when allowResidentReassignment is true.
	 * e.g. [{ id: "ABC123", name: "John D." }]
	 */
  residentList?: { id: string; name: string }[];

		/**
	 * Called when a resident is reassigned to a rotation slot.
	 * Only used when allowResidentReassignment is true.
	 * !!! This has zero effects on PGY4s.
	 */
  onResidentChange?: (rotationId: string, newResidentId: string) => void;
}


const ACADEMIC_MONTHS = [
  "JUL", "AUG", "SEP", "OCT", "NOV", "DEC",
  "JAN", "FEB", "MAR", "APR", "MAY", "JUN",
];


const FALLBACK_COLOR = "#6b7280";



interface RotationDropdownProps {
  residentId: string;
  monthIndex: number;
  rowIndex: number;
  totalRows: number;
  tableHeight: number;
  currentRotation: string;
  rotationTypes: { id: string; name: string }[];
  colorMap: Record<string, string>;
  displayNames: Record<string, string>;
  onSelect: (residentId: string, monthIndex: number, newRotationTypeId: string) => void;
}


function RotationDropdown({
  residentId,
  monthIndex,
  rowIndex,
  totalRows,
  tableHeight,
  currentRotation,
  rotationTypes,
  colorMap,
  displayNames,
  onSelect,
}: RotationDropdownProps) {
  const [open, setOpen] = useState(false);
  const [coords, setCoords] = useState({ top: 0, left: 0, width: 0 });
  const [isLeftAligned, setIsLeftAligned] = useState(true); // Track horizontal flip
  const buttonRef = useRef<HTMLButtonElement>(null);
  const menuRef = useRef<HTMLDivElement>(null);


  const isAbove = rowIndex >= totalRows / 2;
  const dynamicMaxHeight = tableHeight > 0 ? tableHeight / 2 : 200;


  const updatePosition = () => {
    if (buttonRef.current) {
      const rect = buttonRef.current.getBoundingClientRect();
      setCoords({
        top: rect.top,
        left: rect.left,
        width: rect.width,
      });


      // Horizontal Flip Logic: If less than 160px from right edge, flip to right-aligned
      const spaceOnRight = window.innerWidth - rect.left;
      setIsLeftAligned(spaceOnRight > 160);
    }
  };


  useLayoutEffect(() => {
    if (open) {
      updatePosition();
      window.addEventListener("scroll", updatePosition, true);
      window.addEventListener("resize", updatePosition);
    }
    return () => {
      window.removeEventListener("scroll", updatePosition, true);
      window.removeEventListener("resize", updatePosition);
    };
  }, [open]);


  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (
        menuRef.current && !menuRef.current.contains(e.target as Node) &&
        buttonRef.current && !buttonRef.current.contains(e.target as Node)
      ) {
        setOpen(false);
      }
    };
    if (open) document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [open]);


  const color = colorMap[currentRotation] ?? FALLBACK_COLOR;
  const displayName = displayNames[currentRotation] ?? currentRotation;


  const dropdownMenu = (
    <div
      ref={menuRef}
      className="fixed z-[8888] bg-white dark:bg-neutral-800 border border-gray-200 dark:border-gray-700 rounded-lg shadow-lg overflow-hidden"
      style={{
        width: "clamp(100px, 10vw, 140px)",
        // Horizontal logic
        left: isLeftAligned ? `${coords.left}px` : "auto",
        right: isLeftAligned ? "auto" : `${window.innerWidth - (coords.left + coords.width)}px`,
        // Vertical logic
        top: isAbove ? "auto" : `${coords.top + 32}px`,
        bottom: isAbove ? `${window.innerHeight - coords.top + 4}px` : "auto",
        maxHeight: `${dynamicMaxHeight}px`,
								fontSize: "clamp(8px, 0.7vw, 12px)"
      }}
    >
      <div className="overflow-y-auto py-1" style={{ maxHeight: `${dynamicMaxHeight}px` }}>
        {[...rotationTypes]
									.sort((a, b) => {
											const nameA = displayNames[a.name] ?? a.name;
											const nameB = displayNames[b.name] ?? b.name;
											return nameA.localeCompare(nameB);
									}).map((rotation) => (
          <button
            key={rotation.id}
            className={`w-full flex items-center gap-2 px-3 py-2 text-left hover:bg-gray-50 dark:hover:bg-neutral-700 transition-colors ${
              rotation.name === currentRotation ? "font-bold bg-gray-50 dark:bg-neutral-700" : ""
            }`}
            onClick={() => {
              onSelect(residentId, monthIndex, rotation.id);
              setOpen(false);
            }}
          >
            {displayNames[rotation.name] ?? rotation.name}
          </button>
        ))}
      </div>
    </div>
  );


  return (
    <>
      <Button
        ref={buttonRef}
        variant="outline"
        size="sm"
        className="w-[95%] text-white border-2 hover:opacity-80 cursor-pointer font-semibold px-3 py-1 h-auto rounded-full truncate"
        style={{ backgroundColor: color, borderColor: color, fontSize: "clamp(7px, 0.7vw, 14px" }}
        onClick={() => setOpen((prev) => !prev)}
      >
        {displayName}
      </Button>
      {open && createPortal(dropdownMenu, document.body)}
    </>
  );
}


export const RotationScheduleTable: React.FC<RotationScheduleTableProps> = ({
  schedule,
  colorMap,
  displayNames = {},
  rotationTypes,
  readOnly = false,
  allowResidentReassignment = false,
  onRotationChange,
  residentList = [],
  onResidentChange,
}) => {
  const tableContainerRef = useRef<HTMLDivElement>(null);
  const [tableHeight, setTableHeight] = useState(0);


  useLayoutEffect(() => {
    if (tableContainerRef.current) {
      setTableHeight(tableContainerRef.current.clientHeight);
    }
  }, [schedule]);

  const sortedSchedule = useMemo(() => {
    if (allowResidentReassignment) return schedule;
    return [...schedule].sort((a, b) => {
      const lastA = a.resident.last_name.toLowerCase();
      const lastB = b.resident.last_name.toLowerCase();
      if (lastA !== lastB) return lastA.localeCompare(lastB);
      return a.resident.first_name.toLowerCase().localeCompare(b.resident.first_name.toLowerCase());
    });
  }, [schedule, allowResidentReassignment]);


  return (
    <div ref={tableContainerRef} className="overflow-x-auto max-h-[calc(100vh-12rem)] overflow-y-auto w-full">
      <table className="w-full table-fixed divide-y divide-gray-200 dark:divide-gray-700 border-b border-gray-200 dark:border-gray-700">
        <thead className="bg-gray-100 dark:bg-neutral-800">
          <tr>
            <th className="px-3 py-2 text-left font-medium text-gray-500 uppercase tracking-wider sticky left-0 bg-gray-100 dark:bg-neutral-800" style={{ fontSize: "clamp(8px, 0.7vw, 12px)", width: "clamp(50px, 10vw, 110px)" }}>
              Residents
            </th>
            {ACADEMIC_MONTHS.map((month) => (
              <th key={month} className="px-2 py-2 text-center font-medium text-gray-500 uppercase tracking-wider" style={{ fontSize: "clamp(8px, 0.7vw, 12px)"  }}>
                {month}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
          {sortedSchedule.length === 0 ? (
            <tr>
              <td colSpan={13} className="px-6 py-4 text-center text-gray-500 italic">No schedule generated yet.</td>
            </tr>
          ) : (
            sortedSchedule.map((residentSchedule, rowIndex) => {
              const { resident, rotations } = residentSchedule;
              const shortName = `${resident.first_name} ${resident.last_name.charAt(0)}.`;


              return (
                <tr key={resident.resident_id} className="hover:bg-gray-50 dark:hover:bg-neutral-800 divide-x divide-gray-200 dark:divide-gray-700">
                  <td className="px-3 py-2 whitespace-nowrap font-medium text-gray-900 dark:text-gray-100 sticky left-0 bg-white dark:bg-neutral-900 truncate" style={{ fontSize: "clamp(9px, 0.8vw, 14px)" }}>
                    {allowResidentReassignment && residentList.length > 0 ? (
                      <select
                        className=" font-medium bg-transparent border-b border-dotted border-gray-400 focus:outline-none cursor-pointer"
                        value={resident.resident_id}
                        onChange={(e) => {
                          const rotation = rotations[0];
                          if (rotation) onResidentChange?.(rotation.rotationId, e.target.value);
                        }}
                      >
                        {residentList.map((r) => <option key={r.id} value={r.id}>{r.name}</option>)}
                      </select>
                    ) : (
                      shortName
                    )}
                  </td>
                  {ACADEMIC_MONTHS.map((_, monthIndex) => {
                    const rotation = rotations.find((r) => r.academicMonthIndex === monthIndex);
                    return (
                      <td key={monthIndex} className="px-1 py-1.5 whitespace-nowrap text-center">
                        {rotation ? (
                          readOnly ? (
                            <span
                              className="w-[95%] inline-block text-center text-white font-semibold px-3 py-1 rounded-full truncate border-2"
                              style={{ backgroundColor: colorMap[rotation.rotationType.rotationName] ?? FALLBACK_COLOR, borderColor: colorMap[rotation.rotationType.rotationName] ?? FALLBACK_COLOR, fontSize: "clamp(7px, 0.7vw, 12px)" }}
                            >
                              {displayNames[rotation.rotationType.rotationName] ?? rotation.rotationType.rotationName}
                            </span>
                          ) : (
                            <RotationDropdown
                              residentId={resident.resident_id}
                              monthIndex={monthIndex}
                              rowIndex={rowIndex}
                              totalRows={sortedSchedule.length}
                              tableHeight={tableHeight}
                              currentRotation={rotation.rotationType.rotationName}
                              rotationTypes={rotationTypes}
                              colorMap={colorMap}
                              displayNames={displayNames}
                              onSelect={onRotationChange!}
                            />
                          )
                        ) : (
                          <span className="text-gray-300 dark:text-gray-600">—</span>
                        )}
                      </td>
                    );
                  })}
                </tr>
              );
            })
          )}
        </tbody>
      </table>
    </div>
  );
};


export default RotationScheduleTable;
