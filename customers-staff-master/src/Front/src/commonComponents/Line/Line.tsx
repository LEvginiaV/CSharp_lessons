import * as React from "react";

interface ILineProps {
  marginTop?: number;
  marginBottom?: number;
  marginLeft?: number;
}

export const Line: React.SFC<ILineProps> = props => <div style={{ ...props }}>{props.children}</div>;
