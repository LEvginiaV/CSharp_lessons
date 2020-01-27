import { Action } from "redux";
import { ThunkAction } from "redux-thunk";
import { ApiSingleton } from "../../api/new/Api";
import { Guid } from "../../api/new/dto/Guid";
import { WorkerDto } from "../../api/new/dto/WorkerDto";
import { WorkerInfoDto } from "../../api/new/dto/WorkerInfoDto";
import { Scheduler } from "../../common/Scheduler/Scheduler";
import { PersonsHelper } from "../../helpers/PersonsHelper";
import { RootState } from "../rootReducer";
import { WorkersActions, WorkersActionTypes } from "./Workers.actions";

const RefreshWorkerList = "RefreshWorkerList";

class WorkersActionCreatorClass {
  public schedulePeriodicListRefresh: (period: number) => ThunkAction<Promise<void>, RootState, {}, Action> = (
    period: number
  ) => {
    return async dispatch => {
      Scheduler.register(RefreshWorkerList, () => dispatch(this.refreshList), period);
    };
  };

  public create: (workerInfo: WorkerInfoDto) => Promise<string> = async (workerInfo: WorkerInfoDto) => {
    const worker = await ApiSingleton.WorkerApi.create(workerInfo);
    await Scheduler.force(RefreshWorkerList);
    return worker.id;
  };

  public update: (workerId: Guid, workerInfo: WorkerInfoDto) => Promise<void> = async (
    workerId: Guid,
    workerInfo: WorkerInfoDto
  ) => {
    await ApiSingleton.WorkerApi.update(workerId, workerInfo);
    await Scheduler.force(RefreshWorkerList);
  };

  public updateComment: (workerId: Guid, comment: string) => Promise<void> = async (
    workerId: Guid,
    comment: string
  ) => {
    await ApiSingleton.WorkerApi.updateComment(workerId, comment);
    await Scheduler.force(RefreshWorkerList);
  };

  public delete: (workerId: Guid) => Promise<void> = async (workerId: Guid) => {
    await ApiSingleton.WorkerApi.delete(workerId);
    await Scheduler.force(RefreshWorkerList);
  };

  public setWorkers(version: number, workers: WorkerDto[], positionsMap?: string[]): WorkersActions {
    return { type: WorkersActionTypes.SetWorkers, version, workers, positionsMap };
  }

  private refreshList: (dispatch: any, getState: () => RootState) => Promise<void> = async (
    dispatch: any,
    getState: () => RootState
  ) => {
    const result = await ApiSingleton.WorkerApi.readAll(getState().workers.version);
    if (!!result.workers) {
      const sortedWorkers = PersonsHelper.sortWorkers(result.workers);
      const positionsMap = PersonsHelper.getPositionsMap(sortedWorkers);
      dispatch(this.setWorkers(result.version, sortedWorkers, positionsMap));
    }
  };
}

export const WorkersActionCreator = new WorkersActionCreatorClass();
