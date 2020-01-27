import { WorkerDto } from "../../api/new/dto/WorkerDto";

export enum WorkersActionTypes {
  SetWorkers = "SetWorkers",
}

export interface WorkersActions {
  type: WorkersActionTypes.SetWorkers;
  workers: WorkerDto[];
  version: number;
  positionsMap?: string[];
}
