import * as sortBy from "lodash/sortBy";
import memoizeOne from "memoize-one";
import { CustomerDto } from "../api/new/dto/CustomerDto";
import { WorkerDto } from "../api/new/dto/WorkerDto";
import Collator = Intl.Collator;

export class PersonsHelper {
  public static getPositionsMap = (workers: WorkerDto[]): string[] => PersonsHelper.getPositionsMapMemoize(workers);

  public static sortWorkers(workers: WorkerDto[]): WorkerDto[] {
    return PersonsHelper.sortByTwoStringFields<WorkerDto>(workers, "position", "fullName");
  }

  public static sortCustomers(customers: CustomerDto[]): CustomerDto[] {
    return PersonsHelper.sortPersons<CustomerDto>(customers, "name");
  }

  private static getPositionsMapMemoize = memoizeOne(
    (workers: WorkerDto[]): string[] => {
      return PersonsHelper.getWorkersPositionsMap(workers);
    }
  );

  private static compareText = <T>(a: T, b: T, field: keyof T, collator: Collator): number => {
    if (a[field] && b[field]) {
      return collator.compare(a[field].toString(), b[field].toString());
    } else if (a[field]) {
      return -1;
    } else if (b[field]) {
      return 1;
    } else {
      return 0;
    }
  };

  private static sortPersons = <T>(persons: T[], field: keyof T): T[] => {
    const collator = new Intl.Collator("ru-RU");
    return persons.sort((a, b) => PersonsHelper.compareText(a, b, field, collator));
  };

  private static sortByTwoStringFields = <T>(items: T[], field1: keyof T, field2: keyof T): T[] => {
    const collator = new Intl.Collator("ru-RU");
    return items.sort((a, b) => {
      const res1 = PersonsHelper.compareText(a, b, field1, collator);
      return res1 !== 0 ? res1 : PersonsHelper.compareText(a, b, field2, collator);
    });
  };

  private static getWorkersPositionsMap(workers: WorkerDto[]): string[] {
    const withCounts: Array<{ position: string; count: number }> = [];

    workers.forEach(w => {
      if (w.position) {
        const i = withCounts.findIndex(p => p.position === w.position);
        withCounts[i] ? withCounts[i].count++ : withCounts.push({ position: w.position, count: 1 });
      }
    });

    return sortBy(withCounts, p => p.count)
      .reverse()
      .map(p => p.position);
  }
}
