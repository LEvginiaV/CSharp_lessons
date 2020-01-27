import { Guid } from "./Guid";
import { TimeSpan } from "./TimeSpan";

export interface TimeZoneDto {
  id: Guid;
  name: string;
  offset: TimeSpan;
}
