import Button from "@skbkontur/react-ui/components/Button/Button";
import CurrencyLabel from "@skbkontur/react-ui/components/CurrencyLabel/CurrencyLabel";
import DatePicker from "@skbkontur/react-ui/components/DatePicker/DatePicker";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Modal from "@skbkontur/react-ui/components/Modal/Modal";
import Textarea from "@skbkontur/react-ui/components/Textarea/Textarea";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import Token from "@skbkontur/react-ui/components/Token/Token";
import TokenInput, { TokenInputType } from "@skbkontur/react-ui/components/TokenInput/TokenInput";
import * as cn from "classnames";
import memoizeOne from "memoize-one";
import * as React from "react";
import { connect } from "react-redux";
import { ApiSingleton } from "../../api/new/Api";
import { CustomerDto } from "../../api/new/dto/CustomerDto";
import { CustomerInfoDto } from "../../api/new/dto/CustomerInfoDto";
import { Guid } from "../../api/new/dto/Guid";
import { ServiceCalendarRecordDto } from "../../api/new/dto/ServiceCalendar/ServiceCalendarRecordDto";
import {
  CustomerStatusDto,
  RecordStatusDto,
  ServiceCalendarRecordInfoDto,
} from "../../api/new/dto/ServiceCalendar/ServiceCalendarRecordInfoDto";
import { WorkerServiceCalendarDayDto } from "../../api/new/dto/ServiceCalendar/WorkerServiceCalendarDayDto";
import { TimeSpan } from "../../api/new/dto/TimeSpan";
import { WorkerDto } from "../../api/new/dto/WorkerDto";
import { WorkingCalendarRecordDto } from "../../api/new/dto/WorkingCalendar/WorkersScheduleDtos";
import { toDateOnlyISOString } from "../../common/DateHelper";
import { FeatureAppearance } from "../../common/FeatureAppearance";
import { Metrics } from "../../common/MetricsFacade";
import { TimeSpanHelper } from "../../common/TimeSpanHelper";
import { FeedbackType } from "../../commonComponents/Feedback/FeedbackType";
import { TimeInput } from "../../commonComponents/TimeInput/TimeInput";
import { NomenclatureListSearcher, Searcher } from "../../controls/Searcher";
import { DateHelper } from "../../helpers/DateHelper";
import { CustomersActionCreator } from "../../redux/customers";
import { RootState, TypeOfConnect } from "../../redux/rootReducer";
import { NomenclatureCard, ProductCategory } from "../../typings/NomenclatureCard";
import { CalendarHelper, CalendarModalValidation } from "./CalendarHelper";
import * as styles from "./CalendarModal.less";
import { CustomerSelector } from "./CustomerSelector";
import { WorkerSelector } from "./WorkerSelector";

export type CalendarModalInfo = OmitProperty<ServiceCalendarRecordDto, "id">;

type IProps = {
  date: Date;
  workerId: Guid;
  recordId: Nullable<Guid>;
  info: CalendarModalInfo;

  currentWorkerCalendarDays: WorkerServiceCalendarDayDto[];
  currentWorkingDayMap: { [workerId: string]: WorkingCalendarRecordDto[] };
  onClose: () => void;
  onSuccess: () => void;
} & TypeOfConnect<typeof reduxConnector>;

interface IState {
  worker: Nullable<WorkerDto>;
  customer: Nullable<CustomerDto | CustomerInfoDto>;
  newCustomerDiscount?: string;
  services: NomenclatureCard[];
  startTime: TimeSpan;
  endTime: TimeSpan;
  datepickerValue: string;
  comment: string;

  workingDayMap: { [workerId: string]: WorkingCalendarRecordDto[] };
  workersCalendarDays: WorkerServiceCalendarDayDto[];

  validation: CalendarModalValidation;
}

export class CalendarModalView extends React.Component<IProps, IState> {
  private getSearcher: (data: CustomerDto[]) => Searcher<NomenclatureCard> = memoizeOne(NomenclatureListSearcher);

  public constructor(props: IProps) {
    super(props);

    const services = props.info.productIds.reduce((s: NomenclatureCard[], x) => {
      const card = this.props.nomenclature.find(n => n.id === x) || this.props.nonServiceCards.find(n => n.id === x);
      if (card) {
        s.push(card);
      }
      return s;
    }, []);

    this.state = {
      worker: props.workers.find(x => x.id === props.workerId),
      customer: props.customers.find(x => x.id === props.info.customerId),
      services,
      comment: props.info.comment,
      startTime: props.info.period.startTime,
      endTime: props.info.period.endTime,
      datepickerValue: DateHelper.dateToDatePicker(props.date),
      workersCalendarDays: this.props.currentWorkerCalendarDays,
      workingDayMap: this.props.currentWorkingDayMap,
      validation: {},
    };
  }

