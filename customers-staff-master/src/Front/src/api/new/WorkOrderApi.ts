import { b64toBlob } from "../../common/FileHelper";
import HttpClient from "../../common/HttpClient/HttpClient";
import { Guid } from "./dto/Guid";
import { PrintTaskInfoDto } from "./dto/PrintTask/PrintTaskInfoDto";
import { WorkOrderCreateInfoDto } from "./dto/WorkOrder/WorkOrderCreateInfoDto";
import { WorkOrderDto } from "./dto/WorkOrder/WorkOrderDto";
import { WorkOrderItemDto } from "./dto/WorkOrder/WorkOrderItemDto";
import { WorkOrderStatusDto } from "./dto/WorkOrder/WorkOrderStatusDto";
import { IWorkOrderApi } from "./IWorkOrderApi";

export class WorkOrderApi implements IWorkOrderApi {
  private readonly urlPrefix: string;

  constructor(urlPrefix: string, shopId: Guid) {
    this.urlPrefix = `${urlPrefix}/shops/${shopId}/workOrders`;
  }

  public async createOrder(workOrder: WorkOrderDto): Promise<Guid> {
    const result = await HttpClient.post(`${this.urlPrefix}`, workOrder);
    return result as Guid;
  }

  public async getCreateInfoAsync(): Promise<WorkOrderCreateInfoDto> {
    const result = await HttpClient.post(`${this.urlPrefix}/generateNumber`, {});
    return result as WorkOrderCreateInfoDto;
  }

  public async readOrder(orderId: Guid): Promise<WorkOrderDto> {
    const result = await HttpClient.get(`${this.urlPrefix}/${orderId}`, {});
    return result as WorkOrderDto;
  }

  public async readOrders(): Promise<WorkOrderItemDto[]> {
    const result = await HttpClient.get(this.urlPrefix, {});
    return result as WorkOrderItemDto[];
  }

  public async updateOrder(orderId: Guid, order: WorkOrderDto): Promise<void> {
    await HttpClient.put(`${this.urlPrefix}/${orderId}`, order);
  }

  public async updateOrderStatus(orderId: Guid, status: WorkOrderStatusDto): Promise<void> {
    await HttpClient.put(`${this.urlPrefix}/${orderId}/status?workOrderStatus=${status}`, {});
  }

  public async createPrintTask(orderId: Guid, invoice: boolean): Promise<string> {
    return await HttpClient.post(`${this.urlPrefix}/${orderId}/print?invoice=${invoice}`, {});
  }

  public async getTaskStatus(taskId: string): Promise<PrintTaskInfoDto> {
    return await HttpClient.get(`${this.urlPrefix}/printTasks/status/${taskId}`, {});
  }

  public async downloadTaskFile(taskId: string): Promise<Blob> {
    const file = await HttpClient.get(`${this.urlPrefix}/printTasks/file/${taskId}`, {});
    return b64toBlob(file);
  }

  public async removeOrder(orderId: Guid): Promise<void> {
    return await HttpClient.delete(`${this.urlPrefix}/${orderId}`, {});
  }
}
