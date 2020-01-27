import Input, { InputProps } from "@skbkontur/react-ui/Input";
import * as React from "react";

export interface PredicateInputProps extends InputProps {
  predicate: (value: string) => boolean;
  postProcess?: (value: string) => string;
}

export class PredicateInput extends React.Component<PredicateInputProps> {
  private Input: Input | null;

  public focus() {
    this.Input && this.Input.focus();
  }

  public blur() {
    this.Input && this.Input.blur();
  }

  public render(): JSX.Element {
    const { predicate, postProcess, ...inputProps } = this.props;
    return <Input {...inputProps} onChange={this.onChange} ref={el => (this.Input = el)} />;
  }

  private onChange = (_: any, value: string) => {
    const { predicate, postProcess } = this.props;
    if (value && !predicate(value)) {
      this.Input && this.Input.blink();
    } else {
      this.props.onChange && this.props.onChange(_, postProcess ? postProcess(value) : value);
    }
  };
}
