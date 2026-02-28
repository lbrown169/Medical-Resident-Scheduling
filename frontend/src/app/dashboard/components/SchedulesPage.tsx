import React, { useState, useEffect, useCallback } from "react";
import { Card } from "../../../components/ui/card";
import { Button } from "../../../components/ui/button";
import { ConfirmDialog } from "../../../components/ui/confirm-dialog";
import { ChevronUp, ChevronDown, Edit, Trash2, CheckSquare, Square, LayoutList, FileText } from "lucide-react";
import { config } from "../../../config";
import { toast } from "../../../lib/use-toast";
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
} from "@/components/ui/dropdown-menu";
import ScheduleEditModal from "./ScheduleEditModal";
import { Schedule, SchedulesByYear } from "../../../lib/models/Schedule";

interface SchedulesPageProps {
  onNavigateToCalendar: () => void;
}

const SchedulesPage: React.FC<SchedulesPageProps> = ({ onNavigateToCalendar }) => {
  const [schedules, setSchedules] = useState<Schedule[]>([]);
  const [loading, setLoading] = useState(true);
  const [expandedYears, setExpandedYears] = useState<Set<number>>(new Set());

  // Generate schedule state
  const now = new Date();
  const currentYear = now.getFullYear();
  const currentMonth = now.getMonth() + 1; // 1-12
  const currentAYStart = currentMonth <= 6 ? currentYear - 1 : currentYear;
  const nextAYStart = currentAYStart + 1;
  // Available semester options: current AY (Fall + Spring) then next AY (Fall + Spring)
  const semesterOptions: { semester: "Fall" | "Spring"; year: number }[] = [
    { semester: "Fall", year: currentAYStart },
    { semester: "Spring", year: currentAYStart + 1 },
    { semester: "Fall", year: nextAYStart },
    { semester: "Spring", year: nextAYStart + 1 },
  ];
  const [selectedYear, setSelectedYear] = useState<number>(semesterOptions[0].year);
  const [selectedSemester, setSelectedSemester] = useState<"Fall" | "Spring">(semesterOptions[0].semester);
  const [generating, setGenerating] = useState(false);
  const [confirmOpen, setConfirmOpen] = useState(false);

  // Delete confirmation state
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);
  const [scheduleToDelete, setScheduleToDelete] = useState<Schedule | null>(null);

  // Publish/unpublish confirmation state
  const [publishConfirmOpen, setPublishConfirmOpen] = useState(false);
  const [scheduleToToggle, setScheduleToToggle] = useState<Schedule | null>(null);

  // Edit modal state
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [selectedScheduleId, setSelectedScheduleId] = useState<string | null>(null);
  const [selectedScheduleYear, setSelectedScheduleYear] = useState<number | undefined>();
  const [selectedScheduleSemester, setSelectedScheduleSemester] = useState<string | undefined>();

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

  // The "start year" is Fall's year; Spring belongs to the previous fall's Academic Year.
  const getAcademicYearStart = (schedule: Schedule): number => {
    return schedule.semester.id === 1 ? schedule.year : schedule.year - 1;
  };

  const getUniqueAcademicYears = (scheduleList: Schedule[]): number[] => {
    const years = [...new Set(scheduleList.map(getAcademicYearStart))];
    return years.sort((a, b) => a - b);
  };

  const groupSchedulesByAcademicYear = (scheduleList: Schedule[]): SchedulesByYear => {
    const grouped: SchedulesByYear = {};
    scheduleList.forEach((schedule) => {
      const ayStart = getAcademicYearStart(schedule);
      if (!grouped[ayStart]) {
        grouped[ayStart] = [];
      }
      grouped[ayStart].push(schedule);
    });
    // Sort within each Academic Year: published first, then Fall before Spring
    Object.values(grouped).forEach((list) =>
      list.sort((a: Schedule, b: Schedule) => {
        const pubDiff = (b.status.id === 1 ? 1 : 0) - (a.status.id === 1 ? 1 : 0);
        if (pubDiff !== 0) return pubDiff;
        return a.semester.id - b.semester.id;
      })
    );
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

  const handleGenerateSchedule = async (year: number, semester: "Fall" | "Spring") => {
    setGenerating(true);
    try {
      const semesterValue = semester === "Fall" ? 1 : 2;
      const response = await fetch(`${config.apiUrl}/api/algorithm/${year}/${semesterValue}/generate`, {
        method: "POST",
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || "Failed to generate schedule");
      }

      toast({
        variant: "success",
        title: "Schedule Generated",
        description: `New ${semester} ${year} schedule generated successfully!`,
      });

      // Refresh the schedules list and expand the academic year
      await fetchSchedules();
      const ayStart = semester === "Fall" ? year : year - 1;
      setExpandedYears((prev) => new Set([...prev, ayStart]));
    } catch (error) {
      console.error("Error generating schedule:", error);
      toast({
        variant: "destructive",
        title: "Error",
        description: error instanceof Error ? error.message : "Failed to generate schedule. Please try again.",
      });
    } finally {
      setGenerating(false);
    }
  };

  const handleEdit = (scheduleId: string, year: number, semester: string) => {
    setSelectedScheduleId(scheduleId);
    setSelectedScheduleYear(year);
    setSelectedScheduleSemester(semester);
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
          description: `A schedule for ${schedule.semester.name} ${schedule.year} is already published. Unpublish it first.`,
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

  const groupedSchedules = groupSchedulesByAcademicYear(schedules);
  const academicYears = getUniqueAcademicYears(schedules);
  const publishedCount = schedules.filter((s) => s.status.id === 1).length;
  if (loading) {
    return (
      <div className="w-full pt-4 flex flex-col items-center">
        <p>Loading schedules...</p>
      </div>
    );
  }

  return (
    <div className="w-full pt-4 flex flex-col items-center px-4 md:pl-8">
      {/* Schedule Overview Card */}
      <Card className="mb-8 p-6 flex flex-col gap-4 items-center justify-between bg-white dark:bg-neutral-900 shadow-lg rounded-2xl border border-gray-200 dark:border-gray-800">
        <h2 className="text-2xl font-bold flex items-center gap-2 justify-center w-full mb-2">
          <LayoutList className="w-6 h-6 text-blue-600" />
          Schedule Management
        </h2>
        <div className="flex flex-col sm:flex-row gap-4 md:gap-8 items-center">
          <div className="flex flex-col items-center">
            <div className="flex items-center gap-2 mb-1">
              <FileText className="w-5 h-5 text-yellow-500" />
              <span className="text-2xl font-bold text-gray-900 dark:text-white">{schedules.length}</span>
            </div>
            <span className="text-xs text-gray-500">Schedules Generated</span>
          </div>
          <div className="flex flex-col items-center border-t sm:border-t-0 sm:border-l border-gray-200 dark:border-gray-700 pt-4 sm:pt-0 sm:pl-8">
            <div className="flex items-center gap-2 mb-1">
              <FileText className="w-5 h-5 text-green-500" />
              <span className="text-2xl font-bold text-gray-900 dark:text-white">{publishedCount}</span>
            </div>
            <span className="text-xs text-gray-500">Schedules Published</span>
          </div>
          <div className="flex flex-col items-center border-t sm:border-t-0 sm:border-l border-gray-200 dark:border-gray-700 pt-4 sm:pt-0 sm:pl-8">
            <div className="flex">
              <Button
                onClick={() => setConfirmOpen(true)}
                disabled={generating}
                className="bg-blue-500 hover:bg-blue-600 text-white px-6 py-5 text-base font-semibold rounded-r-none shadow-md"
              >
                {generating ? "Generating..." : `Generate ${selectedSemester} ${selectedYear}`}
              </Button>
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button
                    className="bg-blue-500 hover:bg-blue-600 text-white px-3 py-5 rounded-l-none border-l border-blue-400 shadow-md"
                    disabled={generating}
                    aria-label="Choose semester and year"
                  >
                    <ChevronDown className="w-5 h-5" />
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end">
                  {semesterOptions.map((opt) => (
                    <DropdownMenuItem
                      key={`${opt.semester}-${opt.year}`}
                      onClick={() => { setSelectedSemester(opt.semester); setSelectedYear(opt.year); }}
                    >
                      {opt.semester} {opt.year}
                    </DropdownMenuItem>
                  ))}
                </DropdownMenuContent>
              </DropdownMenu>
            </div>
          </div>
        </div>
      </Card>

      {/* Generate Confirm Dialog */}
      <ConfirmDialog
        open={confirmOpen}
        onOpenChange={setConfirmOpen}
        title="Generate new schedule?"
        message={`This will create a new ${selectedSemester} schedule for ${selectedYear}. Continue?`}
        confirmText="Generate"
        cancelText="Cancel"
        onConfirm={() => handleGenerateSchedule(selectedYear, selectedSemester)}
        loading={generating}
        variant="default"
      />

      {/* Saved Schedules Section */}
      <Card className="p-4 sm:p-6 lg:p-8 bg-gray-50 dark:bg-neutral-900 shadow-lg rounded-2xl w-full max-w-6xl flex flex-col gap-4 mb-6 sm:mb-8 border border-gray-200 dark:border-gray-800">
        <h2 className="text-lg sm:text-xl font-bold mb-4">Saved Schedules</h2>

        {academicYears.length === 0 ? (
          <div className="p-6 text-center text-gray-500">
            No schedules found. Generate a new schedule to get started.
          </div>
        ) : (
          <div className="space-y-2">
            {academicYears.map((ayStart) => {
              const aySchedules = groupedSchedules[ayStart] || [];
              const publishedSemesters = aySchedules
                .filter((s) => s.status.id === 1)
                .map((s) => s.semester.name);

              return (
                <Card key={ayStart} className="overflow-hidden">
                  {/* Academic Year Header */}
                  <div className="w-full flex items-center justify-between px-4 py-1">
                    <div className="flex items-center gap-3">
                      <span className="text-lg font-medium">Academic Year {ayStart}-{ayStart + 1}</span>
                      {publishedSemesters.length > 0 && (
                        <span className="text-xs bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-300 px-2 py-0.5 rounded">
                          {publishedSemesters.join(" & ")} Published
                        </span>
                      )}
                      <span className="text-sm text-gray-400">
                        ({aySchedules.length} schedule{aySchedules.length !== 1 ? "s" : ""})
                      </span>
                    </div>
                    <button
                      onClick={() => toggleYear(ayStart)}
                      className="p-2 rounded-full border border-gray-300 dark:border-gray-600 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors cursor-pointer"
                    >
                      {expandedYears.has(ayStart) ? (
                        <ChevronUp className="h-5 w-5" />
                      ) : (
                        <ChevronDown className="h-5 w-5" />
                      )}
                    </button>
                  </div>

                  {/* Expanded Content */}
                  {expandedYears.has(ayStart) && (
                    <div className="border-t border-gray-200 dark:border-gray-700 px-4 pb-4">
                      <table className="w-full mt-2">
                        <tbody>
                          {aySchedules.map((schedule, index) => {
                            const isPublished = schedule.status.id === 1;
                            return (
                              <tr
                                key={schedule.scheduleId}
                                className="border-b border-gray-100 dark:border-gray-700 last:border-b-0"
                              >
                                <td className="py-3 pr-4 text-gray-500 w-12">#{index + 1}</td>
                                <td className="py-3 pr-4">
                                  <span className="text-sm font-medium text-gray-700 dark:text-gray-300">
                                    {schedule.semester.name} {schedule.year}
                                  </span>
                                </td>
                                <td className="py-3">
                                  <div className="flex items-center gap-2">
                                    <button
                                      onClick={() => {
                                        setScheduleToToggle(schedule);
                                        setPublishConfirmOpen(true);
                                      }}
                                      className="transition-colors"
                                    >
                                      {isPublished ? (
                                        <CheckSquare className="h-5 w-5 text-green-500 hover:text-green-700 dark:hover:text-green-300" />
                                      ) : (
                                        <Square className="h-5 w-5 text-yellow-500 hover:text-yellow-700 dark:hover:text-yellow-300" />
                                      )}
                                    </button>
                                    {isPublished ? (
                                      <span className="text-green-600 dark:text-green-400">Published</span>
                                    ) : (
                                      <span className="text-yellow-600 dark:text-yellow-400">Under Review</span>
                                    )}
                                  </div>
                                </td>
                                <td className="py-3 text-right">
                                  <div className="flex items-center justify-end gap-4">
                                    <Button variant="outline" size="sm" className="text-blue-600 border-blue-600 hover:bg-blue-500 hover:text-white" onClick={() => handleEdit(schedule.scheduleId, schedule.year, schedule.semester.name)}>
                                      <Edit className="h-4 w-4 mr-2" /> Edit
                                    </Button>
                                    <Button variant="outline" size="sm" className="text-red-600 border-red-600 hover:bg-red-500 hover:text-white" onClick={() => handleDeleteClick(schedule)}>
                                      <Trash2 className="h-4 w-4 mr-2" /> Delete
                                    </Button>
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
      </Card>

      {/* Delete Confirm Dialog */}
      <ConfirmDialog
        open={deleteConfirmOpen}
        onOpenChange={setDeleteConfirmOpen}
        title="Delete Schedule?"
        message={`Are you sure you want to delete this ${scheduleToDelete?.semester.name} ${scheduleToDelete?.year} schedule? This action cannot be undone.`}
        confirmText="Delete"
        cancelText="Cancel"
        onConfirm={handleConfirmDelete}
        variant="danger"
      />

      {/* Publish/Unpublish Confirm Dialog */}
      <ConfirmDialog
        open={publishConfirmOpen}
        onOpenChange={(open) => {
          setPublishConfirmOpen(open);
          if (!open) setScheduleToToggle(null);
        }}
        title={scheduleToToggle?.status.id === 1 ? "Unpublish Schedule?" : "Publish Schedule?"}
        message={
          scheduleToToggle?.status.id === 1
            ? `Are you sure you want to unpublish the ${scheduleToToggle?.semester.name} ${scheduleToToggle?.year} schedule? It will no longer be visible to residents.`
            : `Are you sure you want to publish this ${scheduleToToggle?.semester.name} ${scheduleToToggle?.year} schedule? It will be visible to all residents.`
        }
        confirmText={scheduleToToggle?.status.id === 1 ? "Unpublish" : "Publish"}
        cancelText="Cancel"
        onConfirm={() => {
          if (scheduleToToggle) {
            handleTogglePublished(scheduleToToggle);
          }
          setPublishConfirmOpen(false);
          setScheduleToToggle(null);
        }}
        variant="default"
      />

      {/* Schedule Edit/Preview Modal */}
      <ScheduleEditModal
        open={editModalOpen}
        onOpenChange={setEditModalOpen}
        scheduleId={selectedScheduleId}
        scheduleYear={selectedScheduleYear}
        scheduleSemester={selectedScheduleSemester}
      />
    </div>
  );
};

export default SchedulesPage;
