import Dropdown, { DropdownProps } from "@skbkontur/react-ui/Dropdown";
import MenuItem from "@skbkontur/react-ui/MenuItem";
import * as React from "react";
import { Guid } from "../../../api/new/dto/Guid";
import { WorkerDto } from "../../../api/new/dto/WorkerDto";
import * as styles from "./WorkerSelectorDropdown.less";

export interface IWorkerSelectorDropdownProps extends DropdownProps {
  workers: WorkerDto[];
  selectedWorkerId?: Guid;
  onChange: (workerId: Guid) => void;
}

export class WorkerSelectorDropdown extends React.Component<IWorkerSelectorDropdownProps> {
  public render(): JSX.Element {
    const { width } = this.props;

    const maxWidth = width && +width ? +width - 36 : undefined;

    return (
      <Dropdown {...this.props} caption={this.renderCaption()}>
        {this.props.workers.map(w => (
          <MenuItem key={w.id} onClick={() => this.props.onChange(w.id)}>
            <div className={styles.workerName} style={{ maxWidth }}>
              {w.fullName}
            </div>
          </MenuItem>
        ))}
      </Dropdown>
    );
  }

  private renderCaption(): JSX.Element {
    const { workers, selectedWorkerId } = this.props;

    const worker = workers.find(x => x.id === selectedWorkerId);

    if (!worker) {
      return <span style={{ color: "#A0A0A0" }}>Выберите сотрудника</span>;
    }

    return <span>{worker.fullName}</span>;
  }
}
