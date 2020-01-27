import Icon from "@skbkontur/react-icons";
import Button from "@skbkontur/react-ui/Button";
import Link from "@skbkontur/react-ui/Link";
import Loader from "@skbkontur/react-ui/Loader";
import Spinner from "@skbkontur/react-ui/Spinner";
import Textarea from "@skbkontur/react-ui/Textarea";
import Toast from "@skbkontur/react-ui/Toast";
import { saveAs } from "file-saver";
import * as React from "react";
import { RouteChildrenProps } from "react-router";
import { ApiSingleton } from "../../../api/new/Api";
import { CustomerDto } from "../../../api/new/dto/CustomerDto";
import { DateTime } from "../../../api/new/dto/DateTime";
import { Guid } from "../../../api/new/dto/Guid";
import { PrintTaskStatusDto } from "../../../api/new/dto/PrintTask/PrintTaskStatusDto";
import { ValidationResultDto } from "../../../api/new/dto/ValidationResultDto";
import { WorkerDto } from "../../../api/new/dto/WorkerDto";
import { WorkOrderDto } from "../../../api/new/dto/WorkOrder/WorkOrderDto";
import { WorkOrderNumberDto } from "../../../api/new/dto/WorkOrder/WorkOrderNumberDto";
import { WorkOrderStatusDto } from "../../../api/new/dto/WorkOrder/WorkOrderStatusDto";
import { AppDataSingleton } from "../../../app/AppData";
import { toDateOnlyISOString } from "../../../common/DateHelper";
import { Metrics } from "../../../common/MetricsFacade";
import { sleep } from "../../../common/PromiseUtils";
import { TimeSpanHelper } from "../../../common/TimeSpanHelper";
import { Line } from "../../../commonComponents/Line/Line";
import { Sticky } from "../../../commonComponents/Sticky/Sticky";
import { DateHelper } from "../../../helpers/DateHelper";
import { StringHelper } from "../../../helpers/StringHelper";
import { ListType } from "../../../models/ListType";
import { NomenclatureCard } from "../../../typings/NomenclatureCard";
import { ContentLayoutFullScreen } from "../../Layout/Layout";
import { SimpleLightbox } from "../../lightboxes/SimpleLightbox";
import { CustomerProductsBlock } from "../CustomerProductsBlock/CustomerProductsBlock";
import { CustomerValuesBlock } from "../CustomerValuesBlock/CustomerValuesBlock";
import { WorkOrderClientBlock } from "../WorkOrderClientBlock/WorkOrderClientBlock";
import { WorkOrderHeader } from "../WorkOrderHeader/WorkOrderHeader";
import { WorkOrderInfoBlock } from "../WorkOrderInfoBlock/WorkOrderInfoBlock";
import { WorkOrderProductsBlock } from "../WorkOrderProductsBlock/WorkOrderProductsBlock";
import { WorkOrderServicesBlock } from "../WorkOrderServicesBlock/WorkOrderServicesBlock";
import * as styles from "./WorkOrderView.less";

export interface IWorkOrderProps extends RouteChildrenProps<{ id?: Guid }> {
  workers: WorkerDto[];
  customers: CustomerDto[];
  cards: NomenclatureCard[];
}

export class WorkOrderView extends React.Component<
  IWorkOrderProps,
  {
    order: Partial<WorkOrderDto> | null;
    saving: boolean;
    printing: boolean;
    servicesTotalSum?: number;
    additionalText?: string;
    receptionDate?: DateTime;
    removing: boolean;
    showRemoveLightbox: boolean;
  }
