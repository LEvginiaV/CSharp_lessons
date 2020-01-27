import HttpClient from "../../common/HttpClient/HttpClient";

import { Guid } from "./dto/Guid";
import { WorkerDto } from "./dto/WorkerDto";
import { WorkerInfoDto } from "./dto/WorkerInfoDto";
import { WorkerListDto } from "./dto/WorkerListDto";
import { IWorkerApi } from "./IWorkerApi";

export class WorkerApi implements IWorkerApi {
  private readonly urlPrefix: string;

  constructor(urlPrefix: string, shopId: Guid) {
    this.urlPrefix = `${urlPrefix}/shops/${shopId}/workers`;
  }

  public async create(worker: WorkerInfoDto): Promise<WorkerDto> {
    const result = await HttpClient.post(`${this.urlPrefix}`, worker);
    return result as WorkerDto;
  }

  public async update(workerId: Guid, worker: WorkerInfoDto): Promise<any> {
    await HttpClient.put(`${this.urlPrefix}/${workerId}`, worker);
  }

  public async updateComment(workerId: Guid, comment: string): Promise<any> {
    await HttpClient.put(`${this.urlPrefix}/${workerId}/comment`, { comment });
  }

  public async read(workerId: Guid): Promise<WorkerDto> {
    const result = await HttpClient.get(`${this.urlPrefix}/${workerId}`, {});
    return result as WorkerDto;
  }

  public async readAll(version?: number): Promise<WorkerListDto> {
    const result = await HttpClient.get(`${this.urlPrefix}`, { version });
    return result as WorkerListDto;
  }

  public async delete(workerId: Guid): Promise<any> {
    await HttpClient.delete(`${this.urlPrefix}/${workerId}`, {});
  }
}
