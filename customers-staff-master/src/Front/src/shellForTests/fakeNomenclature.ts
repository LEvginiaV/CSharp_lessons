import { NomenclatureCard, ProductCategory, UnitType } from "../typings/NomenclatureCard";

const generateCards = (
  id: string,
  name: string,
  price: Nullable<number>,
  productCategoty: ProductCategory,
  unitType: UnitType = UnitType.Piece
): NomenclatureCard => {
  return {
    id,
    name,
    prices: {
      sellPrice: price,
    },
    productCategory: productCategoty,
    unitType,
  };
};

export const fakeNomenclature: NomenclatureCard[] = [
  generateCards("3d944d7f-e200-49cd-ab81-d04331884cf7", "Маникюр", 300, ProductCategory.Service),
  generateCards("bfc83ccb-a31f-4612-803e-3c6c54de67b0", "Педикюр", 500, ProductCategory.Service),
  generateCards("a028af73-4b00-4629-83ff-fa19422596ef", "Креативное покрытие гель-лаком", 700, ProductCategory.Service),
  generateCards("73f9d06c-9f86-4873-96ff-f40c748dfd68", "Перебортовка 16R", 1600, ProductCategory.Service),
  generateCards("247447e4-0c07-48cd-9399-f25bbe2fafa8", "Чистка свечей ВАЗ", 400, ProductCategory.Service),
  generateCards("022ab6d0-7742-4cf5-ad04-68bf675c6d39", "Замена масла ВАЗ", null, ProductCategory.Service),
  generateCards("d67de74e-d042-45a0-8628-0cd179f3cff4", "Развал/схождение ВАЗ", 1200, ProductCategory.Service),
  generateCards("21937450-60fd-4cee-8167-62e51a782b7c", "Снятие", 200, ProductCategory.Service),
  generateCards("2e1e950d-c006-430c-a8ab-72bbc36b5aad", "Окрашивание (длинные)", 2000, ProductCategory.Service),
  generateCards("28e96bf6-2ab3-45dc-a14e-82eb5f2d78e1", "Окрашивание (средние)", 1500, ProductCategory.Service),
  generateCards("d782c64f-f086-4d64-92e1-190912fddf16", "Окрашивание (короткие)", 1200, ProductCategory.Service),
  generateCards("083b6f58-f001-4c0e-bc94-dd686dbf7155", "Стрижка с легкой укладкой", 850, ProductCategory.Service),
  generateCards("eb494d01-db1b-4064-acb1-754bfa12531c", "Сложное окрашивание", 3200, ProductCategory.Service),
  generateCards(
    "e139637f-a08d-4e62-af69-81911b734a9a",
    "СтрижкаГорячимиНожницамиДлиннаяПредлиннаяИПочемуТоСовсемБезПробелов",
    3200,
    ProductCategory.Service
  ),
  generateCards("d2d0c58d-d771-451c-9495-bccf87822e8c", "Вечерняя укладка", 450, ProductCategory.Service),
  generateCards("a89138eb-58a6-444d-9fc4-5d9df408cf1b", "Укладка брашинг", 450, ProductCategory.Service),
  generateCards(
    "6985ada3-76fd-4112-911c-c28fe1643113",
    "Масло машинное",
    2000,
    ProductCategory.NonAlcoholic,
    UnitType.Liter
  ),
  generateCards(
    "80d2927b-cedf-4f91-87bb-d235eb783e2a",
    "Свеча RX-228",
    559,
    ProductCategory.NonAlcoholic,
    UnitType.Piece
  ),
  generateCards(
    "a60acfed-e5e3-4d5e-b1fd-e96bc398b8cd",
    "Краска для волос",
    133.41,
    ProductCategory.NonAlcoholic,
    UnitType.RunningMeter
  ),
  generateCards(
    "e8323e3a-0e89-4460-bcf7-ad1792f94987",
    "Шампунь Head&Shoulders",
    1000,
    ProductCategory.NonAlcoholic,
    UnitType.SquareMeter
  ),
  generateCards(
    "67eff2b3-db1a-48f4-bad2-7014c89e14bf",
    "Тормозные колодки в асс.",
    2100,
    ProductCategory.NonAlcoholic,
    UnitType.Tonne
  ),
  generateCards(
    "f5be562d-baa2-4cfc-8ac9-2eef1dc37641",
    "Лак для ногтей",
    null,
    ProductCategory.NonAlcoholic,
    UnitType.CubicMeter
  ),
  generateCards("2947afaa-0ca7-48b8-b267-d5a852fb73eb", "Бухло", null, ProductCategory.HighAlcoholic, UnitType.Piece),
];
