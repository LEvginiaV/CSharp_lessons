export enum GenderType {
  Male = "male",
  Female = "female",
}

export const MALE_GENDER_TITLE = "Мужской";
export const FEMALE_GENDER_TITLE = "Женский";

export const GenderTypeMap = { [GenderType.Male]: MALE_GENDER_TITLE, [GenderType.Female]: FEMALE_GENDER_TITLE };
