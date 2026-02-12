"use client";

import React, { useState, useEffect, useCallback } from "react";
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogFooter,
  AlertDialogCancel,
} from "@/components/ui/alert-dialog";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { config } from "../../../config";
import { DateResponse } from "../../../lib/models/DateResponse";

interface CalendarEvent {
  id: string;
  title: string;
  start: Date;
  end: Date;
  backgroundColor: string;
  extendedProps?: {
    scheduleId?: string;
    residentId?: string;
    firstName?: string;
    lastName?: string;
    callType?: string;
    dateId?: string;
    pgyLevel?: number | string;
  };
}

interface Resident {
  resident_id: string;
  first_name: string;
  last_name: string;
  graduate_yr: number;
  email: string;
  phone_number?: string;
  total_hours: number;
}

interface ScheduleEditModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  scheduleId: string | null;
  scheduleYear?: number;
}

const ScheduleEditModal: React.FC<ScheduleEditModalProps> = ({
  open,
  onOpenChange,
  scheduleId,
  scheduleYear,
}) => {
  const [events, setEvents] = useState<CalendarEvent[]>([]);
  const [loading, setLoading] = useState(false);
  const [currentDate, setCurrentDate] = useState(new Date());
  const [residents, setResidents] = useState<Resident[]>([]);
  const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(null);

  const monthNames = [
    "January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
  ];

  const dayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

  const getEventColor = (graduateYear?: number) => {
    switch (graduateYear) {
      case 1: return '#ef4444'; // red for PGY 1
      case 2: return '#f97316'; // orange for PGY 2
      case 3: return '#8b5cf6'; // purple for PGY 3
      default: return '#6b7280'; // gray fallback
    }
  };

  const fetchResidents = useCallback(async () => {
    try {
      const response = await fetch(`${config.apiUrl}/api/Residents`);
      if (response.ok) {
        const data: Resident[] = await response.json();
        setResidents(data);
        return data;
      }
    } catch (error) {
      console.error('Error fetching residents:', error);
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
          const eventColor = getEventColor(graduateYear);

          const d = new Date(date.shiftDate);
          d.setMinutes(d.getMinutes() + d.getTimezoneOffset());

          return {
            id: date.dateId,
            title: fullName || '',
            start: d,
            end: d,
            backgroundColor: eventColor,
            extendedProps: {
              scheduleId: date.scheduleId,
              residentId: date.residentId,
              firstName: date.firstName,
              lastName: date.lastName,
              callType: date.callType.description,
              dateId: date.dateId,
              pgyLevel: graduateYear
            }
          };
        });

        setEvents(calendarEvents);

        // Set initial date to the first month that has events
        if (calendarEvents.length > 0) {
          const firstEventDate = calendarEvents
            .map(e => e.start)
            .sort((a, b) => a.getTime() - b.getTime())[0];
          setCurrentDate(new Date(firstEventDate.getFullYear(), firstEventDate.getMonth(), 1));
        }
      }
    } catch (error) {
      console.error('Error fetching schedule events:', error);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (open && scheduleId) {
      const loadData = async () => {
        const residentList = residents.length > 0 ? residents : await fetchResidents();
        await fetchScheduleEvents(scheduleId, residentList);
      };
      loadData();
    }
  }, [open, scheduleId, fetchResidents, fetchScheduleEvents, residents]);

  // Reset selected event when modal closes
  useEffect(() => {
    if (!open) {
      setSelectedEvent(null);
    }
  }, [open]);

  const generateCalendarDays = () => {
    const firstDayOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    const lastDayOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
    const firstDayOfWeek = firstDayOfMonth.getDay();
    const daysInMonth = lastDayOfMonth.getDate();

    const days: { day: number; isCurrentMonth: boolean; date: Date }[] = [];

    // Add days from previous month
    for (let i = 0; i < firstDayOfWeek; i++) {
      const prevMonthDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 0 - (firstDayOfWeek - 1 - i));
      days.push({ day: prevMonthDate.getDate(), isCurrentMonth: false, date: prevMonthDate });
    }

    // Add days of the month
    for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(currentDate.getFullYear(), currentDate.getMonth(), day);
      days.push({ day, isCurrentMonth: true, date });
    }

    // Add days from next month to fill 6 rows
    const remainingCells = 42 - days.length;
    for (let day = 1; day <= remainingCells; day++) {
      const nextMonthDate = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, day);
      days.push({ day, isCurrentMonth: false, date: nextMonthDate });
    }

    return days;
  };

  const getEventsForDate = (date: Date) => {
    return events.filter(event => {
      const eventDate = event.start;
      return eventDate.getTime() === date.getTime();
    });
  };

  const isToday = (date: Date) => {
    const today = new Date();
    return date.toDateString() === today.toDateString();
  };

  // Schedule range: July of scheduleYear to June of scheduleYear + 1
  const minYM = scheduleYear ? scheduleYear * 12 + 6 : null;  // July (month index 6)
  const maxYM = scheduleYear ? (scheduleYear + 1) * 12 + 5 : null;  // June (month index 5)

  const navigateMonth = (direction: 'prev' | 'next') => {
    const newDate = new Date(currentDate);
    newDate.setMonth(newDate.getMonth() + (direction === 'next' ? 1 : -1));
    if (minYM !== null && maxYM !== null) {
      const newYM = newDate.getFullYear() * 12 + newDate.getMonth();
      if (newYM < minYM || newYM > maxYM) return;
    }
    setCurrentDate(newDate);
  };

  const curYM = currentDate.getFullYear() * 12 + currentDate.getMonth();
  const canNavigatePrev = minYM === null || curYM > minYM;
  const canNavigateNext = maxYM === null || curYM < maxYM;

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

  const calendarDays = generateCalendarDays();

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent className="max-w-6xl max-h-[90vh] overflow-hidden flex flex-col">
        <AlertDialogHeader>
          <AlertDialogTitle>
            Schedule Preview {scheduleYear ? `(${scheduleYear})` : ''}
          </AlertDialogTitle>
        </AlertDialogHeader>

        {loading ? (
          <div className="flex items-center justify-center h-96">
            <p className="text-muted-foreground">Loading schedule...</p>
          </div>
        ) : (
          <div className="flex-1 overflow-auto">
            {/* Calendar Navigation */}
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center gap-2">
                <h2 className="text-xl font-semibold">
                  {monthNames[currentDate.getMonth()]} {currentDate.getFullYear()}
                </h2>
                <button
                  onClick={() => navigateMonth('prev')}
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
                  onClick={() => navigateMonth('next')}
                  disabled={!canNavigateNext}
                  className="p-2 hover:bg-muted rounded-full transition-colors disabled:opacity-30 disabled:cursor-not-allowed disabled:hover:bg-transparent"
                >
                  <ChevronRight className="w-5 h-5" />
                </button>
              </div>
              {/* Legend */}
              <div className="flex items-center gap-4 text-xs">
                <div className="flex items-center gap-1">
                  <div className="w-3 h-3 rounded" style={{ backgroundColor: '#ef4444' }}></div>
                  <span>PGY1</span>
                </div>
                <div className="flex items-center gap-1">
                  <div className="w-3 h-3 rounded" style={{ backgroundColor: '#f97316' }}></div>
                  <span>PGY2</span>
                </div>
                <div className="flex items-center gap-1">
                  <div className="w-3 h-3 rounded" style={{ backgroundColor: '#8b5cf6' }}></div>
                  <span>PGY3</span>
                </div>
              </div>
            </div>

            {/* Calendar Grid */}
            <div className="border rounded-lg overflow-hidden">
              {/* Day Headers */}
              <div className="grid grid-cols-7 bg-muted/50 border-b">
                {dayNames.map((day) => (
                  <div key={day} className="px-2 py-2 text-xs font-semibold text-muted-foreground text-center uppercase">
                    {day.substring(0, 3)}
                  </div>
                ))}
              </div>

              {/* Calendar Days */}
              <div className="grid grid-cols-7">
                {calendarDays.map((dayInfo, index) => {
                  const dayEvents = getEventsForDate(dayInfo.date);
                  const isCurrentDay = isToday(dayInfo.date);
                  const isLastRow = index >= 35;

                  return (
                    <div
                      key={index}
                      className={`min-h-24 border-r border-b p-2 ${
                        !dayInfo.isCurrentMonth
                          ? 'text-muted-foreground bg-muted/30'
                          : 'bg-background'
                      } ${isLastRow ? 'border-b-0' : ''} ${index % 7 === 6 ? 'border-r-0' : ''}`}
                    >
                      <div className={`text-sm font-medium mb-1 ${
                        isCurrentDay
                          ? 'w-6 h-6 bg-primary text-primary-foreground rounded-full flex items-center justify-center text-xs font-bold'
                          : ''
                      }`}>
                        {dayInfo.day}
                      </div>
                      <div className="space-y-1">
                        {dayEvents.slice(0, 3).map((event, eventIndex) => (
                          <div
                            key={eventIndex}
                            className="text-xs px-1.5 py-0.5 rounded truncate cursor-pointer hover:opacity-80 transition-opacity"
                            style={{ backgroundColor: event.backgroundColor, color: 'white' }}
                            onClick={() => setSelectedEvent(event)}
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

            {/* Event Detail Popup */}
            {selectedEvent && (
              <div className="fixed inset-0 z-50 flex items-center justify-center" onClick={() => setSelectedEvent(null)}>
                <div className="absolute inset-0 bg-black/50" />
                <div
                  className="relative bg-background border rounded-lg shadow-lg p-4 max-w-sm w-full mx-4"
                  onClick={(e) => e.stopPropagation()}
                >
                  <div className="flex items-start gap-3">
                    <div
                      className="w-3 h-full min-h-16 rounded"
                      style={{ backgroundColor: selectedEvent.backgroundColor }}
                    />
                    <div className="flex-1">
                      <h3 className="font-semibold text-lg">{selectedEvent.title}</h3>
                      <p className="text-sm text-muted-foreground mt-1">
                        {selectedEvent.start.toLocaleDateString('en-US', {
                          weekday: 'long',
                          year: 'numeric',
                          month: 'long',
                          day: 'numeric'
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
                  <button
                    onClick={() => setSelectedEvent(null)}
                    className="absolute top-2 right-2 p-1 hover:bg-muted rounded"
                  >
                    <span className="sr-only">Close</span>
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                    </svg>
                  </button>
                </div>
              </div>
            )}
          </div>
        )}

        <AlertDialogFooter>
          <AlertDialogCancel>Close</AlertDialogCancel>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
};

export default ScheduleEditModal;