  public componentDidMount() {
    this.updateValidation();
  }

  public render() {
    const { validation } = this.state;
    const {
      name: customerNameError,
      phone: customerPhoneError,
      worker: workerError,
      date: dateError,
      startTime: startTimeError,
      endTime: endTimeError,
    } = validation;

    return (
      <Modal width={570} onClose={this.handleClose} data-tid="CalendarRecordModal">
        <Modal.Header>{this.props.recordId ? "Редактирование записи" : "Новая запись"}</Modal.Header>
        <Modal.Body>
          <div className={styles.row}>
            <div className={styles.label}>Сотрудник</div>
            <div className={styles.value}>
              <WorkerSelector
                value={this.state.worker}
                editable={!!this.props.recordId}
                error={workerError}
                workers={this.props.workers}
                onChange={this.handleWorkerChange}
              />
            </div>
          </div>
          <div className={styles.row}>
            <div className={styles.label}>Клиент</div>
            <div className={styles.value}>
              <CustomerSelector
                value={this.state.customer}
                newCustomerDiscount={this.state.newCustomerDiscount}
                errorName={customerNameError}
                errorPhone={customerPhoneError}
                customers={this.props.customers}
                onSave={this.handleCustomerSave}
                onChange={this.handleCustomerChange}
              />
            </div>
          </div>
          <div className={styles.row}>
            <div className={styles.label}>Услуга</div>
            <div className={styles.value} data-tid="ServiceSelector">
              <TokenInput
                width="100%"
                type={TokenInputType.WithReference}
                getItems={this.getItems}
                selectedItems={this.state.services}
                valueToItem={str =>
                  this.props.nomenclature.find(x => x.id === str) || this.props.nonServiceCards.find(x => x.id === str)
                }
                toKey={item => item.id}
                onChange={itemsNew => this.setState({ services: itemsNew })}
                renderItem={x => (
                  <div className={styles.item}>
                    <span>{x.name}</span>
                    {x.prices.sellPrice && <CurrencyLabel value={x.prices.sellPrice} fractionDigits={2} />}
                  </div>
                )}
                renderToken={(item, { isActive, onClick, onRemove }) => (
                  <Token
                    data-tid={"Token"}
                    key={item.id}
                    colors={{
                      idle: "grayIdle",
                      active: "grayActive",
                    }}
                    isActive={isActive}
                    onClick={onClick}
                    onRemove={onRemove}
                  >
                    <span data-tid="ServiceName">{item.name}</span>&nbsp;
                    {item.prices.sellPrice && (
                      <span data-tid="ServicePrice">
                        <CurrencyLabel value={item.prices.sellPrice} fractionDigits={2} />
                      </span>
                    )}
                  </Token>
                )}
              />
            </div>
          </div>
          <div className={styles.row}>
            <div className={styles.label}>Время</div>
            <div className={styles.value}>
              <div className={styles.datetime}>
                <div className={styles.timeInputs}>
                  <TimeInput
                    data-tid="StartTime"
                    error={!!startTimeError && startTimeError.type === "error"}
                    warning={!!startTimeError && startTimeError.type === "warning"}
                    value={this.state.startTime}
                    onChange={v => this.setState({ startTime: v })}
                    onBlur={this.updateValidation}
                  />{" "}
                  &mdash;{" "}
                  <TimeInput
                    data-tid="EndTime"
                    error={!!endTimeError && endTimeError.type === "error"}
                    warning={!!endTimeError && endTimeError.type === "warning"}
                    value={this.state.endTime}
                    onChange={v => this.setState({ endTime: v })}
                    onBlur={this.updateValidation}
                  />
                </div>
                <DatePicker
                  data-tid="Date"
                  error={!!dateError && dateError.type === "error"}
                  warning={!!dateError && dateError.type === "warning"}
                  value={this.state.datepickerValue}
                  onChange={this.handleDateChange}
                  enableTodayLink
                />
              </div>
              {this.renderTimeValidation()}
            </div>
          </div>
          <div className={styles.row}>
            <div className={styles.label}>Комментарий</div>
            <div className={styles.value}>
              <Textarea
                data-tid="Comment"
                value={this.state.comment}
                maxRows={3}
                maxLength={500}
                width="100%"
                onChange={(_, v) => this.setState({ comment: v })}
              />
            </div>
          </div>
        </Modal.Body>
        <Modal.Footer panel>
          <div className={styles.footer}>
            <div className={styles.buttons}>
              <Gapped>
                <Button use="primary" onClick={this.handleSaveServiceRecord} data-tid="Accept">
                  Сохранить
                </Button>
                <Button onClick={this.handleClose} data-tid="Cancel">
                  Отменить
                </Button>
              </Gapped>
            </div>
            {this.renderTotals()}
          </div>
        </Modal.Footer>
      </Modal>
    );
  }

