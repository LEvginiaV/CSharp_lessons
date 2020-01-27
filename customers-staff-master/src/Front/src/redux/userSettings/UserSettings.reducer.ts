import { UserSettingsActions, UserSettingsActionsTypes } from "./UserSettings.actions";
import { UserSettingsState } from "./UserSettings.state";

export const USER_SETTINGS_INITIAL_STATE: UserSettingsState = { settings: null };

export function userSettingsReducer(state: UserSettingsState, action: UserSettingsActions): UserSettingsState {
  switch (action.type) {
    case UserSettingsActionsTypes.UpdateSetting:
      return {
        settings: {
          ...state.settings,
          ...action.settings,
        },
      };
    default:
      return state;
  }
}
