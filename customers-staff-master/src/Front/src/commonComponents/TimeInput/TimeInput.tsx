import Autocomplete from "@skbkontur/react-ui/components/Autocomplete/Autocomplete";
import * as React from "react";
import { TimeSpan } from "../../api/new/dto/TimeSpan";

interface TimeProps {
  disabled?: boolean;
  error?: boolean;
  warning?: boolean;
  value: TimeSpan;
  onChange: (v: TimeSpan) => void;
  onBlur?: () => void;
}

const zerofill = (num: number) => (num > 9 ? num.toString() : "0" + num);

export class TimeInput extends React.Component<TimeProps> {
  public render() {
    return (
      <Autocomplete
        disabled={this.props.disabled}
        error={this.props.error}
        warning={this.props.warning}
        width={60}
        menuMaxHeight={150}
        mask="99:99"
        source={this.getItems}
        value={this.props.value.substring(0, 5)}
        onChange={(_, v) => this.props.onChange(v.length === 5 ? v + ":00" : v)}
        onBlur={this.props.onBlur}
      />
    );
  }

  private getItems = (): Promise<string[]> => {
    const { value } = this.props;

    let result: string[];

    if (value.length < 2) {
      // С шагом в час
      switch (value) {
        case "":
        case "0":
          result = this.getHoursItems(0, 9);
          break;
        case "1":
          result = this.getHoursItems(10, 19);
          break;
        default:
          result = this.getHoursItems(20, 24);
          break;
      }
    } else {
      // Шаг 15 минут
      const start = parseInt(value.substring(0, 2), 10);
      if (start >= 23) {
        result = this.getQuarterHoursItems(23).concat(["24:00"]);
      } else {
        result = this.getQuarterHoursItems(start).concat(this.getQuarterHoursItems(start + 1));
      }
    }

    return Promise.resolve(result);
  };

  private getHoursItems(start: number, end: number) {
    return new Array(end - start + 1).fill(start).map((x, i) => zerofill(x + i) + ":00");
  }

  private getQuarterHoursItems(hour: number) {
    const zhour = zerofill(hour);
    return [zhour + ":00", zhour + ":15", zhour + ":30", zhour + ":45"];
  }
}
