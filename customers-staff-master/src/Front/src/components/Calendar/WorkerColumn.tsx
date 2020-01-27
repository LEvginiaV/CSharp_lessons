import { Add } from "@skbkontur/react-icons";
import * as React from "react";
import { connect } from "react-redux";
import { Guid } from "../../api/new/dto/Guid";
import { ServiceCalendarRecordDto } from "../../api/new/dto/ServiceCalendar/ServiceCalendarRecordDto";
import { CustomerStatusDto } from "../../api/new/dto/ServiceCalendar/ServiceCalendarRecordInfoDto";
import { WorkerServiceCalendarDayDto } from "../../api/new/dto/ServiceCalendar/WorkerServiceCalendarDayDto";
import { TimePeriodDto, WorkingCalendarRecordDto } from "../../api/new/dto/WorkingCalendar/WorkersScheduleDtos";
import { Metrics } from "../../common/MetricsFacade";
import { timeSpan_to_hhmm, TimeSpanHelper } from "../../common/TimeSpanHelper";
import { RootState, TypeOfConnect } from "../../redux/rootReducer";
import { CalendarHelper } from "./CalendarHelper";
import { CalendarRecord } from "./CalendarRecord";
import { CancelRecordModal } from "./CancelRecordModal";
import * as styles from "./WorkerColumn.less";

type Props = {
  info: Nullable<WorkerServiceCalendarDayDto>;
  workingTime: WorkingCalendarRecordDto[];
  nowMinutes: Nullable<number>;
  onStartAdd: (period: TimePeriodDto) => void;
  onEdit: (rec: ServiceCalendarRecordDto) => void;
  onChangeStatus: (recordId: Guid, status: CustomerStatusDto) => void;
  onCancelRecord: (recordId: Guid) => void;
} & TypeOfConnect<typeof reduxConnector>;

interface State {
  modalCancelGuid: Nullable<Guid>;
  hoverMinutesFrom: Nullable<number>;
}

export class WorkerColumnView extends React.Component<Props, State> {
  public state: Readonly<State> = {
    modalCancelGuid: null,
    hoverMinutesFrom: null,
  };

  public render() {
    const a = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23];
    const list = this.props.info ? this.props.info.records : [];

    return (
      <div
        className={styles.workerColumn}
        onMouseMove={this.handleMouseMove}
        onMouseLeave={() => this.setState({ hoverMinutesFrom: null })}
      >
        <div>
          {this.props.workingTime.map((x, i) => this.renderWorkingTimes(i, x.period.startTime, x.period.endTime))}
        </div>
        <div>
          {a.map(hour => (
            <div
              key={hour}
              className={styles.hourDelimiter}
              data-tid="HourDelimiter"
              style={{ top: CalendarHelper.timeToPx(hour * 60) - 1 }}
            />
          ))}
        </div>
        {this.state.hoverMinutesFrom != null && this.renderHoverDiv(this.state.hoverMinutesFrom)}
        {list.map((rec, i) => (
          <div
            key={i}
            className={styles.card}
            style={{ top: CalendarHelper.timeToPx(TimeSpanHelper.toMinutes(rec.period.startTime)) }}
            onMouseMove={e => {
              e.stopPropagation();
              this.setHoverMinutesFrom(null);
            }}
          >
            <CalendarRecord
              customer={this.props.customers.find(x => x.id === rec.customerId)}
              record={rec}
              nomenclature={this.props.nomenclature}
              onEdit={() => this.props.onEdit(rec)}
              onChangeStatus={status => {
                this.props.onChangeStatus(rec.id, status);
                Metrics.calendarStatus({
                  bookingid: rec.id,
                  statusOld: rec.customerStatus,
                  statusNew: status,
                });
              }}
              onCancel={() => {
                this.setState({ modalCancelGuid: rec.id });
                Metrics.calendarDeleteStart({ bookingid: rec.id });
              }}
            />
          </div>
        ))}
        {this.renderNowLine()}
        {this.state.modalCancelGuid && this.renderModalCancel(this.state.modalCancelGuid)}
      </div>
    );
  }

  private renderNowLine() {
    if (this.props.nowMinutes == null) {
      return null;
    }
    const topInPx = CalendarHelper.timeToPx(this.props.nowMinutes);
    return (
      <div key="nowLine" className={styles.nowLine} style={{ top: topInPx }} data-tid="NowLine" data-top={topInPx} />
    );
  }

  private renderWorkingTimes(index: number, startTime: string, endTime: string) {
    const start = CalendarHelper.timeToPx(TimeSpanHelper.toMinutes(startTime));
    const end = CalendarHelper.timeToPx(TimeSpanHelper.toMinutes(endTime));
    const style = {
      height: end - start,
      top: start,
    };
    return <div key={index} className={styles.workingTime} style={style} />;
  }

  private renderHoverDiv(minutes: number) {
    const startTime = TimeSpanHelper.fromMinutes(minutes);
    return (
      <div
        className={styles.hoverDiv}
        style={{ top: CalendarHelper.timeToPx(minutes) }}
        onClick={() =>
          this.props.onStartAdd({
            startTime,
            endTime: TimeSpanHelper.fromMinutes(minutes + 60),
          })
        }
      >
        <Add />
        {timeSpan_to_hhmm(startTime)}
      </div>
    );
  }

  private renderModalCancel(recordId: Guid) {
    return (
      <CancelRecordModal
        recordId={recordId}
        onCancel={(status: CustomerStatusDto) => {
          this.props.onChangeStatus(recordId, status);
          this.setState({ modalCancelGuid: null });
          Metrics.calendarDeleteSuccess({ bookingid: recordId, reason: status });
        }}
        onClose={() => {
          Metrics.calendarDeleteCancel({ bookingid: recordId });
          this.setState({ modalCancelGuid: null });
        }}
      />
    );
  }

  private handleMouseMove = (e: React.MouseEvent<HTMLDivElement>) => {
    const rect = e.currentTarget.getBoundingClientRect();
    const top = e.clientY - rect.top;
    if (this.props.info) {
      const found = this.props.info.records.find(rec => {
        const startPx = CalendarHelper.timeToPx(rec.period.startTime);
        const endPx = CalendarHelper.timeToPx(rec.period.endTime);
        if (startPx != null && endPx != null) {
          return top > startPx && top < endPx;
        }
        return false;
      });
      if (found) {
        this.setState({ hoverMinutesFrom: null });
        return;
      }
    }
    const hoverMinutesFrom = Math.floor(CalendarHelper.pxToTime(top) / 30) * 30;
    this.setHoverMinutesFrom(hoverMinutesFrom);
  };

  private setHoverMinutesFrom = (hoverMinutesFrom: Nullable<number>) => {
    if (this.state.hoverMinutesFrom !== hoverMinutesFrom) {
      this.setState({ hoverMinutesFrom });
    }
  };
}

const reduxConnector = connect(
  (state: RootState) => ({
    customers: state.customers.customers || [],
    nomenclature: state.nomenclature.cards,
  }),
  undefined
);

export const WorkerColumn = reduxConnector(WorkerColumnView);
