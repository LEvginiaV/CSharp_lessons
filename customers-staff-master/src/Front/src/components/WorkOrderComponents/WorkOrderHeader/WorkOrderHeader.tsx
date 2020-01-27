import { Trash } from "@skbkontur/react-icons";
import { ValidationContainer, ValidationWrapperV1 } from "@skbkontur/react-ui-validations";
import DatePicker from "@skbkontur/react-ui/DatePicker";
import Dropdown from "@skbkontur/react-ui/Dropdown";
import Link from "@skbkontur/react-ui/Link";
import MenuItem from "@skbkontur/react-ui/MenuItem";
import * as React from "react";
import { DateTime } from "../../../api/new/dto/DateTime";
import { Guid } from "../../../api/new/dto/Guid";
import { WorkOrderNumberDto } from "../../../api/new/dto/WorkOrder/WorkOrderNumberDto";
import { WorkOrderStatusDto } from "../../../api/new/dto/WorkOrder/WorkOrderStatusDto";
import { formatDatePickerDateToIso, formatIsoDateToDatePicker } from "../../../common/DateHelper";
import { Metrics } from "../../../common/MetricsFacade";
import { PredicateInput } from "../../../commonComponents/PredicateInput/PredicateInput";
import { StringHelper } from "../../../helpers/StringHelper";
import { WorkOrderValidationHelper } from "../../../validation/WorkOrderValidationHelper";
import { BackButton } from "../../BackButton/BackButton";
import { OrderNumberInput } from "./OrderNumberInput";
import * as styles from "./WorkOrderHeader.less";

interface IWorkOrderHeaderProps {
  onBack: () => void;
  orderId: Nullable<Guid>;
  orderNumber: WorkOrderNumberDto;
  receptionDate?: DateTime;
  status: WorkOrderStatusDto;
  showRemove: boolean;
  onRemove: () => void;
  onChangeReceptionDate: (date?: DateTime) => void;
}

export class WorkOrderHeader extends React.Component<
  IWorkOrderHeaderProps,
  {
    localOrderNumber: {
      series: string;
      number: string;
    };
    localReceptionDate?: string;
    localStatus: WorkOrderStatusDto;
    orderNumberUsed: boolean;
  }
