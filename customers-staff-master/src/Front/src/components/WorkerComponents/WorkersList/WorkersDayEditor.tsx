import { Add, Trash } from "@skbkontur/react-icons";
import { ValidationContainer } from "@skbkontur/react-ui-validations";
import { Button, Dropdown, Gapped, MenuItem, Spinner } from "@skbkontur/react-ui/components";
import Link from "@skbkontur/react-ui/Link";
import MenuHeader from "@skbkontur/react-ui/MenuHeader";
import isEqual = require("lodash/isEqual");
import * as React from "react";
import { Guid } from "../../../api/new/dto/Guid";
import {
  getDayOfWeekName,
  toDateMonthNameShortString,
  toDateMonthNameString,
  toDateOnlyISOString,
} from "../../../common/DateHelper";
import { Metrics } from "../../../common/MetricsFacade";
import { TimeRangeSelector } from "../../../commonComponents/TimeRangeSelector/TimeRangeSelector";
import { TimeRangeSelectorValidationHelper } from "../../../commonComponents/TimeRangeSelector/TimeRangeSelectorValidationHelper";
import {
  getAutocompleteForEnd,
  getAutocompleteForStart,
} from "../../../commonComponents/TimeRangeSelector/WorkingCalendarAutocompleteProviders";
import { DateHelper } from "../../../helpers/DateHelper";
import { TimePeriodViewInfo } from "./Calculations/Interfaces";
import * as styles from "./WorkersDayEditor.less";

export interface IWorkersDayEditorProps {
  workerId: Guid;
  date: Date;
  onClose: () => void;
  onSave: (timePeriods: TimePeriodViewInfo[], mode: UpdateTimeMode) => Promise<void>;
  onRemove: (mode: UpdateTimeMode) => void;

  periods?: TimePeriodViewInfo[];
  prevDayPeriods?: TimePeriodViewInfo[];
  nextDayPeriods?: TimePeriodViewInfo[];
  placeholders?: TimePeriodViewInfo[];
}

interface IWorkersDayEditorState {
  isEditing: boolean;
  updateTimeMode: UpdateTimeMode;
  isSaving: boolean;
  savingFailed: boolean;

  times: TimePeriodViewInfo[];
}

export enum UpdateTimeMode {
  OneDay = "OneDay",
  TwoByTwo = "TwoByTwo",
  FiveByTwo = "FiveByTwo",
}

export class WorkersDayEditor extends React.Component<IWorkersDayEditorProps, IWorkersDayEditorState> {
  private ValidationContainer: ValidationContainer | null;
  private readonly TimeRangeSelectors: Array<Nullable<TimeRangeSelector>> = [];

  constructor(props: IWorkersDayEditorProps) {
    super(props);
    const times =
      props.periods && props.periods.length > 0
        ? props.periods
        : props.placeholders && props.placeholders.length > 0
        ? props.placeholders
        : [{ start: "", end: "", overflow: false }];
    this.state = {
      isEditing: !this.hasFilledPeriods(props.periods),
      updateTimeMode: UpdateTimeMode.OneDay,
      isSaving: false,
      savingFailed: false,
      times,
    };
  }

  public shouldComponentUpdate(
    nextProps: Readonly<IWorkersDayEditorProps>,
    nextState: Readonly<IWorkersDayEditorState>,
    _: any
  ): boolean {
    return !isEqual(this.props, nextProps) || !isEqual(this.state, nextState);
  }

  public render(): JSX.Element {
    const { date } = this.props;
    return (
      <div style={{ width: 272 }}>
        <div className={styles.header}>
          <span className={styles.date} data-tid="HeaderDate">
            {toDateMonthNameString(date)}
          </span>
          <span className={styles.dayOfWeek} data-tid="HeaderDayOfWeek">
            {getDayOfWeekName(date)}
          </span>
        </div>
        <ValidationContainer ref={el => (this.ValidationContainer = el)}>
          {this.state.isEditing ? this.renderEditing() : this.renderTimeInfo()}
        </ValidationContainer>
      </div>
    );
  }

