import { CustomerDto } from "../api/new/dto/CustomerDto";

export interface CustomerDtoExtended extends CustomerDto {
  phoneWith8?: string;
}
