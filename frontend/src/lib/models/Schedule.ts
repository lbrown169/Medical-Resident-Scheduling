export interface ScheduleStatus {
  id: number;
  description: string;
}

export interface Schedule {
  scheduleId: string;
  status: ScheduleStatus;
  generatedYear: number;
}

export interface SchedulesByYear {
  [year: number]: Schedule[];
}
