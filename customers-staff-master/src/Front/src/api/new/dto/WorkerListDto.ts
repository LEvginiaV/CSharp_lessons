import { WorkerDto } from "./WorkerDto";

export interface WorkerListDto {
  version: number;
  workers: WorkerDto[];
}
