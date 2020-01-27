import * as React from "react";
import { Redirect, Route, Switch } from "react-router";
import { AppDataSingleton } from "../../app/AppData";
import { toDateOnlyISOString } from "../../common/DateHelper";
import { getLastWorkerListTabTypeToLocalStorage } from "../../common/Scheduler/LocalStorageHelper";
import { ListType } from "../../models/ListType";
import { WorkersListTabType } from "../../models/WorkersListTabType";
import { Calendar } from "../Calendar/Calendar";
import { Customer, Worker } from "../PersonConnectors/PersonConnector";
import { CustomersList, WorkersList } from "../PersonConnectors/PersonListConnector";
import { WorkOrder, WorkOrderList } from "../PersonConnectors/WorkOrderConnector";

const WorkersListRouting: React.SFC = () => {
  return (
    <Switch>
      <Route
        exact
        path={`${AppDataSingleton.prefix}/${ListType.Workers}/${WorkersListTabType.List}`}
        component={WorkersList}
      />
      <Route
        exact
        path={`${AppDataSingleton.prefix}/${ListType.Workers}/${WorkersListTabType.Chart}`}
        component={WorkersList}
      />
      <Route
        path={`${AppDataSingleton.prefix}/${ListType.Workers}/${WorkersListTabType.List}/:id`}
        component={Worker}
      />
      {getWorkersRedirect()}
    </Switch>
  );
};

function getWorkersRedirect() {
  const tabType = getLastWorkerListTabTypeToLocalStorage() || WorkersListTabType.List;
  return (
    <Redirect
      exact
      from={`${AppDataSingleton.prefix}/${ListType.Workers}`}
      to={`${AppDataSingleton.prefix}/${ListType.Workers}/${tabType}`}
    />
  );
}

const CustomersListRouting: React.SFC = () => {
  return (
    <Switch>
      <Route exact path={`${AppDataSingleton.prefix}/${ListType.Customers}`} component={CustomersList} />
      <Route path={`${AppDataSingleton.prefix}/${ListType.Customers}/:id`} component={Customer} />
    </Switch>
  );
};

const CalendarListRouting: React.SFC = () => {
  const date = toDateOnlyISOString(new Date());
  return (
    <Switch>
      <Route exact path={`${AppDataSingleton.prefix}/${ListType.Calendar}/:date`} component={Calendar} />
      <Redirect to={`${AppDataSingleton.prefix}/${ListType.Calendar}/${date}`} />
    </Switch>
  );
};

const WorkOrderListRouting: React.SFC = () => {
  return (
    <Switch>
      <Route exact path={`${AppDataSingleton.prefix}/${ListType.WorkOrders}`} component={WorkOrderList} />
      <Route exact path={`${AppDataSingleton.prefix}/${ListType.WorkOrders}/create`} component={WorkOrder} />
      <Route path={`${AppDataSingleton.prefix}/${ListType.WorkOrders}/:id`} component={WorkOrder} />
    </Switch>
  );
};

export const MainRouting: React.SFC = () => {
  return (
    <Switch>
      <Route path={`${AppDataSingleton.prefix}/${ListType.Workers}`} component={WorkersListRouting} />
      <Route path={`${AppDataSingleton.prefix}/${ListType.Customers}`} component={CustomersListRouting} />
      <Route path={`${AppDataSingleton.prefix}/${ListType.Calendar}`} component={CalendarListRouting} />
      <Route path={`${AppDataSingleton.prefix}/${ListType.WorkOrders}`} component={WorkOrderListRouting} />
      <Redirect to={`${AppDataSingleton.prefix}/${ListType.Workers}`} />
    </Switch>
  );
};
