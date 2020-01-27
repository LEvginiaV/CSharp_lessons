import { Add, ArrowChevronLeft, ArrowChevronRight, Refresh } from "@skbkontur/react-icons";
import { Button, Link, Spinner, Sticky, Tabs } from "@skbkontur/react-ui/components";
import Tab from "@skbkontur/react-ui/components/Tabs/Tab";
import * as cn from "classnames";
import * as React from "react";
import { datesEquals, getDayInfosForMonth, getMonthName } from "../../../common/DateHelper";
import { Caption, CaptionType } from "../../../commonComponents/Caption/Caption";
import { HelpLink } from "../../../commonComponents/HelpLink/HelpLink";
import { WorkersListTabType } from "../../../models/WorkersListTabType";
import * as styles from "./WorkersListHeader.less";

const helpLink = "https://egais.userecho.com/knowledge-bases/2/articles/5208-kak-vesti-bazu-sotrudnikov";
const helpLinkChart =
  "https://egais.userecho.com/knowledge-bases/2/articles/5210-kak-zapolnyat-grafik-rabotyi-sotrudnikov";

export interface IWorkersListHeaderProps {
  isChart: boolean;
  changeTab: (tabType: WorkersListTabType | string) => void;
  month: Date;
  onChangeMonth: (shift: 1 | -1) => void;
  isLoading: boolean;
  isLoadingFailed: boolean;
  addNewWorker: () => void;
  tryLoadCalendarAgain: () => void;
}

interface IWorkersListHeaderState {
  ok: string;
}

export class WorkersListHeader extends React.Component<IWorkersListHeaderProps, IWorkersListHeaderState> {
  public render(): JSX.Element {
    const { isChart } = this.props;
    return (
      <Sticky side={"top"}>
        {isFixed => (
          <div className={cn(styles.headerContainer, isFixed && styles.headerContainerShadow)}>
            <div className={cn(styles.headerLine1, !isChart && styles.headerLine1Small)}>
              <Caption type={CaptionType.H1} className={styles.titleText}>
                Сотрудники
              </Caption>
              <Button size="medium" data-tid="AddButton" use="primary" onClick={this.props.addNewWorker}>
                <Add /> Новый сотрудник
              </Button>
              {isChart && <div className={styles.flexStub} />}
              {isChart && this.renderMonthSwitcher()}
              <div className={styles.helpLinkDefault}>
                <HelpLink
                  caption="Как пользоваться"
                  hintText={isChart ? "Подробнее про график работы" : "Подробнее про заполнение базы сотрудников"}
                  onClick={() => window.open(isChart ? helpLinkChart : helpLink)}
                />
              </div>
            </div>
            <div className={styles.headerLine2}>{isChart && this.renderLoader()}</div>
            <div className={styles.headerLine3}>
              {this.renderTabs()}
              {isChart && this.renderDatesHeader()}
            </div>
            <div className={isFixed ? styles.grayLineFixed : styles.grayLine} />
          </div>
        )}
      </Sticky>
    );
  }

  private renderLoader(): JSX.Element {
    if (this.props.isLoading) {
      return <Spinner type={"mini"} caption={""} />;
    }
    if (this.props.isLoadingFailed) {
      return (
        <span>
          <span className={styles.loadingFailedMessage}>Не удалось загрузить.</span>
          <Link icon={<Refresh />} onClick={this.props.tryLoadCalendarAgain}>
            Попробовать еще раз
          </Link>
        </span>
      );
    }
    return <span />;
  }

  private renderMonthSwitcher(): JSX.Element {
    const rightArrowDisabled = this.isLastAvailableMonth();
    const sameYear = this.props.month.getFullYear() === new Date().getFullYear();
    return (
      <div className={styles.monthSwitcher}>
        <span className={styles.arrowButton} onClick={() => this.props.onChangeMonth(-1)} data-tid="PrevMonth">
          <ArrowChevronLeft />
        </span>
        <span className={sameYear ? styles.monthName : styles.wideMonthName} data-tid="MonthName">
          {this.getMonthNameAndYear()}
        </span>
        <span
          className={rightArrowDisabled ? styles.arrowButtonDisabled : styles.arrowButton}
          onClick={() => this.goToNextMonth(rightArrowDisabled)}
          data-tid="NextMonth"
          data-prop-disabled={rightArrowDisabled}
        >
          <ArrowChevronRight />
        </span>
      </div>
    );
  }

  private goToNextMonth = (stop: boolean) => {
    if (stop) {
      return;
    }
    this.props.onChangeMonth(1);
  };

  private isLastAvailableMonth(): boolean {
    const today = new Date();
    return this.props.month.getMonth() === today.getMonth() && this.props.month.getFullYear() > today.getFullYear();
  }

  private getMonthNameAndYear(): string {
    const year = this.props.month.getFullYear();
    const yearStr = year !== new Date().getFullYear() ? ` ${year}` : "";
    return `${getMonthName(this.props.month)}${yearStr}`;
  }

  private renderTabs(): JSX.Element {
    const { isChart, changeTab } = this.props;
    const tabStyle = { fontSize: 16, height: 41 };
    return (
      <div className={styles.tabsBorder}>
        <div className={isChart ? styles.tableTabs : styles.tabs}>
          <Tabs value={isChart ? WorkersListTabType.Chart : WorkersListTabType.List} onChange={(_, v) => changeTab(v)}>
            <Tab id={WorkersListTabType.List} style={tabStyle}>
              <span data-tid="ListTab">Список</span>
            </Tab>
            <Tab id={WorkersListTabType.Chart} style={tabStyle}>
              <span data-tid="ChartTab">График работы</span>
            </Tab>
          </Tabs>
        </div>
      </div>
    );
  }

  private renderDatesHeader(): JSX.Element {
    const dayInfos = getDayInfosForMonth(this.props.month);
    const today = new Date();
    return (
      <div className={styles.dates}>
        {dayInfos.map((x, idx) => (
          <div key={idx} className={cn(styles.tableCell, x.isWeekend && styles.weekendColors)}>
            {x.date.getDate()}
            <div className={datesEquals(today, x.date) ? styles.todayBorder : undefined} />
          </div>
        ))}
      </div>
    );
  }
}
