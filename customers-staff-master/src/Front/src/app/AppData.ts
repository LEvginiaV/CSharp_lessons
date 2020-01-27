import { ApiSingleton } from "../api/new/Api";
import { Guid } from "../api/new/dto/Guid";
import { FakeCustomerApi } from "../api/new/FakeApis/FakeCustomerApi";
import { FakeServiceCalendarApi } from "../api/new/FakeApis/FakeServiceCalendarApi";
import { FakeTimeZoneApi } from "../api/new/FakeApis/FakeTimeZoneApi";
import { FakeUserSettingsApi } from "../api/new/FakeApis/FakeUserSettingsApi";
import { FakeWorkerApi } from "../api/new/FakeApis/FakeWorkerApi";
import { FakeWorkingCalendarApi } from "../api/new/FakeApis/FakeWorkingCalendarApi";

class AppData {
  public prefix: string;

  public configure(prefix: string, shopId: Guid) {
    console.log(prefix, shopId);
    this.prefix = prefix;
    if (shopId === "") {
      ApiSingleton.injectWorkerApi(new FakeWorkerApi());
      ApiSingleton.injectCustomerApi(new FakeCustomerApi());
      ApiSingleton.injectServiceCalendarApi(new FakeServiceCalendarApi());
      ApiSingleton.injectWorkingCalendarApi(new FakeWorkingCalendarApi());
      ApiSingleton.injectTimeZoneApi(new FakeTimeZoneApi());
      ApiSingleton.injectUserSettingsApi(new FakeUserSettingsApi());
    } else {
      ApiSingleton.inject(shopId);
    }
  }
}

export const AppDataSingleton = new AppData();
