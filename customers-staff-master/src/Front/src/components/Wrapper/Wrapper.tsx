import * as React from "react";
import { connect } from "react-redux";
import { Scheduler } from "../../common/Scheduler/Scheduler";
import { getFetchingPeriodInMilliseconds } from "../../redux/createRootStore";
import { CustomersActionCreator } from "../../redux/customers";
import { unboxThunk } from "../../redux/rootReducer";
import { UserSettingsActionCreator } from "../../redux/userSettings";
import { WorkersActionCreator } from "../../redux/workers";
import * as styles from "./Wrapper.less";

interface IWrapperProps {
  scheduleRefreshWorkersData?: (period: number) => void;
  scheduleRefreshCustomersData?: (period: number) => void;
  loadUserSettings?: () => void;
}

class WrapperView extends React.Component<IWrapperProps, {}> {
  constructor(props: IWrapperProps) {
    super(props);
  }

  public componentDidMount(): void {
    const fetchingPeriod = getFetchingPeriodInMilliseconds();
    this.props.scheduleRefreshWorkersData && this.props.scheduleRefreshWorkersData(fetchingPeriod);
    this.props.scheduleRefreshCustomersData && this.props.scheduleRefreshCustomersData(fetchingPeriod);
    this.props.loadUserSettings && this.props.loadUserSettings();
  }

  public componentWillUnmount(): void {
    Scheduler.clear();
  }

  public render(): JSX.Element {
    return <div className={styles.wrapper}>{this.props.children}</div>;
  }
}

const mapDispatchToProps = {
  scheduleRefreshWorkersData: unboxThunk(WorkersActionCreator.schedulePeriodicListRefresh),
  scheduleRefreshCustomersData: unboxThunk(CustomersActionCreator.schedulePeriodicListRefresh),
  loadUserSettings: unboxThunk(UserSettingsActionCreator.loadSettings),
};

export const Wrapper = connect(
  null,
  mapDispatchToProps
)(WrapperView);
