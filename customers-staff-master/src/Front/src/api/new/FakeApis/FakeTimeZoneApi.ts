import { Guid } from "../dto/Guid";
import { TimeZoneDto } from "../dto/TimeZoneDto";
import { ITimeZoneApi } from "../ITimeZoneApi";

const sleep: (timeout: number) => Promise<any> = (timeout: number) => {
  return new Promise(r => setTimeout(r, timeout));
};

export class FakeTimeZoneApi implements ITimeZoneApi {
  private timeZone?: TimeZoneDto;
  private timeZones: TimeZoneDto[] = [
    { id: "1", name: "Test1", offset: "03:00:00" },
    { id: "2", name: "Test2", offset: "05:00:00" },
  ];
  public async get(): Promise<TimeZoneDto | null> {
    await sleep(100);
    return this.timeZone || null;
  }

  public async getList(): Promise<TimeZoneDto[]> {
    await sleep(100);
    return this.timeZones;
  }

  public async set(timeZoneId: Guid): Promise<void> {
    await sleep(100);
    this.timeZone = this.timeZones.find(x => x.id === timeZoneId);
  }
}
