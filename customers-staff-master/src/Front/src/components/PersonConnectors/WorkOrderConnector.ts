import { connect } from "react-redux";
import { RootState, unboxThunk } from "../../redux/rootReducer";
import { UserSettingsActionCreator } from "../../redux/userSettings";
import { WorkOrderListView } from "../WorkOrderComponents/WorkOrderListView/WorkOrderListView";
import { WorkOrderView } from "../WorkOrderComponents/WorkOrderView/WorkOrderView";

const workOrderConnector = connect((state: RootState) => ({
  customers: state.customers.customers || [],
  workers: state.workers.workers || [],
  cards: state.nomenclature.cards,
}));

const workOrderListConnector = connect(
  (state: RootState) => ({
    customers: state.customers.customers || [],
    cards: state.nomenclature.cards,
    hideWorkOrderOnBoard: state.userSettings.settings && state.userSettings.settings.hideWorkOrderOnBoard === "true",
  }),
  {
    updateSetting: () => unboxThunk(UserSettingsActionCreator.updateSetting)("hideWorkOrderOnBoard", "true"),
  }
);

export const WorkOrder = workOrderConnector(WorkOrderView);
export const WorkOrderList = workOrderListConnector(WorkOrderListView);
