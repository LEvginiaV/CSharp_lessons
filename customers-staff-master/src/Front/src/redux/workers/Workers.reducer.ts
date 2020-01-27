import { WorkersActions, WorkersActionTypes } from "./Workers.actions";
import { WorkersState } from "./Workers.state";

export const WORKERS_INITIAL_STATE: WorkersState = {
  version: 0,
  workers: null,
};

export function workersReducer(state: WorkersState, action: WorkersActions): WorkersState {
  switch (action.type) {
    case WorkersActionTypes.SetWorkers:
      if (state.version === action.version) {
        return state;
      }

      return {
        ...state,
        version: action.version,
        workers: [...action.workers],
        positionsMap: action.positionsMap,
      };
    default:
      return state;
  }
}