> {
  private ValidationContainer: ValidationContainer | null;

  constructor(props: IWorkOrderHeaderProps, state: {}) {
    super(props, state);

    this.state = {
      localOrderNumber: {
        series: props.orderNumber.series,
        number: props.orderNumber.number.toString(),
      },
      localReceptionDate: formatIsoDateToDatePicker(props.receptionDate),
      localStatus: props.status,
      orderNumberUsed: false,
    };
  }

  public getOrderNumber(): WorkOrderNumberDto {
    return {
      series: this.state.localOrderNumber.series,
      number: +this.state.localOrderNumber.number,
    };
  }

  public showOrderNumberUsed() {
    this.setState({ orderNumberUsed: true });
  }

  public getReceptionDate(): DateTime {
    const receptionDate = formatDatePickerDateToIso(this.state.localReceptionDate);

    if (!receptionDate) {
      throw new Error("reception date is null");
    }

    return receptionDate;
  }

  public getStatus(): WorkOrderStatusDto {
    return this.state.localStatus;
  }

  public async isValid(): Promise<boolean> {
    return !!this.ValidationContainer && (await this.ValidationContainer.validate());
  }

  public render(): JSX.Element {
    const { onBack, showRemove, onRemove } = this.props;
    const { localStatus, localReceptionDate, localOrderNumber, orderNumberUsed } = this.state;

    return (
      <div className={styles.root}>
        <BackButton height={106} onClick={onBack} useRightMargin />
        <div className={styles.wrapper}>
          <ValidationContainer ref={el => (this.ValidationContainer = el)}>
            <div className={styles.header}>
              <div className={styles.name}>Заказ-наряд</div>
              <div className={styles.series}>
                <ValidationWrapperV1
                  {...WorkOrderValidationHelper.bindSeriesValidation(localOrderNumber.series, orderNumberUsed)}
                >
                  <PredicateInput
                    predicate={StringHelper.isOrderSeriesValid}
                    postProcess={v => v.toUpperCase()}
                    width={52}
                    selectAllOnFocus={true}
                    onChange={this.onChangeOrderSeries}
                    onBlur={this.handleSeriesNumberBlur}
                    value={localOrderNumber.series}
                    data-tid="SeriesInput"
                  />
                </ValidationWrapperV1>
                <div className={styles.tip}>Серия</div>
              </div>
              <div>—</div>
              <div className={styles.number}>
                <ValidationWrapperV1
                  {...WorkOrderValidationHelper.bindOrderNumberValidation(localOrderNumber.number, orderNumberUsed)}
                >
                  <OrderNumberInput
                    width={68}
                    onChange={this.onChangeOrderNumber}
                    value={localOrderNumber.number}
                    onBlur={this.handleSeriesNumberBlur}
                    data-tid="NumberInput"
                  />
                </ValidationWrapperV1>
                <div className={styles.tip}>Номер</div>
              </div>
              <div>Дата приема</div>
              <div className={styles.date}>
                <ValidationWrapperV1 {...WorkOrderValidationHelper.bindReceptionDateValidation(localReceptionDate)}>
                  <DatePicker
                    value={localReceptionDate || null}
                    onChange={this.onChangeReceptionDate}
                    data-tid="ReceptionDate"
                  />
                </ValidationWrapperV1>
              </div>
              <div>Статус</div>
              <div className={styles.status}>
                <Dropdown width={150} caption={StringHelper.formatOrderStatus(localStatus)} data-tid="WorkOrderStatus">
                  {this.renderStatusMenuItem(WorkOrderStatusDto.New)}
                  {this.renderStatusMenuItem(WorkOrderStatusDto.InProgress)}
                  {this.renderStatusMenuItem(WorkOrderStatusDto.Completed)}
                  {this.renderStatusMenuItem(WorkOrderStatusDto.IssuedToClient)}
                </Dropdown>
              </div>
              <div className={styles.remove}>
                {showRemove && (
                  <Link onClick={onRemove} data-tid="RemoveLink">
                    <Trash /> Удалить
                  </Link>
                )}
              </div>
            </div>
          </ValidationContainer>
        </div>
      </div>
    );
  }

  private renderStatusMenuItem(localStatus: WorkOrderStatusDto) {
    const { orderId } = this.props;
    return (
      <MenuItem
        onClick={() => {
          if (orderId != null && this.state.localStatus !== localStatus) {
            Metrics.workOrderEditStatus({
              docid: orderId,
              statusOld: this.state.localStatus,
              statusNew: localStatus,
              where: "document",
            });
          }
          this.setState({ localStatus });
        }}
      >
        {StringHelper.formatOrderStatus(localStatus)}
      </MenuItem>
    );
  }

  private onChangeReceptionDate = (_: any, date: string) => {
    this.setState({ localReceptionDate: date });
    this.props.onChangeReceptionDate(formatDatePickerDateToIso(date));
  };

  private onChangeOrderSeries = (_: any, value: string) => {
    this.setState({
      localOrderNumber: {
        ...this.state.localOrderNumber,
        series: value.toUpperCase(),
      },
      orderNumberUsed: false,
    });
  };

  private onChangeOrderNumber = (_: any, value: string) => {
    this.setState({
      localOrderNumber: {
        ...this.state.localOrderNumber,
        number: value,
      },
      orderNumberUsed: false,
    });
  };

  private handleSeriesNumberBlur = () => {
    const { number: num, series } = this.state.localOrderNumber;
    const { orderNumber } = this.props;
    if (orderNumber.series !== series || orderNumber.number !== +num) {
      Metrics.workOrderEditSeriesNumber({
        series: orderNumber.series !== series,
        number: orderNumber.number !== +num,
      });
    }
  };
}
