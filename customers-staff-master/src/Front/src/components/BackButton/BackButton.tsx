import { ArrowChevronLeft } from "@skbkontur/react-icons";
import Hint from "@skbkontur/react-ui/components/Hint/Hint";
import * as React from "react";
import * as styles from "./BackButton.less";

interface IBackButtonProps {
  useRightMargin?: boolean;
  onClick: () => void;
  top?: number;
  left?: number;
  width?: number;
  height?: number;
}

export class BackButton extends React.Component<IBackButtonProps, any> {
  public render() {
    const top = this.props.top || 18;
    const left = this.props.left || 0;
    const height = this.props.height || 120;
    const width = this.props.width || 40;
    const wrapperStyles = { width, height, top, left, marginRight: this.props.useRightMargin ? 5 : "unset" };

    return (
      <Hint text="Назад" pos="bottom" disableAnimations={false} useWrapper={true}>
        <div className={styles.backButtonWrapper} style={wrapperStyles}>
          <div
            onClick={this.props.onClick}
            className={styles.backButton}
            style={{ width, height }}
            data-tid="BackButton"
          >
            <div className={styles.backButtonIcon}>
              <ArrowChevronLeft color="#A0A0A0" size="22px" />
            </div>
          </div>
        </div>
      </Hint>
    );
  }
}
