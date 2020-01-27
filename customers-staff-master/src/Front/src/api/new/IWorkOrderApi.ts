import { Guid } from "./dto/Guid";
import { PrintTaskInfoDto } from "./dto/PrintTask/PrintTaskInfoDto";
import { WorkOrderCreateInfoDto } from "./dto/WorkOrder/WorkOrderCreateInfoDto";
import { WorkOrderDto } from "./dto/WorkOrder/WorkOrderDto";
import { WorkOrderItemDto } from "./dto/WorkOrder/WorkOrderItemDto";
import { WorkOrderStatusDto } from "./dto/WorkOrder/WorkOrderStatusDto";

export interface IWorkOrderApi {
  getCreateInfoAsync(): Promise<WorkOrderCreateInfoDto>;
  createOrder(workOrder: WorkOrderDto): Promise<Guid>;
  updateOrder(orderId: Guid, order: WorkOrderDto): Promise<void>;
  updateOrderStatus(orderId: Guid, status: WorkOrderStatusDto): Promise<void>;
  readOrder(orderId: Guid): Promise<WorkOrderDto>;
  removeOrder(orderId: Guid): Promise<void>;
  readOrders(): Promise<WorkOrderItemDto[]>;
  createPrintTask(orderId: Guid, invoice: boolean): Promise<string>;
  getTaskStatus(taskId: string): Promise<PrintTaskInfoDto>;
  downloadTaskFile(taskId: string): Promise<Blob>;
}