  private renderTotals() {
    const total = this.state.services.reduce((s: number | null, i: NomenclatureCard) => {
      return i.prices.sellPrice != null ? (s || 0) + i.prices.sellPrice : s;
    }, null);

    return (
      total != null && (
        <div className={styles.total}>
          <span>Сумма&nbsp;</span>
          <span data-tid="FooterTotal">
            <CurrencyLabel value={total} fractionDigits={2} currencySymbol={"₽"} />
          </span>
        </div>
      )
    );
  }

  private handleWorkerChange = (worker: Nullable<WorkerDto>) => {
    this.setState({ worker });
    this.updateValidation();
  };
  private handleCustomerChange = (customer: Nullable<CustomerDto>, newCustomerDiscount?: string) => {
    this.setState({
      customer,
      newCustomerDiscount,
      validation: {
        ...this.state.validation,
        name: null,
        phone: null,
      },
    });
  };

  private handleDateChange = async (_: any, date: string) => {
    this.setState({ datepickerValue: date });
    await this.loadWorkersInfo(DateHelper.dateFromDatePicker(date));
    this.updateValidation();
  };

  private handleCustomerSave = async () => {
    await this.saveNewCustomer();
  };

  private saveNewCustomer = async (): Promise<CustomerDto | null> => {
    const { customer } = this.state;
    if (customer && !customer.hasOwnProperty("id")) {
      const validation = CalendarHelper.getCustomerValidation(customer);
      if (validation) {
        this.setState({
          validation: {
            ...this.state.validation,
            ...validation,
          },
        });
        return null;
      }
      customer.discount = this.state.newCustomerDiscount ? parseFloat(this.state.newCustomerDiscount) : undefined;
      const id = await this.props.addCustomer(customer);
      const result: CustomerDto = { ...customer, id };
      this.setState({
        customer: result,
      });

      Metrics.clientsCreateSuccess(Metrics.variablesFromCustomer(result, "calendar"));
      return result;
    }
    return customer as CustomerDto;
  };

  private getItems = async (query: string): Promise<NomenclatureCard[]> => {
    if (!query) {
      return this.props.nomenclature;
    }
    return this.getSearcher(this.props.nomenclature).search(query);
  };

  private renderTimeValidation() {
    const { date: dateError, startTime: startTimeError, endTime: endTimeError } = this.state.validation;
    if (this.isFormValid()) {
      return null;
    }
    const timeErrorText = startTimeError ? startTimeError.text : endTimeError ? endTimeError.text : "";
    const timeClass = cn(
      styles.timeError,
      (startTimeError && startTimeError.type === "error") || (endTimeError && endTimeError.type === "error")
        ? styles.errorText
        : styles.warningText
    );
    const dateClass = cn(
      styles.dateError,
      dateError && dateError.type === "error" ? styles.errorText : styles.warningText
    );
    return (
      <div className={styles.timeValidation}>
        <div className={timeClass} data-tid="TimeErrorMessage">
          {timeErrorText}
        </div>
        {dateError && (
          <div className={dateClass} data-tid="DateErrorMessage">
            {dateError.text}
          </div>
        )}
      </div>
    );
  }

  private isFormValid(validation?: Nullable<CalendarModalValidation>, onlyError = false) {
    const errors = Object.values(validation || this.state.validation);

    if (onlyError) {
      return !errors.some(val => !!val && val.type === "error");
    }

    return errors.every(val => !val);
  }

  private updateValidation = () => {
    setTimeout(() => {
      const v = this.getValidation();
      this.setState({ validation: v });
    }, 0);
  };

  private getValidation = (): CalendarModalValidation => {
    return {
      ...CalendarHelper.getDateValidation(this.state.datepickerValue, "date"),
      ...CalendarHelper.getTimeValidation(
        this.state.startTime,
        this.state.endTime,
        "startTime",
        "endTime",
        this.checkOtherRecordAtTime,
        this.checkWorkerWorking
      ),
      ...CalendarHelper.getCustomerValidation(this.state.customer),
      worker: this.state.worker
        ? null
        : {
            type: "error",
            text: "",
          },
    };
  };

