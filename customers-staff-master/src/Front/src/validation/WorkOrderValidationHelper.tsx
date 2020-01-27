import { ValidationBehaviour } from "@skbkontur/react-ui-validations";
import { DateTime } from "../api/new/dto/DateTime";
import { formatDatePickerDateToIso, toDateOnlyISOString } from "../common/DateHelper";
import { StringHelper } from "../helpers/StringHelper";
import { tooltip } from "./ValidationTooltip";

export class WorkOrderValidationHelper {
  public static bindSeriesValidation(series: string, orderNumberUsed: boolean) {
    if (orderNumberUsed) {
      return {
        validationInfo: this.getInfo(true, "Номер занят"),
        renderMessage: tooltip("top center"),
      };
    }

    return {
      validationInfo: this.getInfo(
        !StringHelper.isOrderSeriesValid(series) || series.length !== 2,
        "Серия должна содержать два символа"
      ),
      renderMessage: tooltip("top center"),
    };
  }

  public static bindOrderNumberValidation(orderNumber: string, orderNumberUsed: boolean) {
    if (orderNumberUsed) {
      return {
        validationInfo: this.getInfo(true, "Номер занят"),
        renderMessage: tooltip("top center"),
      };
    }

    return {
      validationInfo: this.getInfo(
        !StringHelper.isOrderNumberValid(orderNumber) || orderNumber.length === 0 || +orderNumber < 1,
        "Номер должен быть больше нуля"
      ),
      renderMessage: tooltip("top center"),
    };
  }

  public static bindReceptionDateValidation(dateString?: string) {
    const date = formatDatePickerDateToIso(dateString);
    const toDate = toDateOnlyISOString(this.addDays(new Date(), 1, 0));

    if (date) {
      if (toDate && date > toDate) {
        return {
          validationInfo: this.getInfo(true, `Дата приемки слишком далеко в будущем`, "lostfocus"),
          renderMessage: tooltip("top center", "ReceptionDateValidation"),
        };
      }

      if (date < "2016-07-01") {
        return {
          validationInfo: this.getInfo(true, `Дата приемки слишком далеко в прошлом`, "lostfocus"),
          renderMessage: tooltip("top center", "ReceptionDateValidation"),
        };
      }
    } else {
      return {
        validationInfo: this.getInfo(true, `Необходимо ввести дату приемки`, "submit"),
        renderMessage: tooltip("top center", "ReceptionDateValidation"),
      };
    }

    return {
      validationInfo: this.getInfo(false, "-", "lostfocus"),
      renderMessage: tooltip("top center", "ReceptionDateValidation"),
    };
  }

  public static bindCompletionDateValidation(
    dateString?: string,
    receptionDate?: DateTime,
    dataTid?: string,
    checkEmpty: boolean = true
  ) {
    receptionDate = receptionDate && receptionDate.substring(0, 10);
    const date = formatDatePickerDateToIso(dateString);
    const toDate = receptionDate ? toDateOnlyISOString(this.addDays(new Date(receptionDate), 0, 1)) : null;

    if (date) {
      if (toDate && date > toDate) {
        return {
          validationInfo: this.getInfo(true, `Дата завершения слишком далеко в будущем`, "lostfocus"),
          renderMessage: tooltip("left middle", dataTid),
        };
      }

      if (receptionDate && date < receptionDate) {
        return {
          validationInfo: this.getInfo(true, `Дата завершения не может быть раньше даты приемки`, "lostfocus"),
          renderMessage: tooltip("left middle", dataTid),
        };
      }

      if (date < "2016-07-01") {
        return {
          validationInfo: this.getInfo(true, `Дата завершения слишком далеко в прошлом`, "lostfocus"),
          renderMessage: tooltip("left middle", dataTid),
        };
      }
    } else if (checkEmpty) {
      return {
        validationInfo: this.getInfo(true, `Необходимо ввести дату завершения`, "submit"),
        renderMessage: tooltip("left middle", dataTid),
      };
    }

    return {
      validationInfo: this.getInfo(false, "-", "lostfocus"),
      renderMessage: tooltip("left middle", dataTid),
    };
  }

  public static bindPhoneValidation(phone?: string) {
    const formatted = phone && phone.replace(/( |-)/g, "");

    return {
      renderMessage: tooltip("right middle"),
      validationInfo: this.getInfo(!!formatted && formatted.length !== 11, "В номере должно быть 11 цифр"),
    };
  }

  public static bindYearValidation(year?: number) {
    const maxYear = new Date().getFullYear();
    return {
      renderMessage: tooltip("right middle"),
      validationInfo: this.getInfo(!!year && (year < 1900 || year > maxYear), `Год от 1900 до ${maxYear}`),
    };
  }

  public static bindStringLengthValidation(str?: string, length: number = 0, dataTid?: string) {
    return {
      renderMessage: tooltip("right middle", dataTid),
      validationInfo: this.getInfo(
        !str || str.length !== length,
        `В строке должно быть ${length} ${StringHelper.pluralize(length, "символ", "символа", "символов")}`,
        !str ? "submit" : "lostfocus"
      ),
    };
  }

  public static bindStringNonEmptyValidation(str?: string, dataTid?: string) {
    return {
      renderMessage: tooltip("right middle", dataTid),
      validationInfo: this.getInfo(!str || !str.trim(), `Строка не должна быть пустая`, "submit"),
    };
  }

  public static bindNameValidation(str?: string) {
    return {
      renderMessage: tooltip("right middle", "NameValidation"),
      validationInfo: this.getInfo(
        !str || !str.trim() || !StringHelper.isNameValid(str),
        `Имя должно быть заполнено`,
        "submit"
      ),
    };
  }

  public static bindNumberValidation(num?: number, dataTid?: string) {
    if (!num) {
      return {
        renderMessage: tooltip("right middle", dataTid),
        validationInfo: this.getInfo(true, "Необходимо ввести значение", "submit"),
      };
    }

    return {
      renderMessage: tooltip("right middle", dataTid),
      validationInfo: this.getInfo(num <= 0, `Значение должно быть положительным`, "submit"),
    };
  }

  private static getInfo(error: boolean, message: string, type: ValidationBehaviour = "lostfocus") {
    return error ? { message, type } : null;
  }

  private static addDays(date: Date, days: number, years: number): Date {
    const res = new Date(date);
    res.setDate(res.getDate() + days);
    res.setFullYear(res.getFullYear() + years);
    return res;
  }
}
