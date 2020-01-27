import { ApplianceCustomerValueDto } from "./ApplianceCustomerValueDto";
import { CustomerValueTypeDto } from "./CustomerValueTypeDto";
import { OtherCustomerValueDto } from "./OtherCustomerValueDto";
import { VehicleCustomerValueDto } from "./VehicleCustomerValueDto";

export interface CustomerValueListDto {
  customerValueType: CustomerValueTypeDto;
  customerValues: ApplianceCustomerValueDto[] | VehicleCustomerValueDto[] | OtherCustomerValueDto[];
}
