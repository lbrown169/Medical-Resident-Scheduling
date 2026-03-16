"use client";

import { config } from "../../../config";
import React, { useState, useEffect } from "react";


import { Card } from "../../../components/ui/card";
import { Button } from "../../../components/ui/button";
import { ConfirmDialog } from "../../../components/ui/confirm-dialog";
import { SubmissionViewDialog } from "./SubmissionViewDialog";
import { RotationScheduleTable } from "./RotationScheduleTable";

import { CalendarRange, Users, UserX, CalendarClock, Trash2, Save, Download, X, Calendar } from "lucide-react";

// individual responses
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


// check how many submissions exist
interface RotationPrefRequestsListResponse {
  count: number;
  rotationPrefRequests: RotationPrefResponse[];
}

// schedule api types
interface RotationTypeResponse {
	rotationTypeId: string;
	rotationName: string;
}

interface RotationResponse {
	rotationId: string;
	scheduleId?: string;
	month: string;
	academicMonthIndex: number; // july is 0, so on and so forth
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


// Passed into RotationScheduleTable as colorMap
const rotationColorMap: Record<string, string> = {
	"Inpatient Psy": "#8b5cf6",
	"Psy Consults":  "#f97316",
	"VA":            "#60a5fa",
	"TMS":           "#84cc16",
	"NFETC":         "#eab308",
	"IOP":           "#22c55e",
	"Community Psy": "#3b82f6",
	"HPC":           "#92400e",
	"Addiction":     "#14b8a6",
	"Forensic":      "#ef4444",
	"CLC":           "#ec4899",
	"Chief":         "#4b5563",
	"Unassigned":    "#6b7280",
};

// Passed into RotationScheduleTable as displayNames
const rotationDisplayNames: Record<string, string> = {
	"Inpatient Psy": "Inpt Psy",
	"Psy Consults":  "Consult",
	"Community Psy": "Comm",
};

interface PGY4RotationScheduleProps {
	residents: { id: string; name: string; email: string; pgyLevel: number | string }[];
}


const PGY4RotationSchedulePage: React.FC<PGY4RotationScheduleProps> = ({
	residents

}) => {
	const [activeTab, setActiveTab] = useState<'schedule' | 'submissions' | 'configure'>('schedule');
	const currentYear = new Date().getFullYear();
	const [selectedYear] = useState<number>(currentYear);
	const deadline = new Date("2026-03-15T23:59:00-05:00"); // !! change to configured version obviously

	// Schedule state
	const [schedules, setSchedules] = useState<Pgy4RotationScheduleResponse[]>([]);
	const [selectedScheduleId, setSelectedScheduleId] = useState<string>("");
	const [loadingSchedules, setLoadingSchedules] = useState(false);
	const [generating, setGenerating] = useState(false);
	const [scheduleError, setScheduleError] = useState<string | null>(null);
	const [rotationTypeNames, setRotationTypeNames] = useState<string[]>([]);

	const selectedSchedule = schedules.find(s => s.pgy4RotationScheduleId === selectedScheduleId) ?? null;

	// State for viewing a resident's rotation
	// State for viewing a resident's rotation — handled inside RotationScheduleTable

	// Submission
	const [submissions, setSubmissions] = useState<RotationPrefResponse[]>([]);
	const [loadingSubmissions, setLoadingSubmissions] = useState(false);

	// State for viewing a resident's submission
	const [viewDialogOpen, setViewDialogOpen] = useState(false);
	const [viewResident, setViewResident] = useState<RotationPrefResponse | null>(null);

	const handleViewSubmission = (submission: RotationPrefResponse) => {
		setViewResident(submission);
		setViewDialogOpen(true);
	};

	// Helper to map RotationPrefResponse to ResidentPreference
	// Use spread to unpack elements
	const toPreference = (s: RotationPrefResponse) => ({
	residentId: s.resident.resident_id,
	residentName: `${s.resident.first_name} ${s.resident.last_name}`,
	priorities: [
		...s.priorities.map(p => p.rotationName),
		...Array(Math.max(0, 8 - s.priorities.length)).fill(""),
	],
	alternatives: [
		...s.alternatives.map(a => a.rotationName),
		...Array(Math.max(0, 3 - s.alternatives.length)).fill(""),
	],
	avoids: [
		...s.avoids.map(a => a.rotationName),
		...Array(Math.max(0, 3 - s.avoids.length)).fill(""),
	],
	additionalNotes: s.additionalNotes ?? "",
	});


	// Extract only PGY-3 residents
	const PGY3Residents = residents.filter (resident =>
		resident.pgyLevel === 3 || resident.pgyLevel === '3'
	);

	// Submission tracking
	const submittedCount = submissions.length;
	const missingCount = PGY3Residents.length - submittedCount;

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
				const published = list.find(s => s.isPublished);
				setSelectedScheduleId(published?.pgy4RotationScheduleId ?? list[0]?.pgy4RotationScheduleId ?? "");
			} catch (err) {
				console.error(err);
				setScheduleError("Failed to load schedules.");
			} finally {
				setLoadingSchedules(false);
			}
		};

		const loadRotationTypes = async () => {
			try {
				const res = await fetch(`${config.apiUrl}/api/rotation-types?pgyYear=4`);
				if (!res.ok) throw new Error("Failed to fetch rotation types");
				const data: RotationTypesListResponse = await res.json();
				// Filter out no colors.
				// !! This will need to change if we add rotation name swapping. Colors may need to be stored.
				const known = Object.keys(rotationColorMap);
				setRotationTypeNames(
					data.rotationTypes
						.map(rt => rt.rotationName)
						.filter(name => known.includes(name))
				);
			} catch (err) {
				console.error("Failed to load rotation types", err);
				setRotationTypeNames(Object.keys(rotationColorMap));
			}
		};

		loadSchedules();
		loadRotationTypes();
	}, []);

	const handleGenerate = async () => {
		try {
			setGenerating(true);
			setScheduleError(null);
			const res = await fetch(`${config.apiUrl}/api/pgy4-rotation-schedule/generate?count=1`); // !! one each is fine right? we can do 5 if wanted but i feel like 1 is best?
			if (!res.ok) {
				const err: UnsubmittedResidentsResponse = await res.json();
				if (err.unsubmittedResidents?.length > 0) {
					const names = err.unsubmittedResidents.map(r => `${r.first_name} ${r.last_name}`).join(", ");
					setScheduleError(`Missing submissions from: ${names}`);
				} else {
					setScheduleError(err.message ?? "Failed to generate schedule.");
				}
				return;
			}
			const data: Pgy4RotationSchedulesListResponse = await res.json();
			const newSchedules = data.schedules ?? [];
			setSchedules(prev => [...prev, ...newSchedules]);
			if (newSchedules[0]) setSelectedScheduleId(newSchedules[0].pgy4RotationScheduleId);
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
			const res = await fetch(
				`${config.apiUrl}/api/pgy4-rotation-schedule/${selectedScheduleId}`,
				{ method: "DELETE" }
			);
			if (!res.ok) throw new Error("Failed to delete schedule");
			const remaining = schedules.filter(s => s.pgy4RotationScheduleId !== selectedScheduleId);
			setSchedules(remaining);
			setSelectedScheduleId(remaining[0]?.pgy4RotationScheduleId ?? "");
		} catch (err) {
			console.error(err);
			setScheduleError("Failed to delete schedule.");
		}
	};

	const handlePublish = async () => {
		if (!selectedScheduleId) return;
		try {
			setScheduleError(null);
			const res = await fetch(
				`${config.apiUrl}/api/pgy4-rotation-schedule/publish/${selectedScheduleId}`,
				{ method: "POST" }
			);
			if (!res.ok) throw new Error("Failed to publish schedule");
			// Update local state so the published marker reflects immediately
			setSchedules(prev => prev.map(s => ({
				...s,
				isPublished: s.pgy4RotationScheduleId === selectedScheduleId,
			})));
		} catch (err) {
			console.error(err);
			setScheduleError("Failed to publish schedule.");
		}
	};

	useEffect(() => {
		const loadSubmissions = async () => {
			try {
				setLoadingSubmissions(true);

				const res = await fetch(
					`${config.apiUrl}/api/rotation-pref-request`
				);

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
	}, []);

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
								<span className="text-2xl font-bold text-gray-900 dark:text-white">{submittedCount}</span>
							</div>
							<span className="text-xs text-gray-500">Submitted</span>
						</div>
						<div className="flex flex-col items-center border-t sm:border-t-0 sm:border-l border-gray-200 dark:border-gray-700 pt-4 sm:pt-0 sm:pl-8">
							<div className="flex items-center gap-2 mb-1">
								<UserX className="w-5 h-5 text-red-500" />
								<span className="text-2xl font-bold text-gray-900 dark:text-white">{missingCount}</span>
							</div>
							<span className="text-xs text-gray-500">Missing</span>
						</div>
						<div className="flex flex-col items-center border-t sm:border-t-0 sm:border-l border-gray-200 dark:border-gray-700 pt-4 sm:pt-0 sm:pl-8">
							<div className="flex items-center gap-2 mb-1">
								<CalendarClock className="w-5 h-5 text-yellow-500" />
								<span className="whitespace-nowrap text-2xl font-bold text-gray-900 dark:text-white">{deadline.toLocaleDateString('en-US', {
									month: 'short',
									day: 'numeric',
									year: 'numeric'
								})}</span>
							</div>
							<span className="text-xs text-gray-500">Submission Deadline</span>
						</div>
						<div className="h-6 sm:h-10 border-t sm:border-t-0 sm:border-l border-gray-200 dark:border-gray-700 mx-0 sm:mx-4 lg:mx-6 hidden sm:block" />
						<div className="flex items-center">
							<Button
								onClick={handleGenerate}
								disabled={generating || schedules.length >= 5}
								className="bg-blue-600 hover:bg-blue-700 text-white font-semibold px-6 py-3 rounded-xl shadow"
							>
								{generating ? "Generating..." : `Generate ${selectedYear} - ${selectedYear + 1} Schedule`}
							</Button>
						</div>

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
							loading={false}
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
			{/*message && <div className="mb-4 text-center text-sm font-medium text-green-600 dark:text-green-400">{message}</div>*/}

			{/* Tab Navigation */}
			<div className="w-full max-w-6xl flex flex-col sm:flex-row gap-1 sm:gap-2 mb-4 sm:mb-6">
				<Button
					variant={activeTab === 'schedule' ? 'default' : 'outline'}
					className={`flex-1 cursor-pointer rounded-b-none sm:rounded-br-none text-xs sm:text-sm py-1 sm:py-2 ${activeTab === 'schedule' ? 'shadow-md' : ''}`}
					onClick={() => setActiveTab('schedule')}
				>
					Current Schedule
				</Button>
				<Button
					variant={activeTab === 'submissions' ? 'default' : 'outline'}
					className={`flex-1 cursor-pointer rounded-b-none text-xs sm:text-sm py-1 sm:py-2 ${activeTab === 'submissions' ? 'shadow-md' : ''}`}
					onClick={() => setActiveTab('submissions')}
				>
					Submissions
				</Button>
				<Button
					variant={activeTab === 'configure' ? 'default' : 'outline'}
					className={`flex-1 cursor-pointer rounded-b-none text-xs sm:text-sm py-1 sm:py-2 ${activeTab === 'configure' ? 'shadow-md' : ''}`}
					onClick={() => setActiveTab('configure')}
				>
					Configure
				</Button>
			</div>










			{/* Tab Content */}
			{activeTab === 'schedule' && (
				<Card className="p-4 sm:p-6 lg:p-8 bg-gray-50 dark:bg-neutral-900 shadow-lg rounded-2xl w-full flex flex-col gap-4 mb-6 sm:mb-8 border border-gray-200 dark:border-gray-800">
					<div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-4 gap-2">
						<h2 className="text-lg sm:text-xl font-bold">Current Schedule</h2>
						<div className="flex gap-2 items-center">
							{schedules.length > 0 && (
								<>
									<select
										className="px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-primary"
										value={selectedScheduleId}
										onChange={e => setSelectedScheduleId(e.target.value)}
									>
										{schedules.map((s, i) => (
											<option key={s.pgy4RotationScheduleId} value={s.pgy4RotationScheduleId}>
												Schedule {i + 1} (Seed: {s.seed}){s.isPublished ? " ✓ Published" : ""}
											</option>
										))}
									</select>
									<Button
										onClick={handlePublish}
										disabled={selectedSchedule?.isPublished}
										className="bg-blue-600 hover:bg-blue-700 text-white text-xs sm:text-sm px-3 py-2"
									>
										{selectedSchedule?.isPublished ? "Published" : "Publish"}
									</Button>
								</>
							)}
							<Button onClick={() => {}} variant="outline" className="flex items-center gap-2 px-1 sm:px-6 py-1 sm:py-3 text-xs sm:text-sm lg:text-base">
								<Save className="h-4 w-4" />
								<span>Save</span>
							</Button>
							<Button onClick={() => {}} className="py-2 flex items-center justify-center gap-2 bg-green-500 text-white hover:bg-green-600">
								<Download className="h-4 w-4" />
								<span>Export</span>
							</Button>
						</div>
					</div>
					{loadingSchedules ? (
						<div className="py-8 text-center text-gray-500">Loading schedules...</div>
					) : (
						<RotationScheduleTable
							schedule={selectedSchedule?.schedule ?? []}
							colorMap={rotationColorMap}
							displayNames={rotationDisplayNames}
							rotationTypes={rotationTypeNames}
							allowResidentReassignment={false}
							onRotationChange={(residentId, monthIndex, newRotation) => {
								// !!! I think the endpoint for this isnt done currently
								console.log("Rotation change:", residentId, monthIndex, newRotation);
							}}
						/>
					)}
				</Card>
			)}





			{activeTab === 'submissions' && (
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
								onConfirm={() => {}}
								variant="danger"
							/>
						</div>
					</div>
					<div className="overflow-x-auto max-h-[32rem] overflow-y-auto w-full">
						<table className="w-full divide-y divide-gray-200 dark:divide-gray-700">
							<thead className="bg-gray-100 dark:bg-neutral-800">
								<tr>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Residents</th>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date of Submission</th> {/* currently we dont support this. either add support or remove */}
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">View Submissions</th>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
								</tr>
							</thead>
							<tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
								{/*check if submissions exists here*/}
								{/* replaced with checking. boxes are created based on number of submissions */}
								{loadingSubmissions ? (
									<tr>
										<td colSpan={4} className="px-6 py-4 text-center">
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
											{submission.resident?.first_name} {submission.resident?.last_name}
										</td>

										<td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
											—
										</td>

										<td className="px-3 py-2 whitespace-nowrap text-sm font-medium">
											<Button
												variant="outline"
												size="sm"
												onClick={() => handleViewSubmission(submission) /* ! REPLACE WITH POP UP VIEWING WINDOW, this just confirm that it works (it does) */} 
											>
											View
											</Button>
										</td>

										<td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
											<Button
												variant="outline"
												size="sm"
												className="text-red-600 border-red-600 hover:bg-red-500 hover:text-white"
											>
											Delete
											</Button>
										</td>
										</tr>
									))
									) : (
									<tr>
										<td colSpan={4} className="px-6 py-4 text-center text-gray-500 italic">
										No submissions yet.
										</td>
									</tr>
									)}
							</tbody>
						</table>
					</div>
				</Card>
			)}










			{activeTab === 'configure' && (
				<Card className="p-4 sm:p-6 lg:p-8 bg-gray-50 dark:bg-neutral-900 shadow-lg rounded-2xl w-full flex flex-col gap-4 mb-6 sm:mb-8 border border-gray-200 dark:border-gray-800">
					<h2 className="text-lg sm:text-xl font-bold mb-4">Settings</h2>
					<div className="flex flex-col  gap-2 mb-4">
						{/*Form Availabity Selection */}
						<div className="space-y-2">
							<div className="flex items-center gap-2">
								<Calendar className="h-4 w-4 text-primary" />
								<label htmlFor="your-shift-date" className="text-sm font-semibold text-foreground">
									Rotation Form Available Start Date
								</label>
							</div>
							<input
								id="rotation-form-available-start-date"
								type="date"
								className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors"
								value={null}
								onChange={() => null}
								min={new Date().toISOString().split('T')[0]}
							/>
						</div>
						<div className="space-y-2">
							<div className="flex items-center gap-2">
								<Calendar className="h-4 w-4 text-primary" />
								<label htmlFor="your-shift-date" className="text-sm font-semibold text-foreground">
									Rotation Form Due Date
								</label>
							</div>
							<input
								id="rotation-form-due-date"
								type="date"
								className="w-full px-3 py-2.5 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors"
								value={null}
								onChange={() => null}
								min={new Date().toISOString().split('T')[0]}
							/>
						</div>
					</div>
					<h2 className="text-lg sm:text-xl font-bold mb-4">Chief Selection</h2>
					<div className="overflow-x-auto max-h-[32rem] overflow-y-auto w-full">

					</div>

				</Card>
			)}

			{/* Rotation change modal is now inside RotationScheduleTable */}

			{viewResident && (
				<SubmissionViewDialog
					open={viewDialogOpen}
					onOpenChange={setViewDialogOpen}
					residentId={viewResident.resident.resident_id}
					residentName={`${viewResident.resident.first_name} ${viewResident.resident.last_name}`}
					prefetchedData={toPreference(viewResident)}
				/>
			)}
		</div>
	);
};



export default PGY4RotationSchedulePage;
