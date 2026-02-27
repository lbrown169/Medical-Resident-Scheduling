export interface ScheduleStatus {
  id: number;
  description: string;
}

export interface ScheduleSemester {
  id: number;
  name: string;
}

export interface Schedule {
  scheduleId: string;
  status: ScheduleStatus;
  year: number;
  semester: ScheduleSemester;
}

export interface SchedulesByYear {
  [year: number]: Schedule[];
}