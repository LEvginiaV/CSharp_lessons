import { DateTime } from "../DateTime";
import { WorkerServiceCalendarDayDto } from "./WorkerServiceCalendarDayDto";

export interface ShopServiceCalendarDayDto {
  date: DateTime;
  workerCalendarDays: WorkerServiceCalendarDayDto[];
}
