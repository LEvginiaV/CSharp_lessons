import ArrowChevronDownIcon from "@skbkontur/react-icons/ArrowChevronDown";
import ArrowChevronRightIcon from "@skbkontur/react-icons/ArrowChevronRight";
import * as React from "react";
import * as styles from "./SpoilerBlock.less";

export interface ISpoilerBlockProps {
  caption: string;
  captionElements?: React.ReactNode;
  openChanged?: (opened: boolean) => void;
}

export class SpoilerBlock extends React.Component<ISpoilerBlockProps, { opened: boolean }> {
  constructor(props: ISpoilerBlockProps, state: {}) {
    super(props, state);

    this.state = {
      opened: false,
    };
  }

  public open() {
    this.setState({ opened: true });
  }

  public render(): React.ReactNode {
    const { opened } = this.state;
    const { caption, captionElements, children } = this.props;

    return (
      <div>
        <div onClick={this.onClick} className={styles.caption} data-tid="SpoilerCaption">
          <div className={styles.icon}>
            {!opened && <ArrowChevronRightIcon size={18} />}
            {opened && <ArrowChevronDownIcon size={18} />}
          </div>
          <div className={styles.text}>{caption}</div>
          {captionElements && opened && <div className={styles.lastBlock}>{captionElements}</div>}
        </div>
        {opened && <div>{children}</div>}
      </div>
    );
  }

  private onClick = () => {
    const { opened } = this.state;
    this.setState({ opened: !opened });
    this.props.openChanged && this.props.openChanged(!opened);
  };
}
