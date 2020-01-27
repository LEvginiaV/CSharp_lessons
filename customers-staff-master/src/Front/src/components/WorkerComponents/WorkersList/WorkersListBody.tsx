import { Paging, Tooltip } from "@skbkontur/react-ui/components";
import * as classnames from "classnames";
import sortBy = require("lodash/sortBy");
import * as React from "react";
import { Link } from "react-router-dom";
import { ApiSingleton } from "../../../api/new/Api";
import { Guid } from "../../../api/new/dto/Guid";
import { WorkerDto } from "../../../api/new/dto/WorkerDto";
import { ShopWorkingCalendarDto } from "../../../api/new/dto/WorkingCalendar/WorkersScheduleDtos";
import { AppDataSingleton } from "../../../app/AppData";
import { getDayInfosForMonth, toDateOnlyISOString } from "../../../common/DateHelper";
import { FeatureAppearance } from "../../../common/FeatureAppearance";
import { Metrics } from "../../../common/MetricsFacade";
import { Caption, CaptionType } from "../../../commonComponents/Caption/Caption";
import { FeedbackType } from "../../../commonComponents/Feedback/FeedbackType";
import { DateHelper } from "../../../helpers/DateHelper";
import { ListType } from "../../../models/ListType";
import { WorkersListTabType } from "../../../models/WorkersListTabType";
import { buildCalendarMap, getDaysCount, getHoursCount } from "./Calculations/CalendarMapBuilder";
import { getApiModels } from "./Calculations/CalendarUpdateTimeCalculator";
import { TimePeriodViewInfo, WorkingCalendarDayViewInfo } from "./Calculations/Interfaces";
import { UpdateTimeMode, WorkersDayEditor } from "./WorkersDayEditor";
import * as styles from "./WorkersListBody.less";

const ItemsPerPage = 30;

export interface IWorkersListBodyProps {
  isChart: boolean;
  workers: WorkerDto[] | null;
  calendar: ShopWorkingCalendarDto | null;
  month: Date;
  reloadCalendar: () => void;
  isLoading: boolean;
  isLoadingFailed: boolean;
}

interface IWorkersListBodyState {
  selectedRow?: number;
  selectedCol?: number;
  defaultLine: WorkingCalendarDayViewInfo[];
  pageNumber: number;
}

export class WorkersListBody extends React.Component<IWorkersListBodyProps, IWorkersListBodyState> {
  constructor(props: Readonly<IWorkersListBodyProps>) {
    super(props);
    this.state = {
      selectedCol: undefined,
      selectedRow: undefined,
      defaultLine: createDefaultDays(props.month),
      pageNumber: 1,
    };
  }

  public componentWillReceiveProps(nextProps: Readonly<IWorkersListBodyProps>, _: any): void {
    if ((nextProps.isLoading && this.state.selectedRow !== undefined) || !nextProps.isChart) {
      this.closeEditor();
    }
    if (nextProps.month !== this.props.month) {
      this.setState({ defaultLine: createDefaultDays(nextProps.month) });
    }
  }

  public render(): React.ReactNode {
    const { isChart, calendar } = this.props;
    const workersForPage = this.getWorkers();
    const calendarMap = buildCalendarMap(calendar);
    return (
      <div>
        <div className={styles.root}>
          <div className={isChart ? styles.namesTable : styles.names}>
            {this.renderNames(workersForPage, calendarMap)}
          </div>
          {this.renderDates(workersForPage, calendarMap)}
        </div>
        {this.renderPaging()}
      </div>
    );
  }

  private getWorkers = (): WorkerDto[] => {
    const { workers } = this.props;
    const { pageNumber } = this.state;
    if (!workers) {
      return [];
    }
    return workers.slice(ItemsPerPage * (pageNumber - 1), ItemsPerPage * pageNumber);
  };

  private getWorkingDays = (map: Map<Guid, WorkingCalendarDayViewInfo[]>, workerId: Guid) => {
    return map.get(workerId) || this.state.defaultLine;
  };

  private changePage = (newPage: number) => this.setState({ pageNumber: newPage });

  private renderPaging(): JSX.Element | null {
    const { workers } = this.props;
    if (!workers) {
      return null;
    }
    const pagesCount = Math.ceil(workers.length / ItemsPerPage);
    if (pagesCount <= 1) {
      return null;
    }
    return (
      <div className={styles.paging}>
        <Paging
          data-tid="Paging"
          useGlobalListener={false}
          activePage={this.state.pageNumber}
          onPageChange={this.changePage}
          pagesCount={pagesCount}
        />
      </div>
    );
  }

