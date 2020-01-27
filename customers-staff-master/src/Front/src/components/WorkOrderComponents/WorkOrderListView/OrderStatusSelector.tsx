import Select, { SelectProps } from "@skbkontur/react-ui/Select";
import * as React from "react";
import { WorkOrderStatusDto } from "../../../api/new/dto/WorkOrder/WorkOrderStatusDto";
import { StringHelper } from "../../../helpers/StringHelper";
import * as styles from "./OrderStatusSelector.less";

export class OrderStatusSelector extends React.Component<
  SelectProps<WorkOrderStatusDto, WorkOrderStatusDto>,
  { mouseOver: boolean; opened: boolean }
> {
  constructor(props: SelectProps<WorkOrderStatusDto, WorkOrderStatusDto>, state: {}) {
    super(props, state);

    this.state = {
      mouseOver: false,
      opened: false,
    };
  }

  public render(): React.ReactNode {
    const { mouseOver, opened } = this.state;
    const { value } = this.props;
    const color = this.getColor(value);
    return (
      <div
        className={styles.root}
        onMouseEnter={() => this.setState({ mouseOver: true })}
        onMouseLeave={() => this.setState({ mouseOver: false })}
        onClick={e => e.stopPropagation()}
      >
        <div className={styles.select}>
          <Select<WorkOrderStatusDto, WorkOrderStatusDto>
            {...this.props}
            renderItem={StringHelper.formatOrderStatus}
            renderValue={StringHelper.formatOrderStatus}
            items={[
              WorkOrderStatusDto.New,
              WorkOrderStatusDto.InProgress,
              WorkOrderStatusDto.Completed,
              WorkOrderStatusDto.IssuedToClient,
            ]}
            onOpen={() => this.setState({ opened: true })}
            onClose={() => this.setState({ opened: false })}
            data-tid="Selector"
          />
        </div>
        {!mouseOver && !opened && color && value && (
          <div className={styles.overlay} style={{ background: color }} data-tid="Overlay">
            {StringHelper.formatOrderStatus(value)}
          </div>
        )}
      </div>
    );
  }

  private getColor(status?: WorkOrderStatusDto): string | undefined {
    switch (status) {
      case WorkOrderStatusDto.InProgress:
        return "#E4F3FF";
      case WorkOrderStatusDto.Completed:
        return "#E2F7DC";
      case WorkOrderStatusDto.IssuedToClient:
        return "#F2F2F2";
    }

    return undefined;
  }
}
