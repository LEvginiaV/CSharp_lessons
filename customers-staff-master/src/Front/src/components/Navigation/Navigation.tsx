import * as classnames from "classnames";
import * as React from "react";
import { Link, Route, Switch } from "react-router-dom";
import { AppDataSingleton } from "../../app/AppData";
import { Metrics } from "../../common/MetricsFacade";
import { Line } from "../../commonComponents/Line/Line";
import { ListType } from "../../models/ListType";
import { NavigationLayout } from "../Layout/Layout";
import { CalendarNav } from "./CalendarNav";
import * as styles from "./Navigation.less";
import * as Tutorial from "./Tutorial.svg";

const tutorialHref = "https://skbkontur.invisionapp.com/share/5QSDP1DZ62X";

const navigateMetrics = {
  [ListType.Calendar]: Metrics.enterCalendar,
  [ListType.WorkOrders]: Metrics.enterWorkOrders,
  [ListType.Workers]: Metrics.enterStaff,
  [ListType.Customers]: Metrics.enterClients,
};

export const Navigation: React.SFC = () => {
  const containsInPath: (url: string, token: string) => boolean = (url: string, token: string) => {
    const tokens = (url || "").split(/\/|\\|\?|\#/);
    return tokens.filter(x => x === token).length > 0;
  };

  const wrapLink = (listType: ListType, caption: string, dataTid?: string): JSX.Element => {
    const isActive = listType && containsInPath(window.location.href, listType);
    const className = classnames(styles.item, isActive && styles.active);

    const link = (
      <Link
        to={`${AppDataSingleton.prefix}/${listType}`}
        className={className}
        data-tid={dataTid}
        onClick={() => navigateMetrics[listType]()}
      >
        {caption}
      </Link>
    );
    if (listType !== ListType.Calendar) {
      return link;
    }
    return (
      <div>
        {link}
        <Switch>
          <Route exact path={`${AppDataSingleton.prefix}/${ListType.Calendar}/:date`} component={CalendarNav} />
        </Switch>
      </div>
    );
  };

  return (
    <NavigationLayout data-tid="NavigationLayout">
      <Line marginBottom={25}>
        {wrapLink(ListType.Calendar, "Календарь", "CalendarLink")}
        {wrapLink(ListType.WorkOrders, "Заказы", "OrdersLink")}
      </Line>
      <Line marginBottom={5}>
        {wrapLink(ListType.Workers, "Сотрудники", "WorkersLink")}
        {wrapLink(ListType.Customers, "Клиенты", "CustomersLink")}
      </Line>
      <div
        className={styles.tutorial}
        onClick={() => {
          Metrics.openTutorial();
          window.open(tutorialHref, "_blank");
        }}
      >
        <div style={{ paddingLeft: 37 }}>
          <img src={Tutorial as any} />
        </div>
        <div style={{ marginBottom: 1 }}>Познакомьтесь</div>
        <div style={{ marginBottom: 7 }}>с новым разделом</div>
        <div className={styles.fakeLink}>Пройти обучение</div>
      </div>
    </NavigationLayout>
  );
};
