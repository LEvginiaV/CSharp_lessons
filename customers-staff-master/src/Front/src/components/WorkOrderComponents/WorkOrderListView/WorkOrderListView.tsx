import Icon from "@skbkontur/react-icons";
import Button from "@skbkontur/react-ui/Button";
import CurrencyLabel from "@skbkontur/react-ui/CurrencyLabel";
import Loader from "@skbkontur/react-ui/Loader";
import Paging from "@skbkontur/react-ui/Paging";
import Toast from "@skbkontur/react-ui/Toast";
import * as H from "history";
import ceil = require("lodash/ceil");
import * as React from "react";
import { ApiSingleton } from "../../../api/new/Api";
import { CustomerDto } from "../../../api/new/dto/CustomerDto";
import { Guid } from "../../../api/new/dto/Guid";
import { WorkOrderItemDto } from "../../../api/new/dto/WorkOrder/WorkOrderItemDto";
import { WorkOrderStatusDto } from "../../../api/new/dto/WorkOrder/WorkOrderStatusDto";
import { AppDataSingleton } from "../../../app/AppData";
import { formatIsoDateToDatePicker } from "../../../common/DateHelper";
import { FeatureAppearance } from "../../../common/FeatureAppearance";
import { Metrics } from "../../../common/MetricsFacade";
import { Caption, CaptionType } from "../../../commonComponents/Caption/Caption";
import { Feedback } from "../../../commonComponents/Feedback/Feedback";
import { FeedbackType } from "../../../commonComponents/Feedback/FeedbackType";
import { HelpLink } from "../../../commonComponents/HelpLink/HelpLink";
import { OnBoardingComponent } from "../../../commonComponents/OnBoardingComponent/OnBoardingComponent";
import { StringHelper } from "../../../helpers/StringHelper";
import { ListType } from "../../../models/ListType";
import { NomenclatureCard } from "../../../typings/NomenclatureCard";
import { ContentLayoutInner } from "../../Layout/Layout";
import { WorkOrderBoard1, WorkOrderBoard2, WorkOrderBoard3 } from "./imgs";
import { OrderStatusSelector } from "./OrderStatusSelector";
import * as styles from "./WorkOrderListView.less";

const PAGE_SIZE = 30;
const helpLink = "https://egais.userecho.com/knowledge-bases/2/articles/5212-kak-sozdat-zakaz-naryad";

export interface IWorkOrderListViewProps {
  history: H.History;
  customers: CustomerDto[];
  cards: NomenclatureCard[];
  hideWorkOrderOnBoard: boolean | null;
  updateSetting: () => void;
}

export class WorkOrderListView extends React.Component<
  IWorkOrderListViewProps,
  {
    orders: WorkOrderItemDto[] | null;
    dirtyOrderStatuses: { [id: string]: WorkOrderStatusDto | undefined };
    pageNumber: number;
  }
