import { ArrowChevronLeft, ArrowChevronRight, Undo } from "@skbkontur/react-icons";
import Link from "@skbkontur/react-ui/components/Link/Link";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import Switcher from "@skbkontur/react-ui/components/Switcher/Switcher";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";

import Center from "@skbkontur/react-ui/Center";
import * as cn from "classnames";
import * as React from "react";
import { connect } from "react-redux";
import { RouteChildrenProps } from "react-router";
import { ApiSingleton } from "../../api/new/Api";
import { Guid } from "../../api/new/dto/Guid";
import { ServiceCalendarRecordDto } from "../../api/new/dto/ServiceCalendar/ServiceCalendarRecordDto";
import { CustomerStatusDto, RecordStatusDto } from "../../api/new/dto/ServiceCalendar/ServiceCalendarRecordInfoDto";
import { ShopServiceCalendarDayDto } from "../../api/new/dto/ServiceCalendar/ShopServiceCalendarDayDto";
import { TimeZoneDto } from "../../api/new/dto/TimeZoneDto";
import { UserSettings } from "../../api/new/dto/UserSettings/UserSettings";
import {
  ShopWorkingDayDto,
  TimePeriodDto,
  WorkingCalendarRecordDto,
} from "../../api/new/dto/WorkingCalendar/WorkersScheduleDtos";
import { AppDataSingleton } from "../../app/AppData";
import { fromDateOnlyISOString, toDateMonthNameString, toDateOnlyISOString } from "../../common/DateHelper";
import { FeatureAppearance } from "../../common/FeatureAppearance";
import { Metrics } from "../../common/MetricsFacade";
import { TimeSpanHelper } from "../../common/TimeSpanHelper";
import { Feedback } from "../../commonComponents/Feedback/Feedback";
import { FeedbackType } from "../../commonComponents/Feedback/FeedbackType";
import { HelpLink } from "../../commonComponents/HelpLink/HelpLink";
import { OnBoardingComponent } from "../../commonComponents/OnBoardingComponent/OnBoardingComponent";
import { DateHelper } from "../../helpers/DateHelper";
import { ListType } from "../../models/ListType";
import { WorkersListTabType } from "../../models/WorkersListTabType";
import { RootState, TypeOfConnect, unboxThunk } from "../../redux/rootReducer";
import { UserSettingsActionCreator } from "../../redux/userSettings";
import * as styles from "./Calendar.less";
import { CalendarHelper } from "./CalendarHelper";
import { CalendarModal, CalendarModalInfo } from "./CalendarModal";
import { CalendarBoard1, CalendarBoard2, CalendarBoard3 } from "./imgs";
import { TimezoneModal } from "./TimezoneModal";
import { WorkerColumn } from "./WorkerColumn";

const weekDays = ["воскресение", "понедельник", "вторник", "среда", "четверг", "пятница", "суббота"];
const helpLink =
  "https://egais.userecho.com/knowledge-bases/2/articles/5211-rabota-s-kalendarem-zapis-priema-klientov-uslugi-sotrudniki";

export const MIN_RECORD_LENGTH = 15;

type CalendarFilter = "all" | "working";

type ICalendarProps = {
  hideWorkOrderOnBoard: boolean | null;
  updateSetting: (key: keyof UserSettings, value: string) => void;
} & TypeOfConnect<typeof reduxConnector> &
  RouteChildrenProps<{ date: string }>;

interface ICalendarState {
  loading: boolean;
  loaded: boolean;
  nowMinutes: Nullable<number>;
  filter: CalendarFilter;
  scrollTop: number;
  scrollLeft: number;
  day: Nullable<ShopServiceCalendarDayDto>;
  workingTimes: Nullable<ShopWorkingDayDto>;

  modal: boolean;
  modalWorkerId: Nullable<Guid>;
  modalRecordId: Nullable<Guid>;
  modalInfo: Nullable<CalendarModalInfo>;

  timezone: Nullable<TimeZoneDto>;
  timezoneModal: boolean;
}

export class CalendarView extends React.Component<ICalendarProps, ICalendarState> {
  public state: Readonly<ICalendarState> = {
    loading: false,
    loaded: false,
    nowMinutes: null,
    filter: "all",
    scrollTop: 320,
    scrollLeft: 0,
    day: null,
    workingTimes: null,

    modal: false,
    modalWorkerId: null,
    modalRecordId: null,
    modalInfo: null,

    timezone: null,
    timezoneModal: false,
  };

  private positionTimeout: Nullable<TimeoutID> = null;
  private body: Nullable<HTMLDivElement> = null;

  public componentDidMount() {
    const { match } = this.props;
    match && match.params.date;
    this.updateNowMinutes();
    this.loadDay(true);
    this.updateTimezone();
  }

