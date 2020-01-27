import DateHelper from '../common/DateHelper';
import {repeatArray} from './helpers';

const groups = [
  {
    id: "c9634d5a-aafd-45b7-b0a2-774886218dbd",
    parentId: null,
    name: "Без группы"
  },
  {
    id: "d9183124-015d-4db6-959d-d5c1efa58228",
    parentId: null,
    name: "Табачные изделия"
  },
  {
    id: "9eda1c6a-1d86-46c8-bc25-a6e8ce247500",
    parentId: null,
    name: "Алкогольная продукция"
  },
  {
    id: "8b11a5f0-6598-4feb-adf6-a2eac2ba4764",
    parentId: "9eda1c6a-1d86-46c8-bc25-a6e8ce247500",
    name: "Пиво"
  },
  {
    id: "8556399f-2f48-4e6d-a6de-6818ff764ce5",
    parentId: "9eda1c6a-1d86-46c8-bc25-a6e8ce247500",
    name: "Крепкие напитки"
  },
  {
    id: "ff98843f-f600-40f8-88d6-889981d82f53",
    parentId: null,
    name: "Закуски"
  },
  {
    id: "ff98843f-f600-40f8-88d6-887581d82f53",
    parentId: null,
    name: "То, что у нас не продается"
  }
];

const extraGroups = [
  {
    id: "5af9042f-41cb-4a2b-b098-52f5767f9c0c",
    parentId: null,
    name: "Хлеб и хлебобулочные изделия"
  },
  {
    id: "2f34c0e6-cd9a-4a81-95a3-eb2fa6ca6fea",
    parentId: "5af9042f-41cb-4a2b-b098-52f5767f9c0c",
    name: "Хлеб и выпечка"
  },
  {
    id: "ae00be9e-4da4-4b4e-8dbd-401c08663b2d",
    parentId: "5af9042f-41cb-4a2b-b098-52f5767f9c0c",
    name: "Тесто и полуфабрикаты"
  },
  {
    id: "913869d2-9806-476a-84fc-e9e7e4d4c41c",
    parentId: null,
    name: "Молочные продукты, сыры и яйцо"
  },
  {
    id: "073bf78a-1aa5-438e-b373-a375bde8979b",
    parentId: "913869d2-9806-476a-84fc-e9e7e4d4c41c",
    name: "Молоко и молочная продукция"
  },
  {
    id: "6a2fa435-8713-4ea5-a196-db010558e5d6",
    parentId: "913869d2-9806-476a-84fc-e9e7e4d4c41c",
    name: "Кисло-молочная продукция"
  },
  {
    id: "b37dec37-baae-428a-a31b-0433919d8180",
    parentId: "913869d2-9806-476a-84fc-e9e7e4d4c41c",
    name: "Масло сливочное, маргарин"
  },
  {
    id: "9c722be6-cdba-47ef-becf-1f84a8b8f8a0",
    parentId: "913869d2-9806-476a-84fc-e9e7e4d4c41c",
    name: "Сыр и творог"
  },
  {
    id: "bee6b30a-bc29-4394-ad64-a116127433f5",
    parentId: "913869d2-9806-476a-84fc-e9e7e4d4c41c",
    name: "Яйца"
  },
  {
    id: "7050287c-31ff-4d47-b7e6-8152c759287d",
    parentId: "913869d2-9806-476a-84fc-e9e7e4d4c41c",
    name: "Майонез"
  },
  {
    id: "9cd83336-c0dd-424b-acde-266dd142ba8f",
    parentId: "913869d2-9806-476a-84fc-e9e7e4d4c41c",
    name: "Молоко и сливки сгущенные"
  },
  {
    id: "35988ba7-b713-41c2-97d7-6a1dad4c2e77",
    parentId: "913869d2-9806-476a-84fc-e9e7e4d4c41c",
    name: "Мороженое"
  },
  {
    id: "e38e2c65-751e-4f93-b9fe-59ad0f4e06f7",
    parentId: null,
    name: "Мясо, рыба, птица"
  },
  {
    id: "c218689a-6a8b-4db9-85f7-dc7276b84d4a",
    parentId: "e38e2c65-751e-4f93-b9fe-59ad0f4e06f7",
    name: "Мясо и полуфабрикаты"
  },
  {
    id: "a5565b19-2880-467b-b919-3a3b7824bdf8",
    parentId: "e38e2c65-751e-4f93-b9fe-59ad0f4e06f7",
    name: "Рыба и морепродукты"
  },
  {
    id: "31d8c794-b071-4d0a-81ee-ddaf02608061",
    parentId: "e38e2c65-751e-4f93-b9fe-59ad0f4e06f7",
    name: "Птица"
  },
  {
    id: "55337d98-632a-4c9f-a8da-ad9d19f16fe0",
    parentId: "e38e2c65-751e-4f93-b9fe-59ad0f4e06f7",
    name: "Замороженные продукты"
  },
  {
    id: "2c906b23-ba50-461b-a174-3b2a916adb9f",
    parentId: "e38e2c65-751e-4f93-b9fe-59ad0f4e06f7",
    name: "Колбасные изделия"
  },
  {
    id: "cab7663d-4132-4bd7-af7c-13ff467ad279",
    parentId: null,
    name: "Овощи и фрукты"
  },
  {
    id: "0d0476cc-726b-4a46-8d43-645b1182de18",
    parentId: "cab7663d-4132-4bd7-af7c-13ff467ad279",
    name: "Овощи"
  },
  {
    id: "a79bb065-bceb-4fc4-a5a9-466a9f3b71ee",
    parentId: "cab7663d-4132-4bd7-af7c-13ff467ad279",
    name: "Фрукты, ягоды"
  },
  {
    id: "5d622219-db18-486c-b14c-23da451023c0",
    parentId: "cab7663d-4132-4bd7-af7c-13ff467ad279",
    name: "Замороженные продукты"
  },
  {
    id: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    parentId: "cab7663d-4132-4bd7-af7c-13ff467ad279",
    name: "Салаты, зелень"
  }
];

