import { Input } from "@skbkontur/react-ui/components";
import { InputProps } from "@skbkontur/react-ui/Input";
import * as React from "react";
import { StringHelper } from "../../helpers/StringHelper";

export class DiscountInput extends React.Component<InputProps> {
  private Input: Input;

  public render(): JSX.Element {
    return (
      <Input
        ref={(el: Input) => (this.Input = el)}
        {...this.props}
        onChange={this.onChange}
        value={StringHelper.formatDiscountValue(this.props.value)}
      />
    );
  }

  private onChange = (_: any, v: string) => {
    const value = StringHelper.getDiscountString(v, this.props.value);
    if (value !== this.props.value) {
      this.props.onChange && this.props.onChange(_, value);
    } else {
      this.Input && this.Input.blink();
    }
  };
}
