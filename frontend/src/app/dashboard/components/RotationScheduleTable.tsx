"use client";

import React, { useState, useRef, useEffect } from "react";
import { Button } from "../../../components/ui/button";



export interface RotationTypeEntry {
	rotationTypeId: string;
	rotationName: string;
}

export interface RotationEntry {
	rotationId: string;
	scheduleId?: string;
	month: string;
	academicMonthIndex: number; // 0 = July, 11 = June
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
	 * !!! Since PGY4s don't need this. Algo revision will need to change this.
	 * !!! This has zero effects on PGY4s, so do whatever you need.
	 */
	onResidentChange?: (rotationId: string, newResidentId: string) => void;
}

// Constants !!! if algo uses different months, add this as prop. I assume its the same between us though?
const ACADEMIC_MONTHS = [
	"JUL", "AUG", "SEP", "OCT", "NOV", "DEC",
	"JAN", "FEB", "MAR", "APR", "MAY", "JUN",
];

const FALLBACK_COLOR = "#6b7280";

// Rotation Dropdown

interface RotationDropdownProps {
	residentId: string;
	monthIndex: number;
	currentRotation: string;
	rotationTypes: { id: string; name: string }[];
	colorMap: Record<string, string>;
	displayNames: Record<string, string>;
	onSelect: (residentId: string, monthIndex: number, newRotationTypeId: string) => void;
}

function RotationDropdown({
	residentId,
	monthIndex,
	currentRotation,
	rotationTypes,
	colorMap,
	displayNames,
	onSelect,
}: RotationDropdownProps) {
	const [open, setOpen] = useState(false);
	const ref = useRef<HTMLDivElement>(null);

	useEffect(() => {
		const handleClickOutside = (e: MouseEvent) => {
			if (ref.current && !ref.current.contains(e.target as Node)) {
				setOpen(false);
			}
		};
		if (open) document.addEventListener("mousedown", handleClickOutside);
		return () => document.removeEventListener("mousedown", handleClickOutside);
	}, [open]);

	const color = colorMap[currentRotation] ?? FALLBACK_COLOR;
	const displayName = displayNames[currentRotation] ?? currentRotation;

	return (
		<div ref={ref} className="relative inline-block">
			<Button
				variant="outline"
				size="sm"
				className="text-white hover:opacity-80 cursor-pointer text-xs font-semibold px-3 py-1 h-auto rounded-full"
				style={{ backgroundColor: color, borderColor: color }}
				onClick={() => setOpen(prev => !prev)}
			>
				{displayName}
			</Button>

			{open && (
				<div className="absolute z-50 top-full mt-1 left-0 min-w-[140px] bg-white dark:bg-neutral-800 border border-gray-200 dark:border-gray-700 rounded-lg shadow-lg overflow-hidden">
					{rotationTypes.map(rotation => {
						const optDisplay = displayNames[rotation.name] ?? rotation.name;
						const isCurrent = rotation.name === currentRotation;
						return (
							<button
								key={rotation.id}
								className={`w-full flex items-center gap-2 px-3 py-2 text-xs text-left hover:bg-gray-50 dark:hover:bg-neutral-700 transition-colors ${isCurrent ? "font-bold bg-gray-50 dark:bg-neutral-700" : ""}`}
								onClick={() => {
									onSelect(residentId, monthIndex, rotation.id);
									setOpen(false);
								}}
							>
								{optDisplay}
							</button>
						);
					})}
				</div>
			)}
		</div>
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
	const handleSelect = (residentId: string, monthIndex: number, newRotation: string) => {
		onRotationChange?.(residentId, monthIndex, newRotation);
	};

	return (
		<div className="overflow-x-auto max-h-[32rem] overflow-y-auto w-full">
			<table className="w-full divide-y divide-gray-200 dark:divide-gray-700">
				<thead className="bg-gray-100 dark:bg-neutral-800">
					<tr>
						<th className="px-3 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider sticky left-0 bg-gray-100 dark:bg-neutral-800">
							Residents
						</th>
						{ACADEMIC_MONTHS.map(month => (
							<th key={month} className="px-2 py-2 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">
								{month}
							</th>
						))}
					</tr>
				</thead>
				<tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
					{schedule.length === 0 ? (
						<tr>
							<td colSpan={13} className="px-6 py-4 text-center text-gray-500 italic">
								No schedule generated yet.
							</td>
						</tr>
					) : (
						schedule.map(residentSchedule => {
							const { resident, rotations } = residentSchedule;
							const shortName = `${resident.first_name} ${resident.last_name.charAt(0)}.`;

							return (
								<tr
									key={resident.resident_id}
									className="hover:bg-gray-50 dark:hover:bg-neutral-800 divide-x divide-gray-200 dark:divide-gray-700"
								>
									<td className="px-3 py-2 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100 sticky left-0 bg-white dark:bg-neutral-900">
										{allowResidentReassignment && residentList.length > 0 ? (
											<select
												className="text-sm font-medium bg-transparent border-b border-dotted border-gray-400 focus:outline-none cursor-pointer"
												value={resident.resident_id}
												onChange={e => {
													// !!! This needs to change for Algo Revision group.
													// Currently passes the first rotationId in the row as a placeholder.
													// You will likely need to change what data is passed here.
													const rotation = rotations[0];
													if (rotation) onResidentChange?.(rotation.rotationId, e.target.value);
												}}
											>
												{residentList.map(r => (
													<option key={r.id} value={r.id}>{r.name}</option>
												))}
											</select>
										) : (
											shortName
										)}
									</td>

									{ACADEMIC_MONTHS.map((_, monthIndex) => {
										const rotation = rotations.find(r => r.academicMonthIndex === monthIndex);

										return (
											<td key={monthIndex} className="px-1 py-1.5 whitespace-nowrap text-center">
												{rotation ? (
													readOnly ? (
														<span
															className="text-white text-xs font-semibold px-3 py-1 rounded-full"
															style={{
																backgroundColor: colorMap[rotation.rotationType.rotationName] ?? FALLBACK_COLOR,
															}}
														>
															{displayNames[rotation.rotationType.rotationName] ?? rotation.rotationType.rotationName}
														</span>
													) : (
														<RotationDropdown
															residentId={resident.resident_id}
															monthIndex={monthIndex}
															currentRotation={rotation.rotationType.rotationName}
															rotationTypes={rotationTypes}
															colorMap={colorMap}
															displayNames={displayNames}
															onSelect={handleSelect}
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
