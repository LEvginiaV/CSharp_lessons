import { Edit, FlagSolid, Ok, Send2 } from "@skbkontur/react-icons";
import CurrencyLabel from "@skbkontur/react-ui/components/CurrencyLabel/CurrencyLabel";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Link from "@skbkontur/react-ui/components/Link/Link";
import Tooltip from "@skbkontur/react-ui/components/Tooltip/Tooltip";
import * as cn from "classnames";
import { NomenclatureCard } from "NomenclatureCard";
import * as React from "react";
import { CustomerDto } from "../../api/new/dto/CustomerDto";
import { ServiceCalendarRecordDto } from "../../api/new/dto/ServiceCalendar/ServiceCalendarRecordDto";
import { CustomerStatusDto } from "../../api/new/dto/ServiceCalendar/ServiceCalendarRecordInfoDto";
import { timeSpan_to_hhmm, TimeSpanHelper } from "../../common/TimeSpanHelper";
import { FormattedPhone } from "../../commonComponents/FormattedPhone/FormattedPhone";
import { StringHelper } from "../../helpers/StringHelper";
import { CalendarHelper } from "./CalendarHelper";
import * as styles from "./CalendarRecord.less";
import { StatusButtons } from "./StatusButtons";

interface Props {
  customer: Nullable<CustomerDto>;
  record: ServiceCalendarRecordDto;
  nomenclature: NomenclatureCard[];
  onEdit: () => void;
  onChangeStatus: (status: CustomerStatusDto) => void;
  onCancel: () => void;
}

export class CalendarRecord extends React.Component<Props> {
  private tooltip: Nullable<Tooltip> = null;
  public render() {
    const { customer } = this.props;
    const heightByMinutes = CalendarHelper.timeToPx(this.getMinutes());
    const smallSize = {
      fontSize: heightByMinutes < 14 ? heightByMinutes : undefined,
      lineHeight: heightByMinutes < 22 ? heightByMinutes - 2 + "px" : undefined,
    };

    const name = (customer && customer.name) || null;
    const phone = (customer && customer.phone) || null;
    return (
      <Tooltip render={this.renderTooltip} trigger="click" ref={ref => (this.tooltip = ref)} data-tid="RecordTooltip">
        <div className={styles.recordWrap} style={{ height: heightByMinutes }} data-tid="Record">
          {this.getMinutes() > 15 ? (
            <div className={cn(styles.record, this.getStatusClass())}>
              <div className={styles.topPad} />
              <div className={styles.lineName} style={smallSize}>
                <div className={styles.icon}>{this.renderIcon()}</div>
                <div className={cn(styles.name, name ? null : styles.empty)}>{name || "Без имени"}</div>
                <div className={cn(styles.phone, phone ? null : styles.empty)}>
                  {phone ? <FormattedPhone value={phone} /> : "Без телефона"}
                </div>
              </div>
              <div className={styles.middlePad} />
              <div className={styles.line2}>{this.renderServiceNames()}</div>
            </div>
          ) : (
            <div className={cn(styles.record, this.getStatusClass())} />
          )}
        </div>
      </Tooltip>
    );
  }

