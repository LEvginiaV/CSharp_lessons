import cloneDeep = require("lodash/cloneDeep");
import last = require("lodash/last");
import sortBy = require("lodash/sortBy");
import { WorkingCalendarDayInfoDto } from "../../../../api/new/dto/WorkingCalendar/WorkersScheduleDtos";
import { getDayInfosForMonth, toDateOnlyISOString } from "../../../../common/DateHelper";
import { Metrics } from "../../../../common/MetricsFacade";
import { hhmm_to_timeSpan } from "../../../../common/TimeSpanHelper";
import { DateHelper } from "../../../../helpers/DateHelper";
import { UpdateTimeMode } from "../WorkersDayEditor";
import { TimePeriodViewInfo, WorkingCalendarDayViewInfo } from "./Interfaces";

export function getApiModels(
  date: Date,
  timePeriods: TimePeriodViewInfo[],
  calendarDays: WorkingCalendarDayViewInfo[],
  index: number,
  mode?: UpdateTimeMode
): WorkingCalendarDayInfoDto[] {
  let current = timePeriods ? cloneDeep(timePeriods) : [];
  current.forEach(period => {
    if (period.start === "24:00") {
      period.start = "00:00";
      Metrics.scheduleEditReplace({ timeKind: "start", date: toDateOnlyISOString(date) });
    }
    if (period.end === "00:00") {
      period.end = "24:00";
      period.overflow = false;
      Metrics.scheduleEditReplace({ timeKind: "end", date: toDateOnlyISOString(date) });
    }
  });

  const next = calendarDays[index + 1].timePeriods ? cloneDeep(calendarDays[index + 1].timePeriods) : [];
  const prev = calendarDays[index - 1].timePeriods ? cloneDeep(calendarDays[index - 1].timePeriods) : [];

  if (!mode || mode === UpdateTimeMode.OneDay) {
    splitOverflowPeriods(prev, current);
    splitOverflowPeriods(current, next);
    splitOverflowPeriods(next, []);
    return [
      createWorkingDayApiDto(date, unionPeriods(current)),
      createWorkingDayApiDto(DateHelper.addDays(date, 1), unionPeriods(next)),
    ];
  }

  current = current.filter(p => isPeriodFilled(p));

  const days = generateDaysByRule(date, current, last(calendarDays).timePeriods, mode);
  splitOverflowPeriods(prev, days[0]);

  const result: WorkingCalendarDayInfoDto[] = [];

  for (let i = 0; i < days.length; i++) {
    if (i + 1 < days.length) {
      splitOverflowPeriods(days[i], days[i + 1]);
    }

    result.push(createWorkingDayApiDto(DateHelper.addDays(date, i), unionPeriods(days[i])));
  }

  return result;
}

function generateDaysByRule(
  date: Date,
  periods: TimePeriodViewInfo[],
  nextMonthPeriods: TimePeriodViewInfo[],
  mode?: UpdateTimeMode
): TimePeriodViewInfo[][] {
  const result: TimePeriodViewInfo[][] = [];

  let counter: number = 0;
  for (const dayInfo of getDayInfosForMonth(date)) {
    if (dayInfo.date < date) {
      continue;
    }
    if (mode === UpdateTimeMode.FiveByTwo) {
      result.push(dayInfo.isWeekend ? [] : cloneDeep(periods));
    }
    if (mode === UpdateTimeMode.TwoByTwo) {
      const setPeriod = counter % 4 === 0 || counter % 4 === 1;
      result.push(setPeriod ? cloneDeep(periods) : []);
      counter++;
    }
  }
  if (result.length > 0) {
    result.push(nextMonthPeriods || []);
  }
  return result;
}

function splitOverflowPeriods(from: TimePeriodViewInfo[], to: TimePeriodViewInfo[]) {
  from
    .filter(x => x.overflow)
    .forEach(x => {
      x.overflow = false;
      if (x.end !== "24:00") {
        to.push({ start: "00:00", end: x.end, overflow: false });
      }
      x.end = "24:00";
    });
}

function unionPeriods(periods: TimePeriodViewInfo[]): TimePeriodViewInfo[] {
  if (periods.length <= 0) {
    return periods;
  }

  periods = sortBy(periods, ["start"]);

  let lastPeriod = periods[0];
  for (let i = 1; i < periods.length; i++) {
    const current = periods[i];
    if (current.start > lastPeriod.end) {
      lastPeriod = current;
    } else {
      if (lastPeriod.end < current.end) {
        lastPeriod.end = current.end;
      }
      current.start = "";
      current.end = "";
    }
  }

  return periods.filter(period => isPeriodFilled(period));
}

function createWorkingDayApiDto(date: Date, timePeriods?: TimePeriodViewInfo[]): WorkingCalendarDayInfoDto {
  return {
    date: toDateOnlyISOString(date),
    records: timePeriods
      ? timePeriods.map(x => ({ period: { startTime: hhmm_to_timeSpan(x.start), endTime: hhmm_to_timeSpan(x.end) } }))
      : [],
  };
}

function isPeriodFilled(period: TimePeriodViewInfo): boolean {
  return period.start.length + period.end.length === 10;
}
