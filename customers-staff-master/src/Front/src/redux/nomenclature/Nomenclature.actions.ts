import { NomenclatureCard } from "../../typings/NomenclatureCard";

export enum NomenclatureActionTypes {
  SetNomenclature = "SetNomenclature",
}

export interface NomenclatureActions {
  type: NomenclatureActionTypes.SetNomenclature;
  cards: NomenclatureCard[];
}