  private hasFilledPeriods = (periods?: TimePeriodViewInfo[]): boolean => {
    return !!periods && periods.some(x => x && !!x.start && !!x.end);
  };

  private tryFocusInputAtIndex(index: number, tryNumber: number) {
    const timeRangeSelector = this.TimeRangeSelectors[index];
    if (timeRangeSelector && timeRangeSelector.FirstAutocompleteValidationWrapper) {
      timeRangeSelector.FirstAutocompleteValidationWrapper.focus();
    } else if (tryNumber < 5) {
      setTimeout(() => this.tryFocusInputAtIndex(index, tryNumber + 1), 50);
    }
  }

  private addNewRange() {
    const times = this.state.times.slice();
    const index = times.length;
    times.push({ start: "", end: "", overflow: false });
    this.setState({ times });
    this.tryFocusInputAtIndex(index, 0);
  }

  private removeRangeAtIndex(index: number) {
    const times = this.state.times.slice();
    times.splice(index, 1);
    this.setState({ times });
  }

  private renderTimeRangeLine(range: TimePeriodViewInfo, idx: number, isLast: boolean): JSX.Element {
    const isAddButton = idx === 0 && this.state.times.length < 5;
    return (
      <div key={idx} className={styles.editingRow} data-tid="TimeRangeLine">
        <div className={styles.title}>{idx === 0 && "Работает"}</div>
        <div>
          {this.renderTimeRangeSelectorByIdx(range, idx)}
          {!range.overflow && <div style={{ height: isLast ? 24 : 9 }} />}
          {range.overflow && (
            <div className={styles.nextDayLabel} data-tid="OverflowText">
              {toDateMonthNameShortString(this.getNextDate())}
            </div>
          )}
        </div>
        <div className={styles.icon}>
          {isAddButton && <Link data-tid="AddLink" icon={<Add />} onClick={() => this.addNewRange()} />}
          {!isAddButton && (
            <div className={styles.deleteIcon}>
              <div data-tid="DeleteLink" onClick={() => this.removeRangeAtIndex(idx)}>
                <Trash />
              </div>
            </div>
          )}
        </div>
      </div>
    );
  }

  private renderEditing(): JSX.Element {
    const rowCount = this.state.times.length;
    return (
      <div data-tid="EditingView">
        <div className={styles.editingHeader}>Установка рабочего времени</div>
        <div>{this.state.times.map((range, idx) => this.renderTimeRangeLine(range, idx, idx + 1 === rowCount))}</div>
        <div className={styles.editingRow}>
          <div className={styles.title} data-tid="FakeBlock">
            Повторять
          </div>
          {this.renderUpdateModeDropDown()}
        </div>
        <div className={styles.errorMessage}>{this.state.savingFailed && <span>Не удалось сохранить</span>}</div>
        <Gapped gap={8}>
          <Button use="primary" width={128} onClick={this.onSave} disabled={this.state.isSaving} data-tid="Save">
            {this.state.isSaving ? <Spinner dimmed type="mini" caption="" /> : "Сохранить"}
          </Button>
          <Button width={128} onClick={this.props.onClose} disabled={this.state.isSaving} data-tid="Cancel">
            Отменить
          </Button>
        </Gapped>
      </div>
    );
  }

  private getIsOverflow = (start: string, end: string): boolean => {
    if (!start || !end) {
      return false;
    }

    if (start.length + end.length === 10) {
      if (end === "00:00") {
        return true;
      }
      if (start === end) {
        return true;
      }
      if (start === "24:00") {
        return false;
      }
    }

    const minLength = Math.min(start.length, end.length);
    return start.substr(0, minLength) > end.substr(0, minLength);
  };

  private renderTimeRangeSelectorByIdx(range: TimePeriodViewInfo, index: number): JSX.Element {
    return (
      <TimeRangeSelector
        ref={el => (this.TimeRangeSelectors[index] = el)}
        startTime={range.start}
        endTime={range.end}
        autocompleteProviderStart={getAutocompleteForStart}
        autocompleteProviderEnd={getAutocompleteForEnd}
        onChange={(start: string, end: string) => {
          const times = this.state.times.slice();
          times[index] = { start, end, overflow: this.getIsOverflow(start, end) };
          this.setState({ times });
        }}
        startValidationFunc={() => TimeRangeSelectorValidationHelper.bindStartTimeValidation(range.start, range.end)}
        endValidationFunc={() => TimeRangeSelectorValidationHelper.bindEndTimeValidation(range.start, range.end)}
      />
    );
  }

