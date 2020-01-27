import sum = require("lodash/sum");
import sumBy = require("lodash/sumBy");
import { Guid } from "../../../../api/new/dto/Guid";
import { ShopWorkingCalendarDto } from "../../../../api/new/dto/WorkingCalendar/WorkersScheduleDtos";
import { timeSpan_to_hhmm } from "../../../../common/TimeSpanHelper";
import { TimePeriodViewInfo, WorkingCalendarDayViewInfo } from "./Interfaces";

export function buildCalendarMap(calendar: ShopWorkingCalendarDto | null): Map<Guid, WorkingCalendarDayViewInfo[]> {
  const result = new Map<Guid, WorkingCalendarDayViewInfo[]>();

  if (calendar && calendar.workingCalendarMap) {
    Object.entries(calendar.workingCalendarMap).forEach(pair => {
      const workerId = pair[0];
      const dayInfos = pair[1];

      const dayViewInfos: WorkingCalendarDayViewInfo[] = dayInfos.map(dayInfo => ({
        date: dayInfo.date,
        timePeriods: dayInfo.records.map(record => ({
          start: timeSpan_to_hhmm(record.period.startTime),
          end: timeSpan_to_hhmm(record.period.endTime),
          overflow: false,
        })),
      }));

      for (let i = 0; i < dayViewInfos.length; i++) {
        dayViewInfos[i].timePeriods = dayViewInfos[i].timePeriods.filter(x => x.start.length + x.end.length > 0);

        if (i + 1 === dayViewInfos.length) {
          break;
        }

        const starts = dayViewInfos[i].timePeriods.filter(p => p.end === "24:00" && p.start !== "00:00");
        const ends = dayViewInfos[i + 1].timePeriods.filter(p => p.start === "00:00" && p.end !== "24:00");

        if (starts.length === 1 && ends.length === 1 && starts[0].start >= ends[0].end) {
          starts[0].end = ends[0].end;
          starts[0].overflow = true;

          ends[0].start = "";
          ends[0].end = "";
        }
      }

      result.set(workerId, dayViewInfos);
    });
  }

  return result;
}

export function getDaysCount(workingDays: WorkingCalendarDayViewInfo[]): string {
  const count = workingDays.slice(1, workingDays.length - 1).filter(x => x.timePeriods && x.timePeriods.length > 0)
    .length;
  if (count > 0) {
    return `${count} дн`;
  }
  return "";
}

export function getHoursCount(workingDays: WorkingCalendarDayViewInfo[]): string {
  const minutes = workingDays
    .slice(1, workingDays.length - 1)
    .filter(d => d.timePeriods && d.timePeriods.length > 0)
    .map(d => sumBy(d.timePeriods.map(r => getDifInMinutes(r))));

  const minutesCount = sum(minutes);

  if (minutesCount === 0) {
    return "";
  }
  const hours = Math.floor(minutesCount / 60);
  const hoursPart = hours > 0 ? `${hours} ч` : "";
  const minPart = minutesCount % 60 === 0 ? "" : ` ${minutesCount % 60} мин`;
  return hoursPart + minPart;
}

function getDifInMinutes(period: TimePeriodViewInfo): number {
  let minutes = period.overflow ? 24 * 60 : 0;

  const endParts = period.end.split(":");
  minutes += Number.parseInt(endParts[1], 10);
  minutes += Number.parseInt(endParts[0], 10) * 60;

  const startParts = period.start.split(":");
  minutes -= Number.parseInt(startParts[1], 10);
  minutes -= Number.parseInt(startParts[0], 10) * 60;

  return minutes;
}
