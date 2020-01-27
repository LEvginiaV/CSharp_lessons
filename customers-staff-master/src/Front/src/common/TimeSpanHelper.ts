import { TimeSpan } from "../api/new/dto/TimeSpan";

// @todo утащить в хелпер
export function hhmm_to_timeSpan(input: string): string {
  const parts = input.split(":");
  const minutes = Number.parseInt(parts[1], 10);
  const hours = Number.parseInt(parts[0], 10);
  if (hours === 24 && minutes === 0) {
    return "1.00:00:00";
  }
  return `${pad2(hours)}:${pad2(minutes)}:00`;
}

export function timeSpan_to_hhmm(input: string): string {
  if (input.length === 0) {
    return "";
  }
  const parts = input.split(".");
  const days = parts.length > 1 ? Number.parseInt(parts[0], 10) : 0;
  const timePart = parts[parts.length > 1 ? 1 : 0];
  const timeParts = timePart.split(":");
  const hours = Number.parseInt(timeParts[0], 10) + days * 24;
  const minutes = Number.parseInt(timeParts[1], 10);

  return `${pad2(hours)}:${pad2(minutes)}`;
}

export function pad2(val: number): string {
  return `${val % 100 > 9 ? "" : "0"}${val % 100}`;
}

const zerofill = (num: number) => (num > 9 ? num.toString() : "0" + num);

interface ParsedTimeSpan {
  sign: boolean;
  hour: number;
  minutes: number;
  seconds: number;
}

class TimeSpanHelperClass {
  public toMinutes(timespan: TimeSpan): number | null {
    const seconds = this.toSeconds(timespan);
    if (seconds == null) {
      return null;
    }
    return Math.floor(seconds / 60);
  }

  public toSeconds(timespan: TimeSpan): number | null {
    const parsed = this.parse(timespan);
    if (!parsed) {
      return null;
    }
    return (parsed.sign ? 1 : -1) * parsed.hour * 3600 + parsed.minutes * 60 + parsed.seconds;
  }

  public toTimezoneText(timespan: Nullable<TimeSpan>) {
    if (timespan == null) {
      return `UTC`;
    }
    const parsed = this.parse(timespan);
    if (!parsed) {
      return `UTC`;
    }
    // return `UTC ${parsed.sign ? "+" : "-"}${parsed.hour}:${zerofill(parsed.minutes)}`;
    return `UTC${parsed.sign ? "+" : "-"}${parsed.hour}`;
  }

  public convert24HoursTo1dayIfNeed(time: string): string {
    return time === "24:00:00" ? "1.00:00:00" : time;
  }

  public fromMinutes(minutesCount: number): string {
    return this.fromSeconds(minutesCount * 60);
  }

  public fromSeconds(secondsCount: number): string {
    const sign = Math.sign(secondsCount);
    const secsCount = Math.abs(secondsCount);
    if (secsCount >= 24 * 60 * 60) {
      return "24:00:00";
    }
    const secs = Math.floor(secsCount);
    const hours = Math.floor(secs / 60 / 60);
    const minutes = Math.floor(secs / 60) - hours * 60;
    const seconds = secs - minutes * 60 - hours * 60 * 60;
    return (sign < 0 ? "-" : "") + `${zerofill(hours)}:${zerofill(minutes)}:${zerofill(seconds)}`;
  }
  private parse(timespan: string): Nullable<ParsedTimeSpan> {
    let ts = timespan;
    if (ts === "1.00:00:00" || ts === "-1.00:00:00") {
      return {
        sign: ts[0] !== "-",
        hour: 24,
        minutes: 0,
        seconds: 0,
      };
    }
    if (!/^-?\d\d:\d\d:\d\d$/.test(ts)) {
      return null;
    }
    let sign = true;
    if (ts[0] === "-") {
      sign = false;
      ts = ts.substring(1);
    }
    const n = timespan.split(":").map(x => parseInt(x, 10));
    if (n[0] >= 0 && n[0] < 24 && n[1] >= 0 && n[1] < 60 && n[2] >= 0 && n[2] < 60) {
      return {
        sign,
        hour: n[0],
        minutes: n[1],
        seconds: n[2],
      };
    }
    if (n[0] === 24 && n[1] === 0 && n[2] === 0) {
      return {
        sign,
        hour: 24,
        minutes: 0,
        seconds: 0,
      };
    }
    return null;
  }
}

export const TimeSpanHelper = new TimeSpanHelperClass();
