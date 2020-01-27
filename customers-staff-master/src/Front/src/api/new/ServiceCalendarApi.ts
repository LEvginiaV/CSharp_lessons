import HttpClient from "../../common/HttpClient/HttpClient";
import { Guid } from "./dto/Guid";
import {
  CustomerStatusDto,
  RecordStatusDto,
  ServiceCalendarRecordInfoDto,
} from "./dto/ServiceCalendar/ServiceCalendarRecordInfoDto";
import { ServiceCalendarRecordUpdateDto } from "./dto/ServiceCalendar/ServiceCalendarRecordUpdateDto";
import { ShopServiceCalendarDayDto } from "./dto/ServiceCalendar/ShopServiceCalendarDayDto";
import { IServiceCalendarApi } from "./IServiceCalendarApi";

export class ServiceCalendarApi implements IServiceCalendarApi {
  private readonly urlPrefix: string;

  constructor(urlPrefix: string, shopId: Guid) {
    this.urlPrefix = `${urlPrefix}/shops/${shopId}/calendar`;
  }

  public async createRecord(date: string, workerId: Guid, record: ServiceCalendarRecordInfoDto): Promise<Guid> {
    const result = await HttpClient.post(`${this.urlPrefix}/${date}/workers/${workerId}/records`, record);
    return result as Guid;
  }

  public async getForDay(date: string, status?: RecordStatusDto): Promise<ShopServiceCalendarDayDto> {
    const result = await HttpClient.get(`${this.urlPrefix}/${date}`, { status });
    return result as ShopServiceCalendarDayDto;
  }

  public async updateRecord(
    date: string,
    workerId: Guid,
    recordId: Guid,
    record: ServiceCalendarRecordUpdateDto
  ): Promise<void> {
    await HttpClient.put(`${this.urlPrefix}/${date}/workers/${workerId}/records/${recordId}`, record);
  }

  public async updateRecordStatus(
    date: string,
    workerId: Guid,
    recordId: Guid,
    newStatus: CustomerStatusDto
  ): Promise<void> {
    await HttpClient.put(
      `${this.urlPrefix}/${date}/workers/${workerId}/records/${recordId}/updateCustomerStatus?newStatus=${newStatus}`,
      {}
    );
  }
}
