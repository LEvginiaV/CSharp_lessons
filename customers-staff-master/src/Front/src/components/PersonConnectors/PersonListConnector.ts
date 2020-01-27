import { connect } from "react-redux";
import { ListType } from "../../models/ListType";
import { WorkersListTabType } from "../../models/WorkersListTabType";
import { CustomersActionCreator } from "../../redux/customers";
import { RootState, unboxThunk } from "../../redux/rootReducer";
import { UserSettingsActionCreator } from "../../redux/userSettings";
import { WorkersActionCreator } from "../../redux/workers";
import { CustomersListView } from "../CustomerComponents/CustomersList/CustomersListView";
import { IWorkersListViewProps, WorkersListView } from "../WorkerComponents/WorkersList/WorkersListView";

const workersConnector = connect(
  (state: RootState, ownProps: IWorkersListViewProps) => ({
    data: state.workers.workers,
    positionsMap: state.workers.positionsMap,
    isChart:
      ownProps.location &&
      ownProps.location.pathname &&
      ownProps.location.pathname.endsWith(`/${WorkersListTabType.Chart}`),
    onAddItem: WorkersActionCreator.create,
    hideWorkersOnBoard:
      state.userSettings.settings === null ? null : state.userSettings.settings.hideWorkersOnBoard === "true",
  }),
  {
    updateSetting: () => unboxThunk(UserSettingsActionCreator.updateSetting)("hideWorkersOnBoard", "true"),
  }
);

const customersConnector = connect(
  (state: RootState) => ({
    data: state.customers.customers,
    itemPathPrefix: `${ListType.Customers}`,
    onAddItem: CustomersActionCreator.create,
    hideCustomersOnBoard: state.userSettings.settings && state.userSettings.settings.hideCustomersOnBoard === "true",
  }),
  {
    updateSetting: () => unboxThunk(UserSettingsActionCreator.updateSetting)("hideCustomersOnBoard", "true"),
  }
);

export const WorkersList = workersConnector(WorkersListView);

export const CustomersList = customersConnector(CustomersListView);
