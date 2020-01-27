import Sticky from "@skbkontur/react-ui/Sticky";
import * as cn from "classnames";
import * as React from "react";
import * as styles from "./Menu.less";
import { CashboxesSvg, CustomersAndStaffSvg, DocumentsSvg, MainSvg, ProductsSvg, ReportsSvg } from "./svg";

export class Menu extends React.Component<{}, {}> {
  private el: HTMLElement | null;

  public render() {
    return (
      <div className={styles.container}>
        <Sticky side="top" offset={0} getStop={this.getStop}>
          <div style={{ width: 100, overflow: "hidden" }}>
            {this.wrap(MainSvg)}
            {this.wrap(CustomersAndStaffSvg, true)}
            {this.wrap(DocumentsSvg)}
            {this.wrap(ProductsSvg)}
            {this.wrap(ReportsSvg)}
            {this.wrap(CashboxesSvg)}
          </div>
        </Sticky>
        <div className={styles.stop} ref={this.refStop} />
      </div>
    );
  }

  public getStop: () => HTMLElement | null = () => {
    return this.el;
  };

  private wrap(svg: any, selected: boolean = false) {
    return (
      <div className={cn(styles.imgDiv, selected ? styles.selected : "")}>
        <span className={styles.imgHelper} />
        <img src={svg} className={styles.img} />
      </div>
    );
  }

  private refStop: (el: HTMLElement | null) => any = el => {
    this.el = el;
  };
}
