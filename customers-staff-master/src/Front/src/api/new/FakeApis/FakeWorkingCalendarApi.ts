/* tslint:disable:no-console */
import { toDateOnlyISOString } from "../../../common/DateHelper";
import { Guid } from "../dto/Guid";
import {
  ShopWorkingCalendarDto,
  ShopWorkingDayDto,
  WorkingCalendarDayInfoDto,
  WorkingCalendarRecordDto,
} from "../dto/WorkingCalendar/WorkersScheduleDtos";
import { IWorkingCalendarApi } from "../IWorkingCalendarApi";

const sleep: (timeout: number) => Promise<any> = (timeout: number) => {
  return new Promise(r => setTimeout(r, timeout));
};

export class FakeWorkingCalendarApi implements IWorkingCalendarApi {
  private workingCalendar: Map<string, ShopWorkingCalendarDto> = new Map();

  public async getForMonth(month: Date): Promise<ShopWorkingCalendarDto> {
    console.log("getForMonth", month);
    await sleep(100);
    return this.getShopCalendarMonth(month);
  }

  public async getForDay(day: Date, _version: number | null): Promise<ShopWorkingDayDto> {
    await sleep(100);
    const d = toDateOnlyISOString(day);
    const month = new Date(day.getFullYear(), day.getMonth());
    const monthStr = toDateOnlyISOString(month);
    const calendarMonth = this.workingCalendar.get(monthStr);
    const workingDayMap: { [workerId: string]: WorkingCalendarRecordDto[] } = {};
    if (calendarMonth) {
      for (const workerId in calendarMonth.workingCalendarMap) {
        if (!calendarMonth.workingCalendarMap.hasOwnProperty(workerId)) {
          continue;
        }
        const dayMap = calendarMonth.workingCalendarMap[workerId].find(x => x.date === d);
        if (dayMap) {
          workingDayMap[workerId] = [...dayMap.records];
        }
      }
    }
    return {
      version: 1,
      date: d,
      workingDayMap,
    };
    // throw new Error("qwe");
  }

  public async update(workerId: Guid, updates: WorkingCalendarDayInfoDto[]): Promise<void> {
    console.log("update", workerId, updates);
    await sleep(100);
    updates.forEach(update => {
      const date = new Date(update.date);
      const calendarDay = this.getWorkerCalendarMonth(date, workerId)[date.getDate() - 1];
      calendarDay.records = update.records;
    });
  }

  private getShopCalendarMonth(date: Date): ShopWorkingCalendarDto {
    date = date || new Date();
    const month = new Date(date.getFullYear(), date.getMonth());
    const monthStr = toDateOnlyISOString(month);
    let calendarMonth = this.workingCalendar.get(monthStr);
    if (!calendarMonth) {
      calendarMonth = {
        month: monthStr,
        workingCalendarMap: {},
        version: 1,
      };
      this.workingCalendar.set(monthStr, calendarMonth);
    }
    return calendarMonth;
  }

  private getWorkerCalendarMonth(date: Date, workerId: Guid): WorkingCalendarDayInfoDto[] {
    const calendarMonth = this.getShopCalendarMonth(date);
    let calendarDays = calendarMonth.workingCalendarMap[workerId];
    if (!calendarDays) {
      calendarDays = [];
      const totalDays = new Date(date.getFullYear(), date.getMonth() + 1, 0).getDate();
      for (let i = 1; i <= totalDays; i++) {
        calendarDays.push({ date: toDateOnlyISOString(new Date(date.getFullYear(), date.getMonth(), i)), records: [] });
      }
      calendarMonth.workingCalendarMap[workerId] = calendarDays;
    }

    return calendarDays;
  }
}
