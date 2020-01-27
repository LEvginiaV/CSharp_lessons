import { DateTime } from "../DateTime";
import { Guid } from "../Guid";
import { ServiceCalendarRecordDto } from "./ServiceCalendarRecordDto";

export interface WorkerServiceCalendarDayDto {
  workerId: Guid;
  date: DateTime;
  records: ServiceCalendarRecordDto[];
}