> {
  private workOrderHeader: WorkOrderHeader | null;
  private workOrderClientBlock: WorkOrderClientBlock | null;
  private workOrderInfoBlock: WorkOrderInfoBlock | null;
  private workOrderServicesBlock: WorkOrderServicesBlock | null;
  private workOrderProductsBlock: WorkOrderProductsBlock | null;
  private customerValuesBlock: CustomerValuesBlock | null;
  private customerProductsBlock: CustomerProductsBlock | null;
  private orderNumber: WorkOrderNumberDto;
  private receptionDate: DateTime;

  constructor(props: IWorkOrderProps, state: {}) {
    super(props, state);
    this.state = {
      order: null,
      saving: false,
      printing: false,
      removing: false,
      showRemoveLightbox: false,
    };
  }

  public async componentDidMount(): Promise<void> {
    const id = this.getId();
    let order: Partial<WorkOrderDto>;
    if (id) {
      order = await ApiSingleton.WorkOrderApi.readOrder(id);
      Metrics.workOrderEditStart({ docid: id });
    } else {
      const infoPromise = ApiSingleton.WorkOrderApi.getCreateInfoAsync();
      const timezonePromise = ApiSingleton.TimeZoneApi.get();
      await Promise.all([infoPromise, timezonePromise]);
      const info = await infoPromise;
      const timezone = await timezonePromise;
      const offset = timezone ? TimeSpanHelper.toMinutes(timezone.offset) : null;
      const todayDate = DateHelper.getTodayDateWithTimeZone(offset);
      order = {
        number: info.orderNumber,
        status: WorkOrderStatusDto.New,
        additionalText: info.additionalText,
        receptionDate: toDateOnlyISOString(todayDate),
      };
      Metrics.workOrderCreateStart();
    }
    this.setState({ order, additionalText: order.additionalText, receptionDate: order.receptionDate });
    this.workOrderClientBlock && this.workOrderClientBlock.focusClient();
  }

  public render(): JSX.Element {
    return (
      <ContentLayoutFullScreen>{this.state.order ? this.renderContent() : this.renderLoader()}</ContentLayoutFullScreen>
    );
  }

  private renderContent(): JSX.Element {
    const { order, saving, printing, servicesTotalSum, additionalText, showRemoveLightbox } = this.state;
    const { workers, customers, history, cards } = this.props;
    if (!order || !order.number || !order.status) {
      return <div />;
    }

    const orderId = this.getId();

    return (
      <div className={styles.root} data-tid="WorkOrderView">
        {showRemoveLightbox && this.renderRemoveLightbox()}
        <WorkOrderHeader
          onBack={() => {
            if (orderId) {
              Metrics.workOrderEditCancel({ docid: orderId });
            } else {
              Metrics.workOrderCreateCancel();
            }
            history.goBack();
          }}
          orderId={orderId}
          orderNumber={order.number}
          receptionDate={order.receptionDate}
          status={order.status}
          showRemove={!!orderId}
          onRemove={() => this.setState({ showRemoveLightbox: true })}
          onChangeReceptionDate={receptionDate => this.setState({ receptionDate })}
          ref={el => (this.workOrderHeader = el)}
          data-tid="HeaderBlock"
        />
        <div className={styles.body}>
          <Line marginTop={30}>
            <div className={styles.firsLineBlock}>
              <WorkOrderClientBlock
                customers={customers}
                selectedCustomerId={order.clientId}
                ref={el => (this.workOrderClientBlock = el)}
                data-tid="ClientBlock"
              />
              <WorkOrderInfoBlock
                phone={order.shopRequisites ? order.shopRequisites.phone : undefined}
                workers={workers}
                warrantyNumber={order.warrantyNumber}
                receptionWorkerId={order.receptionWorkerId}
                completionDatePlanned={order.completionDatePlanned}
                completionDateFact={order.completionDateFact}
                ref={el => (this.workOrderInfoBlock = el)}
                receptionDate={this.state.receptionDate}
                data-tid="InfoBlock"
              />
            </div>
          </Line>
          <Line marginTop={30}>
            <CustomerValuesBlock
              valueList={order.customerValues}
              onHeightUpdated={() => this.setState({})}
              ref={el => (this.customerValuesBlock = el)}
            />
          </Line>
          <Line marginTop={30}>
            <WorkOrderServicesBlock
              cards={cards}
              services={order.shopServices || []}
              workers={workers}
              onHeightUpdated={() => this.setState({})}
              onTotalSumUpdated={totalSum => this.setState({ servicesTotalSum: totalSum })}
              ref={el => (this.workOrderServicesBlock = el)}
            />
          </Line>
          <Line marginTop={30}>
            <WorkOrderProductsBlock
              cards={cards}
              products={order.shopProducts || []}
              onHeightUpdated={() => this.setState({})}
              servicesTotalSum={servicesTotalSum}
              ref={el => (this.workOrderProductsBlock = el)}
            />
          </Line>
          <Line marginTop={30}>
            <CustomerProductsBlock
              products={order.customerProducts || []}
              onHeightUpdated={() => this.setState({})}
              ref={el => (this.customerProductsBlock = el)}
            />
          </Line>
          <Line marginTop={30} marginLeft={30}>
            <div className={styles.additionalTextCaption}>Текст внизу документа</div>
            <Textarea
              maxRows={0}
              rows={5}
              maxLength={2000}
              value={additionalText || ""}
              resize={"none"}
              width={657}
              onChange={(_, v) => this.setState({ additionalText: v })}
              placeholder="Например: условия гарантии или другие детали выполнения заказа"
              data-tid="AdditionalText"
            />
          </Line>
        </div>
        <Sticky side={"bottom"}>
          {fixed => (
            <div className={fixed ? styles.fixedStickyContent : styles.stickyContent}>
              <div className={styles.saveButton}>
                <Button
                  width={138}
                  use={"primary"}
                  onClick={this.onSave}
                  disabled={saving || printing}
                  data-tid="SaveButton"
                >
                  {saving ? <Spinner dimmed type="mini" caption="" /> : "Сохранить"}
                </Button>
              </div>
              <div className={styles.printLink}>
                <Link disabled={saving || printing} onClick={this.onPrint} data-tid="PrintLink">
                  {printing ? <Spinner dimmed type="mini" caption="" /> : <Icon name="Print" />} Напечатать заказ-наряд
                </Link>
              </div>
            </div>
          )}
        </Sticky>
      </div>
    );
  }

  // noinspection JSMethodCanBeStatic
  private renderLoader(): JSX.Element {
    return <Loader className={styles.loader} active={true} type="big" />;
  }

  private renderRemoveLightbox() {
    const { removing } = this.state;
    return (
      <SimpleLightbox
        header="Удаление заказ-наряда"
        body="Заказ-наряд нельзя будет восстановить"
        disableActionButtons={removing}
        acceptButtonCaption="Удалить"
        onAccept={this.onRemove}
        onClose={() => this.setState({ showRemoveLightbox: false, removing: false })}
        acceptButtonUse="danger"
        data-tid="RemoveModal"
      />
    );
  }

  private onPrint = async () => {
    this.setState({ printing: true });
    const orderId = await this.saveOrder();
    if (orderId) {
      Metrics.workOrderPrintPrint({ docid: orderId });
      await Promise.all([this.printOrder(orderId, false), this.printOrder(orderId, true)]);
      if (!this.getId()) {
        this.props.history.replace(`${AppDataSingleton.prefix}/${ListType.WorkOrders}/${orderId}`);
      }
    }
    this.setState({ printing: false });
  };

  private onSave = async () => {
    this.setState({ saving: true });
    const orderId = await this.saveOrder();
    if (orderId) {
      this.props.history.goBack();
    }
    this.setState({ saving: false });
  };

  private onRemove = async () => {
    const orderId = this.getId();
    if (orderId) {
      Metrics.workOrderDeleteDelete({ docid: orderId });
      try {
        this.setState({ removing: true });
        await ApiSingleton.WorkOrderApi.removeOrder(orderId);
        this.props.history.goBack();
      } catch (e) {
        console.error(e);
        Toast.push("Не удалось удалить заказ-наряд");
      }
    }
    this.setState({ removing: false, showRemoveLightbox: false });
  };

  private async saveOrder(): Promise<Nullable<Guid>> {
    if (
      !this.workOrderHeader ||
      !this.workOrderClientBlock ||
      !this.workOrderInfoBlock ||
      !this.workOrderServicesBlock ||
      !this.workOrderProductsBlock ||
      !this.customerValuesBlock ||
      !this.customerProductsBlock ||
      !(await this.workOrderHeader.isValid()) ||
      !(await this.workOrderClientBlock.isValid()) ||
      !(await this.workOrderInfoBlock.isValid()) ||
      !(await this.customerValuesBlock.isValid()) ||
      !(await this.workOrderServicesBlock.isValid()) ||
      !(await this.workOrderProductsBlock.isValid()) ||
      !(await this.customerProductsBlock.isValid())
    ) {
      return null;
    }

    let customerId: Guid;

    try {
      customerId = await this.workOrderClientBlock.saveCustomerAndGetId();
    } catch (e) {
      console.error(e);
      Toast.push("Ошибка при сохранении клиента");
      return null;
    }

    try {
      const order: WorkOrderDto = {
        number: this.workOrderHeader.getOrderNumber(),
        receptionDate: this.workOrderHeader.getReceptionDate(),
        status: this.workOrderHeader.getStatus(),
        shopRequisites: {
          phone: this.workOrderInfoBlock.getPhone() || "",
          address: "",
          inn: "",
          name: "",
        },
        warrantyNumber: this.workOrderInfoBlock.getWarrantyNumber(),
        receptionWorkerId: this.workOrderInfoBlock.getReceptionWorkerId(),
        completionDatePlanned: this.workOrderInfoBlock.getCompletionDatePlanned(),
        completionDateFact: this.workOrderInfoBlock.getCompletionDateFact(),
        clientId: customerId,
        customerProducts: this.customerProductsBlock.getCustomerProducts(),
        shopProducts: this.workOrderProductsBlock.getShopProducts(),
        shopServices: this.workOrderServicesBlock.getShopServices(),
        customerValues: this.customerValuesBlock.getCustomerValues(),
        additionalText: this.state.additionalText ? this.state.additionalText.trim() : "",
      };

      this.orderNumber = order.number;
      this.receptionDate = order.receptionDate;

      let id = this.getId();
      if (id) {
        await ApiSingleton.WorkOrderApi.updateOrder(id, order);
        Metrics.workOrderEditSuccess({ ...Metrics.variablesFromWorkOrder(order), docid: id });
      } else {
        id = await ApiSingleton.WorkOrderApi.createOrder(order);
        Metrics.workOrderCreateSuccess({ ...Metrics.variablesFromWorkOrder(order), docid: id });
      }
      return id;
    } catch (e) {
      console.error(e);
      const validation = e.validationResult as ValidationResultDto;
      if (validation && validation.errorType === "orderNumberUsed") {
        this.workOrderHeader.showOrderNumberUsed();
      } else {
        Toast.push("Ошибка при сохранении заказ-наряда");
      }
      return null;
    }
  }

  private async printOrder(orderId: Guid, invoice: boolean) {
    const typeName = invoice ? "Квитанция к заказ-наряду" : "Заказ-наряд";
    try {
      const taskId = await ApiSingleton.WorkOrderApi.createPrintTask(orderId, invoice);
      for (let i = 0; i < 10; i++) {
        await sleep(1000);
        const info = await ApiSingleton.WorkOrderApi.getTaskStatus(taskId);
        if (info.status === PrintTaskStatusDto.Failed) {
          Toast.push(`Не удалось распечать ${typeName}`);
          console.error(info.errorMessage);
          return;
        }
        if (info.status === PrintTaskStatusDto.Complete) {
          const file = await ApiSingleton.WorkOrderApi.downloadTaskFile(taskId);
          const orderNumber = this.orderNumber;
          const receptionDate = this.receptionDate;
          const orderNumberStr = `${orderNumber.series}-${StringHelper.formatOrderNumber(orderNumber.number)}`;
          saveAs(file, `${typeName}_№${orderNumberStr}_от_${DateHelper.dateToDatePicker(receptionDate)}.docx`);
          return;
        }
      }
    } catch (e) {
      console.error(e);
      Toast.push(`Не удалось распечать ${typeName}`);
    }

    Toast.push(`Не удалось распечать ${typeName}`);
  }

  private getId(): Guid | undefined {
    return this.props.match ? this.props.match.params.id : undefined;
  }
}
