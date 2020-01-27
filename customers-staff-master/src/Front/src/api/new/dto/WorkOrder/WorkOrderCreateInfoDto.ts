import { WorkOrderNumberDto } from "./WorkOrderNumberDto";

export interface WorkOrderCreateInfoDto {
  orderNumber: WorkOrderNumberDto;
  additionalText?: string;
}
