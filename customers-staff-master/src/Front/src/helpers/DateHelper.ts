import { DateTime } from "../api/new/dto/DateTime";

// @todo Срастить 2 dateHelper'а
// @todo оставить отдин pad/zerofill
const zerofill = (num: number) => (num > 9 ? num.toString() : "0" + num);

export class DateHelper {
  public static compareOnlyDates(date1: Date | DateTime, date2: Date | DateTime): -1 | 0 | 1 {
    const d1 = new Date(date1);
    const d2 = new Date(date2);

    d1.setHours(0, 0, 0, 0);
    d2.setHours(0, 0, 0, 0);
    return d1 < d2 ? -1 : d1 > d2 ? 1 : 0;
  }

  public static dateToDatePicker(date: string | Date) {
    const d = new Date(date);
    return zerofill(d.getDate()) + "." + zerofill(d.getMonth() + 1) + "." + zerofill(d.getFullYear());
  }

  public static dateFromDatePicker(date: string): Nullable<Date> {
    const d = date.split(".").map(x => parseInt(x, 10));
    if (d.length !== 3 || isNaN(d[0]) || isNaN(d[1]) || isNaN(d[2])) {
      return null;
    }
    return new Date(d[2], d[1] - 1, d[0], 0, 0, 0);
  }

  public static isToday(date: Date) {
    const date1 = new Date();
    return DateHelper.compareOnlyDates(date, date1) === 0;
  }

  public static isTodayWithTimeZone(date: Date, offsetInMinutes: number | null) {
    const now = new Date();
    const offset = now.getTimezoneOffset();

    if (offsetInMinutes === null || offsetInMinutes === undefined || offset + offsetInMinutes === 0) {
      return this.isToday(date);
    }

    const date1 = new Date(now.setMinutes(offset + offsetInMinutes + now.getMinutes()));
    return DateHelper.compareOnlyDates(date, date1) === 0;
  }

  public static getTodayDateWithTimeZone(offsetInMinutes: number | null) {
    const now = new Date();
    if (offsetInMinutes == null) {
      return now;
    }
    return new Date(now.setMinutes(now.getTimezoneOffset() + offsetInMinutes + now.getMinutes()));
  }

  public static addDays(date: Date, days: number) {
    const oneDayInMilliseconds = 24 * 60 * 60 * 1000;
    return new Date(date.getTime() + oneDayInMilliseconds * Math.round(days));
  }
}
