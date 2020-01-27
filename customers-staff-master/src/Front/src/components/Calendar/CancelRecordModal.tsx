import Button from "@skbkontur/react-ui/components/Button/Button";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Modal from "@skbkontur/react-ui/components/Modal/Modal";
import Radio from "@skbkontur/react-ui/components/Radio/Radio";
import * as React from "react";
import { Guid } from "../../api/new/dto/Guid";
import { CustomerStatusDto } from "../../api/new/dto/ServiceCalendar/ServiceCalendarRecordInfoDto";
import { Metrics } from "../../common/MetricsFacade";
import * as styles from "./CancelRecordModal.less";

interface Props {
  recordId: Guid;
  onCancel: (status: CustomerStatusDto) => void;
  onClose: () => void;
}

interface State {
  error: boolean;
  status: Nullable<CustomerStatusDto>;
}

export class CancelRecordModal extends React.Component<Props, State> {
  public state: Readonly<State> = {
    error: false,
    status: null,
  };

  public render() {
    return (
      <Modal onClose={this.props.onClose} width={400} data-tid="CalendarRecordCancelModal">
        <Modal.Header>Отмена записи</Modal.Header>
        <Modal.Body>
          <Gapped vertical>
            {this.renderRadio(CustomerStatusDto.CanceledBeforeEvent, "Клиент отменил")}
            {this.renderRadio(CustomerStatusDto.NotCome, "Клиент не пришел в назначенное время")}
            {this.renderRadio(CustomerStatusDto.NoService, "Не сможем принять клиента")}
            {this.renderRadio(CustomerStatusDto.Mistake, "Запись сделали по ошибке")}
            {this.state.error && (
              <div className={styles.error} data-tid="ErrorMessage">
                Укажите причину отмены
              </div>
            )}
          </Gapped>
        </Modal.Body>
        <Modal.Footer panel>
          <Gapped>
            <Button use="danger" onClick={this.handleCancel} data-tid="CancelRecord">
              Отменить запись
            </Button>
            <Button onClick={this.props.onClose} data-tid="DoNotCancelRecord">
              Не отменять
            </Button>
          </Gapped>
        </Modal.Footer>
      </Modal>
    );
  }

  private handleCancel = () => {
    if (this.state.status == null) {
      Metrics.calendarDeleteFail({ bookingid: this.props.recordId });
      this.setState({ error: true });
      return;
    }
    this.props.onCancel(this.state.status);
  };

  private renderRadio(status: CustomerStatusDto, label: string) {
    return (
      <Radio
        key={status}
        error={this.state.error}
        value={status}
        checked={this.state.status === status}
        onChange={() => this.setState({ status, error: false })}
      >
        <span data-tid={status.substr(0, 1).toUpperCase() + status.substr(1)}>{label}</span>
      </Radio>
    );
  }
}
