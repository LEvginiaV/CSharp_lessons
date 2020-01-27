import { Action } from "redux";
import { ThunkAction } from "redux-thunk";
import { ApiSingleton } from "../../api/new/Api";
import { UserSettings } from "../../api/new/dto/UserSettings/UserSettings";
import { RootState } from "../rootReducer";
import { UserSettingsActions, UserSettingsActionsTypes } from "./UserSettings.actions";

class UserSettingsActionCreatorClass {
  public updateSetting: (
    settingKey: keyof UserSettings,
    value: string
  ) => ThunkAction<Promise<void>, RootState, {}, Action> = (settingKey: keyof UserSettings, value: string) => {
    return async dispatch => {
      await dispatch(this.createUpdateSettingsAction({ [settingKey]: value }));
      await ApiSingleton.UserSettingsApi.updateSetting(settingKey, value);
      const settings = await ApiSingleton.UserSettingsApi.getSettings();
      dispatch(this.createUpdateSettingsAction(settings));
    };
  };

  public loadSettings: () => ThunkAction<Promise<void>, RootState, {}, Action> = () => {
    return async dispatch => {
      const settings = await ApiSingleton.UserSettingsApi.getSettings();
      dispatch(this.createUpdateSettingsAction(settings));
    };
  };

  public createUpdateSettingsAction(setting: UserSettings): UserSettingsActions {
    return {
      type: UserSettingsActionsTypes.UpdateSetting,
      settings: setting,
    };
  }
}

export const UserSettingsActionCreator = new UserSettingsActionCreatorClass();
