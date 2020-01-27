import { DateTime } from "../../../../api/new/dto/DateTime";

export interface WorkingCalendarDayViewInfo {
  date: DateTime;
  timePeriods: TimePeriodViewInfo[];
}

export interface TimePeriodViewInfo {
  start: string;
  end: string;
  overflow: boolean;
}
