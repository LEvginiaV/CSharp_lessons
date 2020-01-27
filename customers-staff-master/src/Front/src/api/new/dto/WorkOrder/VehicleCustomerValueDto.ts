import { BaseCustomerValueDto } from "./BaseCustomerValueDto";

export interface VehicleCustomerValueDto extends BaseCustomerValueDto{
  vin: string;
  brand: string;
  model: string;
  registerSign: string;
  year?: number;
  bodyNumber: string;
  engineNumber: string;
}
