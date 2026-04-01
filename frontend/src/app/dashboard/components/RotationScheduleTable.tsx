"use client";

import React, { useState, useRef, useEffect, useCallback } from "react";
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
	 * !!! This has zero effects on PGY4s.
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
	const buttonRef = useRef<HTMLButtonElement>(null);
	const dropdownRef = useRef<HTMLDivElement>(null);
	const [pos, setPos] = useState<{ top: number; left: number; width: number; flipUp: boolean }>({ top: 0, left: 0, width: 160, flipUp: false });

	const updatePosition = useCallback(() => {
		if (!buttonRef.current) return;
		const rect = buttonRef.current.getBoundingClientRect();
		const dropdownHeight = 200;
		const spaceBelow = window.innerHeight - rect.bottom;
		const flipUp = spaceBelow < dropdownHeight && rect.top > dropdownHeight;
		setPos({
			top: flipUp ? rect.top : rect.bottom + 4,
			left: rect.right - Math.max(160, rect.width),
			width: Math.max(160, rect.width),
			flipUp,
		});
	}, []);

	useEffect(() => {
		if (!open) return;
		updatePosition();
		const handleClickOutside = (e: MouseEvent) => {
			if (
				dropdownRef.current && !dropdownRef.current.contains(e.target as Node) &&
				buttonRef.current && !buttonRef.current.contains(e.target as Node)
			) {
				setOpen(false);
			}
		};
		const handleScroll = () => updatePosition();
		document.addEventListener("mousedown", handleClickOutside);
		window.addEventListener("scroll", handleScroll, true);
		return () => {
			document.removeEventListener("mousedown", handleClickOutside);
			window.removeEventListener("scroll", handleScroll, true);
		};
	}, [open, updatePosition]);

	const color = colorMap[currentRotation] ?? FALLBACK_COLOR;
	const displayName = displayNames[currentRotation] ?? currentRotation;

	return (
		<>
			<Button
				ref={buttonRef}
				variant="outline"
				size="sm"
				className="text-white hover:opacity-80 cursor-pointer font-semibold px-1.5 py-0.5 h-auto rounded-full whitespace-nowrap"
				style={{ backgroundColor: color, borderColor: color, fontSize: "clamp(7px, 0.7vw, 12px)" }}
				onClick={() => setOpen(prev => !prev)}
			>
				{displayName}
			</Button>

			{open && createPortal(
				<div
					ref={dropdownRef}
					className="fixed z-[9999] bg-white dark:bg-neutral-800 border border-gray-200 dark:border-gray-700 rounded-lg shadow-lg max-h-48 overflow-y-auto overscroll-contain"
					style={{
						top: pos.flipUp ? undefined : pos.top,
						bottom: pos.flipUp ? window.innerHeight - pos.top + 4 : undefined,
						left: pos.left,
						width: pos.width,
					}}
				>
					{rotationTypes.map(rotation => {
						const optDisplay = displayNames[rotation.name] ?? rotation.name;
						const isCurrent = rotation.name === currentRotation;
						return (
							<button
								key={rotation.id}
								className={`w-full flex items-center gap-2 px-2 py-1.5 text-xs text-left hover:bg-gray-50 dark:hover:bg-neutral-700 transition-colors ${isCurrent ? "font-bold bg-gray-50 dark:bg-neutral-700" : ""}`}
								onClick={() => {
									onSelect(residentId, monthIndex, rotation.id);
									setOpen(false);
								}}
							>
								{optDisplay}
							</button>
						);
					})}
				</div>,
				document.body
			)}
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
	const handleSelect = (residentId: string, monthIndex: number, newRotation: string) => {
		onRotationChange?.(residentId, monthIndex, newRotation);
	};

	return (
		<div className="max-h-[calc(100vh-12rem)] overflow-y-auto w-full">
			<table className="w-full table-fixed divide-y divide-gray-200 dark:divide-gray-700">
				<thead className="bg-gray-100 dark:bg-neutral-800">
					<tr>
						<th className="px-2 py-2 text-left font-medium text-gray-500 uppercase tracking-wider sticky left-0 bg-gray-100 dark:bg-neutral-800" style={{ fontSize: "clamp(8px, 0.7vw, 12px)" }}>
							Residents
						</th>
						{ACADEMIC_MONTHS.map(month => (
							<th key={month} className="px-1 py-2 text-center font-medium text-gray-500 uppercase tracking-wider" style={{ fontSize: "clamp(8px, 0.7vw, 12px)" }}>
								{month}
							</th>
						))}
					</tr>
				</thead>
				<tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
					{schedule.length === 0 ? (
						<tr>
							<td colSpan={13} className="px-6 py-4 text-center text-gray-500 italic">
								No rotations found. Use the &quot;Copy&quot; button above to copy a rotation schedule.
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
									<td className="px-2 py-1.5 font-medium text-gray-900 dark:text-gray-100 sticky left-0 bg-white dark:bg-neutral-900 overflow-hidden text-ellipsis whitespace-nowrap" style={{ fontSize: "clamp(9px, 0.8vw, 14px)" }}>
										{allowResidentReassignment && residentList.length > 0 ? (
											<select
												className="w-full font-medium bg-transparent border-b border-dotted border-gray-400 focus:outline-none cursor-pointer truncate" style={{ fontSize: "inherit" }}
												value={resident.resident_id}
												onChange={e => {
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
											<td key={monthIndex} className="px-0.5 py-1 text-center whitespace-nowrap">
												{rotation ? (
													readOnly ? (
														<span
															className="text-white font-semibold px-1.5 py-0.5 rounded-full inline-block whitespace-nowrap"
															style={{
																backgroundColor: colorMap[rotation.rotationType.rotationName] ?? FALLBACK_COLOR,
																fontSize: "clamp(7px, 0.7vw, 12px)",
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
