import * as uuid from "uuid/v4";
import { CustomerDto } from "../dto/CustomerDto";
import { CustomerInfoDto } from "../dto/CustomerInfoDto";
import { CustomerListDto } from "../dto/CustomerListDto";
import { Guid } from "../dto/Guid";
import { ICustomerApi } from "../ICustomerApi";
import { fakeCustomers } from "./fakeData";

//tslint:disable

const sleep: (timeout: number) => Promise<any> = (timeout: number) => {
  return new Promise(r => setTimeout(r, timeout));
};

export class FakeCustomerApi implements ICustomerApi {
  private version: number;
  private customers: CustomerDto[];

  constructor() {
    this.customers = fakeCustomers;
    this.version = 1;
  }

  public async create(customer: CustomerInfoDto): Promise<CustomerDto> {
    console.log("create", customer);
    await sleep(100);
    const result: CustomerDto = {
      ...customer,
      id: uuid(),
    };
    this.customers.push(result);
    this.version++;
    return result;
  }

  public async update(customerId: Guid, customer: CustomerInfoDto): Promise<any> {
    console.log("update", customerId, customer);
    await sleep(100);
    if (this.customers.findIndex(x => x.id === customerId) < 0) {
      throw new Error("404");
    }

    this.customers = this.customers.map(x => {
      if (x.id === customerId) {
        return { ...x, ...customer };
      }
      return x;
    });
    this.version++;
  }

  public async updateComment(customerId: Guid, comment: string): Promise<any> {
    console.log("updateComment", customerId, comment);
    await sleep(100);
    if (this.customers.findIndex(x => x.id === customerId) < 0) {
      throw new Error("404");
    }

    this.customers = this.customers.map(x => {
      if (x.id === customerId) {
        return { ...x, additionalInfo: comment };
      }
      return x;
    });
    this.version++;
  }

  public async read(customerId: Guid): Promise<CustomerDto> {
    console.log("read", customerId);
    await sleep(100);
    const index = this.customers.findIndex(x => x.id === customerId);
    if (index < 0) {
      throw new Error("404");
    }
    return this.customers[index];
  }

  public async readAll(version?: number): Promise<CustomerListDto> {
    console.log("readAll", version);
    await sleep(100);
    return {
      version: this.version,
      customers: version != null && version < this.version ? this.customers : [],
    };
  }

  public async delete(customerId: Guid): Promise<any> {
    console.log("delete", customerId);
    await sleep(100);
    if (this.customers.findIndex(x => x.id === customerId) < 0) {
      throw new Error("404");
    }
    this.customers = this.customers.filter(x => x.id !== customerId);
    this.version++;
  }
}
