/*
* Задумка такая, здесь указывать только те поля которые мы используем
* Если они поменяются в маркете, то там компиляция разлетиться
* А если поменяются нам не нужные, то будет всё ок
* */

export interface NomenclatureCard {
  id: string;
  // naturalId: Nullable<number>;
  name: string;
  // capacity: Nullable<number>;
  // alcoholByVolumeRange: Nullable<AlcoholByVolumeRangeForApi>;
  // packed: boolean;
  // canChangePack: boolean;
  productCategory: ProductCategory;
  // alcoholCategory: Nullable<AlcoholCategory>;
  // barCodes: Nullable<string[]>;
  // egaisCodes: Nullable<string[]>;
  // isDeleted: boolean;
  // isArchived: boolean;
  // archiveDate: Nullable<DateTime>;
  // needToProcess: boolean;
  // needToShowSellPriceTrigger: boolean;
  // lastShownTriggerInfo: Nullable<SellPriceTriggerInfoModel>;
  // currentTriggerInfos: Nullable<SellPriceTriggerInfoModel[]>;
  // packs: Nullable<NomenclatureCardPack[]>;
  // withoutBarcode: boolean;
  // groupId: Nullable<string>;
  // forceProcessed: boolean;
  // vatRate: Nullable<VatRateType>;
  prices: CardPriceDetailsModel;
  // lastPrintedPriceTagHash: Nullable<string>;
  // taxSystem: Nullable<TaxSystemType>;
  unitType: UnitType;
  // vendorCode: Nullable<string>;
}

export enum ProductCategory {
  Unknown = "Unknown",
  HighAlcoholic = "HighAlcoholic",
  LowAlcoholic = "LowAlcoholic",
  NonAlcoholic = "NonAlcoholic",
  Tobacco = "Tobacco",
  AnotherExcise = "AnotherExcise",
  Service = "Service",
  Prepayment = "Prepayment",
}

export enum UnitType {
  Kilogram = "Kilogram",
  Meter = "Meter",
  Liter = "Liter",
  SquareMeter = "SquareMeter",
  CubicMeter = "CubicMeter",
  Tonne = "Tonne",
  RunningMeter = "RunningMeter",
  Piece = "Piece",
}

export interface CardPriceDetailsModel {
  sellPrice: Nullable<number>;
  // percent: Nullable<number>;
  // dividerInKopecks: Nullable<number>;
  // buyPrice: Nullable<number>;
  // buyPriceDate: Nullable<DateTime>;
  // priceType: NomenclatureCardPriceType;
  // wayBillId: Nullable<string>;
  // isDiscountDisabled: boolean;
}
//
// export enum NomenclatureCardPriceType {
//   Unknown = "Unknown",
//   FixPrice = "FixPrice",
//   WithoutPrice = "WithoutPrice",
//   PriceRule = "PriceRule",
// }
