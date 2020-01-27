import { DateTime } from "../api/new/dto/DateTime";
import { pad2 } from "./TimeSpanHelper";

const MONTH_NAMES: string[] = [
  "января",
  "февраля",
  "марта",
  "апреля",
  "мая",
  "июня",
  "июля",
  "августа",
  "сентября",
  "октября",
  "ноября",
  "декабря",
];

const MONTH_NAMES_NOMINATIVE: string[] = [
  "Январь",
  "Февраль",
  "Март",
  "Апрель",
  "Май",
  "Июнь",
  "Июль",
  "Август",
  "Сентябрь",
  "Октябрь",
  "Ноябрь",
  "Декабрь",
];

const SHORT_MONTH_NAMES: string[] = [
  "янв",
  "фев",
  "мар",
  "апр",
  "мая",
  "июн",
  "июл",
  "авг",
  "сен",
  "окт",
  "ноя",
  "дек",
];

const DAY_OF_WEEK_NAMES: string[] = ["воскресенье", "понедельник", "вторник", "среда", "четверг", "пятница", "суббота"];

export interface DayInfo {
  date: Date;
  isWeekend: boolean;
}

export function datesEquals(a: Date, b: Date): boolean {
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate();
}

export function getDayInfosForMonth(date: Date): DayInfo[] {
  const year = date.getFullYear();
  const month = date.getMonth();
  const from = new Date(year, month, 1);
  const to = new Date(year, month + 1, 0);

  const result: DayInfo[] = [];
  for (const i = from; i <= to; i.setDate(i.getDate() + 1)) {
    result.push({
      date: new Date(i.getFullYear(), i.getMonth(), i.getDate()),
      isWeekend: i.getDay() === 6 || i.getDay() === 0,
    });
  }
  return result;
}

export function toDateMonthNameString(date: Date): string {
  return `${date.getDate()} ${MONTH_NAMES[date.getMonth()]}`;
}

export function toDateMonthNameShortString(date: Date): string {
  return `${date.getDate()} ${SHORT_MONTH_NAMES[date.getMonth()]}`;
}

export function toDateOnlyISOString(date: Date): string {
  return `${date.getFullYear()}-${pad2(date.getMonth() + 1)}-${pad2(date.getDate())}`;
}

export function getDayOfWeekName(date: Date): string {
  return DAY_OF_WEEK_NAMES[date.getDay()];
}

export function getMonthName(date: Date): string {
  return MONTH_NAMES_NOMINATIVE[date.getMonth()];
}

export function fromDateOnlyISOString(date: Nullable<string>): null | Date {
  if (!date) {
    return null;
  }
  if (!/^\d{4}-\d{2}-\d{2}$/.test(date)) {
    return null;
  }
  const parts = date.split("-").map(x => parseInt(x, 10));
  const result = new Date(parts[0], parts[1] - 1, parts[2]);
  return isNaN(result.getTime()) ? null : result;
}

export function formatDatePickerDateToIso(date?: string): DateTime | undefined {
  if (!date) {
    return undefined;
  }
  const components = date.split(".");
  return `${components[2]}-${components[1]}-${components[0]}`;
}

export function formatIsoDateToDatePicker(date?: DateTime): string | undefined {
  if (!date) {
    return undefined;
  }
  const components = date.substring(0, 10).split("-");
  return `${components[2]}.${components[1]}.${components[0]}`;
}
