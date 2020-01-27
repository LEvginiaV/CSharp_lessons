import { applyMiddleware, createStore, Store } from "redux";
import { createLogger } from "redux-logger";
import thunk from "redux-thunk";
import { rootReducer, RootState } from "./rootReducer";

const logger = createLogger({
  predicate: () => !process.env.NODE_ENV || process.env.NODE_ENV === "development",
  collapsed: true,
  diff: true,
});

export const createRootStore: () => Store<RootState> = () => {
  return !process.env.NODE_ENV || process.env.NODE_ENV === "development"
    ? (applyMiddleware(thunk, logger)(createStore)(rootReducer) as Store<RootState>)
    : (applyMiddleware(thunk)(createStore)(rootReducer) as Store<RootState>);
};

export const getFetchingPeriodInMilliseconds: () => number = () => {
  return !process.env.NODE_ENV || process.env.NODE_ENV === "development" ? 15000 : 60000;
};
