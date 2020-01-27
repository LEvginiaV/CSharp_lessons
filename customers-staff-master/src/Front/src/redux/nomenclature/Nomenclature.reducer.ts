import { NomenclatureActions, NomenclatureActionTypes } from "./Nomenclature.actions";
import { NomenclatureState } from "./Nomenclature.state";

export const NOMENCLATURE_INITIAL_STATE: NomenclatureState = {
  cards: [],
};

export function nomenclatureReducer(state: NomenclatureState, action: NomenclatureActions): NomenclatureState {
  switch (action.type) {
    case NomenclatureActionTypes.SetNomenclature:
      return {
        ...state,
        cards: [...action.cards],
      };
    default:
      return state;
  }
}
