import ComboBox from "@skbkontur/react-ui/components/ComboBox/ComboBox";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import * as cn from "classnames";
import memoizeOne from "memoize-one";
import * as React from "react";
import { CustomerDto } from "../../api/new/dto/CustomerDto";
import { WorkerDto } from "../../api/new/dto/WorkerDto";
import { Searcher, WorkersListSearcher } from "../../controls/Searcher";
import { CalendarModalErrorInfo } from "./CalendarHelper";
import * as styles from "./WorkerSelector.less";

interface Props {
  value: Nullable<WorkerDto>;
  editable: boolean;
  error: Nullable<CalendarModalErrorInfo>;
  workers: WorkerDto[];
  onChange: (worker: Nullable<WorkerDto>) => void;
}

interface State {
  focused: boolean;
}

export class WorkerSelector extends React.Component<Props, State> {
  public state: Readonly<State> = {
    focused: false,
  };

  private getSearcher: (data: CustomerDto[]) => Searcher<WorkerDto> = memoizeOne(WorkersListSearcher);

  public render() {
    const { value } = this.props;
    return (
      <div>
        {value == null && this.renderCombobox()}
        {value && this.renderView()}
      </div>
    );
  }

  private renderView() {
    const { value, editable } = this.props;
    if (!value) {
      return;
    }
    return (
      <div className={cn(styles.view, editable && styles.editable)}>
        <div className={styles.name} data-tid="WorkerNameLabel">
          {value.fullName}
        </div>
        <div className={styles.position}>{value.position}</div>
        {editable && (
          <div className={styles.kebabIcon}>
            <Kebab size="large" data-tid="WorkerKebab">
              <MenuItem onClick={this.handleClear}>Выбрать другого сотрудника</MenuItem>
            </Kebab>
          </div>
        )}
      </div>
    );
  }

  private renderCombobox() {
    return (
      <ComboBox
        data-tid="WorkerComboBox"
        autoFocus
        error={!!this.props.error}
        getItems={this.getItems}
        onChange={(_, item) => this.props.onChange(item)}
        onUnexpectedInput={this.handleClear}
        placeholder="Введите имя или должность"
        value={this.props.value}
        renderItem={this.renderComboboxItem}
        renderValue={item => item.fullName}
        valueToString={item => item.fullName || ""}
        width="100%"
        onFocus={() => this.setState({ focused: true })}
        onBlur={() => this.setState({ focused: false })}
      />
    );
  }

  private renderComboboxItem = (item: WorkerDto) => {
    return (
      <div className={styles.comboboxItem}>
        <div className={styles.name}>{item.fullName}</div>
        <div className={styles.position}>{item.position}</div>
      </div>
    );
  };

  private handleClear = () => {
    this.props.onChange(null);
  };

  private getItems = (query: string): Promise<WorkerDto[]> => {
    const abc = query ? this.getSearcher(this.props.workers).search(query) : this.props.workers;
    return Promise.resolve(abc.slice(0, 10));
  };
}
