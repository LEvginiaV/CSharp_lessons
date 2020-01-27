import * as React from "react";

interface TimeProps {
  value: Nullable<string>;
}

export class FormattedPhone extends React.Component<TimeProps> {
  public render() {
    const { value } = this.props;
    const toRender = value ? value.replace(/(\+?7|8)?(\d{3})(\d{3})(\d{2})(\d{2})/, "+7 $2 $3-$4-$5") : "";
    return <span>{toRender.trim()}</span>;
  }
}
