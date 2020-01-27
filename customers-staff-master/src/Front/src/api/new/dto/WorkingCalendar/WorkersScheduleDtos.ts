import { DateTime } from "../DateTime";
import { TimeSpan } from "../TimeSpan";

export interface ShopWorkingCalendarDto {
  version: number;
  month: string;
  workingCalendarMap: { [workerId: string]: WorkingCalendarDayInfoDto[] };
}

export interface WorkingCalendarDayInfoDto {
  date: DateTime;
  records: WorkingCalendarRecordDto[];
}

export interface WorkingCalendarRecordDto {
  period: TimePeriodDto;
}

export interface TimePeriodDto {
  startTime: TimeSpan;
  endTime: TimeSpan;
}

export interface ShopWorkingDayDto {
  version: number;
  date: string;
  workingDayMap: { [workerId: string]: WorkingCalendarRecordDto[] };
}
