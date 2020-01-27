import { Guid } from "./dto/Guid";
import {
  CustomerStatusDto,
  RecordStatusDto,
  ServiceCalendarRecordInfoDto,
} from "./dto/ServiceCalendar/ServiceCalendarRecordInfoDto";
import { ServiceCalendarRecordUpdateDto } from "./dto/ServiceCalendar/ServiceCalendarRecordUpdateDto";
import { ShopServiceCalendarDayDto } from "./dto/ServiceCalendar/ShopServiceCalendarDayDto";

export interface IServiceCalendarApi {
  createRecord(date: string, workerId: Guid, record: ServiceCalendarRecordInfoDto): Promise<Guid>;
  updateRecord(date: string, workerId: Guid, recordId: Guid, record: ServiceCalendarRecordUpdateDto): Promise<void>;
  updateRecordStatus(date: string, workerId: Guid, recordId: Guid, newStatus: CustomerStatusDto): Promise<void>;
  getForDay(date: string, status?: RecordStatusDto): Promise<ShopServiceCalendarDayDto>;
}
