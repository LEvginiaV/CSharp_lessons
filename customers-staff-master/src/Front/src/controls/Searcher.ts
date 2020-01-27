import { CustomerDto } from "../api/new/dto/CustomerDto";
import { WorkerDto } from "../api/new/dto/WorkerDto";
// @ts-ignore
import * as PocketElasticSearcher from "../common/pocket-elasticsearch/pocketElasticSearcher.js";
import { CustomerDtoExtended } from "../models/CustomerDtoExtended";
import { NomenclatureCard } from "../typings/NomenclatureCard";

export class Searcher<T> {
  private elastic: any;

  constructor(
    data: T[],
    tokenPaths: Array<keyof T>,
    exactPaths: Array<keyof T> = [],
    fullSubstringPaths: Array<keyof T> = []
  ) {
    this.elastic = new PocketElasticSearcher.default(data, tokenPaths, exactPaths, fullSubstringPaths);
  }

  public search(searchString: string): T[] {
    return this.elastic && searchString ? this.elastic.search(searchString) : [];
  }
}

export const WorkersListSearcher = (data: WorkerDto[]) => new Searcher<WorkerDto>(data, ["fullName", "position"]);

export const CustomerListSearcher = (data: CustomerDto[]) => {
  const extendedData = data.map(x => {
    return {
      phoneWith8: x.phone ? "8" + x.phone.substring(1) : undefined,
      ...x,
    };
  });
  return new Searcher<CustomerDtoExtended>(extendedData, ["name", "customId", "phoneWith8"], [], ["phone"]);
};

export const NomenclatureListSearcher = (data: NomenclatureCard[]) => new Searcher<NomenclatureCard>(data, ["name"]);
