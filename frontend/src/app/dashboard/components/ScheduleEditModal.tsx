"use client";

import React, { useState, useEffect, useCallback, useMemo } from "react";
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogFooter,
  AlertDialogCancel,
} from "@/components/ui/alert-dialog";
import { ChevronLeft, ChevronRight, Pencil, Trash2, Plus, X } from "lucide-react";
import { config } from "../../../config";
import { DateResponse } from "../../../lib/models/DateResponse";
import { CallType } from "../../../lib/models/CallType";
import { ConfirmDialog } from "../../../components/ui/confirm-dialog";
import { toast } from "../../../lib/use-toast";
import { CalendarEvent } from "../../../lib/models/CalendarEvent";
import { Resident } from "../../../lib/models/Resident";

interface ScheduleEditModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  scheduleId: string | null;
  scheduleYear?: number;
  scheduleSemester?: string;
}

// Hardcoded until a GET /api/dates/call-types endpoint is added
const callTypeOptions: CallType[] = [
  { id: 0, description: "Short" },
  { id: 1, description: "Saturday (24h)" },
  { id: 2, description: "Saturday (12h)" },
  { id: 3, description: "Sunday (12h)" },
];

const toDateInputValue = (date: Date): string => {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
};

const getEventColor = (graduateYear?: number) => {
  switch (graduateYear) {
    case 1: return "#ef4444";
    case 2: return "#f97316";
    case 3: return "#8b5cf6";
    default: return "#6b7280";
  }
};

const monthNames = [
  "January", "February", "March", "April", "May", "June",
  "July", "August", "September", "October", "November", "December",
];
const dayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

