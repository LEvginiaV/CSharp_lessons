import { Guid } from "./Guid";
import { WorkerInfoDto } from "./WorkerInfoDto";

export interface WorkerDto extends WorkerInfoDto {
  id: Guid;
  code: number;
}
