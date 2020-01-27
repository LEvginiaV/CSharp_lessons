import { NomenclatureCard } from "../../typings/NomenclatureCard";
import { NomenclatureActions, NomenclatureActionTypes } from "./Nomenclature.actions";

class NomenclatureActionCreatorClass {
  public setNomenclature(cards: NomenclatureCard[]): NomenclatureActions {
    return { type: NomenclatureActionTypes.SetNomenclature, cards };
  }
}

export const NomenclatureActionCreator = new NomenclatureActionCreatorClass();
