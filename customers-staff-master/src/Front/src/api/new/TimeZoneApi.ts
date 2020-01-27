import HttpClient from "../../common/HttpClient/HttpClient";
import { Guid } from "./dto/Guid";
import { TimeZoneDto } from "./dto/TimeZoneDto";
import { ITimeZoneApi } from "./ITimeZoneApi";

export class TimeZoneApi implements ITimeZoneApi {
  private readonly urlPrefix: string;

  constructor(urlPrefix: string, shopId: Guid) {
    this.urlPrefix = `${urlPrefix}/shops/${shopId}/timezone`;
  }

  public async get(): Promise<TimeZoneDto | null> {
    const result = await HttpClient.get(this.urlPrefix, {});
    return result ? (result as TimeZoneDto) : null;
  }

  public async getList(): Promise<TimeZoneDto[]> {
    const result = await HttpClient.get(`${this.urlPrefix}/list`, {});
    return result as TimeZoneDto[];
  }

  public async set(timeZoneId: Guid): Promise<void> {
    await HttpClient.post(`${this.urlPrefix}?timeZoneId=${timeZoneId}`, {});
  }
}
