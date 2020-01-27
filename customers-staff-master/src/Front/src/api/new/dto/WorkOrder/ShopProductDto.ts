import { Guid } from "../Guid";

export interface ShopProductDto {
  productId: Guid;
  quantity: number;
  price?: number;
}
