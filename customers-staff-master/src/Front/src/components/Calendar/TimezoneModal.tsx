import Button from "@skbkontur/react-ui/components/Button/Button";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import Modal from "@skbkontur/react-ui/components/Modal/Modal";
import Select from "@skbkontur/react-ui/components/Select/Select";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import * as React from "react";
import { ApiSingleton } from "../../api/new/Api";
import { TimeZoneDto } from "../../api/new/dto/TimeZoneDto";
import { TimeSpanHelper } from "../../common/TimeSpanHelper";

interface Props {
  value: Nullable<TimeZoneDto>;
  onChange: (timezone: Nullable<TimeZoneDto>) => void;
  onClose: () => void;
}

interface State {
  list: TimeZoneDto[] | null;
  value: Nullable<TimeZoneDto>;
}

export class TimezoneModal extends React.Component<Props, State> {
  public state: Readonly<State> = {
    list: null,
    value: this.props.value,
  };

  public componentDidMount() {
    this.updateTimezone();
  }

  public render() {
    return (
      <Modal onClose={this.props.onClose} width={500} data-tid="TimeZoneModal">
        <Modal.Header>Настройка времени в календаре</Modal.Header>

        <Modal.Body>
          <Loader active={this.state.list == null}>
            <Gapped vertical>
              {this.state.list == null && <span data-tid="TimeZoneLoader" />}
              <div>Часовой пояс</div>
              <Select<Nullable<TimeZoneDto>>
                data-tid="TimeZoneSelect"
                width="100%"
                items={this.state.list || []}
                value={this.state.value}
                onChange={(_, value: TimeZoneDto) => this.setState({ value })}
                renderItem={this.renderItem}
                renderValue={this.renderItem}
              />
            </Gapped>
          </Loader>
        </Modal.Body>
        <Modal.Footer panel>
          <Gapped>
            <Button use="primary" disabled={this.state.value == null} onClick={this.handleSave} data-tid="Accept">
              Сохранить
            </Button>
          </Gapped>
        </Modal.Footer>
      </Modal>
    );
  }

  private renderItem = (x: TimeZoneDto) => {
    return TimeSpanHelper.toTimezoneText(x.offset) + " " + x.name;
  };

  private handleSave = async () => {
    const { value } = this.state;
    if (!value) {
      return;
    }
    try {
      await ApiSingleton.TimeZoneApi.set(value.id);
      this.props.onChange(value);
    } catch (e) {
      Toast.push("Не удалось сохранить часовой пояс");
      console.error(e);
      this.props.onClose();
    }
  };

  private async updateTimezone() {
    try {
      const list = await ApiSingleton.TimeZoneApi.getList();
      this.setState({ list });
    } catch (e) {
      Toast.push("Не удалось загрузить список часовых поясов");
      console.error(e);
      this.props.onClose();
    }
  }
}
