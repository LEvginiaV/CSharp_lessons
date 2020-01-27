import { CustomerDto } from "../api/new/dto/CustomerDto";
import { WorkerInfoDto } from "../api/new/dto/WorkerInfoDto";
import { WorkOrderDto } from "../api/new/dto/WorkOrder/WorkOrderDto";
import { UpdateTimeMode } from "../components/WorkerComponents/WorkersList/WorkersDayEditor";

interface Payload {
  [key: string]: any;
}

class MetricsFacade {
  private shopId: string;
  private useConsole: boolean;

  // Навигация
  public enterMain = () => this.sendMetrics("StaffAndClients", "Enter", "Main");
  public enterCalendar = () => this.sendMetrics("StaffAndClients", "Enter", "Calendar");
  public enterWorkOrders = () => this.sendMetrics("StaffAndClients", "Enter", "WorkOrders");
  public enterStaff = () => this.sendMetrics("StaffAndClients", "Enter", "Staff");
  public enterClients = () => this.sendMetrics("StaffAndClients", "Enter", "Clients");
  public openTutorial = () => this.sendMetrics("StaffAndClients", "Open", "Tutorial");

  // Клиенты
  public clientsCreateStart = (payload: Payload = {}) => this.sendMetrics("Clients", "Create", "Start", payload);
  public clientsCreateSuccess = (payload: Payload = {}) => this.sendMetrics("Clients", "Create", "Success", payload);
  public clientsCreateFail = (payload: Payload = {}) => this.sendMetrics("Clients", "Create", "Fail", payload);
  public clientsCreateCancel = (payload: Payload = {}) => this.sendMetrics("Clients", "Create", "Cancel", payload);

  public clientsEditStart = (payload: Payload = {}) => this.sendMetrics("Clients", "Edit", "Start", payload);
  public clientsEditSuccess = (payload: Payload = {}) => this.sendMetrics("Clients", "Edit", "Success", payload);
  public clientsEditFail = (payload: Payload = {}) => this.sendMetrics("Clients", "Edit", "Fail", payload);
  public clientsEditCancel = (payload: Payload = {}) => this.sendMetrics("Clients", "Edit", "Cancel", payload);

  public clientsComment = (payload: Payload = {}) => this.sendMetrics("Clients", "Edit", "Comment", payload);
  public clientsValidate = (payload: Payload = {}) => this.sendMetrics("Clients", "CreateOrEdit", "Validate", payload);

  // Сотрудники
  public staffCreateStart = (payload: Payload = {}) => this.sendMetrics("Staff", "Create", "Start", payload);
  public staffCreateSuccess = (payload: Payload = {}) => this.sendMetrics("Staff", "Create", "Success", payload);
  public staffCreateFail = (payload: Payload = {}) => this.sendMetrics("Staff", "Create", "Fail", payload);
  public staffCreateCancel = (payload: Payload = {}) => this.sendMetrics("Staff", "Create", "Cancel", payload);

  public staffEditStart = (payload: Payload = {}) => this.sendMetrics("Staff", "Edit", "Start", payload);
  public staffEditSuccess = (payload: Payload = {}) => this.sendMetrics("Staff", "Edit", "Success", payload);
  public staffEditFail = (payload: Payload = {}) => this.sendMetrics("Staff", "Edit", "Fail", payload);
  public staffEditCancel = (payload: Payload = {}) => this.sendMetrics("Staff", "Edit", "Cancel", payload);

  public staffComment = (payload: Payload = {}) => this.sendMetrics("Staff", "Edit", "Comment", payload);
  public staffValidate = (payload: Payload = {}) => this.sendMetrics("Staff", "CreateOrEdit", "Validate", payload);

  // График работы
  public scheduleCreateStart = (payload: Payload = {}) => this.sendMetrics("Schedule", "Create", "Start", payload);
  public scheduleCreateSuccess = (payload: Payload = {}) => this.sendMetrics("Schedule", "Create", "Success", payload);
  public scheduleCreateFail = (payload: Payload = {}) => this.sendMetrics("Schedule", "Create", "Fail", payload);
  public scheduleCreateCancel = (payload: Payload = {}) => this.sendMetrics("Schedule", "Create", "Cancel", payload);

  public scheduleEditStart = (payload: Payload = {}) => this.sendMetrics("Schedule", "Edit", "Start", payload);
  public scheduleEditSuccess = (payload: Payload = {}) => this.sendMetrics("Schedule", "Edit", "Success", payload);
  public scheduleEditFail = (payload: Payload = {}) => this.sendMetrics("Schedule", "Edit", "Fail", payload);
  public scheduleEditCancel = (payload: Payload = {}) => this.sendMetrics("Schedule", "Edit", "Cancel", payload);
  public scheduleEditReplace = (payload: Payload = {}) => this.sendMetrics("Schedule", "Edit", "Replace", payload);

  public scheduleHoliday = (payload: Payload = {}) => this.sendMetrics("Schedule", "Edit", "Holiday", payload);
  public scheduleValidate = (payload: Payload = {}) =>
    this.sendMetrics("Schedule", "CreateOrEdit", "Validate", payload);

  /// Календарь
  public calendarCreateStart = (payload: Payload = {}) => this.sendMetrics("Calendar", "Create", "Start", payload);
  public calendarCreateSuccess = (payload: Payload = {}) => this.sendMetrics("Calendar", "Create", "Success", payload);
  public calendarCreateFail = (payload: Payload = {}) => this.sendMetrics("Calendar", "Create", "Fail", payload);
  public calendarCreateCancel = (payload: Payload = {}) => this.sendMetrics("Calendar", "Create", "Cancel", payload);

