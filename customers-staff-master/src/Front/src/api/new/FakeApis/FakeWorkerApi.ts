import * as uuid from "uuid/v4";
import { Guid } from "../dto/Guid";
import { WorkerDto } from "../dto/WorkerDto";
import { WorkerInfoDto } from "../dto/WorkerInfoDto";
import { WorkerListDto } from "../dto/WorkerListDto";
import { IWorkerApi } from "../IWorkerApi";
import { fakeWorkers } from "./fakeData";
//tslint:disable

const sleep: (timeout: number) => Promise<any> = (timeout: number) => {
  return new Promise(r => setTimeout(r, timeout));
};

export class FakeWorkerApi implements IWorkerApi {
  private version: number;
  private code: number;
  private workers: WorkerDto[];

  constructor() {
    this.workers = fakeWorkers;
    this.version = 1;
    this.code = 1000;
  }

  public async create(worker: WorkerInfoDto): Promise<WorkerDto> {
    console.log("Create", worker);
    await sleep(100);
    const result: WorkerDto = {
      ...worker,
      id: uuid(),
      code: this.code++,
    };
    this.workers.push(result);
    this.version++;

    return result;
  }

  public async update(workerId: Guid, worker: WorkerInfoDto): Promise<any> {
    console.log("Update", workerId, worker);
    await sleep(100);
    if (this.workers.findIndex(x => x.id === workerId) < 0) {
      throw new Error("404");
    }

    this.workers = this.workers.map(x => {
      if (x.id === workerId) {
        return { ...x, ...worker };
      }
      return x;
    });
    this.version++;
  }

  public async updateComment(workerId: Guid, comment: string): Promise<any> {
    console.log("UpdateComment", workerId, comment);
    await sleep(100);
    if (this.workers.findIndex(x => x.id === workerId) < 0) {
      throw new Error("404");
    }

    this.workers = this.workers.map(x => {
      if (x.id === workerId) {
        return { ...x, additionalInfo: comment };
      }
      return x;
    });
    this.version++;
  }

  public async read(workerId: Guid): Promise<WorkerDto> {
    console.log("Update", workerId);
    await sleep(100);
    const index = this.workers.findIndex(x => x.id === workerId);
    if (index < 0) {
      throw new Error("404");
    }

    return this.workers[index];
  }

  public async readAll(version?: number): Promise<WorkerListDto> {
    console.log("readAll", version, this.version);
    await sleep(100);
    const result = {
      version: this.version,
      workers: version == null || version < this.version ? this.workers : [],
    };
    console.log("result", result);
    return result;
  }

  public async delete(workerId: Guid): Promise<any> {
    console.log("delete", workerId);
    await sleep(100);
    if (this.workers.findIndex(x => x.id === workerId) < 0) {
      throw new Error("404");
    }
    this.workers = this.workers.filter(x => x.id !== workerId);
    this.version++;
  }
}