  private renderNames(
    workersForPage: WorkerDto[],
    calendarMap: Map<Guid, WorkingCalendarDayViewInfo[]>
  ): JSX.Element | null {
    if (!this.props.workers) {
      return null;
    }
    return (
      <div>{workersForPage.map((x, idx) => this.renderNameItem(x, idx, this.getWorkingDays(calendarMap, x.id)))}</div>
    );
  }

  private renderNameItem = (
    item: WorkerDto,
    i: number,
    workingDays: WorkingCalendarDayViewInfo[]
  ): JSX.Element | null => {
    if (!item) {
      return null;
    }

    const { isChart } = this.props;
    const nameClassNames = classnames(styles.name, !item.fullName && styles.nameDimmed);
    const redirectLink = `${AppDataSingleton.prefix}/${ListType.Workers}/${WorkersListTabType.List}/${item.id}`;
    return (
      <Link data-tid="WorkerItem" to={redirectLink} key={i} className={styles.item}>
        <div className={styles.itemInner}>
          <div className={styles.nameCellLineWrapper}>
            <div data-tid="Name" className={nameClassNames}>
              {item.fullName || "Имя не указано"}
            </div>
            {isChart && (
              <div className={styles.nameCellCounter} data-tid="HoursCounter">
                {getHoursCount(workingDays)}
              </div>
            )}
          </div>
          <div className={styles.nameCellLineWrapper}>
            <Caption data-tid="Position" type={CaptionType.Gray} className={styles.description}>
              {item.position || "Должность не указана"}
            </Caption>
            {isChart && (
              <Caption type={CaptionType.Gray} className={styles.nameCellCounterGray} data-tid="DaysCounter">
                {getDaysCount(workingDays)}
              </Caption>
            )}
          </div>
        </div>
      </Link>
    );
  };

  private renderLoadingHover(workerCount: number): JSX.Element | null {
    if (!this.props.isLoading && !this.props.isLoadingFailed) {
      return null;
    }
    const { defaultLine } = this.state;
    const width = (defaultLine.length - 2) * 22 + 1;
    const height = workerCount * 67;
    return <div data-tid="LoadingHover" className={styles.loadingHover} style={{ width, height }} />;
  }

  private renderDates(
    workersForPage: WorkerDto[],
    calendarMap: Map<Guid, WorkingCalendarDayViewInfo[]>
  ): JSX.Element | null {
    const { workers, isChart } = this.props;
    if (!workers) {
      return null;
    }
    return (
      <div className={isChart ? styles.timeTableWrapper : styles.timeTableHideWrapper}>
        {isChart && this.renderLoadingHover(workersForPage.length)}
        <div>
          {workersForPage.map((worker, idx) =>
            this.renderDatesRow(worker, idx, this.getWorkingDays(calendarMap, worker.id))
          )}
        </div>
      </div>
    );
  }

  private renderDatesRow(worker: WorkerDto, rowIdx: number, days: WorkingCalendarDayViewInfo[]): JSX.Element | null {
    const dayInfos = getDayInfosForMonth(this.props.month);
    return (
      <div key={rowIdx} data-tid="CalendarRow" style={{ display: "flex" }}>
        {dayInfos.map((dayInfo, idx) => {
          const colIdx = idx + 1;
          return (
            <div
              key={colIdx}
              data-tid="CalendarCell"
              className={this.getTimeCellStyle(dayInfo.isWeekend, days[colIdx])}
              onClick={() => {
                this.hasRecord(days[colIdx]) || Metrics.scheduleCreateStart();
                this.openEditor(rowIdx, colIdx);
              }}
            >
              {this.isSelected(rowIdx, colIdx) && (
                <Tooltip
                  data-tid={"WorkersDayEditor"}
                  useWrapper={false}
                  trigger={"opened"}
                  pos={colIdx < 15 ? "bottom left" : "bottom right"}
                  allowedPositions={colIdx < 15 ? ["bottom left", "top left"] : ["bottom right", "top right"]}
                  disableAnimations={false}
                  onCloseClick={this.closeEditor}
                  render={() => (
                    <WorkersDayEditor
                      workerId={worker.id}
                      onSave={(timePeriods, mode) =>
                        this.handleSaveWorkerDay(worker.id, dayInfo.date, timePeriods, days, colIdx, mode)
                      }
                      onRemove={mode => this.updateTime(worker.id, dayInfo.date, [], days, colIdx, mode)}
                      onClose={this.closeEditor}
                      date={dayInfo.date}
                      periods={days[colIdx].timePeriods}
                      prevDayPeriods={days[colIdx - 1].timePeriods}
                      nextDayPeriods={days[colIdx + 1].timePeriods}
                      placeholders={this.getTimePeriodPlaceholder(days, colIdx)}
                    />
                  )}
                >
                  <div style={{ width: 19, height: 64 }}>
                    <div className={styles.selectedBorderForced} />
                  </div>
                </Tooltip>
              )}
              {!this.isSelected(rowIdx, colIdx) && (
                <div style={{ width: 19, height: 64 }}>
                  <div className={styles.selectedBorder} />
                </div>
              )}
              <div data-tid={this.hasRecord(days[colIdx]) ? "Filled" : "Empty"} />
            </div>
          );
        })}
      </div>
    );
  }

