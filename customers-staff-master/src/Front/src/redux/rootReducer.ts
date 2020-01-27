import { InferableComponentEnhancerWithProps } from "react-redux";
import { Action, Reducer } from "redux";
import { ThunkAction } from "redux-thunk";
import { CUSTOMERS_INITIAL_STATE, CustomersActions, customersReducer, CustomersState } from "./customers";
import {
  NOMENCLATURE_INITIAL_STATE,
  NomenclatureActions,
  nomenclatureReducer,
  NomenclatureState,
} from "./nomenclature";
import {
  USER_SETTINGS_INITIAL_STATE,
  UserSettingsActions,
  userSettingsReducer,
  UserSettingsState,
} from "./userSettings";
import { WORKERS_INITIAL_STATE, WorkersActions, workersReducer, WorkersState } from "./workers";

// Решение взято отсюда https://habr.com/post/431452/
type CutMiddleFunction<T> = T extends (...arg: infer Args) => (...args: any[]) => infer R ? (...arg: Args) => R : never;
export const unboxThunk = <Args extends any[], R, S, E, A extends Action>(
  thunkFn: (...args: Args) => ThunkAction<R, S, E, A>
) => (thunkFn as any) as CutMiddleFunction<typeof thunkFn>; /**/
export type TypeOfConnect<T> = T extends InferableComponentEnhancerWithProps<infer Props, infer _> ? Props : never;

export interface RootState {
  workers: WorkersState;
  customers: CustomersState;
  nomenclature: NomenclatureState;
  userSettings: UserSettingsState;
}

const INITIAL_STATE: RootState = {
  workers: WORKERS_INITIAL_STATE,
  customers: CUSTOMERS_INITIAL_STATE,
  nomenclature: NOMENCLATURE_INITIAL_STATE,
  userSettings: USER_SETTINGS_INITIAL_STATE,
};

export interface ResetAction {
  type: "ResetAction";
}

type CommonAction = CustomersActions | WorkersActions | ResetAction | NomenclatureActions | UserSettingsActions;

export const rootReducer: Reducer<RootState> = (state: RootState = INITIAL_STATE, action: CommonAction): RootState => {
  if (action.type === "ResetAction") {
    return {
      ...INITIAL_STATE,
    };
  }
  return {
    workers: workersReducer(state.workers, action as WorkersActions),
    customers: customersReducer(state.customers, action as CustomersActions),
    nomenclature: nomenclatureReducer(state.nomenclature, action as NomenclatureActions),
    userSettings: userSettingsReducer(state.userSettings, action as UserSettingsActions),
  };
};