  public componentDidUpdate(prevProps: Readonly<ICalendarProps>): void {
    const prevDate = prevProps.match ? prevProps.match.params.date : "";
    const curDate = this.props.match ? this.props.match.params.date : "";
    if (prevDate !== curDate) {
      this.loadDay(true);
    }
  }

  public componentWillUnmount() {
    this.positionTimeout && clearTimeout(this.positionTimeout);
  }

  public render(): JSX.Element | null {
    if (!this.props.hideWorkOrderOnBoard) {
      return this.renderOnBoard();
    }

    return (
      <div className={styles.calendar} data-tid="Calendar">
        {FeatureAppearance.shouldShow(FeedbackType.CalendarsFeedback) && (
          <Feedback {...FeatureAppearance.getProps(FeedbackType.CalendarsFeedback)} />
        )}
        {this.renderTop()}
        {this.renderBody()}
        {this.renderModalInfo()}
        {this.state.timezoneModal && this.renderTimezoneModal()}
      </div>
    );
  }

  public renderTop(renderSwitcher: boolean = true) {
    const dateToShow = this.getDateFromUrl();
    const isToday = this.isToday(dateToShow);
    const secondLine: string[] = dateToShow ? [weekDays[dateToShow.getDay()]] : [];
    if (isToday) {
      secondLine.push("сегодня");
    }
    const switcherItems = [
      {
        label: "Все сотрудники",
        value: "all",
      },
      {
        label: "Работающие",
        value: "working",
      },
    ];

    return (
      <div className={styles.top}>
        <div className={styles.dateWrap}>
          <Link icon={<ArrowChevronLeft />} onClick={() => this.handleChangeDate(-1)} data-tid={"PrevDay"} />
          <div className={styles.date}>
            {toDateMonthNameString(dateToShow)}
            <div className={styles.note}>{secondLine.join(", ")}</div>
          </div>
          <Link icon={<ArrowChevronRight />} onClick={() => this.handleChangeDate(1)} data-tid={"NextDay"} />
          {isToday || (
            <div className={styles.toToday}>
              <Link icon={<Undo />} onClick={() => this.handleChangeDate(this.getToday())}>
                На сегодня
              </Link>
            </div>
          )}
        </div>

        {renderSwitcher && (
          <div className={styles.helpLink}>
            <HelpLink
              caption="Как пользоваться"
              onClick={() => window.open(helpLink)}
              hintText="Подробнее про работу с календарем"
            />
          </div>
        )}
        {renderSwitcher && (
          <Switcher
            data-tid="WorkingFilter"
            label=""
            items={switcherItems}
            value={this.state.filter}
            onChange={(_, value) => this.setState({ filter: value as CalendarFilter })}
          />
        )}
      </div>
    );
  }

  private getToday(): Date {
    const offset = this.state.timezone ? TimeSpanHelper.toMinutes(this.state.timezone.offset) : null;
    return DateHelper.getTodayDateWithTimeZone(offset);
  }

  private isToday(date: Date): boolean {
    return this.state.timezone
      ? DateHelper.isTodayWithTimeZone(date, TimeSpanHelper.toMinutes(this.state.timezone.offset))
      : DateHelper.isToday(date);
  }

  private renderOnBoard() {
    const { hideWorkOrderOnBoard } = this.props;

    return (
      <div className={styles.calendar} data-tid="Calendar">
        {this.renderTop(false)}
        {hideWorkOrderOnBoard !== null && (
          <OnBoardingComponent
            pages={[
              {
                headerTextLines: ["Если к вам приходят клиенты по записи,", "попробуйте вести календарь в Маркете"],
                imageUrl: CalendarBoard1,
                footerTextLines: [
                  "Наглядный вид записи клиентов к мастерам,",
                  "видно загруженность сотрудников на день",
                ],
              },
              {
                headerTextLines: ["Если к вам приходят клиенты по записи,", "попробуйте вести календарь в Маркете"],
                imageUrl: CalendarBoard2,
                footerTextLines: ["Автоматическое пополнение базы контактов", "при записи новых клиентов"],
              },
              {
                headerTextLines: ["Если к вам приходят клиенты по записи,", "попробуйте вести календарь в Маркете"],
                imageUrl: CalendarBoard3,
                footerTextLines: [
                  "Cтатусы помогут отмечать состояния. Видно, по каким записям",
                  "нужно позвонить клиентам и напомнить о визите",
                ],
              },
            ]}
            startText="Начать работу с календарем"
            onFinish={this.handleStartWorking}
          />
        )}
        {hideWorkOrderOnBoard === null && (
          <Center>
            <Loader active={true} type="big" />
          </Center>
        )}
      </div>
    );
  }

