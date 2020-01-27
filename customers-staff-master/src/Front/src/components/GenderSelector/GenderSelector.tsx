import { Switcher } from "@skbkontur/react-ui/components";
import * as React from "react";
import { FEMALE_GENDER_TITLE, GenderType, GenderTypeMap, MALE_GENDER_TITLE } from "../../models/GenderType";
import * as styles from "./GenderSelector.less";

interface IGenderSelectorProps {
  onSelect: (gender: GenderType | null) => void;
  selected?: GenderType;
}

export class GenderSelector extends React.Component<IGenderSelectorProps> {
  public render(): JSX.Element {
    return (
      <div className={styles.wrapper}>
        <Switcher
          items={[GenderTypeMap[GenderType.Male], GenderTypeMap[GenderType.Female]]}
          value={this.props.selected && GenderTypeMap[this.props.selected]}
          onChange={this.onChange}
        />
      </div>
    );
  }

  private onChange = (_: any, value: string) => {
    const gender =
      value === MALE_GENDER_TITLE ? GenderType.Male : value === FEMALE_GENDER_TITLE ? GenderType.Female : null;
    this.props.onSelect(gender);
  };
}
