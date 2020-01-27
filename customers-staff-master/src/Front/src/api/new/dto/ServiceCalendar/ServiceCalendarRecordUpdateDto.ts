import { DateTime } from "../DateTime";
import { Guid } from "../Guid";
import { ServiceCalendarRecordInfoDto } from "./ServiceCalendarRecordInfoDto";

export interface ServiceCalendarRecordUpdateDto extends ServiceCalendarRecordInfoDto {
  updatedDate?: DateTime;
  updatedWorkerId?: Guid;
}
