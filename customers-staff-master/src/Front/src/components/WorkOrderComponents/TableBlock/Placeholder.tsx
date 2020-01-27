import * as React from "react";
import * as styles from "./Placeholder.less";

export interface IPlaceholderProps {
  width: number | string;
  align: "left" | "right";
  value?: string;
  marginLeft?: number;
  marginRight?: number;
}

export class Placeholder extends React.Component<IPlaceholderProps> {
  public render(): JSX.Element {
    const { value, align, width, marginLeft, marginRight } = this.props;

    return (
      <div
        className={styles.root}
        style={{ width, textAlign: align, paddingLeft: marginLeft, paddingRight: marginRight }}
      >
        <div className={styles.placeholder}>{value}</div>
      </div>
    );
  }
}
