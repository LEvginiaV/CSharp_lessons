/* tslint:disable:no-console */
import uuid = require("uuid");
import { Guid } from "../dto/Guid";
import {
  CustomerStatusDto,
  RecordStatusDto,
  ServiceCalendarRecordInfoDto,
} from "../dto/ServiceCalendar/ServiceCalendarRecordInfoDto";
import { ServiceCalendarRecordUpdateDto } from "../dto/ServiceCalendar/ServiceCalendarRecordUpdateDto";
import { ShopServiceCalendarDayDto } from "../dto/ServiceCalendar/ShopServiceCalendarDayDto";
import { WorkerServiceCalendarDayDto } from "../dto/ServiceCalendar/WorkerServiceCalendarDayDto";
import { IServiceCalendarApi } from "../IServiceCalendarApi";

const sleep: (timeout: number) => Promise<any> = (timeout: number) => {
  return new Promise(r => setTimeout(r, timeout));
};

export class FakeServiceCalendarApi implements IServiceCalendarApi {
  private calendarMap: Map<string, ShopServiceCalendarDayDto>;

  constructor() {
    this.calendarMap = new Map();
  }

  public async createRecord(date: string, workerId: Guid, record: ServiceCalendarRecordInfoDto): Promise<Guid> {
    console.log("createRecords", date, workerId, record);
    await sleep(100);
    const workerDay = this.getWorkerDay(date, workerId);
    const id = uuid();
    workerDay.records.push({
      id,
      recordStatus: RecordStatusDto.Active,
      customerStatus: CustomerStatusDto.Active,
      ...record,
    });
    return id;
  }

  public async getForDay(date: string, status?: RecordStatusDto): Promise<ShopServiceCalendarDayDto> {
    console.log("getForDay", date, status);
    await sleep(100);
    const calendarDay = this.getCalendarDay(date);

    if (!status) {
      return calendarDay;
    }

    const returnDay: ShopServiceCalendarDayDto = {
      date: calendarDay.date,
      workerCalendarDays: [],
    };

    calendarDay.workerCalendarDays.forEach(d => {
      returnDay.workerCalendarDays.push({
        date: d.date,
        workerId: d.workerId,
        records: d.records.filter(x => x.recordStatus === status),
      });
    });

    return returnDay;
  }

  public async updateRecord(
    date: string,
    workerId: Guid,
    recordId: Guid,
    record: ServiceCalendarRecordUpdateDto
  ): Promise<void> {
    console.log("updateRecords", date, workerId, record);
    await sleep(100);
    const workerDay = this.getWorkerDay(date, workerId);
    const updateWorkerDay = this.getWorkerDay(record.updatedDate || date, record.updatedWorkerId || workerId);

    const recordIndex = workerDay.records.findIndex(x => x.id === recordId);

    if (recordIndex < 0) {
      throw new Error("404");
    }

    const currentRecord = workerDay.records[recordIndex];

    workerDay.records = workerDay.records.splice(recordIndex, 1);
    updateWorkerDay.records.push({ ...currentRecord, ...record });
  }

  public async updateRecordStatus(
    date: string,
    workerId: Guid,
    recordId: Guid,
    newStatus: CustomerStatusDto
  ): Promise<void> {
    console.log("updateRecordStatus", date, workerId, newStatus);
    await sleep(100);
    const workerDay = this.getWorkerDay(date, workerId);

    if (workerDay.records.findIndex(x => x.id === recordId) < 0) {
      throw new Error("404");
    }

    workerDay.records = workerDay.records.map(x => {
      if (x.id === recordId) {
        return { ...x, customerStatus: newStatus };
      }
      return x;
    });
  }

  private getCalendarDay(date: string): ShopServiceCalendarDayDto {
    let calendarDay = this.calendarMap.get(date);

    if (!calendarDay) {
      calendarDay = { date, workerCalendarDays: [] };
      this.calendarMap.set(date, calendarDay);
    }

    return calendarDay;
  }

  private getWorkerDay(date: string, workerId: Guid): WorkerServiceCalendarDayDto {
    const calendarDay = this.getCalendarDay(date);
    let workerDay = calendarDay.workerCalendarDays.find(v => v.workerId === workerId);

    if (!workerDay) {
      workerDay = {
        date: calendarDay.date,
        workerId,
        records: [],
      };
      calendarDay.workerCalendarDays.push(workerDay);
    }

    return workerDay;
  }
}
