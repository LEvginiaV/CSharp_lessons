import * as React from "react";
import { CSSProperties } from "react";
import * as styles from "./Grid.less";

interface IGridProps {
  columns: number[];
  children: React.ReactNode | React.ReactNode[];
  alignItems?: "flex-start" | "flex-end" | "baseline" | "stretch" | null;
}

export class Grid extends React.Component<IGridProps, {}> {
  public render(): JSX.Element {
    return (
      <div className={styles.container} style={this.getContainerStyles()}>
        {React.Children.map(this.props.children, this.wrapChild)}
      </div>
    );
  }

  private wrapChild = (child: React.ReactNode, i: number): JSX.Element | null => {
    if (!child) {
      return null;
    }

    const cellStyles = {
      boxSizing: "border-box",
      alignItems: "center",
      width: this.props.columns[i],
      flex: "0 0 auto",
    };

    return (
      <div className={styles.item} style={cellStyles as CSSProperties} key={i}>
        {child}
      </div>
    );
  };

  private getContainerStyles(): { [key: string]: string } {
    const alignItems = this.props.alignItems || "center";
    return {
      alignItems,
    };
  }
}