const ScheduleEditModal: React.FC<ScheduleEditModalProps> = ({
  open,
  onOpenChange,
  scheduleId,
  scheduleYear,
  scheduleSemester,
}) => {
  const [events, setEvents] = useState<CalendarEvent[]>([]);
  const [loading, setLoading] = useState(false);
  const [currentDate, setCurrentDate] = useState(new Date());
  const [residents, setResidents] = useState<Resident[]>([]);
  const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(null);

  // Edit/Add state
  const [editMode, setEditMode] = useState<"view" | "edit">("view");
  const [addDay, setAddDay] = useState<Date | null>(null);
  const [formData, setFormData] = useState({ residentId: "", shiftDate: "", callType: 0 });
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);
  // Single state drives all three confirm dialogs
  const [confirmDialog, setConfirmDialog] = useState<null | "add" | "update" | "delete">(null);
  const [hoveredCellIndex, setHoveredCellIndex] = useState<number | null>(null);

  const fetchResidents = useCallback(async () => {
    try {
      const response = await fetch(`${config.apiUrl}/api/Residents`);
      if (response.ok) {
        const data: Resident[] = await response.json();
        setResidents(data);
        return data;
      }
    } catch (error) {
      console.error("Error fetching residents:", error);
    }
    return [];
  }, []);

  const fetchScheduleEvents = useCallback(async (scheduleId: string, residentList: Resident[]) => {
    setLoading(true);
    try {
      const response = await fetch(`${config.apiUrl}/api/dates?schedule_id=${scheduleId}`);
      if (response.ok) {
        const dates: DateResponse[] = await response.json();

        const calendarEvents = dates.map((date: DateResponse) => {
          const fullName = date.firstName && date.lastName
            ? `${date.firstName} ${date.lastName}`
            : date.residentId;

          const resident = residentList.find(r => r.resident_id === date.residentId);
          const graduateYear = resident?.graduate_yr;

          const d = new Date(date.shiftDate);
          d.setMinutes(d.getMinutes() + d.getTimezoneOffset());

          return {
            id: date.dateId,
            title: fullName || "",
            start: d,
            end: d,
            backgroundColor: getEventColor(graduateYear),
            extendedProps: {
              scheduleId: date.scheduleId,
              residentId: date.residentId,
              firstName: date.firstName,
              lastName: date.lastName,
              callType: date.callType.description,
              callTypeId: date.callType.id,
              dateId: date.dateId,
              pgyLevel: graduateYear,
              hours: date.hours,
            },
          };
        });

        setEvents(calendarEvents);

        if (calendarEvents.length > 0) {
          const firstEventDate = calendarEvents
            .map(e => e.start)
            .sort((a, b) => a.getTime() - b.getTime())[0];
          setCurrentDate(new Date(firstEventDate.getFullYear(), firstEventDate.getMonth(), 1));
        }
      }
    } catch (error) {
      console.error("Error fetching schedule events:", error);
    } finally {
      setLoading(false);
    }
  }, []);

  const refreshEvents = useCallback(async () => {
    if (!scheduleId) return;
    const residentList = residents.length > 0 ? residents : await fetchResidents();
    await fetchScheduleEvents(scheduleId, residentList);
  }, [scheduleId, residents, fetchResidents, fetchScheduleEvents]);

  useEffect(() => {
    if (open && scheduleId) {
      const loadData = async () => {
        const residentList = await fetchResidents();
        await fetchScheduleEvents(scheduleId, residentList);
      };
      loadData();
    }
  }, [open, scheduleId, fetchResidents, fetchScheduleEvents]);

  // Reset state when modal closes
  useEffect(() => {
    if (!open) {
      setSelectedEvent(null);
      setEditMode("view");
      setAddDay(null);
      setConfirmDialog(null);
    }
  }, [open]);

  // --- Derived data ---

  // O(1) date lookup: timestamp → events on that day
  const eventsByDate = useMemo(() => {
    const map = new Map<number, CalendarEvent[]>();
    events.forEach(e => {
      const key = e.start.getTime();
      if (!map.has(key)) map.set(key, []);
      map.get(key)!.push(e);
    });
    return map;
  }, [events]);

  // Per-resident scheduled hours for the sidebar
  const residentHours = useMemo(() => {
    const map = new Map<string, { id: string; firstName: string; lastName: string; pgyLevel: number | string | undefined; hours: number; color: string }>();
    events.forEach(e => {
      const id = e.extendedProps?.residentId;
      if (!id) return;
      if (!map.has(id)) {
        map.set(id, {
          id,
          firstName: e.extendedProps?.firstName || "",
          lastName: e.extendedProps?.lastName || "",
          pgyLevel: e.extendedProps?.pgyLevel,
          hours: 0,
          color: e.backgroundColor,
        });
      }
      map.get(id)!.hours += e.extendedProps?.hours ?? 0;
    });
    return Array.from(map.values()).sort((a, b) => {
      if (a.pgyLevel !== b.pgyLevel) return (Number(a.pgyLevel) || 0) - (Number(b.pgyLevel) || 0);
      return `${a.firstName} ${a.lastName}`.localeCompare(`${b.firstName} ${b.lastName}`);
    });
  }, [events]);

  // Residents sorted by PGY then name for the form dropdown
  const sortedResidents = useMemo(() =>
    [...residents].sort((a, b) =>
      a.graduate_yr - b.graduate_yr ||
      `${a.first_name} ${a.last_name}`.localeCompare(`${b.first_name} ${b.last_name}`)
    ),
    [residents]
  );

  // Calendar grid: 42 cells, memoized on month change
  const calendarDays = useMemo(() => {
    const firstDayOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    const lastDayOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
    const firstDayOfWeek = firstDayOfMonth.getDay();
    const daysInMonth = lastDayOfMonth.getDate();

    const days: { day: number; isCurrentMonth: boolean; date: Date }[] = [];

    for (let i = 0; i < firstDayOfWeek; i++) {
      const d = new Date(currentDate.getFullYear(), currentDate.getMonth(), 0 - (firstDayOfWeek - 1 - i));
      days.push({ day: d.getDate(), isCurrentMonth: false, date: d });
    }
    for (let day = 1; day <= daysInMonth; day++) {
      days.push({ day, isCurrentMonth: true, date: new Date(currentDate.getFullYear(), currentDate.getMonth(), day) });
    }
    const remaining = 42 - days.length;
    for (let day = 1; day <= remaining; day++) {
      days.push({ day, isCurrentMonth: false, date: new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, day) });
    }
    return days;
  }, [currentDate]);

  // --- Handlers ---

  const handleEventClick = (event: CalendarEvent) => {
    setAddDay(null);
    setSelectedEvent(event);
    setEditMode("view");
  };

  const handleDayClick = (date: Date, isCurrentMonth: boolean) => {
    if (!isCurrentMonth) return;
    setSelectedEvent(null);
    setEditMode("view");
    setFormData({ residentId: "", shiftDate: toDateInputValue(date), callType: 0 });
    setAddDay(date);
  };

  const openEditMode = () => {
    if (!selectedEvent) return;
    setFormData({
      residentId: selectedEvent.extendedProps?.residentId || "",
      shiftDate: toDateInputValue(selectedEvent.start),
      callType: selectedEvent.extendedProps?.callTypeId ?? 0,
    });
    setEditMode("edit");
  };

  const closePopup = () => {
    setSelectedEvent(null);
    setAddDay(null);
    setEditMode("view");
  };

  // Month range based on semester:
  // Fall: July (index 6) to December (index 11) of scheduleYear
  // Spring: January (index 0) to June (index 5) of scheduleYear
  const minYM = scheduleYear != null
    ? scheduleSemester === "Spring"
      ? scheduleYear * 12 + 0   // January
      : scheduleYear * 12 + 6   // July
    : null;
  const maxYM = scheduleYear != null
    ? scheduleSemester === "Spring"
      ? scheduleYear * 12 + 5   // June
      : scheduleYear * 12 + 11  // December
    : null;

  const createDate = async () => {
    if (!scheduleId || !formData.residentId || !formData.shiftDate) return;
    setSaving(true);
    try {
      const response = await fetch(`${config.apiUrl}/api/dates`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ scheduleId, residentId: formData.residentId, shiftDate: formData.shiftDate, callType: formData.callType }),
      });
      if (response.ok || response.status === 201) {
        toast({ variant: "success", title: "Date Added", description: "The date has been added to the schedule." });
        setAddDay(null);
        await refreshEvents();
      } else {
        const err = await response.json().catch(() => null);
        throw new Error(err?.message || "Failed to add date");
      }
    } catch (error) {
      toast({ variant: "destructive", title: "Error", description: error instanceof Error ? error.message : "Failed to add date." });
    } finally {
      setSaving(false);
    }
  };

  const updateDate = async () => {
    const dateId = selectedEvent?.extendedProps?.dateId;
    if (!dateId || !formData.residentId || !formData.shiftDate) return;
    setSaving(true);
    try {
      const response = await fetch(`${config.apiUrl}/api/dates/${dateId}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ residentId: formData.residentId, shiftDate: formData.shiftDate, callType: formData.callType }),
      });
      if (response.ok) {
        toast({ variant: "success", title: "Date Updated", description: "The date has been updated." });
        setSelectedEvent(null);
        setEditMode("view");
        await refreshEvents();
      } else {
        const err = await response.json().catch(() => null);
        throw new Error(err?.message || "Failed to update date");
      }
    } catch (error) {
      toast({ variant: "destructive", title: "Error", description: error instanceof Error ? error.message : "Failed to update date." });
    } finally {
      setSaving(false);
    }
  };

  const deleteDate = async () => {
    const dateId = selectedEvent?.extendedProps?.dateId;
    if (!dateId) return;
    setDeleting(true);
    try {
      const response = await fetch(`${config.apiUrl}/api/dates/${dateId}`, { method: "DELETE" });
      if (response.ok || response.status === 204) {
        toast({ variant: "success", title: "Date Deleted", description: "The date has been removed from the schedule." });
        setConfirmDialog(null);
        setSelectedEvent(null);
        await refreshEvents();
      } else {
        throw new Error("Failed to delete date");
      }
    } catch (error) {
      toast({ variant: "destructive", title: "Error", description: error instanceof Error ? error.message : "Failed to delete date." });
    } finally {
      setDeleting(false);
    }
  };

  // --- Navigation ---
  const curYM = currentDate.getFullYear() * 12 + currentDate.getMonth();
  const canNavigatePrev = minYM === null || curYM > minYM;
  const canNavigateNext = maxYM === null || curYM < maxYM;

  const navigateMonth = (direction: "prev" | "next") => {
    const newDate = new Date(currentDate);
    newDate.setMonth(newDate.getMonth() + (direction === "next" ? 1 : -1));
    if (minYM !== null && maxYM !== null) {
      const newYM = newDate.getFullYear() * 12 + newDate.getMonth();
      if (newYM < minYM || newYM > maxYM) return;
    }
    setCurrentDate(newDate);
  };

  const goToToday = () => {
    const today = new Date();
    if (minYM !== null && maxYM !== null) {
      const todayYM = today.getFullYear() * 12 + today.getMonth();
      if (todayYM < minYM || todayYM > maxYM) return;
    }
    setCurrentDate(today);
  };

  const isTodayInScheduleYear = minYM === null || maxYM === null || (() => {
    const todayYM = new Date().getFullYear() * 12 + new Date().getMonth();
    return todayYM >= minYM && todayYM <= maxYM;
  })();

  const isToday = (date: Date) => date.toDateString() === new Date().toDateString();

  const dateMin = scheduleYear ? `${scheduleYear}-07-01` : undefined;
  const dateMax = scheduleYear ? `${scheduleYear + 1}-06-30` : undefined;

  // --- Form ---

  const renderForm = (onSave: () => void, onCancel: () => void, title: string) => (
    <div>
      <div className="flex items-center justify-between mb-3">
        <h3 className="font-semibold text-lg">{title}</h3>
        <button onClick={onCancel} className="p-1 hover:bg-muted rounded">
          <X className="w-4 h-4" />
        </button>
      </div>
      <div className="space-y-3">
        <div>
          <label className="text-xs font-medium text-muted-foreground block mb-1">Resident</label>
          <select
            className="w-full border rounded px-2 py-1.5 text-sm bg-background"
            value={formData.residentId}
            onChange={e => setFormData(f => ({ ...f, residentId: e.target.value }))}
          >
            <option value="">Select a resident...</option>
            {sortedResidents.map(r => (
              <option key={r.resident_id} value={r.resident_id}>
                {r.first_name} {r.last_name} (PGY{r.graduate_yr})
              </option>
            ))}
          </select>
        </div>
        <div>
          <label className="text-xs font-medium text-muted-foreground block mb-1">Date</label>
          <input
            type="date"
            className="w-full border rounded px-2 py-1.5 text-sm bg-background"
            value={formData.shiftDate}
            min={dateMin}
            max={dateMax}
            onChange={e => setFormData(f => ({ ...f, shiftDate: e.target.value }))}
          />
        </div>
        <div>
          <label className="text-xs font-medium text-muted-foreground block mb-1">Call Type</label>
          <select
            className="w-full border rounded px-2 py-1.5 text-sm bg-background"
            value={formData.callType}
            onChange={e => setFormData(f => ({ ...f, callType: Number(e.target.value) }))}
          >
            {callTypeOptions.map(ct => (
              <option key={ct.id} value={ct.id}>{ct.description}</option>
            ))}
          </select>
        </div>
        <div className="flex gap-2 pt-1">
          <button
            onClick={onSave}
            disabled={saving || !formData.residentId || !formData.shiftDate}
            className="flex-1 px-3 py-1.5 text-sm font-medium bg-primary text-primary-foreground rounded hover:bg-primary/90 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            {saving ? "Saving..." : "Save"}
          </button>
          <button
            onClick={onCancel}
            disabled={saving}
            className="flex-1 px-3 py-1.5 text-sm font-medium border rounded hover:bg-muted transition-colors"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );

  return (
    <>
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent className="max-w-6xl max-h-[90vh] overflow-hidden flex flex-col">
        <AlertDialogHeader>
          <AlertDialogTitle>
            Schedule Preview {scheduleSemester && scheduleYear ? `(${scheduleSemester} ${scheduleYear})` : ''}
          </AlertDialogTitle>
        </AlertDialogHeader>

        {loading ? (
          <div className="flex items-center justify-center h-96">
            <p className="text-muted-foreground">Loading schedule...</p>
          </div>
        ) : (
          <div className="flex-1 overflow-hidden flex gap-4">

            {/* Calendar area */}
            <div className="flex-1 overflow-auto min-w-0">

              {/* Navigation */}
              <div className="flex items-center justify-between mb-4">
                <div className="flex items-center gap-2">
                  <h2 className="text-xl font-semibold">
                    {monthNames[currentDate.getMonth()]} {currentDate.getFullYear()}
                  </h2>
                  <button
                    onClick={() => navigateMonth("prev")}
                    disabled={!canNavigatePrev}
                    className="p-2 hover:bg-muted rounded-full transition-colors disabled:opacity-30 disabled:cursor-not-allowed disabled:hover:bg-transparent"
                  >
                    <ChevronLeft className="w-5 h-5" />
                  </button>
                  {isTodayInScheduleYear && (
                    <button
                      onClick={goToToday}
                      className="px-3 py-1 text-sm font-medium text-primary hover:bg-primary/10 rounded-lg transition-colors"
                    >
                      Today
                    </button>
                  )}
                  <button
                    onClick={() => navigateMonth("next")}
                    disabled={!canNavigateNext}
                    className="p-2 hover:bg-muted rounded-full transition-colors disabled:opacity-30 disabled:cursor-not-allowed disabled:hover:bg-transparent"
                  >
                    <ChevronRight className="w-5 h-5" />
                  </button>
                </div>
                <div className="flex items-center gap-4">
                  <div className="flex items-center gap-3 text-xs">
                    <div className="flex items-center gap-1">
                      <div className="w-3 h-3 rounded" style={{ backgroundColor: "#ef4444" }} />
                      <span>PGY1</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <div className="w-3 h-3 rounded" style={{ backgroundColor: "#f97316" }} />
                      <span>PGY2</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <div className="w-3 h-3 rounded" style={{ backgroundColor: "#8b5cf6" }} />
                      <span>PGY3</span>
                    </div>
                  </div>
                  <span className="text-xs text-muted-foreground">Click a date to add new shift</span>
                </div>
              </div>

              {/* Calendar Grid */}
              <div className="border rounded-lg overflow-hidden">
                <div className="grid grid-cols-7 bg-muted/50 border-b">
                  {dayNames.map((day) => (
                    <div key={day} className="px-2 py-2 text-xs font-semibold text-muted-foreground text-center uppercase">
                      {day.substring(0, 3)}
                    </div>
                  ))}
                </div>

                <div className="grid grid-cols-7">
                  {calendarDays.map((dayInfo, index) => {
                    const dayEvents = eventsByDate.get(dayInfo.date.getTime()) ?? [];
                    const isCurrentDay = isToday(dayInfo.date);
                    const isLastRow = index >= 35;

                    return (
                      <div
                        key={index}
                        className={`min-h-32 border-r border-b p-2 transition-colors ${
                          !dayInfo.isCurrentMonth
                            ? "text-muted-foreground bg-muted/30"
                            : "bg-background cursor-pointer hover:bg-muted/20"
                        } ${isLastRow ? "border-b-0" : ""} ${index % 7 === 6 ? "border-r-0" : ""}`}
                        onClick={() => handleDayClick(dayInfo.date, dayInfo.isCurrentMonth)}
                        onMouseEnter={() => dayInfo.isCurrentMonth && setHoveredCellIndex(index)}
                        onMouseLeave={() => setHoveredCellIndex(null)}
                      >
                        <div className="flex items-center justify-between mb-1">
                          <div className={`text-sm font-medium ${
                            isCurrentDay
                              ? "w-6 h-6 bg-primary text-primary-foreground rounded-full flex items-center justify-center text-xs font-bold"
                              : ""
                          }`}>
                            {dayInfo.day}
                          </div>
                          {dayInfo.isCurrentMonth && (
                            <div className={`w-5 h-5 rounded-full bg-blue-500 flex items-center justify-center transition-opacity ${hoveredCellIndex === index ? "opacity-100" : "opacity-0"}`}>
                              <Plus className="w-3 h-3 text-white" />
                            </div>
                          )}
                        </div>
                        <div className="space-y-1">
                          {dayEvents.slice(0, 3).map((event, eventIndex) => (
                            <div
                              key={eventIndex}
                              className="text-xs px-1.5 py-0.5 rounded truncate cursor-pointer hover:opacity-80 transition-opacity"
                              style={{ backgroundColor: event.backgroundColor, color: "white" }}
                              onClick={(e) => { e.stopPropagation(); handleEventClick(event); }}
                              onMouseEnter={(e) => { e.stopPropagation(); setHoveredCellIndex(null); }}
                              onMouseLeave={(e) => { e.stopPropagation(); setHoveredCellIndex(index); }}
                              title={`${event.title} - ${event.extendedProps?.callType}`}
                            >
                              {event.title}
                            </div>
                          ))}
                          {dayEvents.length > 3 && (
                            <div className="text-xs text-muted-foreground px-1.5 py-0.5 bg-muted/50 rounded text-center">
                              +{dayEvents.length - 3} more
                            </div>
                          )}
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>

              {/* Event Detail / Edit Popup */}
              {selectedEvent && (
                <div className="fixed inset-0 z-50 flex items-center justify-center" onClick={closePopup}>
                  <div className="absolute inset-0 bg-black/50" />
                  <div
                    className="relative bg-background border rounded-lg shadow-lg p-4 max-w-sm w-full mx-4"
                    onClick={(e) => e.stopPropagation()}
                  >
                    {editMode === "view" ? (
                      <>
                        <div className="flex items-start gap-3">
                          <div
                            className="w-3 h-full min-h-16 rounded flex-shrink-0"
                            style={{ backgroundColor: selectedEvent.backgroundColor }}
                          />
                          <div className="flex-1">
                            <h3 className="font-semibold text-lg">{selectedEvent.title}</h3>
                            <p className="text-sm text-muted-foreground mt-1">
                              {selectedEvent.start.toLocaleDateString("en-US", {
                                weekday: "long", year: "numeric", month: "long", day: "numeric",
                              })}
                            </p>
                            <p className="text-sm mt-2">
                              <span className="font-medium">Call Type:</span> {selectedEvent.extendedProps?.callType}
                            </p>
                            {selectedEvent.extendedProps?.pgyLevel && (
                              <p className="text-sm">
                                <span className="font-medium">PGY Level:</span> {selectedEvent.extendedProps.pgyLevel}
                              </p>
                            )}
                          </div>
                        </div>
                        <div className="flex gap-2 mt-4">
                          <button
                            onClick={openEditMode}
                            className="flex-1 flex items-center justify-center gap-1.5 px-3 py-1.5 text-sm font-medium border border-blue-600 text-blue-600 rounded hover:bg-blue-500 hover:text-white transition-colors"
                          >
                            <Pencil className="w-3.5 h-3.5" />
                            Edit
                          </button>
                          <button
                            onClick={() => setConfirmDialog("delete")}
                            className="flex-1 flex items-center justify-center gap-1.5 px-3 py-1.5 text-sm font-medium border border-red-600 text-red-600 rounded hover:bg-red-500 hover:text-white transition-colors"
                          >
                            <Trash2 className="w-3.5 h-3.5" />
                            Delete
                          </button>
                        </div>
                        <button
                          onClick={closePopup}
                          className="absolute top-2 right-2 p-1 hover:bg-muted rounded"
                        >
                          <span className="sr-only">Close</span>
                          <X className="w-4 h-4" />
                        </button>
                      </>
                    ) : (
                      renderForm(() => setConfirmDialog("update"), () => setEditMode("view"), "Edit Date")
                    )}
                  </div>
                </div>
              )}

              {/* Add Date Popup */}
              {addDay && !selectedEvent && (
                <div className="fixed inset-0 z-50 flex items-center justify-center" onClick={() => setAddDay(null)}>
                  <div className="absolute inset-0 bg-black/50" />
                  <div
                    className="relative bg-background border rounded-lg shadow-lg p-4 max-w-sm w-full mx-4"
                    onClick={(e) => e.stopPropagation()}
                  >
                    {renderForm(() => setConfirmDialog("add"), () => setAddDay(null), "Add Date")}
                  </div>
                </div>
              )}
            </div>

            {/* Resident Hours Sidebar */}
            <div className="w-52 flex-shrink-0 overflow-auto border-l pl-4">
              <h3 className="text-sm font-semibold mb-3">Residents ({residentHours.length})</h3>
              <div className="space-y-2">
                {residentHours.map((r) => (
                  <div key={r.id} className="flex items-center justify-between gap-2">
                    <div className="flex items-center gap-1.5 min-w-0">
                      <div className="w-2.5 h-2.5 rounded-full flex-shrink-0" style={{ backgroundColor: r.color }} />
                      <span className="text-xs truncate">{r.firstName} {r.lastName}</span>
                    </div>
                    <div className="flex items-center gap-1.5 flex-shrink-0 text-xs text-muted-foreground">
                      <span>PGY{r.pgyLevel}</span>
                      <span className="font-medium text-foreground">{r.hours}h</span>
                    </div>
                  </div>
                ))}
                {residentHours.length === 0 && (
                  <p className="text-xs text-muted-foreground">No shifts scheduled.</p>
                )}
              </div>
            </div>
          </div>
        )}

        <AlertDialogFooter>
          <AlertDialogCancel>Close</AlertDialogCancel>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>

    {/* Confirm dialogs — must live outside AlertDialog to avoid dialog nesting */}
    <ConfirmDialog
      open={confirmDialog === "add"}
      onOpenChange={(open) => { if (!open) setConfirmDialog(null); }}
      title="Add Date?"
      message={`Add a shift for ${residents.find(r => r.resident_id === formData.residentId)?.first_name ?? "this resident"} on ${formData.shiftDate}?`}
      confirmText="Add"
      cancelText="Cancel"
      onConfirm={createDate}
      loading={saving}
      variant="default"
    />
    <ConfirmDialog
      open={confirmDialog === "update"}
      onOpenChange={(open) => { if (!open) setConfirmDialog(null); }}
      title="Save Changes?"
      message={`Save changes to ${selectedEvent?.title}'s shift?`}
      confirmText="Save"
      cancelText="Cancel"
      onConfirm={updateDate}
      loading={saving}
      variant="default"
    />
    <ConfirmDialog
      open={confirmDialog === "delete"}
      onOpenChange={(open) => { if (!open) setConfirmDialog(null); }}
      title="Delete Date?"
      message={`Are you sure you want to remove ${selectedEvent?.title}'s shift on ${selectedEvent?.start.toLocaleDateString("en-US", { month: "long", day: "numeric", year: "numeric" })}? This cannot be undone.`}
      confirmText="Delete"
      cancelText="Cancel"
      onConfirm={deleteDate}
      loading={deleting}
      variant="danger"
    />
    </>
  );
};

export default ScheduleEditModal;
