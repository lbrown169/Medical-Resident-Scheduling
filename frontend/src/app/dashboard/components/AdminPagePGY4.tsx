// frontend/src/app/dashboard/components/AdminPagePGY4
"use client";

import React, { useState, useEffect } from "react";
import { Button } from  "../../../components/ui/button";
import { Users, UserX, Calendar } from "lucide-react";
import SubmissionViewDialog from "./SubmissionViewDialog";
import ClearConfirmDialog from "./ClearConfirmDialog";

// Types
interface Submission {
  residentId: string;
  residentName: string;
  dateOfSubmission: string;
}

// Mock data for testing - replace with API call
const MOCK_SUBMISSIONS: Submission[] = [
  { residentId: "1", residentName: "Indresh S.", dateOfSubmission: "12/24/2025" },
  { residentId: "2", residentName: "Lawrence C.", dateOfSubmission: "12/24/2025" },
  { residentId: "3", residentName: "Rachel P.", dateOfSubmission: "12/24/2025" },
  { residentId: "4", residentName: "Rashmi R.", dateOfSubmission: "12/24/2025" },
  { residentId: "5", residentName: "Alexis S.", dateOfSubmission: "12/24/2025" },
  { residentId: "6", residentName: "Carolyn L.", dateOfSubmission: "12/24/2025" },
  { residentId: "7", residentName: "Lauren M.", dateOfSubmission: "12/24/2025" },
];

type TabType = "current-schedule" | "submissions" | "configure";

