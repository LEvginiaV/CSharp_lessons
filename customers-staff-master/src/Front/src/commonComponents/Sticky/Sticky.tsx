import UiSticky, { StickyProps } from "@skbkontur/react-ui/Sticky";
import * as React from "react";
import { TestUtils } from "../../helpers/TestUtils";

export class Sticky extends React.Component<StickyProps> {
  public render() {
    if (TestUtils.isTestingMode()) {
      // @ts-ignore
      const children = typeof this.props.children === "function" ? this.props.children() : this.props.children;
      return <div>{children}</div>;
    } else {
      return <UiSticky {...this.props}>{this.props.children}</UiSticky>;
    }
  }
}
