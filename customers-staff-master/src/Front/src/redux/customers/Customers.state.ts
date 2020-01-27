import { CustomerDto } from "../../api/new/dto/CustomerDto";

export interface CustomersState {
  customers: CustomerDto[] | null;
  version: number;
}
