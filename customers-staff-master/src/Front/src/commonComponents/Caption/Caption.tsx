import * as classnames from "classnames";
import * as React from "react";
import * as styles from "./Caption.less";

export enum CaptionType {
  H1 = "H1",
  H3 = "H3",
  Gray = "gray",
  GraySmall = "graySmall",
}

export interface ICaptionProps {
  type?: CaptionType;
  className?: string;
  usePadding?: boolean;
  required?: boolean;
  align?: "left" | "center" | "right";
}

export class Caption extends React.Component<ICaptionProps> {
  public render(): JSX.Element {
    const { type, className, children, usePadding, required, align } = this.props;
    const captionClassName = classnames(
      styles.caption,
      type ? styles[type] : null,
      className,
      required && styles.required
    );

    const additionalStyles = { padding: usePadding ? "8px 0" : null, textAlign: align };

    return (
      <div className={captionClassName} style={additionalStyles as any}>
        {children}
      </div>
    );
  }
}
