import { UserSettings } from "../../api/new/dto/UserSettings/UserSettings";

export enum UserSettingsActionsTypes {
  UpdateSetting = "UpdateSetting",
}

export interface UserSettingsActions {
  type: UserSettingsActionsTypes.UpdateSetting;
  settings: UserSettings;
}
