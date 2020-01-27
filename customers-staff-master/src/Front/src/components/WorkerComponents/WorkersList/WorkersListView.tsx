import { Loader } from "@skbkontur/react-ui/components";
import Toast from "@skbkontur/react-ui/Toast";
import * as H from "history";
import * as React from "react";
import { RouteProps } from "react-router";
import { ApiSingleton } from "../../../api/new/Api";
import { WorkerDto } from "../../../api/new/dto/WorkerDto";
import { ShopWorkingCalendarDto } from "../../../api/new/dto/WorkingCalendar/WorkersScheduleDtos";
import { AppDataSingleton } from "../../../app/AppData";
import { FeatureAppearance } from "../../../common/FeatureAppearance";
import { Metrics } from "../../../common/MetricsFacade";
import { CancellationToken } from "../../../common/Scheduler/CancellationToken";
import { setLastWorkerListTabTypeToLocalStorage } from "../../../common/Scheduler/LocalStorageHelper";
import { Scheduler } from "../../../common/Scheduler/Scheduler";
import { Caption, CaptionType } from "../../../commonComponents/Caption/Caption";
import { Feedback } from "../../../commonComponents/Feedback/Feedback";
import { FeedbackType } from "../../../commonComponents/Feedback/FeedbackType";
import { OnBoardingComponent } from "../../../commonComponents/OnBoardingComponent/OnBoardingComponent";
import { ListType } from "../../../models/ListType";
import { WorkersListTabType } from "../../../models/WorkersListTabType";
import { getFetchingPeriodInMilliseconds } from "../../../redux/createRootStore";
import { EmptyPage } from "../../EmptyPage/EmptyPage";
import * as styles from "../../Layout/Layout.less";
import { WorkerEditor } from "../WorkerEditor/WorkerEditor";
import { WorkersBoard1, WorkersBoard2, WorkersBoard3 } from "./imgs";
import { WorkersListBody } from "./WorkersListBody";
import { WorkersListHeader } from "./WorkersListHeader";
import * as localStyles from "./WorkersListView.less";

const LoadCalendarTask: string = "LoadCalendarTask";

export interface IWorkersListViewProps extends RouteProps {
  data: WorkerDto[] | null;
  onAddItem: (worker: Partial<WorkerDto>) => Promise<string>;
  history: H.History;
  positionsMap?: string[];
  isChart: boolean;
  hideWorkersOnBoard: boolean | null;
  updateSetting: () => void;
}

interface IWorkersListViewState {
  shopWorkingCalendarDto: ShopWorkingCalendarDto | null;
  month: Date;
  dataVersion: number | null;
  isLoading: boolean;
  isLoadingFailed: boolean;
  showWorkerEditor: boolean;
  isCreatingNewWorker: boolean;
}

export class WorkersListView extends React.Component<IWorkersListViewProps, IWorkersListViewState> {
  private token: CancellationToken | null = null;
  private readonly fetchingPeriod: number | null;
  constructor(props: Readonly<IWorkersListViewProps>) {
    super(props);
    this.state = {
      shopWorkingCalendarDto: null,
      month: new Date(),
      isLoading: true,
      isLoadingFailed: false,
      showWorkerEditor: false,
      isCreatingNewWorker: false,
      dataVersion: null,
    };
    this.fetchingPeriod = getFetchingPeriodInMilliseconds();
  }

  public async componentDidMount(): Promise<void> {
    if (this.props.isChart) {
      await this.loadCalendar(null);
    }
  }

  public async componentWillReceiveProps(nextProps: Readonly<IWorkersListViewProps>): Promise<void> {
    if (!this.props.isChart && nextProps.isChart) {
      const date = this.state.shopWorkingCalendarDto ? this.state.month : null;
      await this.loadCalendar(date);
    }

    if (this.props.isChart && !nextProps.isChart) {
      Scheduler.unregister(LoadCalendarTask);
    }
  }

  public render(): JSX.Element {
    const { isChart, hideWorkersOnBoard } = this.props;
    const feedbackType = isChart ? FeedbackType.WorkersSchedulesFeedback : FeedbackType.WorkersCardsFeedback;

    if (hideWorkersOnBoard === false) {
      return (
        <div className={styles.contentInner} data-tid="WorkerList">
          {this.renderOnBoard()}
        </div>
      );
    }
    const isEmpty = hideWorkersOnBoard === null || this.props.data === null || this.props.data.length === 0;
    return (
      <div>
        {FeatureAppearance.shouldShow(feedbackType) && <Feedback {...FeatureAppearance.getProps(feedbackType)} />}
        <div className={styles.contentInner} style={isEmpty ? {} : { paddingTop: 0 }} data-tid="WorkerList">
          {isEmpty && this.renderEmpty()}
          {!isEmpty && this.renderHeader()}
          {!isEmpty && this.renderBody()}
          {this.state.showWorkerEditor && this.renderWorkerEditor()}
        </div>
      </div>
    );
  }

  private renderWorkerEditor = () => {
    return (
      <WorkerEditor
        heading={"Новый сотрудник"}
        disableActionButtons={this.state.isCreatingNewWorker}
        onClose={this.closeWorkerEditor}
        acceptButtonCaption="Добавить"
        positionsMap={this.props.positionsMap}
        onSave={this.onAddWorker}
      />
    );
  };

  private renderBody = () => {
    return (
      <WorkersListBody
        isChart={this.props.isChart}
        workers={this.props.data}
        calendar={this.state.shopWorkingCalendarDto}
        month={this.state.month}
        reloadCalendar={() => Scheduler.force(LoadCalendarTask)}
        isLoading={this.state.isLoading}
        isLoadingFailed={this.state.isLoadingFailed}
      />
    );
  };