  private handleSaveServiceRecord = async () => {
    const { recordId } = this.props;
    const validation = this.getValidation();
    if (!this.isFormValid(validation, true)) {
      this.sendSaveFail();
      this.setState({ validation });
      return;
    }

    // Этот if после валидации не должен выполниться
    if (!this.state.worker) {
      return;
    }

    const date = DateHelper.dateFromDatePicker(this.state.datepickerValue);
    if (!date) {
      return;
    }

    FeatureAppearance.activate(FeedbackType.CalendarsFeedback);

    const customer = await this.saveNewCustomer();

    const dto: ServiceCalendarRecordInfoDto = {
      customerId: customer ? customer.id : undefined,
      productIds: this.state.services.map(x => x.id),
      period: {
        startTime: this.state.startTime,
        endTime: TimeSpanHelper.convert24HoursTo1dayIfNeed(this.state.endTime),
      },
      comment: this.state.comment,
    };

    try {
      const payload = {
        bookingid: recordId,
        workerid: this.props.workerId,
        clientid: dto.customerId,
        services: dto.productIds.length,
        startTime: dto.period.startTime,
        endTime: dto.period.endTime,
        date: toDateOnlyISOString(date),
        status: this.props.info.customerStatus || CustomerStatusDto.Active,
      };
      if (recordId) {
        await ApiSingleton.ServiceCalendarApi.updateRecord(
          toDateOnlyISOString(this.props.date),
          this.props.workerId,
          recordId,
          {
            ...dto,
            updatedDate: DateHelper.compareOnlyDates(this.props.date, date) ? toDateOnlyISOString(date) : undefined,
            updatedWorkerId: this.props.workerId !== this.state.worker.id ? this.state.worker.id : undefined,
          }
        );
        Metrics.calendarEditSuccess(payload);
      } else {
        const id = await ApiSingleton.ServiceCalendarApi.createRecord(
          toDateOnlyISOString(date),
          this.props.workerId,
          dto
        );
        Metrics.calendarCreateSuccess({ ...payload, bookingid: id });
      }
      this.props.onSuccess();
    } catch (e) {
      if (e.hasOwnProperty("validationResult") && e.validationResult.errorType === "periodSlot") {
        this.setState({
          validation: {
            startTime: {
              type: "error",
              text: "Попадает на другую запись, сотрудник будет занят",
            },
            endTime: {
              type: "error",
              text: "Попадает на другую запись, сотрудник будет занят",
            },
          },
        });
        this.sendSaveFail();
        return;
      }
      Toast.push("Ошибка сохранения");
      console.error(e);
    }
  };

  private loadWorkersInfo = async (date: Nullable<Date>) => {
    if (!date) {
      return;
    }

    try {
      const data = await ApiSingleton.ServiceCalendarApi.getForDay(toDateOnlyISOString(date), RecordStatusDto.Active);
      const busy = await ApiSingleton.WorkingCalendarApi.getForDay(date, null);

      this.setState({
        workersCalendarDays: data.workerCalendarDays,
        workingDayMap: busy.workingDayMap,
      });
    } catch (e) {
      console.error("Error while fetching day", e);
    }
  };

  private checkOtherRecordAtTime = (startMinutes: number, endMinutes: number) => {
    const { worker } = this.state;
    if (!worker) {
      return false;
    }

    const day = this.state.workersCalendarDays.find(x => x.workerId === worker.id);
    if (!day) {
      return false;
    }

    return !!day.records.find(x => {
      const start = TimeSpanHelper.toMinutes(x.period.startTime);
      const end = TimeSpanHelper.toMinutes(x.period.endTime);

      if (x.id === this.props.recordId) {
        return false;
      }
      return (
        (start != null && start > startMinutes && start < endMinutes) ||
        (end != null && end > startMinutes && end < endMinutes)
      );
    });
  };

  private checkWorkerWorking = (startMinutes: number, endMinutes: number) => {
    const { worker } = this.state;
    if (!worker) {
      return true;
    }

    const day = this.state.workingDayMap[worker.id];
    if (!day) {
      return false;
    }

    return day.some(x => {
      const start = TimeSpanHelper.toMinutes(x.period.startTime);
      const end = TimeSpanHelper.toMinutes(x.period.endTime);

      return (
        start != null &&
        end != null &&
        (startMinutes >= start && startMinutes <= end && (endMinutes >= start && endMinutes <= end))
      );
    });
  };

  private handleClose = () => {
    const { recordId } = this.props;
    if (recordId) {
      Metrics.calendarEditCancel({ bookingid: recordId });
    } else {
      Metrics.calendarCreateCancel();
    }
    this.props.onClose();
  };

  private sendSaveFail() {
    const { recordId } = this.props;
    if (recordId) {
      Metrics.calendarEditFail({ bookingid: recordId });
    } else {
      Metrics.calendarCreateFail();
    }
  }
}

const reduxConnector = connect(
  (state: RootState) => ({
    customers: state.customers.customers || [],
    workers: state.workers.workers || [],
    nomenclature: state.nomenclature.cards.filter(x => x.productCategory === ProductCategory.Service),
    nonServiceCards: state.nomenclature.cards.filter(x => x.productCategory !== ProductCategory.Service),
    addCustomer: CustomersActionCreator.create,
  }),
  {}
);

export const CalendarModal = reduxConnector(CalendarModalView);