> {
  constructor(props: IWorkOrderListViewProps, state: {}) {
    super(props, state);

    this.state = { orders: null, dirtyOrderStatuses: {}, pageNumber: 1 };
  }

  public async componentDidMount(): Promise<void> {
    await this.updateOrders();
  }

  public render(): JSX.Element {
    const { hideWorkOrderOnBoard } = this.props;
    if (hideWorkOrderOnBoard === false) {
      return this.renderOnBoard();
    }
    const { orders, pageNumber } = this.state;

    const pageCount = orders ? ceil(orders.length / PAGE_SIZE) : 0;
    const sliceFrom = PAGE_SIZE * (pageNumber - 1);
    const sliceTo = PAGE_SIZE * pageNumber;
    const slicedOrders = orders ? orders.slice(sliceFrom, sliceTo) : [];
    const issuedOrders = slicedOrders.filter(x => x.status === WorkOrderStatusDto.IssuedToClient);
    const notIssuedOrders = slicedOrders.filter(x => x.status !== WorkOrderStatusDto.IssuedToClient);

    return (
      <div data-tid="WorkOrderList">
        {FeatureAppearance.shouldShow(FeedbackType.WorkOrdersFeedback) && (
          <Feedback {...FeatureAppearance.getProps(FeedbackType.WorkOrdersFeedback)} />
        )}
        <ContentLayoutInner>
          <div className={styles.root}>
            {(hideWorkOrderOnBoard === null || orders === null) && (
              <div>
                <Caption type={CaptionType.H1}>Список заказов</Caption>
                <Loader className={styles.loader} active={true} type={"big"} />
              </div>
            )}
            {hideWorkOrderOnBoard !== null && orders !== null && (
              <div>
                <div className={styles.header}>
                  <div className={styles.caption}>Список заказов</div>
                  {!!orders.length && <div className={styles.addButton}>{this.renderAddButton()}</div>}
                  <div className={styles.helpLink}>
                    <HelpLink
                      caption="Как заполнить заказ-наряд"
                      hintText="Подробнее про работу с&nbsp;заказ-нарядом"
                      onClick={() => window.open(helpLink)}
                    />
                  </div>
                </div>
                {!!orders.length && (
                  <div className={styles.table}>
                    <div className={styles.caption}>
                      <div className={styles.name} />
                      <div className={styles.date}>Дата приема</div>
                      <div className={styles.sum}>Сумма</div>
                      <div className={styles.status}>Статус</div>
                    </div>
                    <div data-tid="NotIssuedOrders">{notIssuedOrders.map(x => this.renderOrderItem(x))}</div>
                    {notIssuedOrders.length > 0 && issuedOrders.length > 0 && (
                      <div className={styles.rowContent}>
                        <div className={styles.subCaption}>Завершенные</div>
                      </div>
                    )}
                    <div data-tid="IssuedOrders">{issuedOrders.map(x => this.renderOrderItem(x))}</div>
                    {pageCount > 1 && (
                      <div className={styles.paging}>
                        <Paging
                          activePage={pageNumber > pageCount ? pageCount : pageNumber}
                          pagesCount={pageCount}
                          onPageChange={num => this.setState({ pageNumber: num })}
                          useGlobalListener
                        />
                      </div>
                    )}
                  </div>
                )}
                {!orders.length && (
                  <div className={styles.emptyPage}>
                    <div className={styles.text}>Создавайте и печатайте заказ-наряды, их список</div>
                    <div className={styles.text}>и статусы будут отображаться на этой старнице</div>
                    <div className={styles.addButton}>{this.renderAddButton()}</div>
                  </div>
                )}
              </div>
            )}
          </div>
        </ContentLayoutInner>
      </div>
    );
  }

  private _addButtonCallback = (redirectLink: string): void => {
    FeatureAppearance.activate(FeedbackType.WorkOrdersFeedback);
    this.props.history.push(redirectLink);
  };

  private renderOnBoard(): JSX.Element {
    return (
      <ContentLayoutInner>
        <div className={styles.root}>
          <div className={styles.header}>
            <div className={styles.caption}>Список заказов</div>
          </div>
          <OnBoardingComponent
            onFinish={this.props.updateSetting}
            startText="Начать работу с заказами"
            pages={[
              {
                headerTextLines: [
                  "Если вы оформляете заказ-наряды на выполенные работы,",
                  "попробуйте заполнять их в Маркете",
                ],
                imageUrl: WorkOrderBoard3,
                footerTextLines: ["Аккуратная и компактная печатная форма"],
              },
              {
                headerTextLines: [
                  "Если вы оформляете заказ-наряды на выполенные работы,",
                  "попробуйте заполнять их в Маркете",
                ],
                imageUrl: WorkOrderBoard2,
                footerTextLines: [
                  "Товары и услуги подставляются из вашего каталога,",
                  "не придется каждый раз печатать названия",
                ],
              },
              {
                headerTextLines: [
                  "Если вы оформляете заказ-наряды на выполенные работы,",
                  "попробуйте заполнять их в Маркете",
                ],
                imageUrl: WorkOrderBoard1,
                footerTextLines: [
                  "Cтатусы помогут отмечать состояния. Видно, по каким заказам",
                  "нужно позвонить клиентам и сказать, что все готово",
                ],
              },
            ]}
          />
        </div>
      </ContentLayoutInner>
    );
  }

  private renderAddButton() {
    const redirectLink = `${AppDataSingleton.prefix}/${ListType.WorkOrders}/create`;
    return (
      <Button
        size="medium"
        width={174}
        use={"primary"}
        onClick={() => this._addButtonCallback(redirectLink)}
        data-tid="AddButton"
      >
        <Icon name="Add" /> Новый заказ-наряд
      </Button>
    );
  }

  private renderOrderItem(item: WorkOrderItemDto): JSX.Element | null {
    const redirectLink = `${AppDataSingleton.prefix}/${ListType.WorkOrders}/${item.id}`;
    const firsProduct = this.props.cards.find(x => x.id === item.firstProductId);
    const customer = this.props.customers.find(x => x.id === item.clientId);
    const customerName =
      customer && [customer.name, StringHelper.formatPhoneString(customer.phone)].filter(x => !!x).join(", ");

    return (
      <div
        key={item.id}
        className={styles.row}
        onClick={() => this.props.history.push(redirectLink)}
        data-tid="WorkOrderItem"
      >
        <div className={styles.rowContent}>
          <div className={styles.name}>
            <div className={styles.orderName} data-tid="Name">
              {"Заказ-наряд " + item.number.series + "-" + StringHelper.formatOrderNumber(item.number.number)}
            </div>
            <div className={styles.orderCustomer} data-tid="Customer">
              {customerName || "-"}
            </div>
            <div className={styles.orderDescription} data-tid="Description">
              {firsProduct ? firsProduct.name : "-"}
            </div>
          </div>
          <div className={styles.date} data-tid="ReceptionDate">
            {formatIsoDateToDatePicker(item.receptionDate)}
          </div>
          <div className={styles.sum}>
            <CurrencyLabel value={item.totalSum} currencySymbol={"₽"} data-tid="TotalSum" />
          </div>
          <div className={styles.status}>
            <OrderStatusSelector
              width={149}
              value={this.getOrderStatus(item)}
              onChange={(_, v) => this.onUpdateStatus(item.id, v)}
              data-tid="Status"
            />
          </div>
        </div>
      </div>
    );
  }

  private getOrderStatus(order: WorkOrderItemDto): WorkOrderStatusDto {
    return this.state.dirtyOrderStatuses[order.id] || order.status;
  }

  private onUpdateStatus = async (orderId: Guid, status: WorkOrderStatusDto) => {
    const { orders, dirtyOrderStatuses } = this.state;
    if (orders == null) {
      return;
    }
    const order = orders.find(x => x.id === orderId);
    if (!order || this.getOrderStatus(order) === status) {
      return;
    }

    dirtyOrderStatuses[orderId] = status;
    this.setState({ dirtyOrderStatuses });
    Metrics.workOrderEditStatus({
      docid: orderId,
      statusOld: order.status,
      statusNew: status,
      where: "list",
    });

    try {
      await ApiSingleton.WorkOrderApi.updateOrderStatus(orderId, status);
      await this.updateOrders();
    } catch (e) {
      Toast.push("Ошибка при сохранении статуса");
    } finally {
      this.clearOrderStatus(orderId);
    }
  };

  private clearOrderStatus(orderId: Guid) {
    const { dirtyOrderStatuses } = this.state;
    dirtyOrderStatuses[orderId] = undefined;
    this.setState({ dirtyOrderStatuses });
  }

  private async updateOrders() {
    const orders = await ApiSingleton.WorkOrderApi.readOrders();
    this.setState({
      orders: orders
        .filter(x => x.status !== WorkOrderStatusDto.IssuedToClient)
        .concat(orders.filter(x => x.status === WorkOrderStatusDto.IssuedToClient)),
    });
  }
}
