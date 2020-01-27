export class NumberHelper {
  public static round(num: number): number {
    return +(num + 0.000001).toFixed(2);
  }
}
