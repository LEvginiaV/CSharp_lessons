import { BirthdayDto } from "../api/new/dto/BirthdayDto";
import { CustomerValueTypeDto } from "../api/new/dto/WorkOrder/CustomerValueTypeDto";
import { WorkOrderStatusDto } from "../api/new/dto/WorkOrder/WorkOrderStatusDto";
import { months } from "../commonComponents/BirthdayControl/BirthdayControl";
import { UnitType } from "../typings/NomenclatureCard";

export class StringHelper {
  public static getBirthdayString(birthday?: BirthdayDto): string | null {
    if (!birthday || (!birthday.day && !birthday.month && !birthday.year)) {
      return null;
    }

    const { day, month, year } = birthday;
    const dayString = day || "";
    const currentMonth = months.filter(m => m.value === month && m.dateString);
    const monthString = currentMonth && currentMonth[0] ? currentMonth[0].dateString : "";
    return `${dayString} ${monthString}` + (year ? ` ${year} г.` : "");
  }

  public static getDiscountString(currentValue?: string, prevValue?: string): string {
    if (!currentValue) {
      return "";
    }

    currentValue = currentValue.replace(",", ".");

    if (/^\d{1,2}(\.\d{0,2})?$/g.test(currentValue)) {
      return currentValue;
    } else {
      return prevValue || "";
    }
  }

  public static printDiscountString(value?: number): string {
    return StringHelper.formatDiscountString(value) ? `${StringHelper.formatDiscountString(value)}%` : "нет";
  }

  public static formatDiscountString(value?: string | number): string {
    let v = this.formatDiscountValue(value);
    const valueArr = v.split(",");
    if (valueArr && valueArr[1] && valueArr[1].length === 1) {
      v = `${valueArr[0]},${valueArr[1]}0`;
    }

    if (v && v.charAt(v.length - 1) === ",") {
      v = v.substr(0, v.length - 1);
    }

    return v;
  }

  public static formatDiscountValue(value?: string | number): string {
    return value ? value.toString().replace(".", ",") : value === 0 ? "0" : "";
  }

  public static formatDiscountNumber(value?: string | number): number {
    if (!value) {
      return 0;
    } else {
      value = value.toString().replace(",", ".");
      return Number(value) ? Number(value) : 0;
    }
  }

  public static isNameValid(value?: string): boolean {
    return value ? /^[а-яёА-ЯЁa-zA-Z0-9 \.-]+$/.test(value) : false;
  }

  public static isNullOrEmpty(value?: string): boolean {
    return !value || value === "";
  }

  public static getNumberCase = (num: number, unitCase1: string, unitCase2: string, unitCase5: string) => {
    if (num % 100 >= 11 && num % 100 <= 19) {
      return `${num} ${unitCase5}`;
    }

    switch (num % 10) {
      case 1:
        return `${num} ${unitCase1}`;
      case 2:
      case 3:
      case 4:
        return `${num} ${unitCase2}`;
      default:
        return `${num} ${unitCase5}`;
    }
  };

  public static formatPhone(phone?: string): string | undefined {
    return phone ? `7${phone}`.replace(/[ \-]/g, "") : undefined;
  }

  public static formatPhoneString(phone?: string): string {
    if (!phone) {
      return "";
    }

    return "+7 " + this.formatPhoneToInput(phone.substring(1));
  }

  public static formatPhoneToInput(phone?: string): string {
    if (!phone) {
      return "";
    }

    if (phone && phone.charAt(0) === "7") {
      phone = phone.substr(1);
    }

    const code = phone.substr(0, 3);
    const num = phone.substr(3, phone.length);

    return `${code} ${num.substr(0, 3)}-${num.substr(3, 2)}-${num.substr(5, 2)}`.replace(/\s?-+$/g, "");
  }

  public static removeMoreWhitespaces(str: string, withTrim: boolean = true): string {
    str = str ? str.replace(/\s{2,}/g, " ") : str;
    if (withTrim) {
      str = str.trim();
    }
    return str;
  }

  public static isOrderSeriesValid(series: string): boolean {
    return series ? series.length <= 2 && /^[А-ЖИК-НП-ЦШЩЭ-Я]+$/.test(series.toUpperCase()) : false;
  }

  public static isOrderNumberValid(series: string): boolean {
    return series ? series.length <= 6 && /^[0-9]*$/.test(series) : false;
  }

  public static formatOrderNumber(orderNumber: number): string {
    let result = orderNumber.toString();
    while (result.length < 6) {
      result = "0" + result;
    }

    return result;
  }

  public static formatOrderStatus(status: WorkOrderStatusDto): string {
    switch (status) {
      case WorkOrderStatusDto.New:
        return "Новый заказ";
      case WorkOrderStatusDto.InProgress:
        return "В работе";
      case WorkOrderStatusDto.Completed:
        return "Выполнен";
      case WorkOrderStatusDto.IssuedToClient:
        return "Выдан клиенту";
    }
  }

  public static formatCustomerValueType(type: CustomerValueTypeDto): string {
    switch (type) {
      case CustomerValueTypeDto.Vehicle:
        return "Автомобиль";
      case CustomerValueTypeDto.Appliances:
        return "Техника";
      case CustomerValueTypeDto.Other:
        return "Другое";
    }
  }

  public static formatUnitType(unitType?: UnitType): string | undefined {
    switch (unitType) {
      case UnitType.Kilogram:
        return "кг";
      case UnitType.Meter:
        return "м";
      case UnitType.Liter:
        return "л";
      case UnitType.SquareMeter:
        return "м²";
      case UnitType.CubicMeter:
        return "м³";
      case UnitType.Tonne:
        return "т";
      case UnitType.RunningMeter:
        return "пог. м";
      case UnitType.Piece:
        return "шт";
      default:
        return undefined;
    }
  }

  public static capitalizeFirstLetter(str: string): string {
    return str.charAt(0).toUpperCase() + str.slice(1);
  }

  public static pluralize(n: number, one: string, few: string, many: string) {
    if (n % 10 === 1 && n % 100 !== 11) {
      return one;
    } else if (n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) {
      return few;
    } else {
      return many;
    }
  }
}
