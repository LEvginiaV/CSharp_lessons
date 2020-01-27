import { Autocomplete } from "@skbkontur/react-ui/components";
import * as React from "react";
import { ReactElement, ReactNode } from "react";

export class AutocompleteValidationWrapper extends React.Component<
  {
    onMouseLeave?: () => void;
    onMouseEnter?: () => void;
    onFocus?: () => void;
    onBlur?: () => void;
    onChange?: () => void;
    onInput?: () => void;
    error?: boolean;
  },
  { value: any }
> {
  public state = {
    value: null,
  };
  private child: Autocomplete;

  public shouldComponentUpdate(nextProps: Readonly<{ error?: boolean; children: ReactNode }>): boolean {
    const nextOnlyChild = React.Children.only(nextProps.children) as ReactElement;
    const onlyChild = React.Children.only(this.props.children) as ReactElement;
    return nextProps.error !== this.props.error || nextOnlyChild.props.value !== onlyChild.props.value;
  }

  public render(): React.ReactNode {
    const { children, onMouseLeave, onMouseEnter, onFocus, onBlur, error, onChange } = this.props;
    const onlyChild = React.Children.only(children) as ReactElement;

    const renderedChild = React.cloneElement(onlyChild, {
      onMouseLeave,
      onMouseEnter,
      onFocus,
      onBlur,
      error,
      onChange: (e: any, v: any) => {
        onlyChild.props.onChange(e, v);
        onChange && onChange();
      },
      ref: (el: Autocomplete) => (this.child = el),
    });
    return <div>{renderedChild}</div>;
  }

  public focus() {
    this.child && this.child.focus();
  }

  public blur() {
    this.child && this.child.blur();
  }
}