  private renderHeader = () => {
    return (
      <WorkersListHeader
        isChart={this.props.isChart}
        changeTab={this.changeTabType}
        month={this.state.month}
        onChangeMonth={this.changeMonth}
        isLoading={this.state.isLoading}
        isLoadingFailed={this.state.isLoadingFailed}
        addNewWorker={this.openWorkerEditor}
        tryLoadCalendarAgain={() => this.loadCalendar(this.state.month)}
      />
    );
  };

  private renderEmpty = () => (
    <div>
      <Caption type={CaptionType.H1}>Сотрудники</Caption>
      {this.props.hideWorkersOnBoard === null || this.props.data === null ? (
        <Loader
          data-tid="Loader"
          active={true}
          type="big"
          className={localStyles.emptyLoader}
          caption={"Загружаем список сотрудников"}
        />
      ) : (
        <EmptyPage
          data-tid="EmptyPersonList"
          onAdd={this.openWorkerEditor}
          emptyButtonCaption="Новый сотрудник"
          emptyCaption={
            <span>
              Добавьте сотрудников, чтобы работать
              <br /> с календарем, графиком работы и заказами
            </span>
          }
        />
      )}
    </div>
  );

  private renderOnBoard() {
    return (
      <div>
        <Caption type={CaptionType.H1}>Сотрудники</Caption>
        <div style={{ height: 40 }} />
        <OnBoardingComponent
          onFinish={this.props.updateSetting}
          startText="Начать работу со справочником"
          pages={[
            {
              headerTextLines: [
                "Добавьте справочник сотрудников, чтобы работать с календарем,",
                "составлять график работы и собирать статистику продаж",
              ],
              imageUrl: WorkersBoard1,
              footerTextLines: ["График работы на месяц,", "рабочие дни и часы считаются автоматически"],
            },
            {
              headerTextLines: [
                "Добавьте справочник сотрудников, чтобы работать с календарем,",
                "составлять график работы и собирать статистику продаж",
              ],
              imageUrl: WorkersBoard2,
              footerTextLines: [
                "В новом приложении Контур.Касса можно указывать,",
                "какие товары продал сотрудник или какие услуги оказал",
              ],
            },
            {
              headerTextLines: [
                "Добавьте справочник сотрудников, чтобы работать с календарем,",
                "составлять график работы и собирать статистику продаж",
              ],
              imageUrl: WorkersBoard3,
              footerTextLines: [
                "На основе отчета по сотрудникам можно рассчитывать зарплату,",
                "если она зависит от продаж или оказанных услуг",
              ],
            },
          ]}
        />
      </div>
    );
  }

  private showAddToast(guid: string) {
    const to = `${AppDataSingleton.prefix}/${ListType.Workers}/${WorkersListTabType.List}/${guid}`;

    Toast.push("Сотрудник добавлен", {
      label: "Показать",
      handler: () => {
        this.props.history.push(to);
        Toast.close();
      },
    });
  }

  private onAddWorker = async (worker: WorkerDto) => {
    try {
      FeatureAppearance.activate(FeedbackType.WorkersCardsFeedback);
      this.setState({ isCreatingNewWorker: true });
      const id = await this.props.onAddItem(worker);
      this.showAddToast(id);
      this.closeWorkerEditor();
      Metrics.staffCreateSuccess({
        ...Metrics.variablesFromWorker(worker),
        cardid: id,
      });
    } catch (e) {
      Toast.push("Ошибка при сохранении");
    } finally {
      this.setState({ isCreatingNewWorker: false });
    }
  };

  private closeWorkerEditor = () => this.setState({ showWorkerEditor: false });
  private openWorkerEditor = () => {
    Metrics.staffCreateStart();
    FeatureAppearance.activate(FeedbackType.WorkersCardsFeedback);
    this.setState({ showWorkerEditor: true });
  };

  private changeTabType = async (type: WorkersListTabType) => {
    setLastWorkerListTabTypeToLocalStorage(type);
    this.props.history.replace(`${AppDataSingleton.prefix}/${ListType.Workers}/${type}`);
  };

  private changeMonth = async (offset: 1 | -1) => {
    const m = this.state.month;
    const month = new Date(m.getFullYear(), m.getMonth() + offset, 1);
    this.setState({
      month,
      shopWorkingCalendarDto: null,
      dataVersion: null,
    });
    await this.loadCalendar(month);
  };

  private loadCalendarInternal = async (
    date: Date | null,
    version: number | null,
    token: CancellationToken | null
  ): Promise<Date | null> => {
    const currentVersion = version || this.state.dataVersion;
    const result = await ApiSingleton.WorkingCalendarApi.getForMonth(date, currentVersion);
    const newDate = new Date(result.month);
    if (token !== null && token.isCanceled) {
      return null;
    } else {
      if (currentVersion !== result.version) {
        this.setState({ shopWorkingCalendarDto: result, month: newDate, dataVersion: result.version });
      }
    }
    return newDate;
  };

  private loadCalendar = async (date: Date | null) => {
    this.refreshToken();
    this.setState({ isLoadingFailed: false, isLoading: true });
    try {
      Scheduler.unregister(LoadCalendarTask);
      date = await this.loadCalendarInternal(date, null, this.token);
      if (date === null) {
        return;
      }
      Scheduler.registerWithCancel(
        LoadCalendarTask,
        token => this.loadCalendarInternal(date, null, token),
        this.fetchingPeriod || 60000
      );
    } catch (e) {
      console.error(e);
      this.setState({ isLoadingFailed: true });
    } finally {
      this.setState({ isLoading: false });
    }
  };

  private refreshToken() {
    if (this.token) {
      this.token.isCanceled = true;
    }
    this.token = new CancellationToken();
  }
}
