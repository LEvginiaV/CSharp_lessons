import { ValidationWrapperV1, ValidationWrapperV1Props } from "@skbkontur/react-ui-validations";
import { ComboBox } from "@skbkontur/react-ui/components";
import Input from "@skbkontur/react-ui/components/Input/Input";
import * as React from "react";

interface IMonthMap {
  value: number;
  label: string;
  dateString: string;
}

export const months: IMonthMap[] = [
  { value: 1, label: "Январь", dateString: "января" },
  { value: 2, label: "Февраль", dateString: "февраля" },
  { value: 3, label: "Март", dateString: "марта" },
  { value: 4, label: "Апрель", dateString: "апреля" },
  { value: 5, label: "Май", dateString: "мая" },
  { value: 6, label: "Июнь", dateString: "июня" },
  { value: 7, label: "Июль", dateString: "июля" },
  { value: 8, label: "Август", dateString: "августа" },
  { value: 9, label: "Сентябрь", dateString: "сентября" },
  { value: 10, label: "Октябрь", dateString: "октября" },
  { value: 11, label: "Ноябрь", dateString: "ноября" },
  { value: 12, label: "Декабрь", dateString: "декабря" },
];

export interface IBirthday {
  year?: number;
  day?: number;
  month?: number;
}

interface IBirthdayStrings {
  year?: string;
  day?: string;
  month?: string;
}

interface IBirthdayControlProps {
  onChange: (birthday: IBirthday) => void;
  birthday?: IBirthday;
  dayValidationProps?: Partial<ValidationWrapperV1Props>;
  monthValidationProps?: Partial<ValidationWrapperV1Props>;
  yearValidationProps?: Partial<ValidationWrapperV1Props>;
}

export class BirthdayControl extends React.Component<IBirthdayControlProps, {}> {
  private Inputs: any = {};

  public render(): JSX.Element {
    const { day, month, year } = this.prepareBirthday();

    return (
      <div>
        <span style={{ marginRight: 5 }}>
          <ValidationWrapperV1 {...this.props.dayValidationProps as ValidationWrapperV1Props}>
            <Input
              data-tid="BirthdayDay"
              ref={(el: Input) => (this.Inputs.day = el)}
              placeholder="День"
              width={53}
              value={day}
              onChange={(_, v) => this.onFieldChange(v, "day")}
            />
          </ValidationWrapperV1>
        </span>
        <span style={{ marginRight: 5 }}>
          <ValidationWrapperV1 {...this.props.monthValidationProps as ValidationWrapperV1Props}>
            <ComboBox<IMonthMap>
              data-tid="BirthdayMonth"
              getItems={this.getMonthsItems}
              onChange={this.onMonthChange}
              onUnexpectedInput={this.onUnexpectedInput}
              width={95}
              placeholder="Месяц"
              value={this.getMonthValue(month)}
              valueToString={item => item.label}
              renderItem={item => item.label}
            />
          </ValidationWrapperV1>
        </span>
        <ValidationWrapperV1 {...this.props.yearValidationProps as ValidationWrapperV1Props}>
          <Input
            data-tid="BirthdayYear"
            ref={(el: Input) => (this.Inputs.year = el)}
            placeholder="Год"
            width={53}
            value={year}
            onChange={(_, v) => this.onFieldChange(v, "year", 4)}
          />
        </ValidationWrapperV1>
      </div>
    );
  }

  private prepareBirthday(): IBirthdayStrings {
    const { day = null, month = null, year = null } = this.props.birthday || {};

    return {
      day: day !== null ? day.toString() : "",
      month: month !== null ? month.toString() : "",
      year: year !== null ? year.toString() : "",
    };
  }

  private getMonthValue(month?: string): Nullable<IMonthMap> {
    const current = months.filter(m => m.value.toString() === month);
    return month && month !== "0" ? current && current[0] : null;
  }

  private onMonthChange = (_: any, v: IMonthMap | null) => {
    this.onFieldChange(v ? v.value.toString() : null, "month");
  };

  private onUnexpectedInput = (query: string) => {
    if (!query) {
      this.onMonthChange(null, null);
      return;
    }

    const m = !Number(query)
      ? months.find(x => x.label.toLowerCase() === query.toLowerCase())
      : months.find(x => x.value.toString() === query || `0${x.value}` === query);

    if (m) {
      this.onMonthChange(null, m);
      return m;
    }

    if (Number.isFinite(Number(query))) {
      this.onFieldChange(query, "month");
    }

    this.onFieldChange("0", "month");

    return;
  };

  private getMonthsItems = (query: string) => {
    return Promise.resolve(
      months.filter(
        x =>
          x.label.toLowerCase().includes(query.toLowerCase()) || x.value.toString() === query || `0${x.value}` === query
      )
    );
  };

  private onFieldChange(value: string | null = null, field: keyof IBirthdayStrings, maxLength = 2) {
    if (value && (!Number.isFinite(Number(value)) || value.length > maxLength)) {
      this.Inputs && this.Inputs[field] && this.Inputs[field].blink();
    } else {
      const newValue = value === null || value === "" ? undefined : Number(value);
      const birthday = { ...this.props.birthday, ...{ [field]: newValue } };
      this.props.onChange(birthday);
    }
  }
}
