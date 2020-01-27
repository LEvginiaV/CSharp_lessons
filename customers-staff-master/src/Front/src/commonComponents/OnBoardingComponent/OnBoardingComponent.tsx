import Icon from "@skbkontur/react-icons";
import Button from "@skbkontur/react-ui/Button";
import * as cn from "classnames";
import { ProgressBarBullets } from "market-ui/dist";
import * as React from "react";
import * as styles from "./OnBoardingComponent.less";

export interface OnBoardingPage {
  headerTextLines: string[];
  imageUrl: any;
  footerTextLines: string[];
}

export interface IOnBoardingComponentProps {
  pages: OnBoardingPage[];
  onFinish: () => void;
  startText?: string;
}

export class OnBoardingComponent extends React.Component<IOnBoardingComponentProps, { pageIndex: number }> {
  constructor(props: IOnBoardingComponentProps, state: {}) {
    super(props, state);

    this.state = {
      pageIndex: 0,
    };
  }

  public render(): React.ReactNode {
    const { pages, startText } = this.props;
    const { pageIndex } = this.state;
    const page = pages[pageIndex];
    return (
      <div className={styles.root}>
        <div className={styles.header}>
          {page.headerTextLines.map((x, i) => (
            <div key={i}>{x}</div>
          ))}
        </div>
        <div className={styles.body}>
          <div
            onClick={this.onClickLeft}
            className={pageIndex === 0 ? cn([styles.left, styles.disabledArrow]) : styles.left}
          >
            <Icon name="ArrowChevronLeft" />
          </div>
          <div className={styles.center}>
            <img src={page.imageUrl} alt="" />
          </div>
          <div
            onClick={() => this.onClickRight()}
            className={pageIndex === pages.length - 1 ? cn([styles.right, styles.disabledArrow]) : styles.right}
          >
            <Icon name="ArrowChevronRight" />
          </div>
        </div>
        <div className={styles.progress}>
          <ProgressBarBullets currentStep={pageIndex} lastStep={pages.length - 1} />
        </div>
        <div className={styles.footer}>
          {page.footerTextLines.map((x, i) => (
            <div key={i}>{x}</div>
          ))}
        </div>
        <div>
          <Button
            onClick={() => this.onClickRight(true)}
            width={pageIndex !== pages.length - 1 || !startText ? 140 : undefined}
            use="primary"
          >
            {pageIndex !== pages.length - 1 ? "Далее" : startText || "Начать работу"}
          </Button>
        </div>
      </div>
    );
  }

  private onClickLeft = () => {
    const { pageIndex } = this.state;
    if (pageIndex > 0) {
      this.setState({ pageIndex: pageIndex - 1 });
    }
  };

  private onClickRight = (finish: boolean = false) => {
    const { pageIndex } = this.state;
    const { pages, onFinish } = this.props;
    if (pageIndex < pages.length - 1) {
      this.setState({ pageIndex: pageIndex + 1 });
    } else if (finish) {
      onFinish();
    }
  };
}
