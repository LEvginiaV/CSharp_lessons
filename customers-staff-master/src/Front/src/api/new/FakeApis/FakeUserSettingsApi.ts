import { sleep } from "../../../common/PromiseUtils";
import { UserSettings } from "../dto/UserSettings/UserSettings";
import { IUserSettingsApi } from "../IUserSettingsApi";

export class FakeUserSettingsApi implements IUserSettingsApi {
  private settings = {
    hideWorkOrderOnBoard: "false",
    hideCalendarOnBoard: "false",
    hideWorkersOnBoard: "false",
    hideCustomersOnBoard: "false",
  };

  public async getSettings(): Promise<UserSettings> {
    await sleep(200);
    return this.settings;
  }

  public async updateSetting(key: keyof UserSettings, value: string): Promise<void> {
    await sleep(200);
    this.settings[key] = value;
  }
}
