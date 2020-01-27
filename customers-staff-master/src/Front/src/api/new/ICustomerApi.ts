import { CustomerDto } from "./dto/CustomerDto";
import { CustomerInfoDto } from "./dto/CustomerInfoDto";
import { CustomerListDto } from "./dto/CustomerListDto";
import { Guid } from "./dto/Guid";

export interface ICustomerApi {
  create(customer: CustomerInfoDto): Promise<CustomerDto>;
  update(customerId: Guid, customer: CustomerInfoDto): Promise<any>;
  updateComment(customerId: Guid, comment: string): Promise<any>;
  read(customerId: Guid): Promise<CustomerDto>;
  delete(customerId: Guid): Promise<any>;
  readAll(version?: number): Promise<CustomerListDto>;
}
