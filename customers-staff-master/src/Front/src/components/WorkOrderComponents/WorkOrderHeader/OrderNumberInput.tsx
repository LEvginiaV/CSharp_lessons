import Input, { InputProps } from "@skbkontur/react-ui/Input";
import * as React from "react";
import { ChangeEvent } from "react";
import { StringHelper } from "../../../helpers/StringHelper";

export class OrderNumberInput extends React.Component<InputProps, { isFocused: boolean }> {
  private Input: Input | null;

  constructor(props: InputProps, state: {}) {
    super(props, state);

    this.state = {
      isFocused: false,
    };
  }

  public componentDidUpdate(_: Readonly<InputProps>, prevState: Readonly<{ isFocused: boolean }>): void {
    if (this.Input && !prevState.isFocused && this.state.isFocused) {
      this.Input.selectAll();
    }
  }

  public focus() {
    this.Input && this.Input.focus();
  }

  public blur() {
    this.Input && this.Input.blur();
  }

  public render(): JSX.Element {
    const value =
      this.state.isFocused || !this.props.value ? this.props.value : StringHelper.formatOrderNumber(+this.props.value);
    return (
      <Input
        {...this.props}
        value={value}
        onFocus={this.onFocus}
        onBlur={this.onBlur}
        onChange={this.onChange}
        ref={el => (this.Input = el)}
      />
    );
  }

  // @ts-ignore
  private onFocus = (event: FocusEvent<HTMLInputElement>) => {
    this.setState({ isFocused: true });
    this.props.onFocus && this.props.onFocus(event);
  };

  // @ts-ignore
  private onBlur = (event: FocusEvent<HTMLInputElement>) => {
    this.setState({ isFocused: false });
    this.props.onBlur && this.props.onBlur(event);
  };

  private onChange = (_: ChangeEvent<HTMLInputElement>, value: string) => {
    if (value && !StringHelper.isOrderNumberValid(value)) {
      this.Input && this.Input.blink();
    } else {
      this.props.onChange && this.props.onChange(_, value);
    }
  };
}