const AdminPagePGY4: React.FC = () => {
  // State
  const [activeTab, setActiveTab] = useState<TabType>("submissions");
  const [submissions, setSubmissions] = useState<Submission[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [clearDialogOpen, setClearDialogOpen] = useState(false);
  const [isClearing, setIsClearing] = useState(false); 

  // Dialog state
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedResident, setSelectedResident] = useState<{id: string, name: string} | null>(null);

  // Stats
  const [stats] = useState({
    submitted: 24,
    missing: 0,
    submissionDeadline: 0,
  });

  // Fetch submissions on mount
  useEffect(() => {
    fetchSubmissions();
  }, []);

  const fetchSubmissions = async () => {
    setIsLoading(true);
    try {
      // TODO: Replace with actual API call
      // const response = await fetch(`${config.apiUrl}/api/pgy4/submissions`);
      // const data = await response.json();
      // setSubmissions(data);
      
      // Using mock data for now
      setSubmissions(MOCK_SUBMISSIONS);
    } catch (error) {
      console.error("Error fetching submissions:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleViewSubmission = (residentId: string, residentName: string) => {
    setSelectedResident({ id: residentId, name: residentName });
    setDialogOpen(true);
  };

  const handleDeleteSubmission = async (residentId: string) => {
    // TODO: Implement delete functionality
    console.log("Delete submission for resident:", residentId);
    // After delete, refresh the list
    // await fetchSubmissions();
  };

  const handleClearAll = async () => {
    setIsClearing(true);
    try {
      // TODO: Implement actual clear API call
      // await fetch(`${config.apiUrl}/api/pgy4/submissions`, { method: 'DELETE' });
      console.log("Clear all submissions");
      setSubmissions([]); // Clear the local state
    } catch (error) {
      console.error("Error clearing submissions:", error);
    } finally {
      setIsClearing(false);
    }
  };

  const handleGenerateSchedule = () => {
    // TODO: Implement schedule generation
    console.log("Generate new schedule");
  };

  return (
    <div className="w-full max-w-6xl mx-auto p-6">
      {/* Header */}
      <div className="text-center mb-8">
        <h1 className="text-2xl font-bold flex items-center justify-center gap-2">
          <span className="w-6 h-6 border-2 border-gray-400 rounded-full"></span>
          PGY-4 Rotation Dashboard
        </h1>
      </div>

      {/* Stats Cards */}
      <div className="flex justify-center gap-8 mb-8">
        <div className="flex items-center gap-2">
          <Users className="w-5 h-5 text-gray-600" />
          <span className="text-2xl font-bold">{stats.submitted}</span>
          <span className="text-sm text-gray-500">Submitted</span>
        </div>
        <div className="flex items-center gap-2">
          <UserX className="w-5 h-5 text-red-500" />
          <span className="text-2xl font-bold">{stats.missing}</span>
          <span className="text-sm text-gray-500">Missing</span>
        </div>
        <div className="flex items-center gap-2">
          <Calendar className="w-5 h-5 text-gray-600" />
          <span className="text-2xl font-bold">{stats.submissionDeadline}</span>
          <span className="text-sm text-gray-500">Submission Deadline</span>
        </div>
        
        <Button 
          onClick={handleGenerateSchedule}
          className="bg-teal-500 hover:bg-teal-600 text-white rounded-full px-6"
        >
          Generate New Schedule
        </Button>
      </div>

      {/* Tabs */}
      <div className="flex border-b border-gray-200 dark:border-gray-700 mb-6">
        <button
          className={`flex-1 py-3 text-center font-medium transition-colors ${
            activeTab === "current-schedule"
              ? "border-b-2 border-gray-900 dark:border-white text-gray-900 dark:text-white"
              : "text-gray-500 hover:text-gray-700 dark:hover:text-gray-300"
          }`}
          onClick={() => setActiveTab("current-schedule")}
        >
          Current Schedule
        </button>
        <button
          className={`flex-1 py-3 text-center font-medium transition-colors ${
            activeTab === "submissions"
              ? "bg-gray-900 dark:bg-gray-700 text-white rounded-t-lg"
              : "text-gray-500 hover:text-gray-700 dark:hover:text-gray-300"
          }`}
          onClick={() => setActiveTab("submissions")}
        >
          Submissions
        </button>
        <button
          className={`flex-1 py-3 text-center font-medium transition-colors ${
            activeTab === "configure"
              ? "border-b-2 border-gray-900 dark:border-white text-gray-900 dark:text-white"
              : "text-gray-500 hover:text-gray-700 dark:hover:text-gray-300"
          }`}
          onClick={() => setActiveTab("configure")}
        >
          Configure
        </button>
      </div>

      {/* Tab Content */}
      {activeTab === "submissions" && (
        <div className="bg-white dark:bg-neutral-900 rounded-lg border border-gray-200 dark:border-gray-700 p-6">
          {/* Submissions Header */}
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-xl font-semibold">Submissions</h2>
            <Button
              variant="destructive"
              size="sm"
              onClick={() => setClearDialogOpen(true)}
              className="bg-red-500 hover:bg-red-600"
            >
              ✕ Clear
            </Button>
            <ClearConfirmDialog
              open={clearDialogOpen}
              onOpenChange={setClearDialogOpen}
              onConfirm={handleClearAll}
              isLoading={isClearing}
            />
          </div>

          {/* Submissions Table */}
          {isLoading ? (
            <div className="text-center py-10 text-gray-500">Loading submissions...</div>
          ) : (
            <table className="w-full">
              <thead>
                <tr className="text-left text-sm text-gray-500 uppercase tracking-wider">
                  <th className="pb-3 font-medium">Residents</th>
                  <th className="pb-3 font-medium">Date of Submission</th>
                  <th className="pb-3 font-medium">View Submissions</th>
                  <th className="pb-3 font-medium">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100 dark:divide-gray-800">
                {submissions.map((submission) => (
                  <tr key={submission.residentId} className="text-sm">
                    <td className="py-3">{submission.residentName}</td>
                    <td className="py-3 text-gray-500">{submission.dateOfSubmission}</td>
                    <td className="py-3">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleViewSubmission(submission.residentId, submission.residentName)}
                        className="text-xs"
                      >
                        View
                      </Button>
                    </td>
                    <td className="py-3">
                      <button
                        onClick={() => handleDeleteSubmission(submission.residentId)}
                        className="text-red-500 hover:text-red-700 text-sm font-medium"
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}

          {submissions.length === 0 && !isLoading && (
            <div className="text-center py-10 text-gray-500">
              No submissions yet.
            </div>
          )}
        </div>
      )}

      {activeTab === "current-schedule" && (
        <div className="bg-white dark:bg-neutral-900 rounded-lg border border-gray-200 dark:border-gray-700 p-6">
          <h2 className="text-xl font-semibold mb-4">Current Schedule</h2>
          <p className="text-gray-500">Schedule content will be displayed here.</p>
        </div>
      )}

      {activeTab === "configure" && (
        <div className="bg-white dark:bg-neutral-900 rounded-lg border border-gray-200 dark:border-gray-700 p-6">
          <h2 className="text-xl font-semibold mb-4">Configure</h2>
          <p className="text-gray-500">Configuration options will be displayed here.</p>
        </div>
      )}

      {/* Submission View Dialog */}
      {selectedResident && (
        <SubmissionViewDialog
          open={dialogOpen}
          onOpenChange={setDialogOpen}
          residentId={selectedResident.id}
          residentName={selectedResident.name}
        />
      )}
    </div>
  );
};

export default AdminPagePGY4;