  private async handleSaveWorkerDay(
    workerId: Guid,
    date: Date,
    timePeriods: TimePeriodViewInfo[],
    days: WorkingCalendarDayViewInfo[],
    index: number,
    mode: UpdateTimeMode
  ) {
    await this.updateTime(workerId, date, timePeriods, days, index, mode);
    const filledPeriods = timePeriods.filter(p => p.start.length + p.end.length === 10);
    const startTime = filledPeriods.length > 0 ? sortBy(filledPeriods, ["start"])[0].start : "";
    const endTime =
      filledPeriods.length > 0 ? sortBy(filledPeriods, ["overflow", "end"])[filledPeriods.length - 1].end : "";
    const payload = {
      cardid: workerId,
      startDate: toDateOnlyISOString(date),
      repeat: Metrics.variablesGetScheduleMode(mode),
      overflow: filledPeriods.some(x => x.overflow),
      startTime,
      endTime,
      periodsCount: filledPeriods.length,
      periods: filledPeriods,
    };

    if (this.hasRecord(days[index])) {
      Metrics.scheduleEditSuccess(payload);
    } else {
      Metrics.scheduleCreateSuccess(payload);
    }
  }

  private updateTime = async (
    workerId: Guid,
    date: Date,
    periods: TimePeriodViewInfo[],
    days: WorkingCalendarDayViewInfo[],
    index: number,
    mode?: UpdateTimeMode
  ) => {
    FeatureAppearance.activate(FeedbackType.WorkersSchedulesFeedback);
    const updates = getApiModels(date, periods, days, index, mode);
    if (updates.length > 0) {
      await ApiSingleton.WorkingCalendarApi.update(workerId, updates);
      await this.props.reloadCalendar();
    }
    this.closeEditor();
  };

  private getTimePeriodPlaceholder = (
    days: WorkingCalendarDayViewInfo[],
    idx: number
  ): TimePeriodViewInfo[] | undefined => {
    if (days[idx].timePeriods.length > 0 && days[idx].timePeriods.some(x => x.start !== "00:00")) {
      return undefined;
    }

    for (let i = days.length - 2; i >= 1; i--) {
      const day = days[i];
      if (day && day.timePeriods && day.timePeriods.length > 0) {
        return day.timePeriods.map(p => ({ start: p.start, end: p.end, overflow: p.overflow }));
      }
    }
    return undefined;
  };

  private openEditor = (rowIdx: number, colIdx: number) => {
    if (this.props.isLoading) {
      return;
    }
    this.setState({ selectedRow: rowIdx, selectedCol: colIdx });
  };

  private closeEditor = () => {
    this.setState({ selectedRow: undefined, selectedCol: undefined });
  };

  private isSelected = (rowIdx: number, colIdx: number): boolean => {
    return this.state.selectedRow === rowIdx && this.state.selectedCol === colIdx;
  };

  private hasRecord = (dayInfo: WorkingCalendarDayViewInfo): boolean => {
    return dayInfo && dayInfo.timePeriods && dayInfo.timePeriods.length > 0;
  };

  private getTimeCellStyle(isWeekend: boolean, dayInfo: WorkingCalendarDayViewInfo): any {
    return classnames(styles.timeCell, {
      [styles.timeCellWeekend]: isWeekend,
      [styles.timeCellWithData]: this.hasRecord(dayInfo),
    });
  }
}

function createDefaultDays(month: Date): WorkingCalendarDayViewInfo[] {
  const result: WorkingCalendarDayViewInfo[] = [];
  const dayInfos = getDayInfosForMonth(month);

  const prevDate = DateHelper.addDays(dayInfos[0].date, -1);
  result.push({ date: toDateOnlyISOString(prevDate), timePeriods: [] });

  dayInfos.forEach(info => result.push({ date: toDateOnlyISOString(info.date), timePeriods: [] }));

  const nextDate = DateHelper.addDays(dayInfos[dayInfos.length - 1].date, 1);
  result.push({ date: toDateOnlyISOString(nextDate), timePeriods: [] });

  return result;
}
