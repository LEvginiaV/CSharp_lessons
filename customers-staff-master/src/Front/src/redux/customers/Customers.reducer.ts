import { CustomersActions, CustomersActionTypes } from "./Customers.actions";
import { CustomersState } from "./Customers.state";

export const CUSTOMERS_INITIAL_STATE: CustomersState = {
  version: 0,
  customers: null,
};

export function customersReducer(state: CustomersState, action: CustomersActions): CustomersState {
  switch (action.type) {
    case CustomersActionTypes.SetCustomers:
      if (state.version === action.version) {
        return state;
      }

      return {
        ...state,
        version: action.version,
        customers: [...action.customers],
      };
    default:
      return state;
  }
}
