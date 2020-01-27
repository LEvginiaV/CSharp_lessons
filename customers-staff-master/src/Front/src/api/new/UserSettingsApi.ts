import HttpClient from "../../common/HttpClient/HttpClient";
import { UserSettings } from "./dto/UserSettings/UserSettings";
import { IUserSettingsApi } from "./IUserSettingsApi";

export class UserSettingsApi implements IUserSettingsApi {
  private readonly urlPrefix: string;

  constructor(urlPrefix: string) {
    this.urlPrefix = `${urlPrefix}/user/settings`;
  }

  public async getSettings(): Promise<UserSettings> {
    return await HttpClient.get(`${this.urlPrefix}`, {});
  }

  public async updateSetting(key: keyof UserSettings, value: string): Promise<void> {
    await HttpClient.put(`${this.urlPrefix}/${key}?value=${value}`, {});
  }
}
