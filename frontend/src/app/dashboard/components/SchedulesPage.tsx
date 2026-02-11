import React, { useState, useEffect, useCallback } from "react";
import { Card } from "../../../components/ui/card";
import { Button } from "../../../components/ui/button";
import { ChevronUp, ChevronDown, Edit, Trash2, CheckSquare, Square } from "lucide-react";
import { config } from "../../../config";
import { toast } from "../../../lib/use-toast";
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
} from "@/components/ui/dropdown-menu";
import { ConfirmDialog } from "./ConfirmDialog";
import ScheduleEditModal from "./ScheduleEditModal";

interface ScheduleStatus {
  id: number;
  description: string;
}

interface Schedule {
  scheduleId: string;
  status: ScheduleStatus;
  generatedYear: number;
}

interface SchedulesByYear {
  [year: number]: Schedule[];
}

interface SchedulesPageProps {
  onNavigateToCalendar: () => void;
}

const SchedulesPage: React.FC<SchedulesPageProps> = ({ onNavigateToCalendar }) => {
  const [schedules, setSchedules] = useState<Schedule[]>([]);
  const [loading, setLoading] = useState(true);
  const [expandedYears, setExpandedYears] = useState<Set<number>>(new Set());

  // Generate schedule state
  const currentYear = new Date().getFullYear();
  const [selectedYear, setSelectedYear] = useState<number>(currentYear);
  const [generating, setGenerating] = useState(false);
  const [confirmOpen, setConfirmOpen] = useState(false);

  // Delete confirmation state
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);
  const [scheduleToDelete, setScheduleToDelete] = useState<Schedule | null>(null);

  // Edit modal state
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [selectedScheduleId, setSelectedScheduleId] = useState<string | null>(null);
  const [selectedScheduleYear, setSelectedScheduleYear] = useState<number | undefined>();

  const fetchSchedules = useCallback(async () => {
    try {
      const response = await fetch(`${config.apiUrl}/api/schedules`);
      if (response.ok) {
        const data: Schedule[] = await response.json();
        setSchedules(data);
      } else {
        setSchedules([]);
      }
    } catch (error) {
      console.error("Error fetching schedules:", error);
      setSchedules([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchSchedules();
  }, [fetchSchedules]);

  const getUniqueYears = (scheduleList: Schedule[]): number[] => {
    const years = [...new Set(scheduleList.map((s) => s.generatedYear))];
    return years.sort((a, b) => a - b); // Sort ascending
  };

  const groupSchedulesByYear = (scheduleList: Schedule[]): SchedulesByYear => {
    const grouped: SchedulesByYear = {};
    scheduleList.forEach((schedule) => {
      if (!grouped[schedule.generatedYear]) {
        grouped[schedule.generatedYear] = [];
      }
      grouped[schedule.generatedYear].push(schedule);
    });
    return grouped;
  };

  const toggleYear = (year: number) => {
    setExpandedYears((prev) => {
      const newSet = new Set(prev);
      if (newSet.has(year)) {
        newSet.delete(year);
      } else {
        newSet.add(year);
      }
      return newSet;
    });
  };

  const handleGenerateSchedule = async (year: number) => {
    setGenerating(true);
    try {
      const response = await fetch(`${config.apiUrl}/api/algorithm/training/${year}`, {
        method: "POST",
      });

      if (!response.ok) {
        throw new Error("Failed to generate schedule");
      }

      toast({
        variant: "success",
        title: "Schedule Generated",
        description: `New schedule for ${year} generated successfully!`,
      });

      // Refresh the schedules list and expand the new year
      await fetchSchedules();
      setExpandedYears((prev) => new Set([...prev, year]));
    } catch (error) {
      console.error("Error generating schedule:", error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to generate schedule. Please try again.",
      });
    } finally {
      setGenerating(false);
    }
  };

  const handleEdit = (scheduleId: string, year: number) => {
    setSelectedScheduleId(scheduleId);
    setSelectedScheduleYear(year);
    setEditModalOpen(true);
  };

  const handleDeleteClick = (schedule: Schedule) => {
    // Check if the schedule is published (status.id === 1)
    if (schedule.status.id === 1) {
      toast({
        variant: "destructive",
        title: "Cannot Delete Published Schedule",
        description: "You must unpublish the schedule before deleting it.",
      });
      return;
    }

    // If it's under review, show confirmation dialog
    setScheduleToDelete(schedule);
    setDeleteConfirmOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!scheduleToDelete) return;

    try {
      const response = await fetch(`${config.apiUrl}/api/schedules/${scheduleToDelete.scheduleId}`, {
        method: "DELETE",
      });
      if (response.ok || response.status === 204) {
        toast({
          variant: "success",
          title: "Schedule Deleted",
          description: "The schedule has been deleted successfully.",
        });
        fetchSchedules();
      } else {
        throw new Error("Failed to delete schedule");
      }
    } catch (error) {
      console.error("Error deleting schedule:", error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to delete schedule. Please try again.",
      });
    } finally {
      setDeleteConfirmOpen(false);
      setScheduleToDelete(null);
    }
  };

  const handleTogglePublished = async (schedule: Schedule) => {
    const isCurrentlyPublished = schedule.status.id === 1;
    const newStatus = isCurrentlyPublished ? 0 : 1; // Toggle: 0 = UnderReview, 1 = Published

    try {
      const response = await fetch(`${config.apiUrl}/api/schedules/${schedule.scheduleId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ status: newStatus }),
      });

      if (response.ok) {
        toast({
          variant: "success",
          title: isCurrentlyPublished ? "Schedule Unpublished" : "Schedule Published",
          description: `The schedule has been ${isCurrentlyPublished ? "unpublished" : "published"}.`,
        });
        await fetchSchedules();

        // Navigate to the calendar to show updated schedule
        onNavigateToCalendar();
      } else if (response.status === 409) {
        // Conflict - another schedule is already published for this year
        toast({
          variant: "destructive",
          title: "Cannot Publish",
          description: `A schedule for ${schedule.generatedYear} is already published. Unpublish it first.`,
        });
      } else {
        throw new Error("Failed to update schedule");
      }
    } catch (error) {
      console.error("Error updating schedule:", error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to update schedule. Please try again.",
      });
    }
  };

  const groupedSchedules = groupSchedulesByYear(schedules);
  const years = getUniqueYears(schedules);

  if (loading) {
    return (
      <div className="w-full pt-4 flex flex-col items-center">
        <p>Loading schedules...</p>
      </div>
    );
  }

  return (
    <div className="w-full max-w-4xl mx-auto pt-4">
      {/* Generate New Schedule Button with Year Dropdown */}
      <div className="flex justify-center mb-8">
        <div className="flex">
          <Button
            onClick={() => setConfirmOpen(true)}
            disabled={generating}
            className="bg-blue-500 hover:bg-blue-600 text-white px-8 py-6 text-lg rounded-r-none"
          >
            {generating ? "Generating..." : `Generate ${selectedYear} Schedule`}
          </Button>
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                className="bg-blue-500 hover:bg-blue-600 text-white px-3 py-6 rounded-l-none border-l border-blue-400"
                disabled={generating}
                aria-label="Choose year"
              >
                <ChevronDown className="w-5 h-5" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuItem onClick={() => setSelectedYear(currentYear - 1)}>
                Generate for {currentYear - 1}
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => setSelectedYear(currentYear)}>
                Generate for {currentYear}
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => setSelectedYear(currentYear + 1)}>
                Generate for {currentYear + 1}
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => setSelectedYear(currentYear + 2)}>
                Generate for {currentYear + 2}
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>

      {/* Confirm Dialog */}
      <ConfirmDialog
        open={confirmOpen}
        onOpenChange={setConfirmOpen}
        title="Generate new schedule?"
        message={`This will create a new schedule for ${selectedYear}. Continue?`}
        confirmText="Generate"
        cancelText="Cancel"
        onConfirm={() => handleGenerateSchedule(selectedYear)}
        loading={generating}
        variant="default"
      />

      {/* Saved Schedules Section */}
      <div className="mb-4">
        <h2 className="text-xl text-gray-400 mb-4">Saved Schedules</h2>

        {years.length === 0 ? (
          <Card className="p-6 text-center text-gray-500">
            No schedules found. Generate a new schedule to get started.
          </Card>
        ) : (
          <div className="space-y-2">
            {years.map((year) => {
              const yearSchedules = groupedSchedules[year] || [];
              const publishedSchedule = yearSchedules.find((s) => s.status.id === 1);

              return (
                <Card key={year} className="overflow-hidden">
                  {/* Year Header */}
                  <button
                    className="w-full flex items-center justify-between p-4 hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors"
                    onClick={() => toggleYear(year)}
                  >
                    <div className="flex items-center gap-3">
                      <span className="text-lg font-medium">{year}</span>
                      {publishedSchedule && (
                        <span className="text-xs bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-300 px-2 py-0.5 rounded">
                          Published
                        </span>
                      )}
                      <span className="text-sm text-gray-400">
                        ({yearSchedules.length} schedule{yearSchedules.length !== 1 ? "s" : ""})
                      </span>
                    </div>
                    {expandedYears.has(year) ? (
                      <ChevronUp className="h-5 w-5" />
                    ) : (
                      <ChevronDown className="h-5 w-5" />
                    )}
                  </button>

                  {/* Expanded Content */}
                  {expandedYears.has(year) && (
                    <div className="border-t border-gray-200 dark:border-gray-700 px-4 pb-4">
                      <table className="w-full mt-2">
                        <tbody>
                          {yearSchedules.map((schedule, index) => {
                            const isPublished = schedule.status.id === 1;
                            return (
                              <tr
                                key={schedule.scheduleId}
                                className="border-b border-gray-100 dark:border-gray-700 last:border-b-0"
                              >
                                <td className="py-3 pr-4 text-gray-500 w-12">#{index + 1}</td>
                                <td className="py-3">
                                  <button
                                    onClick={() => handleTogglePublished(schedule)}
                                    className="flex items-center gap-2 hover:opacity-80"
                                  >
                                    {isPublished ? (
                                      <>
                                        <CheckSquare className="h-5 w-5 text-green-500" />
                                        <span className="text-green-600 dark:text-green-400">Published</span>
                                      </>
                                    ) : (
                                      <>
                                        <Square className="h-5 w-5 text-gray-400" />
                                        <span className="text-gray-500 dark:text-gray-400">Under Review</span>
                                      </>
                                    )}
                                  </button>
                                </td>
                                <td className="py-3 text-right">
                                  <div className="flex items-center justify-end gap-4">
                                    <button
                                      onClick={() => handleEdit(schedule.scheduleId, schedule.generatedYear)}
                                      className="flex items-center gap-1 text-blue-500 hover:text-blue-600 cursor-pointer"
                                    >
                                      Edit
                                      <Edit className="h-4 w-4" />
                                    </button>
                                    <button
                                      onClick={() => handleDeleteClick(schedule)}
                                      className="flex items-center gap-1 text-red-500 hover:text-red-600 cursor-pointer"
                                    >
                                      Delete
                                      <Trash2 className="h-4 w-4" />
                                    </button>
                                  </div>
                                </td>
                              </tr>
                            );
                          })}
                        </tbody>
                      </table>
                    </div>
                  )}
                </Card>
              );
            })}
          </div>
        )}
      </div>

      {/* Delete Confirmation Dialog */}
      <ConfirmDialog
        open={deleteConfirmOpen}
        onOpenChange={setDeleteConfirmOpen}
        title="Delete Schedule?"
        message={`Are you sure you want to delete this schedule for ${scheduleToDelete?.generatedYear}? This action cannot be undone.`}
        confirmText="Delete"
        cancelText="Cancel"
        onConfirm={handleConfirmDelete}
        variant="danger"
      />

      {/* Schedule Edit/Preview Modal */}
      <ScheduleEditModal
        open={editModalOpen}
        onOpenChange={setEditModalOpen}
        scheduleId={selectedScheduleId}
        scheduleYear={selectedScheduleYear}
      />
    </div>
  );
};

export default SchedulesPage;
