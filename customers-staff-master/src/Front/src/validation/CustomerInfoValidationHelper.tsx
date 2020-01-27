import { RenderErrorMessage, ValidationInfo } from "@skbkontur/react-ui-validations";
import * as React from "react";
import { IBirthday } from "../commonComponents/BirthdayControl/BirthdayControl";
import { ValidationHelper } from "./ValidationHelper";

export interface ValidationWrapperPartialProps {
  validationInfo: ValidationInfo | null;
  renderMessage?: RenderErrorMessage;
}

export class CustomerInfoValidationHelper {
  public static readonly customerValidationMessage = (
    <div>
      Укажите имя, телефон
      <br /> или номер карты клиента
    </div>
  );

  public static bindCustomerNameValidation(
    name?: string,
    phone?: string,
    customId?: string
  ): ValidationWrapperPartialProps {
    if (!name && !phone && !customId) {
      return ValidationHelper.bindRequiredField(name, this.customerValidationMessage);
    } else {
      return this.valid;
    }
  }

  public static bindCustomerPhoneValidation(
    phone?: string,
    name?: string,
    customId?: string
  ): ValidationWrapperPartialProps {
    if (!name && !phone && !customId) {
      return ValidationHelper.bindRequiredField(name, this.customerValidationMessage);
    } else {
      return ValidationHelper.bindSimplePhoneValidation(phone);
    }
  }

  public static bindCustomerCustomIdValidation(customId?: string, name?: string, phone?: string) {
    return this.bindCustomerNameValidation(name, phone, customId);
  }

  public static bindBirthdayDayValidation(birthday?: IBirthday) {
    return this.birthdayValidation(birthday)[0] || this.valid;
  }

  public static bindBirthdayMonthValidation(birthday?: IBirthday) {
    return this.birthdayValidation(birthday)[1] || this.valid;
  }

  public static bindBirthdayYearValidation(birthday?: IBirthday) {
    return this.birthdayValidation(birthday)[2] || this.valid;
  }

  private static readonly valid = {
    validationInfo: null,
  };

  private static readonly invalidOnLostfocus = {
    validationInfo: {
      message: "",
      type: "lostfocus",
    },
  };

  private static readonly invalidOnSubmit = {
    validationInfo: {
      message: "",
      type: "submit",
    },
  };

  private static birthdayValidation(birthday?: IBirthday) {
    const results: Array<Nullable<object>> = [null, null, null];

    if (!birthday) {
      return [this.valid, this.valid, this.valid];
    }

    if ((birthday.day === 0 || birthday.day) && (birthday.day < 1 || birthday.day > 31)) {
      results[0] = this.invalidOnLostfocus;
    }

    if ((birthday.month === 0 || birthday.month) && (birthday.month < 1 || birthday.month > 12)) {
      results[1] = this.invalidOnLostfocus;
    }

    if ((birthday.year === 0 || birthday.year) && (birthday.year > new Date().getFullYear() || birthday.year < 1900)) {
      results[2] = this.invalidOnLostfocus;
    }

    if (results.some(x => !!x) || (!birthday.day && !birthday.month)) {
      return results;
    }

    if (!birthday.day || !birthday.month) {
      if (!birthday.day) {
        results[0] = this.invalidOnSubmit;
      }
      if (!birthday.month) {
        results[1] = this.invalidOnSubmit;
      }
      return results;
    }

    if (!this.isValidDate(birthday.day, birthday.month, birthday.year)) {
      results[0] = this.invalidOnLostfocus;
      return results;
    }

    if (this.isFuture(birthday.day, birthday.month, birthday.year)) {
      results[0] = this.invalidOnLostfocus;
      results[1] = this.invalidOnLostfocus;
      results[2] = this.invalidOnLostfocus;
    }

    return results;
  }

  private static isValidDate(day: number, month: number, year?: number): boolean {
    switch (month) {
      case 2:
        if (year) {
          return this.isLeapYear(year) ? day <= 29 : day < 29;
        } else {
          return day <= 29;
        }
      case 4:
      case 6:
      case 9:
      case 11:
        return day <= 30;
      default:
        return day <= 31;
    }
  }

  private static isFuture(day: number, month: number, year?: number): boolean {
    if (!year) {
      return false;
    }

    const date = new Date();
    const currentDay = date.getDate();
    const currentMonth = date.getMonth() + 1;
    const currentYear = date.getFullYear();

    return year > currentYear || (year >= currentYear && month >= currentMonth && day > currentDay);
  }

  private static isLeapYear(year: number): boolean {
    if (year % 4 !== 0) {
      return false;
    }
    if (year % 100 === 0) {
      return year % 400 === 0;
    }
    return true;
  }
}
