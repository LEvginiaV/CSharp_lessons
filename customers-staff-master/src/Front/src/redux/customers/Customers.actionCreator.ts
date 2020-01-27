import { Action } from "redux";
import { ThunkAction } from "redux-thunk";
import { ApiSingleton } from "../../api/new/Api";
import { CustomerDto } from "../../api/new/dto/CustomerDto";
import { CustomerInfoDto } from "../../api/new/dto/CustomerInfoDto";
import { Guid } from "../../api/new/dto/Guid";
import { Scheduler } from "../../common/Scheduler/Scheduler";
import { PersonsHelper } from "../../helpers/PersonsHelper";
import { RootState } from "../rootReducer";
import { CustomersActions, CustomersActionTypes } from "./Customers.actions";

const RefreshCustomerList = "RefreshCustomerList";

class CustomersActionCreatorClass {
  public schedulePeriodicListRefresh: (period: number) => ThunkAction<void, RootState, {}, Action> = (
    period: number
  ) => {
    return async dispatch => {
      Scheduler.register(RefreshCustomerList, () => dispatch(this.refreshList), period);
    };
  };

  public create: (customerInfo: CustomerInfoDto) => Promise<string> = async (customerInfo: CustomerInfoDto) => {
    const customer = await ApiSingleton.CustomerApi.create(customerInfo);
    await Scheduler.force(RefreshCustomerList);
    return customer.id;
  };

  public update: (customerId: Guid, customerInfo: CustomerInfoDto) => Promise<void> = async (
    customerId: Guid,
    customerInfo: CustomerInfoDto
  ) => {
    await ApiSingleton.CustomerApi.update(customerId, customerInfo);
    await Scheduler.force(RefreshCustomerList);
  };

  public updateComment: (customerId: Guid, comment: string) => Promise<void> = async (
    customerId: Guid,
    comment: string
  ) => {
    await ApiSingleton.CustomerApi.updateComment(customerId, comment);
    await Scheduler.force(RefreshCustomerList);
  };

  public delete: (customerId: Guid) => Promise<void> = async (customerId: Guid) => {
    await ApiSingleton.CustomerApi.delete(customerId);
    await Scheduler.force(RefreshCustomerList);
  };

  public setCustomers(version: number, customers: CustomerDto[]): CustomersActions {
    return { type: CustomersActionTypes.SetCustomers, version, customers };
  }

  private refreshList: (dispatch: any, getState: () => RootState) => Promise<void> = async (
    dispatch: any,
    getState: () => RootState
  ) => {
    const result = await ApiSingleton.CustomerApi.readAll(getState().customers.version);
    if (!!result.customers) {
      const sortedCustomers = PersonsHelper.sortCustomers(result.customers);
      dispatch(this.setCustomers(result.version, sortedCustomers));
    }
  };
}

export const CustomersActionCreator = new CustomersActionCreatorClass();
