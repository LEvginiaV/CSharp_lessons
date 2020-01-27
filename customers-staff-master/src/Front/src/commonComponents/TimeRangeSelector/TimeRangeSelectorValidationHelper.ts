import { RenderErrorMessage, ValidationBehaviour, ValidationInfo } from "@skbkontur/react-ui-validations";
import { TimeSpan } from "../../api/new/dto/TimeSpan";
import { tooltip } from "../../validation/ValidationTooltip";

export interface ValidationResult {
  validationInfo: ValidationInfo | null;
  renderMessage: RenderErrorMessage;
}

export class TimeRangeSelectorValidationHelper {
  public static bindEndTimeValidation(startTime: TimeSpan, endTime: TimeSpan): ValidationResult {
    const validationResult = this.validateSingleDate(endTime, startTime.length, "ErrorMessageTooltipEnd");
    if (!!validationResult) {
      return validationResult;
    }

    if (!this.validateSingleDate(startTime, 0, "ErrorMessageTooltipEnd")) {
      const message = this.validateTimeRangeForWorkingCalendar(startTime, endTime);

      return {
        validationInfo: this.getInfo(!!message, message || "", "submit"),
        renderMessage: tooltip("top center", "ErrorMessageTooltipEnd"),
      };
    }

    return {
      validationInfo: this.getInfo(false, ""),
      renderMessage: tooltip("top center", "ErrorMessageTooltipEnd"),
    };
  }

  public static bindStartTimeValidation(startTime: TimeSpan, endTime: TimeSpan): ValidationResult {
    const validationResult = this.validateSingleDate(startTime, endTime.length, "ErrorMessageTooltipStart");
    if (!!validationResult) {
      return validationResult;
    }

    if (!this.validateSingleDate(endTime, 0, "ErrorMessageTooltipStart")) {
      const message = this.validateTimeRangeForWorkingCalendar(startTime, endTime);

      return {
        validationInfo: this.getInfo(!!message, message || "", "submit"),
        renderMessage: tooltip("top center", "ErrorMessageTooltipStart"),
      };
    }

    return {
      validationInfo: this.getInfo(false, ""),
      renderMessage: tooltip("top center", "ErrorMessageTooltipStart"),
    };
  }

  private static getInfo(error: boolean, message: string, type: ValidationBehaviour = "lostfocus") {
    return error ? { message, type } : null;
  }

  private static validateTimeRangeForWorkingCalendar(start: string, end: string): string | null {
    const startParts = start.split(":");
    let startH = Number.parseInt(startParts[0], 10);
    const startM = Number.parseInt(startParts[1], 10);

    if (startH === 24 && startM === 0) {
      startH = 0;
    }

    const endParts = end.split(":");
    let endH = Number.parseInt(endParts[0], 10);
    const endM = Number.parseInt(endParts[1], 10);

    if (endH === 0 && endM === 0) {
      endH = 24;
    }

    const difInMinutes = (endH - startH) * 60 + endM - startM;

    if (difInMinutes > 0 && difInMinutes < 15) {
      return "Нельзя сделать запись короче 15 минут";
    }

    return null;
  }

  private static validateSingleDate(time: TimeSpan, otherTimeLength: number, dataTid: string) {
    if (time.length + otherTimeLength === 0) {
      return {
        validationInfo: this.getInfo(false, ""),
        renderMessage: tooltip("top center", dataTid),
      };
    }

    if (time.length === 0) {
      return {
        validationInfo: this.getInfo(true, "Укажите время", "submit"),
        renderMessage: tooltip("top center", dataTid),
      };
    }

    if (time.length < 5) {
      return {
        validationInfo: this.getInfo(true, "В земных сутках нет такого времени"),
        renderMessage: tooltip("top center", dataTid),
      };
    }
    const parts = time.split(":");
    const hh = Number.parseInt(parts[0], 10);
    const mm = Number.parseInt(parts[1], 10);

    if (!this.hoursValid(hh) || !this.minutesValid(hh, mm)) {
      return {
        validationInfo: this.getInfo(true, "В земных сутках нет такого времени"),
        renderMessage: tooltip("top center", dataTid),
      };
    }

    return null;
  }

  private static hoursValid(h: number): boolean {
    return 0 <= h && h <= 24;
  }

  private static minutesValid(h: number, m: number): boolean {
    if (h === 24 && m !== 0) {
      return false;
    }
    return 0 <= m && m <= 59;
  }
}
