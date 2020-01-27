import { FlagSolid, Ok, Send2 } from "@skbkontur/react-icons";
import * as cn from "classnames";
import * as React from "react";
import { CustomerStatusDto } from "../../api/new/dto/ServiceCalendar/ServiceCalendarRecordInfoDto";
import * as styles from "./StatusButtons.less";

interface Props {
  value: CustomerStatusDto;
  onChange: (value: CustomerStatusDto) => void;
}

export class StatusButtons extends React.Component<Props> {
  public render() {
    const { value } = this.props;
    return (
      <div className={styles.wrap}>
        {value === CustomerStatusDto.Active && <span data-tid="ActiveButtonSelected" />}
        <div
          data-tid="ActiveButton"
          className={cn(styles.button, styles.statusActive, value === CustomerStatusDto.Active ? styles.active : null)}
          onClick={() => this.props.onChange(CustomerStatusDto.Active)}
        >
          <FlagSolid /> Записали
        </div>
        {value === CustomerStatusDto.ActiveAccepted && <span data-tid="MessageButtonSelected" />}
        <div
          data-tid="MessageButton"
          className={cn(
            styles.button,
            styles.statusAccepted,
            value === CustomerStatusDto.ActiveAccepted ? styles.active : null
          )}
          onClick={() => this.props.onChange(CustomerStatusDto.ActiveAccepted)}
        >
          <Send2 /> Напомнили
        </div>
        {value === CustomerStatusDto.Completed && <span data-tid="CompletedButtonSelected" />}
        <div
          data-tid="CompletedButton"
          className={cn(
            styles.button,
            styles.statusComplete,
            value === CustomerStatusDto.Completed ? styles.active : null
          )}
          onClick={() => this.props.onChange(CustomerStatusDto.Completed)}
        >
          <Ok /> Выполнили
        </div>
      </div>
    );
  }
}
