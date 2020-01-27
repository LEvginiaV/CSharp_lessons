import { CustomerDto } from "./CustomerDto";

export interface CustomerListDto {
  version: number;
  customers: CustomerDto[];
}
