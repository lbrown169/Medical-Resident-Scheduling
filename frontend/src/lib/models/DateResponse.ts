import { CallType } from "./CallType";

export interface DateResponse {
  dateId: string;
  callType: CallType;
  residentId?: string;
  shiftDate: string;
  scheduleId: string;
  firstName?: string;
  lastName?: string;
}
