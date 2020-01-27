import { Guid } from "./dto/Guid";
import {
  ShopWorkingCalendarDto,
  ShopWorkingDayDto,
  WorkingCalendarDayInfoDto,
} from "./dto/WorkingCalendar/WorkersScheduleDtos";

export interface IWorkingCalendarApi {
  getForMonth(month: Date | null, version: number | null): Promise<ShopWorkingCalendarDto>;
  getForDay(day: Date, version: number | null): Promise<ShopWorkingDayDto>;
  update(workerId: Guid, updates: WorkingCalendarDayInfoDto[]): Promise<void>;
}
