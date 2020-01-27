import { GenderType } from "../../../models/GenderType";
import { CustomerDto } from "../dto/CustomerDto";
import { WorkerDto } from "../dto/WorkerDto";

export const fakeWorkers: WorkerDto[] = [
  { fullName: "Александра", position: "Парикмахер-стилист", code: 123, phone: "79991234567", id: "1111" },
  {
    fullName:
      "БлександраБлександраБлександраБлександраБлександраБлександраБлександраБлександраБлександраБлександраБлександраБлександраБлександраБлександраБлександраБлександраБлександра",
    position:
      "очень длинный Парикмахер-стилист очень длинный Парикмахер-стилист очень длинный Парикмахер-стилист очень длинный Парикмахер-стилист очень длинный Парикмахер-стилист",
    code: 123,
    phone: "79991234567",
    id: "111123",
  },
  { fullName: "Глександра", position: "Парикмахер-стилист", code: 123, phone: "79991234567", id: "1111444" },
  { fullName: "Александра Ивановна", code: 123, phone: "79991234567", id: "11110009" },
  {
    fullName: "Яблонский Иннокентий Викторович",
    position: "Маляр-дизайнер",
    code: 456,
    phone: "78120000000",
    id: "2222",
  },
  {
    fullName: "Константинопольский Варфоломей Иванович",
    position: "Мастер маникюра и педикюра",
    code: 246,
    phone: "73433332215",
    id: "3333",
    additionalInfo:
      "По космогонической гипотезе Джеймса Джинса, сарос многопланово выбирает близкий эксцентриситет. Угловое расстояние гасит непреложный параллакс.",
  },
  { fullName: "Иван Васильевич", position: "Стажер", code: 121314, id: "4444" },
  { fullName: "Ёстественный чувак", position: "Стажер", code: 33322, id: "5555" },
  { fullName: "Е в имени", position: "Стажер-2", code: 33322, id: "55553434" },
  { fullName: "Ё опять у другого чувака", position: "Стажер-3", code: 33322, id: "0909095" },
];

export const fakeCustomers: CustomerDto[] = [
  {
    name:
      "Константинопольский Варфоломей Валентинович Варфоломей Валентинович Варфоломей Валентинович Варфоломей Валентинович Варфоломей Валентинович",
    email: "aa@aa.com",
    discount: 99,
    customId: "12345",
    birthday: { day: 1, month: 3, year: 1900 },
    gender: GenderType.Male,
    phone: "73433332215",
    additionalInfo:
      "По космогонической гипотезе Джеймса Джинса, сарос многопланово выбирает близкий эксцентриситет. Угловое расстояние гасит непреложный параллакс.",
    id: "1111",
  },
  {
    name: "Александра",
    email: "aa@aa.com",
    gender: GenderType.Female,
    phone: "79991234567",
    id: "2222",
  },
  {
    name: "ЯАлександра",
    email: "aa@aa.com",
    gender: GenderType.Female,
    phone: "79991234567",
    id: "2222333",
  },
  {
    name: ".УУУ АА",
    email: "aa@aa.com",
    gender: GenderType.Female,
    phone: "79991234567",
    id: "222123",
  },
  {
    name: "-Игнат",
    email: "aa@aa.com",
    gender: GenderType.Female,
    phone: "79991234567",
    birthday: { day: 25, month: 3, year: 1989 },
    id: "00009",
  },
  {
    name: "Ёпрст",
    email: "aaffffffff@3333333333333aa.com",
    discount: 40,
    phone: "78120000000",
    id: "3333",
  },
  {
    name: "Епрст",
    email: "aaffffffff@3333333333333aa.com",
    discount: 40,
    customId: "4787478747874787",
    phone: "78120000000",
    id: "98989898",
  },
  { name: "Бесполый чувак", email: "aa@aa.com", discount: 22.9, id: "4444" },
  { name: "Ёжкин кот", email: "aa@aa.com", discount: 22.9, id: "44444442" },
  { name: "Васька", email: "aa@aa.com", discount: 99.99, id: "5555" },
];
