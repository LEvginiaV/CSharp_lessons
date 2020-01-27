import { WorkersListTabType } from "../../models/WorkersListTabType";
import { FeatureAppearance } from "../FeatureAppearance";
import { localStorageWrapper } from "./LocalStorage";

export function setLastWorkerListTabTypeToLocalStorage(type: WorkersListTabType) {
  const userInfo = FeatureAppearance.getUserInfo();
  localStorageWrapper.set(`${userInfo.userId}_${userInfo.shopId}_lastWorkerListTabType`, type);
}

export function getLastWorkerListTabTypeToLocalStorage(): WorkersListTabType | null {
  const userInfo = FeatureAppearance.getUserInfo();
  return localStorageWrapper.get(`${userInfo.userId}_${userInfo.shopId}_lastWorkerListTabType`);
}
