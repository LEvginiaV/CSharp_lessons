import { CustomerApi } from "./CustomerApi";
import { Guid } from "./dto/Guid";
import { ICustomerApi } from "./ICustomerApi";
import { IServiceCalendarApi } from "./IServiceCalendarApi";
import { ITimeZoneApi } from "./ITimeZoneApi";
import { IWorkerApi } from "./IWorkerApi";
import { IWorkingCalendarApi } from "./IWorkingCalendarApi";
import { IWorkOrderApi } from "./IWorkOrderApi";
import { ServiceCalendarApi } from "./ServiceCalendarApi";
import { TimeZoneApi } from "./TimeZoneApi";
import { WorkerApi } from "./WorkerApi";
import { WorkingCalendarApi } from "./WorkingCalendarApi";
import { WorkOrderApi } from "./WorkOrderApi";
import { IUserSettingsApi } from "./IUserSettingsApi";
import { UserSettingsApi } from "./UserSettingsApi";

export class Api {
  public CustomerApi: ICustomerApi;
  public WorkerApi: IWorkerApi;
  public WorkingCalendarApi: IWorkingCalendarApi;
  public ServiceCalendarApi: IServiceCalendarApi;
  public TimeZoneApi: ITimeZoneApi;
  public WorkOrderApi: IWorkOrderApi;
  public UserSettingsApi: IUserSettingsApi;

  public inject(shopId: Guid) {
    this.CustomerApi = new CustomerApi("/customersApi", shopId);
    this.WorkerApi = new WorkerApi("/customersApi", shopId);
    this.WorkingCalendarApi = new WorkingCalendarApi("/customersApi", shopId);
    this.ServiceCalendarApi = new ServiceCalendarApi("/customersApi", shopId);
    this.TimeZoneApi = new TimeZoneApi("/customersApi", shopId);
    this.WorkOrderApi = new WorkOrderApi("/customersApi", shopId);
    this.UserSettingsApi = new UserSettingsApi("/customersApi");
  }

  public injectCustomerApi(customerApi: ICustomerApi) {
    this.CustomerApi = customerApi;
  }

  public injectWorkerApi(workerApi: IWorkerApi) {
    this.WorkerApi = workerApi;
  }

  public injectServiceCalendarApi(serviceCalendarApi: IServiceCalendarApi) {
    this.ServiceCalendarApi = serviceCalendarApi;
  }

  public injectWorkingCalendarApi(workingCalendarApi: IWorkingCalendarApi) {
    this.WorkingCalendarApi = workingCalendarApi;
  }

  public injectTimeZoneApi(timeZoneApi: ITimeZoneApi) {
    this.TimeZoneApi = timeZoneApi;
  }

  public injectUserSettingsApi(userSettingsApi: IUserSettingsApi) {
    this.UserSettingsApi = userSettingsApi;
  }
}

export const ApiSingleton = new Api();