const products = [
  {
    productId: "0",
    name: "Samuel Adams Boston Lager",
    barcodes: ["bc01", "bc02"],
    groupId: "8b11a5f0-6598-4feb-adf6-a2eac2ba4764",
    taxSystem: "bts",
    vatRate: "main",
    isWeight: false,
    isAlco: true,
    isDeleted: false,
    productUnit: "piece",
    vendorCode: "889911",
  },     // id 0
  {
    productId: "1",
    name: "Guinness",
    barcodes: ["bc11", "bc12"],
    groupId: "8b11a5f0-6598-4feb-adf6-a2eac2ba4764",
    taxSystem: "bts",
    vatRate: "main",
    isWeight: true,
    isAlco: true,
    isDeleted: false,
    productUnit: "liter",
    vendorCode: "8899",
  },                      // id 1
  {
    productId: "2",
    name: "Pringles",
    barcodes: ["bc21", "bc22", "bc23"],
    groupId: "ff98843f-f600-40f8-88d6-889981d82f53",
    taxSystem: "bts",
    vatRate: "main",
    isWeight: false,
    isAlco: false,
    isDeleted: true,
    productUnit: "piece",
    vendorCode: "889922",
  },                      // id 2
  {
    productId: "3",
    name: "Cracker",
    barcodes: ["bc31", "bc32", "bc33"],
    groupId: "ff98843f-f600-40f8-88d6-889981d82f53",
    taxSystem: "bts",
    vatRate: "main",
    isWeight: true,
    isAlco: false,
    isDeleted: true,
    productUnit: "kilogram",
    vendorCode: "889922",
  },                       // id 3
  {
    productId: "4",
    name: "Grey Goose Vodka",
    barcodes: ["bc41", "bc42", "bc43"],
    groupId: "8556399f-2f48-4e6d-a6de-6818ff764ce5",
    taxSystem: "bts",
    vatRate: "preferential",
    isWeight: false,
    isAlco: true,
    isDeleted: false,
    productUnit: "piece",
    vendorCode: "889922",
  },              // id 4
  {
    productId: "5",
    name: "Jameson",
    barcodes: ["bc51", "bc52", "bc53"],
    groupId: "8556399f-2f48-4e6d-a6de-6818ff764ce5",
    taxSystem: "bts",
    vatRate: "noVat",
    isWeight: false,
    isAlco: true,
    isDeleted: false,
    productUnit: "piece",
    vendorCode: "889922",
  },                       // id 5
  {
    productId: "6",
    name: "Пивасик разливной",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "8b11a5f0-6598-4feb-adf6-a2eac2ba4764",
    taxSystem: "stii",
    vatRate: "main",
    isWeight: false,
    isAlco: true,
    isDeleted: false,
    productUnit: "liter",
    vendorCode: "889922",
    packs: [
      {
        nomenclature: 61,
        capacity: 0.89,
        price: 50,
        barCode: "bc61"
      },
      {
        nomenclature: 62,
        capacity: 0.44,
        price: 30,
        barCode: "bc62"
      },
      {
        nomenclature: 63,
        capacity: 1.37,
        price: 70,
        barCode: "bc63"
      }
    ]
  },              // id 6
];

const productsWithTaxWarnings = [
  {
    productId: "66",
    name: "Cigarettes without TaxSystem",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "d9183124-015d-4db6-959d-d5c1efa58228",
    taxSystem: null,
    vatRate: null,
    isWeight: false,
    isAlco: false,
    isDeleted: false,
    productUnit: "piece",
  },  // id 66
  {
    productId: "77",
    name: "Cigarettes without VatRate",
    barcodes: ["bc71", "bc72", "bc73"],
    groupId: "d9183124-015d-4db6-959d-d5c1efa58228",
    taxSystem: "bts",
    vatRate: null,
    isWeight: false,
    isAlco: false,
    isDeleted: false,
    productUnit: "piece",
  },    // id 77
];