  private renderTooltip = () => {
    const { record, customer } = this.props;
    const services = this.getServiceCards();
    const total = services.reduce((s: number | null, i: NomenclatureCard) => {
      return i.prices.sellPrice != null ? (s || 0) + i.prices.sellPrice : s;
    }, null);
    return (
      <div className={styles.tooltip}>
        <div className={styles.head}>
          <div className={styles.time}>
            <span data-tid="StartTime">{timeSpan_to_hhmm(record.period.startTime)}</span>&nbsp;&mdash;&nbsp;
            <span data-tid="EndTime">{timeSpan_to_hhmm(record.period.endTime)}</span>
          </div>
          <div className={styles.links}>
            <Gapped>
              <Link icon={<Edit />} onClick={this.props.onEdit} data-tid="ChangeRecord">
                Изменить
              </Link>
              <Link onClick={this.props.onCancel} data-tid="CancelRecord">
                Отменить запись
              </Link>
            </Gapped>
          </div>
        </div>
        {this.renderCustomerInfo(customer) || <div className={styles.empty}>Клиент не указан</div>}
        {services.length > 0 && (
          <div className={styles.services}>
            <div className={services.length > 1 ? styles.delimiter : undefined}>
              {services.map((s, i) => (
                <div key={i} className={styles.element} data-tid="ServiceItem">
                  <div className={styles.name} data-tid="ServiceName">
                    {s.name}
                  </div>
                  {s.prices.sellPrice && (
                    <div className={styles.price} data-tid="ServicePrice">
                      <CurrencyLabel value={s.prices.sellPrice} fractionDigits={2} currencySymbol={"₽"} />
                    </div>
                  )}
                </div>
              ))}
            </div>
            {services.length > 1 && total != null && (
              <div className={styles.total}>
                <CurrencyLabel value={total} fractionDigits={2} currencySymbol={"₽"} />
              </div>
            )}
          </div>
        )}
        {record.comment && (
          <div className={styles.comment} data-tid="Comment">
            {record.comment}
          </div>
        )}
        <StatusButtons value={record.customerStatus} onChange={this.handleChangeStatus} />
      </div>
    );
  };

  private renderCustomerInfo = (customer: Nullable<CustomerDto>): JSX.Element | null => {
    if (!customer) {
      return null;
    }
    const hasDiscount = customer.discount !== undefined && Number.isFinite(customer.discount);
    return (
      <div className={styles.person}>
        <div className={styles.flex}>
          <div className={cn(styles.text, customer.name ? null : styles.empty)} data-tid="CustomerName">
            {customer.name || "Без имени"}
          </div>
          <div className={cn(styles.value, customer.phone ? null : styles.empty)} data-tid="CustomerPhone">
            {customer.phone ? <FormattedPhone value={customer.phone} /> : "Без телефона"}
          </div>
        </div>
        {(customer.additionalInfo || hasDiscount) && (
          <div className={cn(styles.flex, styles.line2)}>
            <div className={styles.text}>{customer.additionalInfo}</div>
            {hasDiscount && (
              <div className={styles.value}>
                скидка&nbsp;
                <span data-tid="CustomerDiscount">{StringHelper.formatDiscountString(customer.discount)}</span>%
              </div>
            )}
          </div>
        )}
      </div>
    );
  };

  private handleChangeStatus = (status: CustomerStatusDto) => {
    // @todo как будто костыль
    this.tooltip && this.tooltip.setState({ opened: false });
    this.props.onChangeStatus(status);
  };

  private getServiceCards() {
    return this.props.record.productIds.reduce((s: NomenclatureCard[], x) => {
      const card = this.props.nomenclature.find(n => n.id === x);
      if (card) {
        s.push(card);
      }
      return s;
    }, []);
  }
  private renderServiceNames() {
    return this.getServiceCards()
      .map(x => x.name)
      .join(" + ");
  }

  private renderIcon() {
    switch (this.props.record.customerStatus) {
      case CustomerStatusDto.Active:
        return <FlagSolid />;
      case CustomerStatusDto.ActiveAccepted:
        return <Send2 />;
      case CustomerStatusDto.Completed:
        return <Ok />;
      default:
        return null;
    }
  }

  private getMinutes() {
    const { period } = this.props.record;
    const start = TimeSpanHelper.toMinutes(period.startTime);
    const end = TimeSpanHelper.toMinutes(period.endTime);
    if (end == null || start == null) {
      return 0;
    }
    return Math.floor(end - start);
  }

  private getStatusClass() {
    switch (this.props.record.customerStatus) {
      case CustomerStatusDto.Active:
        return styles.statusActive;
      case CustomerStatusDto.ActiveAccepted:
        return styles.statusAccepted;
      case CustomerStatusDto.Completed:
        return styles.statusCompleted;
      default:
        return null;
    }
  }
}
