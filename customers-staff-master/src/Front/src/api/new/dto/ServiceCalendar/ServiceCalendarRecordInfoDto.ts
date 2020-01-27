import { Guid } from "../Guid";
import { TimePeriodDto } from "../WorkingCalendar/WorkersScheduleDtos";

export interface ServiceCalendarRecordInfoDto {
  customerId?: Guid;
  productIds: Guid[];
  period: TimePeriodDto;
  comment: string;
}

export enum CustomerStatusDto {
  Active = "active",
  ActiveAccepted = "activeAccepted",
  Completed = "completed",
  CanceledBeforeEvent = "canceledBeforeEvent",
  NotCome = "notCome",
  NoService = "noService",
  Mistake = "mistake",
}

export enum RecordStatusDto {
  Active = "active",
  Canceled = "canceled",
  Removed = "removed",
}