  private renderUpdateModeDropDown(): JSX.Element {
    return (
      <Dropdown caption={getUpdateTimeModeName(this.state.updateTimeMode)} data-tid="CalendarMode" width={172}>
        {this.getDropDownMenuItem(UpdateTimeMode.OneDay)}
        {this.getDropDownMenuItem(UpdateTimeMode.TwoByTwo)}
        {this.getDropDownMenuItem(UpdateTimeMode.FiveByTwo)}
        <MenuHeader>
          <div>График начнется с этого дня</div>
          <div>и до конца месяца</div>
        </MenuHeader>
      </Dropdown>
    );
  }

  private getDropDownMenuItem(mode: UpdateTimeMode): JSX.Element {
    return <MenuItem onClick={() => this.setState({ updateTimeMode: mode })}>{getUpdateTimeModeName(mode)}</MenuItem>;
  }

  private onSave = async () => {
    const { updateTimeMode, times } = this.state;

    const nonEmptyTimes = times.filter(x => x.start.length + x.end.length > 0);

    if (nonEmptyTimes.length === 0) {
      return this.onRemove();
    }

    if (!this.ValidationContainer || !(await this.ValidationContainer.validate())) {
      return;
    }

    try {
      this.setState({ isSaving: true });
      await this.props.onSave(nonEmptyTimes, updateTimeMode);
    } catch (e) {
      console.error(e);
      this.setState({ savingFailed: true, isSaving: false });
    }
  };

  private onRemove = async () => {
    try {
      this.setState({ isSaving: true });
      await this.props.onRemove(this.state.updateTimeMode);
      Metrics.scheduleHoliday({ startDate: toDateOnlyISOString(this.props.date) });
    } catch (e) {
      console.error(e);
      this.setState({ savingFailed: true, isSaving: false });
    }
  };

  private renderTimeInfo(): JSX.Element {
    const { times } = this.state;
    return (
      <div data-tid="InfoView" style={{ paddingTop: 10 }}>
        {times.map((timePeriod, idx) => (
          <div key={idx} className={styles.timeInfo} data-tid="TimeInfoLine">
            <Gapped>
              <span data-tid="StartTimeText">{timePeriod.start}</span>—
              <span data-tid="EndTimeText">{timePeriod.end}</span>
              {timePeriod.overflow && (
                <span className={styles.timeInfoNextDate} data-tid="OverflowText">
                  {toDateMonthNameString(this.getNextDate())}
                </span>
              )}
            </Gapped>
          </div>
        ))}
        <div className={styles.errorMessage}>{this.state.savingFailed && <span>Не удалось сохранить</span>}</div>
        <div>
          <Gapped gap={8}>
            <Button
              use="primary"
              width={102}
              onClick={this.handleEditClick}
              disabled={this.state.isSaving}
              data-tid="Edit"
            >
              Изменить
            </Button>
            <Button width={154} onClick={this.onRemove} disabled={this.state.isSaving} data-tid="Remove">
              {this.state.isSaving ? <Spinner dimmed type="mini" caption="" /> : "Сделать выходным"}
            </Button>
          </Gapped>
        </div>
      </div>
    );
  }

  private handleEditClick = () => {
    Metrics.scheduleEditStart({ cardid: this.props.workerId });
    this.setState({ isEditing: true });
  };

  private getNextDate = () => DateHelper.addDays(this.props.date, 1);
}

function getUpdateTimeModeName(mode: UpdateTimeMode): string {
  switch (mode) {
    case UpdateTimeMode.OneDay:
      return "Только этот день";
    case UpdateTimeMode.TwoByTwo:
      return "2/2";
    case UpdateTimeMode.FiveByTwo:
      return "5/2";
    default:
      return "";
  }
}
