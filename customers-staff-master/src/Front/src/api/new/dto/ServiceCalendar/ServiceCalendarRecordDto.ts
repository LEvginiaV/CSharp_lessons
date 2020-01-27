import { Guid } from "../Guid";
import { CustomerStatusDto, RecordStatusDto, ServiceCalendarRecordInfoDto } from "./ServiceCalendarRecordInfoDto";

export interface ServiceCalendarRecordDto extends ServiceCalendarRecordInfoDto {
  id: Guid;
  recordStatus: RecordStatusDto;
  customerStatus: CustomerStatusDto;
}
