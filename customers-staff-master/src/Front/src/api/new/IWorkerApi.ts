import { Guid } from "./dto/Guid";
import { WorkerDto } from "./dto/WorkerDto";
import { WorkerInfoDto } from "./dto/WorkerInfoDto";
import { WorkerListDto } from "./dto/WorkerListDto";

export interface IWorkerApi {
  create(worker: WorkerInfoDto): Promise<WorkerDto>;
  update(workerId: Guid, worker: WorkerInfoDto): Promise<any>;
  updateComment(workerId: Guid, comment: string): Promise<any>;
  read(workerId: Guid): Promise<WorkerDto>;
  delete(workerId: Guid): Promise<any>;
  readAll(version?: number): Promise<WorkerListDto>;
}
