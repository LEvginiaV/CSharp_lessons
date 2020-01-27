import { Edit, Trash } from "@skbkontur/react-icons";
import Link from "@skbkontur/react-ui/components/Link/Link";
import * as classnames from "classnames";
import * as React from "react";
import { GenderType } from "../../models/GenderType";
import { Avatar } from "../Avatar/Avatar";
import { BackButton } from "../BackButton/BackButton";
import * as styles from "./PersonHeader.less";

interface IPersonHeader {
  fullName: string;
  description?: string | JSX.Element | null;
  onEdit?: () => void; // TODO: required
  onRemove?: () => void; // TODO: required
  onBack: () => void; // TODO: required
  height?: number;
  genderType?: GenderType;
  useGender?: boolean;
  dimmedDescription?: boolean;
}

export class PersonHeader extends React.Component<IPersonHeader, {}> {
  public render(): JSX.Element {
    const { onBack, fullName, genderType, useGender, onEdit, description, onRemove, dimmedDescription } = this.props;
    const fullNameClassName = classnames(styles.name, !fullName && styles.dimmed);
    const descriptionClassName = classnames(styles.description, dimmedDescription && styles.dimmed);

    return (
      <div className={styles.wrapper}>
        <BackButton data-tid="BackButton" onClick={onBack} useRightMargin />
        <div className={styles.header}>
          <Avatar data-tid="Avatar" name={fullName} gender={genderType} useGender={useGender} />
          <div className={onRemove ? styles.widePersonInfo : styles.personInfo}>
            <div className={styles.nameWrapper}>
              <div data-tid="PersonName" className={fullNameClassName}>
                {fullName || "Имя не указано"}
              </div>
              <div className={styles.editLink}>
                <Link icon={<Edit />} data-tid="PersonEditLink" onClick={onEdit}>
                  Изменить
                </Link>
              </div>
            </div>
            <div data-tid="PersonDescription" className={descriptionClassName}>
              {description}
            </div>
          </div>
          {onRemove && (
            <div className={styles.service}>
              <span className={styles.removeLink}>
                <Link icon={<Trash />} data-tid="PersonRemoveLink" onClick={onRemove}>
                  Удалить
                </Link>
              </span>
            </div>
          )}
        </div>
      </div>
    );
  }
}
