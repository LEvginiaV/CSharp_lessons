import { CancellationToken } from "./CancellationToken";
import { Task } from "./Task";

class SchedulerImpl {
  private timers: Map<string, number>;
  private tasks: Map<string, Task>;

  constructor() {
    this.timers = new Map<string, number>();
    this.tasks = new Map<string, Task>();
  }

  public register(taskName: string, action: () => Promise<any>, period: number) {
    this.registerWithCancel(taskName, (_: CancellationToken): Promise<any> => action(), period);
  }

  public registerWithCancel(taskName: string, action: (ct: CancellationToken) => Promise<any>, period: number) {
    if (this.tasks.has(taskName)) {
      return;
    }
    this.tasks.set(taskName, new Task(action, period));
    this.timers.delete(taskName);
    this.__executeAndSchedule(taskName);
  }

  public unregister(taskName: string) {
    const task = this.tasks.get(taskName);
    if (task) {
      task.cancellationToken.isCanceled = true;
    }
    this.tasks.delete(taskName);
    const currentTimeout = this.timers.get(taskName);
    if (currentTimeout) {
      clearTimeout(currentTimeout);
      this.timers.delete(taskName);
    }
  }

  public force(taskName: string) {
    const task = this.tasks.get(taskName);
    if (!task) {
      return Promise.resolve();
    }
    this.unregister(taskName);
    this.registerWithCancel(taskName, task.action, task.period);
    return Promise.resolve();
  }

  public clear() {
    this.tasks.forEach((_, key) => this.unregister(key));
  }

  private __execute(taskName: string) {
    const task = this.tasks.get(taskName);
    if (!task) {
      return Promise.reject();
    }
    return task.action(task.cancellationToken).catch(err => console.error("Error while scheduling " + taskName, err));
  }

  private __executeAndSchedule(taskName: string) {
    const self = this;
    return self
      .__execute(taskName)
      .then(() => {
        self.__schedule(taskName);
      })
      .catch(() => {
        return;
      });
  }

  private __schedule(taskName: string) {
    const self = this;
    const task = self.tasks.get(taskName);
    if (task && !self.timers.get(taskName)) {
      self.timers.set(taskName, setTimeout(() => {
        if (!self.timers.get(taskName)) {
          return;
        }
        self.timers.delete(taskName);
        self.__executeAndSchedule(taskName);
      }, task.period) as any);
    }
  }
}

export const Scheduler = new SchedulerImpl();