  private renderBody() {
    const a = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24];
    const { workers } = this.props;
    const { timezone } = this.state;
    const list = this.state.filter === "working" ? this.props.workers.filter(x => this.isWorkerWorking(x.id)) : workers;

    if (list.length === 0) {
      return this.renderEmptyList();
    }
    const { scrollTop, scrollLeft } = this.state;
    return (
      <Loader active={this.state.loading} type="big">
        {this.state.loaded && <span data-tid="DataLoaded" />}
        <div className={styles.body} ref={this.bodyRef} onScroll={this.handleScroll}>
          <div className={styles.corner} style={{ transform: `translate(${scrollLeft}px, ${scrollTop}px)` }}>
            <Link onClick={() => this.setState({ timezoneModal: true })} data-tid="TimeZoneLink">
              {TimeSpanHelper.toTimezoneText(timezone && timezone.offset)}
            </Link>
          </div>
          <div className={styles.firstCol} style={{ transform: `translate(${scrollLeft}px, 0)` }}>
            {a.map((hour: number) => (
              <div key={hour} className={styles.cell}>
                {hour === 24 || <div className={styles.value}>{hour}</div>}
              </div>
            ))}
            {this.state.nowMinutes != null && this.isToday(this.getDateFromUrl()) && (
              <div
                key="nowCircle"
                className={styles.nowCircle}
                style={{ top: CalendarHelper.timeToPx(this.state.nowMinutes) }}
              />
            )}
          </div>

          <div className={cn(styles.row, styles.firstRow)} style={{ transform: `translate(0,${scrollTop}px)` }}>
            {list.map(worker => (
              <div key={worker.id} className={styles.cell}>
                {worker.fullName}
                <div className={styles.desc}>{worker.position}</div>
              </div>
            ))}
          </div>
          <div className={styles.row}>{list.map(w => this.renderDay(w.id))}</div>
        </div>
      </Loader>
    );
  }

  private renderEmptyList() {
    return (
      <div className={styles.emptyList}>
        {this.state.filter === "working" ? (
          <div>
            Нет работающих сотрудников
            <br />
            <Link onClick={this.handleGoWorkers}>Перейти к графику работы</Link>
          </div>
        ) : (
          <div data-tid="EmptyCalendarMessage">
            Добавьте сотрудников,
            <br />
            чтобы работать с календарем
          </div>
        )}
      </div>
    );
  }

  private renderDay(workerId: Guid) {
    const { day } = this.state;
    if (!day) {
      return;
    }

    const wday = day.workerCalendarDays.find(x => x.workerId === workerId);
    const nowMinutes = this.isToday(this.getDateFromUrl()) ? this.state.nowMinutes : null;
    return (
      <div key={workerId} className={styles.cell} data-tid="WorkerColumn">
        <WorkerColumn
          info={wday}
          workingTime={this.getWorkingTime(workerId)}
          nowMinutes={nowMinutes}
          onStartAdd={(period: TimePeriodDto) => {
            Metrics.calendarCreateStart();
            this.setState({
              modal: true,
              modalWorkerId: workerId,
              modalRecordId: null,
              modalInfo: {
                recordStatus: RecordStatusDto.Active,
                customerStatus: CustomerStatusDto.Active,
                customerId: undefined,
                productIds: [],
                period,
                comment: "",
              },
            });
          }}
          onEdit={(rec: ServiceCalendarRecordDto) => {
            Metrics.calendarEditStart({ bookingid: rec.id });
            this.setState({
              modal: true,
              modalWorkerId: workerId,
              modalRecordId: rec.id,
              modalInfo: {
                ...rec,
              },
            });
          }}
          onChangeStatus={(recordId, status) => this.handleChangeStatus(workerId, recordId, status)}
          onCancelRecord={recordId =>
            this.handleChangeStatus(workerId, recordId, CustomerStatusDto.CanceledBeforeEvent)
          }
        />
      </div>
    );
  }

  private renderModalInfo() {
    return (
      this.state.modal &&
      this.state.modalWorkerId &&
      this.state.modalInfo &&
      this.state.day &&
      this.state.workingTimes && (
        <CalendarModal
          date={this.getDateFromUrl()}
          recordId={this.state.modalRecordId}
          workerId={this.state.modalWorkerId}
          info={this.state.modalInfo}
          currentWorkerCalendarDays={this.state.day.workerCalendarDays}
          currentWorkingDayMap={this.state.workingTimes.workingDayMap}
          onClose={() => this.setState({ modal: false })}
          onSuccess={() => {
            this.setState({ modal: false });
            this.loadDay();
          }}
        />
      )
    );
  }

  private renderTimezoneModal() {
    return (
      <TimezoneModal
        value={this.state.timezone}
        onChange={timezone => {
          this.setState({ timezone, timezoneModal: false });
          this.updateNowMinutes(timezone);
        }}
        onClose={() => this.setState({ timezoneModal: false })}
      />
    );
  }

  private getWorkingTime = (workerId: Guid): WorkingCalendarRecordDto[] => {
    if (this.state.workingTimes) {
      return this.state.workingTimes.workingDayMap[workerId] || [];
    }
    return [];
  };

  private isWorkerWorking = (workerId: Guid) => {
    return this.getWorkingTime(workerId).length > 0;
  };

  private handleChangeDate = (daysOrDate: number | Date) => {
    let newDate;
    if (typeof daysOrDate === "number") {
      newDate = this.getDateFromUrl();
      newDate.setDate(newDate.getDate() + daysOrDate);
    } else {
      newDate = new Date(daysOrDate);
    }

    const url = `${AppDataSingleton.prefix}/${ListType.Calendar}/${toDateOnlyISOString(newDate)}`;
    this.props.history.push(url);
  };

  private handleGoWorkers = () => {
    const url = `${AppDataSingleton.prefix}/${ListType.Workers}/${WorkersListTabType.Chart}`;
    this.props.history.push(url);
  };

  private handleScroll = () => {
    if (!this.body) {
      return;
    }

    this.setState({
      scrollTop: this.body.scrollTop,
      scrollLeft: this.body.scrollLeft,
    });
  };

  private scrollTo = (div: HTMLDivElement) => {
    const { scrollTop, scrollLeft } = this.state;
    if (HTMLElement.prototype.scrollTo) {
      div.scrollTo({ top: scrollTop, left: scrollLeft });
    } else {
      div.scrollTop = scrollTop;
      div.scrollLeft = scrollLeft;
    }
  };

  private loadDay = async (showLoading = false) => {
    const dateObject = this.getDateFromUrl();
    const date = toDateOnlyISOString(dateObject);
    this.setState({
      loading: showLoading,
      loaded: false,
    });

    try {
      const [data, wtime] = await Promise.all([
        ApiSingleton.ServiceCalendarApi.getForDay(date, RecordStatusDto.Active),
        ApiSingleton.WorkingCalendarApi.getForDay(dateObject, null),
      ]);

      if (toDateOnlyISOString(this.getDateFromUrl()) === date) {
        this.setState({ day: data, workingTimes: wtime, loading: false, loaded: true });
      }
    } catch (e) {
      console.error("Error while loading ServiceCalendarDay", e);
      Toast.push("Не удалось загрузить данные");
      this.setState({ loading: false });
    }
  };

  private updateNowMinutes = (timezone?: Nullable<TimeZoneDto>) => {
    const zone = timezone || this.state.timezone;
    this.positionTimeout && clearTimeout(this.positionTimeout);
    const offset = zone ? TimeSpanHelper.toMinutes(zone.offset) || 0 : 0;
    const now = new Date();
    const minutes = now.getUTCHours() * 60 + now.getUTCMinutes() + offset;
    const nowMinutes = minutes % (24 * 60);
    if (this.state.nowMinutes !== nowMinutes) {
      this.setState({ nowMinutes });
    }
    this.positionTimeout = window.setTimeout(this.updateNowMinutes, 60 * 1000);
  };

  private handleChangeStatus = async (workerId: Guid, recordId: Guid, status: CustomerStatusDto) => {
    if (!this.state.day) {
      return;
    }
    try {
      await ApiSingleton.ServiceCalendarApi.updateRecordStatus(this.state.day.date, workerId, recordId, status);
      await this.loadDay();
    } catch (e) {
      console.error("Error updateing record status", e);
      Toast.push("При обновлении данных произошла ошибка");
      return;
    }
  };

  private async updateTimezone() {
    try {
      const timezone = await ApiSingleton.TimeZoneApi.get();
      this.setState({ timezone });
      this.updateNowMinutes(timezone);
    } catch (e) {
      console.error(e);
    }
  }

  private getDateFromUrl(): Date {
    const dateFromUrl = fromDateOnlyISOString(this.props.match && this.props.match.params.date);
    return dateFromUrl || new Date();
  }

  private bodyRef = (ref: HTMLDivElement | null) => {
    this.body = ref;
    this.body && this.scrollTo(this.body);
  };

  private handleStartWorking = () => {
    this.props.updateSetting();
    Metrics.calendarStartWorking();
  };
}

const reduxConnector = connect(
  (state: RootState) => ({
    workers: state.workers.workers || [],
    hideWorkOrderOnBoard:
      state.userSettings.settings === null ? null : state.userSettings.settings.hideCalendarOnBoard === "true",
  }),
  {
    updateSetting: () => unboxThunk(UserSettingsActionCreator.updateSetting)("hideCalendarOnBoard", "true"),
  }
);

export const Calendar = reduxConnector(CalendarView);
