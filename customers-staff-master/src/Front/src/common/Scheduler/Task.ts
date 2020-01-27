import { CancellationToken } from "./CancellationToken";

export class Task {
  public action: (token: CancellationToken) => Promise<void>;
  public cancellationToken: CancellationToken;
  public period: number;

  constructor(action: (token: CancellationToken) => Promise<void>, period: number) {
    this.action = action;
    this.period = period;
    this.cancellationToken = new CancellationToken();
  }
}
