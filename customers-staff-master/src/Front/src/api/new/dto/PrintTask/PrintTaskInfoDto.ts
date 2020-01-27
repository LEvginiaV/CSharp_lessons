import { PrintTaskStatusDto } from "./PrintTaskStatusDto";

export interface PrintTaskInfoDto {
  id: string;
  errorMessage: string;
  status: PrintTaskStatusDto;
}
