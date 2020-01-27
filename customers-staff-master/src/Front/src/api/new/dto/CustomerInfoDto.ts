import { GenderType } from "../../../models/GenderType";
import { BirthdayDto } from "./BirthdayDto";

export interface CustomerInfoDto {
  name?: string;
  birthday?: BirthdayDto;
  phone?: string;
  email?: string;
  customId?: string;
  discount?: number;
  gender?: GenderType;
  additionalInfo?: string;
}
