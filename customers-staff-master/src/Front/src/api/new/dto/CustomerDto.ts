import { CustomerInfoDto } from "./CustomerInfoDto";
import { Guid } from "./Guid";

export interface CustomerDto extends CustomerInfoDto {
  id: Guid;
}
