import { Guid } from "./dto/Guid";
import { TimeZoneDto } from "./dto/TimeZoneDto";

export interface ITimeZoneApi {
  get(): Promise<TimeZoneDto | null>;
  set(timeZoneId: Guid): Promise<void>;
  getList(): Promise<TimeZoneDto[]>;
}
