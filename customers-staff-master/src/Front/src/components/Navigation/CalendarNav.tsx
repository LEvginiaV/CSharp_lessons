import Calendar, { CalendarDateShape } from "@skbkontur/react-ui/components/Calendar/Calendar";
import * as React from "react";
import { RouteChildrenProps } from "react-router";
import { AppDataSingleton } from "../../app/AppData";
import { fromDateOnlyISOString } from "../../common/DateHelper";
import { pad2 } from "../../common/TimeSpanHelper";
import { ListType } from "../../models/ListType";
import * as styles from "./CalendarNav.less";

interface Props extends RouteChildrenProps<{ date: string }> {}

export class CalendarNav extends React.Component<Props> {
  public render() {
    const { match } = this.props;
    const date = match ? fromDateOnlyISOString(match.params.date) : null;
    if (!date) {
      return null;
    }
    const cds: CalendarDateShape = {
      year: date.getFullYear(),
      month: date.getMonth(),
      date: date.getDate(),
    };
    return (
      <div className={styles.calendarWrap}>
        <div className={styles.sizeReducer}>
          <Calendar value={cds} onSelect={this.onSelect} />
        </div>
      </div>
    );
  }

  private onSelect = (date: CalendarDateShape) => {
    const datePart = `${date.year}-${pad2(date.month + 1)}-${pad2(date.date)}`;
    if (this.props.match && datePart === this.props.match.params.date) {
      return;
    }
    const url = `${AppDataSingleton.prefix}/${ListType.Calendar}/${datePart}`;
    this.props.history.push(url);
  };
}
