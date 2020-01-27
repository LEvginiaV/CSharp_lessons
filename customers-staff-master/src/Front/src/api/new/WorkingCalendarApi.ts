import { toDateOnlyISOString } from "../../common/DateHelper";
import HttpClient from "../../common/HttpClient/HttpClient";
import { Guid } from "./dto/Guid";
import {
  ShopWorkingCalendarDto,
  ShopWorkingDayDto,
  WorkingCalendarDayInfoDto,
} from "./dto/WorkingCalendar/WorkersScheduleDtos";
import { IWorkingCalendarApi } from "./IWorkingCalendarApi";

export class WorkingCalendarApi implements IWorkingCalendarApi {
  private readonly urlPrefix: string;

  constructor(urlPrefix: string, shopId: Guid) {
    this.urlPrefix = `${urlPrefix}/shops/${shopId}/workers`;
  }

  public async getForMonth(month: Date | null, version: number | null): Promise<ShopWorkingCalendarDto> {
    const monthPart = month === null ? "" : `/${toDateOnlyISOString(month)}`;
    const versionPart = version === null || month === null ? "" : `?version=${version}`;
    const result = await HttpClient.get(`${this.urlPrefix}/v1/shopWorkingCalendar${monthPart}${versionPart}`, {});
    return result as ShopWorkingCalendarDto;
  }

  public async getForDay(day: Date, version: number | null): Promise<ShopWorkingDayDto> {
    const result = await HttpClient.get(`${this.urlPrefix}/shopWorkingCalendar/days/${toDateOnlyISOString(day)}`, {
      version: version || -1,
    });
    return result;
  }

  public async update(workerId: Guid, updates: WorkingCalendarDayInfoDto[]): Promise<void> {
    await HttpClient.put(`${this.urlPrefix}/${workerId}/schedule`, updates);
  }
}
