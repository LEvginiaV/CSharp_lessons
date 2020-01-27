import DatePicker from "@skbkontur/react-ui/components/DatePicker/DatePicker";
import { CustomerDto } from "../../api/new/dto/CustomerDto";
import { CustomerInfoDto } from "../../api/new/dto/CustomerInfoDto";
import { TimeSpanHelper } from "../../common/TimeSpanHelper";
import { DateHelper } from "../../helpers/DateHelper";
import { MIN_RECORD_LENGTH } from "./Calendar";

export interface CalendarModalErrorInfo {
  type: "error" | "warning";
  text: string;
}

export interface CalendarModalValidation {
  [key: string]: Nullable<CalendarModalErrorInfo>;
}

export const HEIGHT_PER_MINUTE = 0.6;
class CalendarHelperClass {
  public pxToTime(px: number) {
    return Math.floor(px / HEIGHT_PER_MINUTE);
  }
  public timeToPx(time: null | number | string | Date) {
    if (time == null) {
      return 0;
    }
    if (typeof time === "number") {
      return Math.floor(time * HEIGHT_PER_MINUTE);
    }
    if (typeof time === "string" && /^\d\d:\d\d:\d\d$/.test(time)) {
      const parts = time.split(":");
      const mins = parseInt(parts[0], 10) * 60 + parseInt(parts[1], 10);
      return Math.floor(mins * HEIGHT_PER_MINUTE);
    }
    const date = new Date(time);
    const minutes = date.getHours() * 60 + date.getMinutes();
    return Math.floor(minutes * HEIGHT_PER_MINUTE);
  }

  public getCustomerValidation(customer: Nullable<CustomerDto | CustomerInfoDto>): Nullable<CalendarModalValidation> {
    if (!customer || customer.hasOwnProperty("id")) {
      return null;
    }
    if (!customer.name && !customer.phone) {
      return {
        phone: {
          type: "error",
          text: "",
        },
        name: {
          type: "error",
          text: "",
        },
      };
    }

    const unformatted = customer.phone && customer.phone.replace(/( |-)/g, "");
    if (unformatted && unformatted.length !== 11) {
      return {
        phone: {
          type: "error",
          text: "В номере должно быть 11 цифр",
        },
        name: null,
      };
    }

    return null;
  }

  public getDateValidation(datepickerValue: string, name: string): Nullable<CalendarModalValidation> {
    const dateObject = DateHelper.dateFromDatePicker(datepickerValue);
    let error: CalendarModalErrorInfo;
    if (!datepickerValue || !dateObject) {
      error = {
        type: "error",
        text: "Укажите дату",
      };
    } else if (!DatePicker.validate(datepickerValue)) {
      error = {
        type: "error",
        text: "",
      };
    } else if (
      DateHelper.compareOnlyDates(dateObject, new Date(new Date().getTime() + 365 * 24 * 60 * 60 * 1000)) === 1
    ) {
      error = {
        type: "error",
        text: "Дата слишком далеко в будущем",
      };
    } else if (DateHelper.compareOnlyDates(dateObject, new Date()) === -1) {
      error = {
        type: "warning",
        text: "Дата в прошлом",
      };
    } else {
      return null;
    }
    return { [name]: error };
  }

  public getTimeValidation = (
    startTime: string,
    endTime: string,
    fieldStart: string,
    fieldEnd: string,
    checkOtherRecordAtTime: (start: number, end: number) => boolean,
    checkWorkerWorking: (start: number, end: number) => boolean
  ): Nullable<CalendarModalValidation> => {
    const errorCreator = this.getTimeErrorCreator(fieldStart, fieldEnd);
    if (!startTime || !endTime) {
      return errorCreator("Укажите время записи", !startTime && "error", !endTime && "error");
    }

    const startMinutes = TimeSpanHelper.toMinutes(startTime);
    const endMinutes = TimeSpanHelper.toMinutes(endTime);
    if (startMinutes == null || endMinutes == null) {
      return errorCreator(
        "В земных сутках нет такого времени",
        startMinutes == null && "error",
        endMinutes == null && "error"
      );
    }

    if (endMinutes - startMinutes < 0) {
      return errorCreator("Начало периода не должно быть позже конца", "error", "error");
    }

    if (endMinutes - startMinutes < MIN_RECORD_LENGTH) {
      return errorCreator("Нельзя сделать запись короче 15 минут", "error", "error");
    }

    if (checkOtherRecordAtTime(startMinutes, endMinutes)) {
      return errorCreator("Попадает на другую запись, сотрудник будет занят", "error", "error");
    }
    if (!checkWorkerWorking(startMinutes, endMinutes)) {
      return errorCreator("Попадет на нерабочее время сотрудника", "warning", "warning");
    }

    return null;
  };

  private getTimeErrorCreator(fieldStart: string, fieldEnd: string) {
    return (
      text: string,
      typeStart: false | null | "error" | "warning",
      typeEnd: false | null | "error" | "warning"
    ): CalendarModalValidation => {
      return {
        [fieldStart]: typeStart ? { type: typeStart, text } : null,
        [fieldEnd]: typeEnd ? { type: typeEnd, text } : null,
      };
    };
  }
}

export const CalendarHelper = new CalendarHelperClass();
