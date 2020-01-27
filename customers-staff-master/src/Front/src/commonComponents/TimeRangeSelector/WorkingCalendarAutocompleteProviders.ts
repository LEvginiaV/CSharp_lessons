import { pad2 } from "../../common/TimeSpanHelper";

export function getAutocompleteForStart(input: string): Promise<string[]> {
  return getItems(input, 6);
}

export function getAutocompleteForEnd(input: string): Promise<string[]> {
  return getItems(input, 17);
}

function getItems(input: string, defaultHour: number): Promise<string[]> {
  const hours = getStartHour(input) || defaultHour;
  const addMinutes = !!input && input.length > 1;
  const result: string[] = [];
  for (let h = hours; h < 24; h++) {
    const hh = pad2(h);
    result.push(`${hh}:00`);
    if (addMinutes) {
      result.push(`${hh}:15`);
      result.push(`${hh}:30`);
      result.push(`${hh}:45`);
    }
  }
  result.push("24:00");
  return Promise.resolve(result);
}

function getStartHour(text: string): number | null {
  if (!text || text.length === 0) {
    return null;
  }
  if (text.length === 1) {
    const c = Number.parseInt(text[0], 10);
    if (c === 0 || c === 1 || c === 2) {
      return c * 10;
    }
    return null;
  }
  const h = Number.parseInt(text.substr(0, 2), 10);
  if (h < 24) {
    return h;
  }
  if (h === 24) {
    return 23;
  }
  return null;
}
