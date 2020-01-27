import { ProductCategory, UnitType } from "../../../typings/NomenclatureCard";

export interface MarketApiProduct {
  id: string;
  name: string;
  quantity: string;
  productUnit: UnitType;
  productCategory: ProductCategory;
  pricesInfo: MarketApiPriceInfo;
  isDeleted: boolean;
}

export interface MarketApiPriceInfo {
  sellPrice: Nullable<number>;
}
