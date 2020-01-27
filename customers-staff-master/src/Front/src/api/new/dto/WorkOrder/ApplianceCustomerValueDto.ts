import { BaseCustomerValueDto } from "./BaseCustomerValueDto";

export interface ApplianceCustomerValueDto extends BaseCustomerValueDto {
  name: string;
  brand: string;
  model: string;
  number: string;
  year?: number;
}
