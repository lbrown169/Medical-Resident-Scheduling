"use client";

import React, { useState } from "react";
import { Card } from "../../../components/ui/card";
import { Button } from "../../../components/ui/button";
import { CalendarRange, Users, UserX, CalendarClock, Trash2, Save, Download, X, Calendar } from "lucide-react";
import { ConfirmDialog } from "./ConfirmDialog";


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

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const rotationOptions: { value: string; label: string; color: string }[] = [
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


interface PGY4RotationScheduleProps {
	residents: { id: string; name: string; email: string; pgyLevel: number | string }[];

}



function Modal({ open, onClose, title, children }: { open: boolean; onClose: () => void; title: string; children: React.ReactNode }) {
	if (!open) return null;
	return (
		<div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
			<div className="bg-card p-8 rounded-xl shadow-lg max-w-2xl w-full relative">
				<button onClick={onClose} className="absolute top-4 right-4 text-xl font-bold">&times;</button>
				<h2 className="text-2xl font-bold mb-4">{title}</h2>
				<div className="overflow-y-auto max-h-[60vh]">{children}</div>
			</div>
		</div>
	);
}


const PGY4RotationSchedulePage: React.FC<PGY4RotationScheduleProps> = ({
	residents

}) => {
	const [activeTab, setActiveTab] = useState<'schedule' | 'submissions' | 'configure'>('schedule');
	const [generating] = useState(false);
	const currentYear = new Date().getFullYear();
	const [selectedYear] = useState<number>(currentYear);
	const deadline = new Date("2026-03-15T23:59:00-05:00");


	const [showRotationChangeModal, setShowRotationChangeModal] = useState(false);

	// Extract only PGY-3 residents
	const PGY3Residents = residents.filter (resident =>
		resident.pgyLevel === 3 || resident.pgyLevel === '3'
	);

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
								<span className="text-2xl font-bold text-gray-900 dark:text-white">NULL</span>
							</div>
							<span className="text-xs text-gray-500">Submitted</span>
						</div>
						<div className="flex flex-col items-center border-t sm:border-t-0 sm:border-l border-gray-200 dark:border-gray-700 pt-4 sm:pt-0 sm:pl-8">
							<div className="flex items-center gap-2 mb-1">
								<UserX className="w-5 h-5 text-red-500" />
								<span className="text-2xl font-bold text-gray-900 dark:text-white">NULL</span>
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
								onClick={() => {}}
								disabled={generating}
								className="bg-blue-600 hover:bg-blue-700 text-white font-semibold px-6 py-3 rounded-xl shadow"
							>
								{generating ? "Generating..." : `Generate ${selectedYear} - ${selectedYear + 1} Schedule`}
							</Button>

							{/*
              <ConfirmDialog
                open={confirmOpen}
                onOpenChange={setConfirmOpen}
                title="Generate new schedule?"
                message={`This will overwrite the current schedule for ${selectedYear}. Continue?`}
                confirmText="Generate"
                cancelText="Cancel"
                onConfirm={() => {}}
                loading={generating}
                variant="default"
              />
              */}
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
							onConfirm={() => { }}
							loading={false}
							variant="danger"
						/>
					</div>
				</div>
			</Card>
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
						<div className="flex gap-2">
							<Button onClick={null} variant="outline" className="flex items-center gap-2 px-1 sm:px-6 py-1 sm:py-3 text-xs sm:text-sm lg:text-base">
								<Save className="h-4 w-4" />
								<span>Save</span>
							</Button>
							<Button onClick={null} className="py-2 flex items-center justify-center gap-2 bg-green-500 text-white hover:bg-green-600">
								<Download className="h-4 w-4" />
								<span>Export</span>
							</Button>
						</div>
					</div>
					<div className="overflow-x-auto max-h-96 overflow-y-auto w-full">
						<table className="w-full divide-y divide-gray-200 dark:divide-gray-700">
							<thead className="bg-gray-100 dark:bg-neutral-800 ">
								<tr>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider ">Residents</th>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">JUL &apos;{selectedYear % 100}</th>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">AUG &apos;{selectedYear % 100}</th>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">SEP &apos;{selectedYear % 100}</th>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">OCT &apos;{selectedYear % 100}</th>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">NOV &apos;{selectedYear % 100}</th>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">DEC &apos;{selectedYear % 100}</th>
								<th className="px-1 sm:px-3 py-2 sm:py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">JAN &apos;{(selectedYear + 1) % 100}</th>
								<th className="px-1 sm:px-3 py-2 sm:py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">FEB &apos;{(selectedYear + 1) % 100}</th>
								<th className="px-1 sm:px-3 py-2 sm:py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">MAR &apos;{(selectedYear + 1) % 100}</th>
								<th className="px-1 sm:px-3 py-2 sm:py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">APR &apos;{(selectedYear + 1) % 100}</th>
								<th className="px-1 sm:px-3 py-2 sm:py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">MAY &apos;{(selectedYear + 1) % 100}</th>
								<th className="px-1 sm:px-3 py-2 sm:py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">JUN &apos;{(selectedYear + 1) % 100}</th>
								</tr>
							</thead>
							<tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
								{/* Check if schedule exists*/}
								{1 > 0 ? (
									PGY3Residents.map((PGY3Resident) =>(
										<tr key={PGY3Resident.id} className="hover:bg-gray-50 dark:hover:bg-neutral-800 divide-x divide-gray-200 dark:divide-gray-700">
											<td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100 ">{PGY3Resident.name}</td>

											<td className="px-3 py-2 whitespace-nowrap text-center text-sm font-medium">
											<div className="w-full">
												<Button variant="outline" size="sm" className=" w-full bg-purple-500 text-white hover:bg-purple-700 cursor-pointer"
													onClick={() => setShowRotationChangeModal(true)}>
													Addiction
												</Button>
											</div>
										</td>
										</tr>
									))
								) : (
									<tr>
										<td colSpan={13} className="px-6 py-4 text-center text-gray-500 italic">No schedule generated yet.</td>
									</tr>
								)}
							</tbody>
							
						</table>
					</div>
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
								title="Clear all vacation requests?"
								message="This action cannot be undone."
								confirmText="Clear"
								cancelText="Cancel"
								onConfirm={null}
								variant="danger"
							/>
						</div>
					</div>
					<div className="overflow-x-auto max-h-96 overflow-y-auto w-full">
						<table className="w-full divide-y divide-gray-200 dark:divide-gray-700">
							<thead className="bg-gray-100 dark:bg-neutral-800">
								<tr>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Residents</th>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date of Submission</th>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">View Submissions</th>
									<th className="px-1 sm:px-3 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
								</tr>
							</thead>
							<tbody className="bg-white divide-y divide-gray-200 dark:bg-neutral-900 dark:divide-gray-700">
								{/*check if submissions exists here*/}
								{1 > 0 ? (
									<tr className="hover:bg-gray-50 dark:hover:bg-neutral-800">
										<td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-100">first name last name</td>
										<td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">10-10-1010</td>
										<td className="px-3 py-2 whitespace-nowrap text-sm font-medium">
											<Button variant="outline" size="sm" className="border-gray-300 dark:border-gray-600 rounded text-sm bg-white dark:bg-neutral-800 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500" onClick={() => (null)}>
												View
											</Button>
										</td>
										<td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
											<Button variant="outline" size="sm" className="text-red-600 border-red-600 hover:bg-red-500 hover:text-white" onClick={() => null}>
												Delete
											</Button>
										</td>
									</tr>
								) : (
									<tr>
										<td colSpan={4} className="px-6 py-4 text-center text-gray-500 italic">No submissions yet.</td>
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
					<div className="overflow-x-auto max-h-96 overflow-y-auto w-full">

					</div>

				</Card>
			)}

			{/* Modals*/}
			<Modal
				open={showRotationChangeModal}
				onClose={() => setShowRotationChangeModal(false)}
				title="Change Rotation"
			>
				<div className="p-6 space-y-6">
					<div className="grid grid-cols-4 gap-2">
						<label className="block font-semibold text-forground mb-2">Resident:</label>
						<p>Lawrence C.</p>
						<div></div>
						<div></div>
						<label className="block font-semibold text-forground mb-2">Month:</label>
						<p>June</p>
					</div>

					<div>
						<label className="block font-semibold text-forground mb-2"> Current Rotation</label>
						<div className="w-full px-4 py-3 border border-border rounded-lg bg-background text-foreground">
							<p>Addiction</p>
						</div>
					</div>
					<div>
						<label className="block font-semibold text-forground mb-2">Select New Rotation</label>
						<div className="relative">
							<select className="w-full px-4 py-3 border border-border rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors appearance-none cursor-pointer">
								<option value="inpatient">Inpatient</option>
								<option value="emergency">Emergency Medicine</option>
								<option value="surgery">Surgery</option>
								<option value="pediatrics">Pediatrics</option>
								<option value="icu">ICU</option>
							</select>
							<div className="absolute inset-y-0 right-0 flex items-center px-3 pointer-events-none">
								<svg className="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
									<path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
								</svg>
							</div>
						</div>
					</div>
				</div>

			</Modal>
		</div>

	);
};

export default PGY4RotationSchedulePage;