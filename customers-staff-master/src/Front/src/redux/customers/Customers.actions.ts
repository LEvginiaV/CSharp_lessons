import { CustomerDto } from "../../api/new/dto/CustomerDto";

export enum CustomersActionTypes {
  SetCustomers = "SetCustomers",
}

export interface CustomersActions {
  type: CustomersActionTypes.SetCustomers;
  customers: CustomerDto[];
  version: number;
}
