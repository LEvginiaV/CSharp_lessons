import { DateTime } from "../DateTime";
import { Guid } from "../Guid";
import { CustomerProductDto } from "./CustomerProductDto";
import { CustomerValueListDto } from "./CustomerValueListDto";
import { ShopProductDto } from "./ShopProductDto";
import { ShopRequisitesDto } from "./ShopRequisitesDto";
import { ShopServiceDto } from "./ShopServiceDto";
import { WorkOrderNumberDto } from "./WorkOrderNumberDto";
import { WorkOrderStatusDto } from "./WorkOrderStatusDto";

export interface WorkOrderDto {
  number: WorkOrderNumberDto;
  shopRequisites: ShopRequisitesDto;
  receptionDate: DateTime;
  completionDatePlanned: DateTime;
  completionDateFact?: DateTime;
  warrantyNumber?: string;
  receptionWorkerId?: Guid;
  status: WorkOrderStatusDto;
  clientId: Guid;
  additionalText?: string;
  customerValues: CustomerValueListDto;
  customerProducts: CustomerProductDto[];
  shopProducts: ShopProductDto[];
  shopServices: ShopServiceDto[];
}
