import HttpClient from "../../common/HttpClient/HttpClient";

import { CustomerDto } from "./dto/CustomerDto";
import { CustomerInfoDto } from "./dto/CustomerInfoDto";
import { CustomerListDto } from "./dto/CustomerListDto";
import { Guid } from "./dto/Guid";
import { ICustomerApi } from "./ICustomerApi";

export class CustomerApi implements ICustomerApi {
  private urlPrefix: string;

  constructor(urlPrefix: string, shopId: Guid) {
    this.urlPrefix = `${urlPrefix}/shops/${shopId}/customers`;
  }

  public async create(customer: CustomerInfoDto): Promise<CustomerDto> {
    const result = await HttpClient.post(`${this.urlPrefix}`, customer);
    return result as CustomerDto;
  }

  public async update(customerId: Guid, customer: CustomerInfoDto): Promise<any> {
    await HttpClient.put(`${this.urlPrefix}/${customerId}`, customer);
  }

  public async updateComment(customerId: Guid, comment: string): Promise<any> {
    await HttpClient.put(`${this.urlPrefix}/${customerId}/comment`, { comment });
  }

  public async read(customerId: Guid): Promise<CustomerDto> {
    const result = await HttpClient.get(`${this.urlPrefix}/${customerId}`, {});
    return result as CustomerDto;
  }

  public async readAll(version?: number): Promise<CustomerListDto> {
    const result = await HttpClient.get(`${this.urlPrefix}`, { version });
    return result as CustomerListDto;
  }

  public async delete(_customerId: Guid): Promise<any> {
    throw new Error("not implemented");
  }
}
