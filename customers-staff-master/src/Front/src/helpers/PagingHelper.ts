export class PagingHelper {
  public static getSlice<T>(data: T[], personsPerPage: number, skip?: number, take?: number): T[] {
    if (data && (skip !== null || skip !== undefined || take !== null || take !== undefined)) {
      return data.length > personsPerPage ? data.slice(skip, take) : data;
    } else {
      return [];
    }
  }
}