  public calendarEditStart = (payload: Payload = {}) => this.sendMetrics("Calendar", "Edit", "Start", payload);
  public calendarEditSuccess = (payload: Payload = {}) => this.sendMetrics("Calendar", "Edit", "Success", payload);
  public calendarEditFail = (payload: Payload = {}) => this.sendMetrics("Calendar", "Edit", "Fail", payload);
  public calendarEditCancel = (payload: Payload = {}) => this.sendMetrics("Calendar", "Edit", "Cancel", payload);

  public calendarDeleteStart = (payload: Payload = {}) => this.sendMetrics("Calendar", "Delete", "Start", payload);
  public calendarDeleteSuccess = (payload: Payload = {}) => this.sendMetrics("Calendar", "Delete", "Success", payload);
  public calendarDeleteFail = (payload: Payload = {}) => this.sendMetrics("Calendar", "Delete", "Fail", payload);
  public calendarDeleteCancel = (payload: Payload = {}) => this.sendMetrics("Calendar", "Delete", "Cancel", payload);

  public calendarStartWorking = (payload: Payload = {}) =>
    this.sendMetrics("Calendar", "Create", "StartWorking", payload);
  public calendarStatus = (payload: Payload = {}) => this.sendMetrics("Calendar", "Edit", "Status", payload);
  public calendarValidate = (payload: Payload = {}) =>
    this.sendMetrics("Calendar", "CreateOrEdit", "Validate", payload);
  public calendarWarning = (payload: Payload = {}) => this.sendMetrics("Calendar", "CreateOrEdit", "Warning", payload);

  // Заказ-наряд
  public workOrderCreateStart = (payload: Payload = {}) => this.sendMetrics("WorkOrder", "Create", "Start", payload);
  public workOrderCreateSuccess = (payload: Payload = {}) =>
    this.sendMetrics("WorkOrder", "Create", "Success", payload);
  public workOrderCreateCancel = (payload: Payload = {}) => this.sendMetrics("WorkOrder", "Create", "Cancel", payload);

  public workOrderEditStart = (payload: Payload = {}) => this.sendMetrics("WorkOrder", "Edit", "Start", payload);
  public workOrderEditSuccess = (payload: Payload = {}) => this.sendMetrics("WorkOrder", "Edit", "Success", payload);
  public workOrderEditCancel = (payload: Payload = {}) => this.sendMetrics("WorkOrder", "Edit", "Cancel", payload);

  public workOrderEditStatus = (payload: Payload = {}) => this.sendMetrics("WorkOrder", "Edit", "Status", payload);
  public workOrderEditSeriesNumber = (payload: Payload = {}) =>
    this.sendMetrics("WorkOrder", "Edit", "SeriesNumber", payload);
  public workOrderPrintPrint = (payload: Payload = {}) => this.sendMetrics("WorkOrder", "Print", "Print", payload);
  public workOrderDeleteDelete = (payload: Payload = {}) => this.sendMetrics("WorkOrder", "Delete", "Delete", payload);

  // Хелперы для Payload
  public variablesFromCustomer(customer: Partial<CustomerDto>, where: "list" | "calendar" | "workorder"): Payload {
    return {
      cardid: customer.id || undefined,
      name: customer.name || null,
      phone: customer.phone || null,
      discount: customer.discount || null,
      gender: customer.gender || null,
      birthday: customer.birthday || null,
      email: customer.email || null,
      comment: customer.additionalInfo || null,
      where,
    };
  }

  public variablesFromWorker(worker: Partial<WorkerInfoDto>): Payload {
    return {
      name: worker.fullName || null,
      phone: worker.phone || null,
      position: worker.position || null,
      comment: worker.additionalInfo || null,
    };
  }

  public variablesFromWorkOrder(_order: WorkOrderDto): Payload {
    const valueType: string =
      _order.customerValues.customerValues && _order.customerValues.customerValues.length > 0
        ? _order.customerValues.customerValueType
        : "none";

    return {
      values: valueType,
      services: _order.shopServices && _order.shopServices.length > 0,
      products: _order.shopProducts && _order.shopProducts.length > 0,
      customerProducts: _order.customerProducts && _order.customerProducts.length > 0,
      comment: !!_order.additionalText && _order.additionalText.length > 0,
    };
  }

  public variablesGetScheduleMode(mode: UpdateTimeMode | any): string {
    switch (mode) {
      case UpdateTimeMode.OneDay:
        return "1";
      case UpdateTimeMode.TwoByTwo:
        return "2/2";
      case UpdateTimeMode.FiveByTwo:
        return "5/2";
      default:
        return mode.toString();
    }
  }

  // Шляпа
  public init(shopId: string, useConsole: boolean) {
    this.shopId = shopId;
    this.useConsole = useConsole;
  }

  private sendMetrics(category: string, action: string, name: string, payload = {}): void {
    const extendedPayload = { ...payload, shopId: this.shopId };
    if (this.useConsole) {
      console.info("[metrics]", category, action, name, extendedPayload);
      return;
    }

    // @todo разобраться с тайпингами
    // @ts-ignore
    window._paq && window._paq.push(["trackEvent", category, action, name, JSON.stringify(extendedPayload)]);
  }
}

export const Metrics = new MetricsFacade();
