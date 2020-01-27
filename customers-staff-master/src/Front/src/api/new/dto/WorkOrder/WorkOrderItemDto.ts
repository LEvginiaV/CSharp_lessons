import { DateTime } from "../DateTime";
import { Guid } from "../Guid";
import { WorkOrderNumberDto } from "./WorkOrderNumberDto";
import { WorkOrderStatusDto } from "./WorkOrderStatusDto";

export interface WorkOrderItemDto {
  id: Guid;
  number: WorkOrderNumberDto;
  status: WorkOrderStatusDto;
  receptionDate: DateTime;
  totalSum: number;
  firstProductId?: Guid;
  clientId: Guid;
}
