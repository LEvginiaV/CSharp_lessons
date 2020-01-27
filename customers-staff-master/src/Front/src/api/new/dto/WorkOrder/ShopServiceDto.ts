import { Guid } from "../Guid";

export interface ShopServiceDto {
  productId: Guid;
  quantity: number;
  price?: number;
  workerId?: Guid;
}
