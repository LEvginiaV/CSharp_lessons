import { UserSettings } from "./dto/UserSettings/UserSettings";

export interface IUserSettingsApi {
  getSettings(): Promise<UserSettings>;
  updateSetting(key: keyof UserSettings, value: string): Promise<void>;
}