const productsWithoutGroup = [
  {
    productId: "12",
    name: "Lighter",
    barcodes: ["911911911"],
    groupId: "c9634d5a-aafd-45b7-b0a2-774886218dbd",
    taxSystem: "bts",
    vatRate: "main",
    isWeight: false,
    isAlco: false,
    isDeleted: false,
    productUnit: "piece",
  },    // id 12
];
const productsInfo = [
  {
    productId: "0",
    name: "Samuel Adams Boston Lager",
    quantity: 10,
    buyPrice: 100,
    buyPriceSum: 1000,
    sellPrice: 200,
    sellPriceSum: 2000,
    overPrice: 100,
    barcodes: ["bc01", "bc02"],
    groupId: "c9634d5a-aafd-45b7-b0a2-774886218dbd",
    taxSystem: "bts",
    productUnit: "piece",
    isDeleted: false,
    vendorCode: "889922",
  },
  {
    productId: "1",
    name: "Guinness",
    quantity: 20,
    buyPrice: 100,
    buyPriceSum: 2000,
    sellPrice: 300,
    sellPriceSum: 3000,
    overPrice: 100,
    barcodes: ["bc11", "bc12"],
    groupId: "c9634d5a-aafd-45b7-b0a2-774886218dbd",
    taxSystem: "bts",
    productUnit: "liter",
    isDeleted: false,
    vendorCode: "859922",
  },
  {
    productId: "6",
    name: "Пивасик с нулевым остатком",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "8b11a5f0-6598-4feb-adf6-a2eac2ba4764",
    taxSystem: "stii",
    quantity: 0,
    buyPrice: 100,
    buyPriceSum: 0,
    sellPrice: 300,
    sellPriceSum: 0,
    overPrice: 100,
    productUnit: "liter",
    isDeleted: false,
    vendorCode: "889422",
  },
  {
    productId: "7",
    name: "Пивасик без розничной цены",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "8b11a5f0-6598-4feb-adf6-a2eac2ba4764",
    taxSystem: "stii",
    quantity: 10,
    buyPrice: 80,
    buyPriceSum: 800,
    sellPrice: null,
    sellPriceSum: null,
    overPrice: 100,
    productUnit: "liter",
    isDeleted: false,
    vendorCode: "889932",
  },
  {
    productId: "8",
    name: "Пивасик без закупочной цены",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "8b11a5f0-6598-4feb-adf6-a2eac2ba4764",
    taxSystem: "stii",
    quantity: 10,
    buyPrice: null,
    buyPriceSum: null,
    sellPrice: 90,
    sellPriceSum: 900,
    overPrice: 100,
    productUnit: "liter",
    isDeleted: false,
    vendorCode: "889912",
  },
  {
    productId: "9",
    name: "Салатик архивный",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
    isArchived: true,
  },
  {
    productId: "9",
    name: "Салатик архивный",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 0,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
    isArchived: true,
  },
  {
    productId: "9",
    name: "Салатик архивный",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: -123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
    isArchived: true,
  },
  {
    productId: "9",
    name: "Салатик удалённый",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: true,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  },
  {
    productId: "9",
    name: "Салатик для пагинации",
    barcodes: ["bc61", "bc62", "bc63"],
    groupId: "2cb3b530-7716-49b5-8b38-f682fc5959d8",
    taxSystem: "stii",
    quantity: 123,
    buyPrice: 123,
    buyPriceSum: 15129,
    sellPrice: 456,
    sellPriceSum: 56088,
    overPrice: 100,
    productUnit: "kilogram",
    isDeleted: false,
  }
];

class BalanceDataGenerator {
  _date = DateHelper.localDateToNormalFormat(new Date());
  _lessProducts = false;
  _withoutProducts = false;
  _moreProducts = false;

  withLessProducts() {
    this._lessProducts = true;
    return this;
  }

  withMoreProducts() {
    this._moreProducts = true;
    return this;
  }

  withoutProducts() {
    this._withoutProducts = true;
    return this;
  }

  getTableData() {
    return {
      date: this._date,
      taxSystems: ["stii", "bts"],
      groups: groups,
      productsInfo: productsInfo,
      sortType: "salePriceDown",
      stickyHatHeight: 100,
    };
  }

  make() {
    let products = this._withoutProducts
      ? []
      : this._lessProducts
        ? productsInfo.slice(0, 2)
        : this._moreProducts
          ? repeatArray(productsInfo, 300)
          : productsInfo;
    return {
      date: this._date,
      taxSystems: ["stii", "bts"],
      groups: groups,
      buyPriceTotal: this._withoutProducts ? 0 : this._moreProducts ? 137758670 : 487928,
      sellPriceTotal: this._withoutProducts ? 0 : this._moreProducts ? 508250540 : 1800716,
      productInfo: products,
      stickyHatHeight: 100,
      sortType: "salePriceDown",
    };
  }
}

export default BalanceDataGenerator;

export {groups, extraGroups, products};
