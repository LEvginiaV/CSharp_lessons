import { Add } from "@skbkontur/react-icons";
import { Button } from "@skbkontur/react-ui/components";
import * as React from "react";
import * as styles from "./EmptyPage.less";

export interface IEmptyPageProps {
  emptyCaption: string | JSX.Element;
  emptyButtonCaption?: string;
  onAdd?: () => void;
}

export const EmptyPage: React.StatelessComponent<IEmptyPageProps> = props => {
  return (
    <div className={styles.wrapper}>
      {props.emptyCaption}
      <div className={styles.button}>
        <Button
          size="medium"
          data-tid="EmptyPersonListAddButton"
          use="primary"
          onClick={() => props.onAdd && props.onAdd()}
        >
          <Add /> {props.emptyButtonCaption}
        </Button>
      </div>
    </div>
  );
};
