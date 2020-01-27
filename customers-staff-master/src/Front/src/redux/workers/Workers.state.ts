import { WorkerDto } from "../../api/new/dto/WorkerDto";

export interface WorkersState {
  workers: WorkerDto[] | null;
  version: number;
  positionsMap?: string[];
}
