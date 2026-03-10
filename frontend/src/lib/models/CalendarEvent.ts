export interface CalendarEvent {
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
