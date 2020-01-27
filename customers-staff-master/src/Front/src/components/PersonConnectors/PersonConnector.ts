import * as find from "lodash/find";
import { connect } from "react-redux";
import { ListType } from "../../models/ListType";
import { CustomersActionCreator } from "../../redux/customers";
import { RootState } from "../../redux/rootReducer";
import { WorkersActionCreator } from "../../redux/workers";
import { CustomerView, ICustomerProps } from "../CustomerComponents/Customer/CustomerView";
import { IWorkerProps, WorkerView } from "../WorkerComponents/Worker/WorkerView";

const mapStateToProps = (listType: ListType) => (
  state: RootState,
  ownProps: IWorkerProps | ICustomerProps
): Partial<IWorkerProps | ICustomerProps> => {
  let data;
  let positionsMap = null;
  let version = 0;
  if (listType === ListType.Customers) {
    data = state.customers.customers;
    version = state.customers.version;
  } else {
    data = state.workers.workers;
    version = state.workers.version;
    positionsMap = state.workers.positionsMap;
  }

  const guid = ownProps.match && ownProps.match.params && ownProps.match.params.id;
  const currentItem = data && guid && find(data, (w: any) => w && w.id === guid);
  const onBack = () => ownProps.history.goBack();

  return {
    onBack,
    positionsMap,
    version,
    ...currentItem,
    ...getActions(listType),
  };
};

const getActions = (listType: ListType) => {
  if (listType === ListType.Workers) {
    return {
      onSave: WorkersActionCreator.update,
      onSaveComment: WorkersActionCreator.updateComment,
      onRemove: WorkersActionCreator.delete,
    };
  }
  if (listType === ListType.Customers) {
    return {
      onSave: CustomersActionCreator.update,
      onSaveComment: CustomersActionCreator.updateComment,
      onRemove: CustomersActionCreator.delete,
    };
  }
  return {};
};

export const Worker = connect(
  () => mapStateToProps(ListType.Workers),
  {}
)(WorkerView);

export const Customer = connect(
  () => mapStateToProps(ListType.Customers),
  {}
)(CustomerView);
