import Input, { InputProps } from "@skbkontur/react-ui/Input";
import * as React from "react";
import { StringHelper } from "../../../helpers/StringHelper";

export class SeriesInput extends React.Component<InputProps> {
  private Input: Input | null;

  public focus() {
    this.Input && this.Input.focus();
  }

  public blur() {
    this.Input && this.Input.blur();
  }

  public render(): JSX.Element {
    return <Input {...this.props} onChange={this.onChange} ref={el => (this.Input = el)} />;
  }

  private onChange = (_: any, value: string) => {
    if (value && !StringHelper.isOrderSeriesValid(value)) {
      this.Input && this.Input.blink();
    } else {
      this.props.onChange && this.props.onChange(_, value.toUpperCase());
    }
  };
}
