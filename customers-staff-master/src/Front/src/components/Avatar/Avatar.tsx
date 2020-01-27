import * as classnames from "classnames";
import * as React from "react";
import { GenderType } from "../../models/GenderType";
import * as styles from "./Avatar.less";
import * as Female from "./female-avatar.svg";
import * as Male from "./male-avatar.svg";

interface IAvatarProps {
  name?: string;
  gender?: GenderType;
  useGender?: boolean;
}

export class Avatar extends React.Component<IAvatarProps> {
  public render(): JSX.Element | null {
    if (this.props.useGender) {
      return this.renderGender();
    } else if (this.props.name) {
      return <div className={styles.avatar}>{this.getAvatarChars(this.props.name)}</div>;
    } else {
      return <div className={classnames(styles.avatar, styles.dimmed)}>?</div>;
    }
  }

  private renderGender(): JSX.Element {
    // TODO: resolve paths in css-loader for beauty loading images
    const { gender, name } = this.props;
    const nameCaption = gender ? "" : name ? this.getAvatarChars(name) : "?";
    const backgroundImage = gender ? (gender === GenderType.Female ? Female : Male) : null;
    const className = classnames(gender ? styles.gender : [styles.avatar, styles.dimmed]);

    return (
      <div className={className} style={{ backgroundImage: `url(${backgroundImage})` }}>
        {nameCaption}
      </div>
    );
  }

  private getAvatarChars(name: string): string {
    const nameArr = name && name.split(" ");
    const second = nameArr && nameArr[1] ? nameArr[1].charAt(0) : "";
    return nameArr ? `${nameArr[0].charAt(0)}${second}` : name.charAt(0);
  }
}
