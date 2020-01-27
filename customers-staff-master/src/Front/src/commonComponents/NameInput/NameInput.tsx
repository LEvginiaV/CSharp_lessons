import Input from "@skbkontur/react-ui/components/Input/Input";
import { InputProps } from "@skbkontur/react-ui/Input";
import * as React from "react";
import { StringHelper } from "../../helpers/StringHelper";

export class NameInput extends React.Component<InputProps> {
  private Input: Input | null;

  constructor(props: any, state: any) {
    super(props, state);
    this.state = { name: "" };
  }

  public focus() {
    this.Input && this.Input.focus();
  }

  public render(): JSX.Element {
    return <Input {...this.props} onChange={this.onNameChange} ref={el => (this.Input = el)} />;
  }

  private onNameChange = (_: any, newValue: string) => {
    const { value } = this.props;
    if (newValue && !StringHelper.isNameValid(newValue) && (value != null && value.length < newValue.length)) {
      this.Input && this.Input.blink();
    } else {
      this.props.onChange && this.props.onChange(_, newValue);
    }
  };
}
