import { ValidationContainer, ValidationWrapperV1 } from "@skbkontur/react-ui-validations";
import { ComboBox } from "@skbkontur/react-ui/components";
import Textarea from "@skbkontur/react-ui/components/Textarea/Textarea";
import * as each from "lodash/each";
import * as React from "react";
import { WorkerDto } from "../../../api/new/dto/WorkerDto";
import { Caption } from "../../../commonComponents/Caption/Caption";
import { Grid } from "../../../commonComponents/Grid/Grid";
import { Line } from "../../../commonComponents/Line/Line";
import { NameInput } from "../../../commonComponents/NameInput/NameInput";
import { PhoneInput } from "../../../commonComponents/PhoneInput/PhoneInput";
import { StringHelper } from "../../../helpers/StringHelper";
import { ValidationHelper } from "../../../validation/ValidationHelper";
import { PersonEditor } from "../../PersonEditor/PersonEditor";

interface IWorkerEditorProps {
  heading: string;
  acceptButtonCaption?: string;
  form?: Partial<WorkerDto>;
  onSave?: (worker: Partial<WorkerDto>) => void;
  onClose?: () => void;
  positionsMap?: string[];
  disableActionButtons: boolean;
}

interface IWorkerEditorState extends Partial<WorkerDto> {
  position: string;
}

export class WorkerEditor extends React.Component<IWorkerEditorProps, IWorkerEditorState> {
  private ValidationContainer: ValidationContainer | null;
  private NameInput: NameInput | null;

  constructor(props: IWorkerEditorProps, state: IWorkerEditorState) {
    super(props, state);
    this.state = { position: "" };
    this.NameInput = null;
  }

  public componentDidMount() {
    this.initForm();
    this.NameInput && this.NameInput.focus();
  }

  public render(): JSX.Element {
    const { fullName, phone, additionalInfo, position } = this.state;
    const inputProps = { width: "100%" };

    return (
      <PersonEditor
        data-tid="WorkerEditor"
        acceptButtonCaption={this.props.acceptButtonCaption || "Сохранить"}
        heading={this.props.heading}
        onSave={this.onSave}
        onClose={() => this.props.onClose && this.props.onClose()}
        disableActionButtons={this.props.disableActionButtons}
      >
        <ValidationContainer ref={el => (this.ValidationContainer = el)}>
          <WrapLine>
            <Caption required>Имя</Caption>
            <ValidationWrapperV1 {...ValidationHelper.bindRequiredField(fullName)}>
              <NameInput
                data-tid="Name"
                {...inputProps}
                maxLength={120}
                placeholder="Как зовут сотрудника"
                value={fullName || ""}
                onChange={(_, v) => this.setState({ fullName: v })}
                ref={el => (this.NameInput = el)}
              />
            </ValidationWrapperV1>
          </WrapLine>
          <WrapLine>
            <Caption>Должность</Caption>
            <ComboBox<string>
              {...inputProps}
              maxLength={120}
              maxMenuHeight={155}
              data-tid="Position"
              getItems={this.getPositionItems}
              onChange={(_, p: string) => {
                this.setState({ position: p });
              }}
              onUnexpectedInput={(p: string) => {
                this.setState({ position: p });
              }}
              renderItem={p => p}
              renderValue={p => p}
              value={position}
              valueToString={p => p}
            />
          </WrapLine>
          <WrapLine marginBottom={52}>
            <Caption>Телефон</Caption>
            <ValidationWrapperV1 {...ValidationHelper.bindSimplePhoneValidation(phone)}>
              <PhoneInput
                data-tid="Phone"
                width={160}
                value={phone || ""}
                onChange={(_, v) => this.setState({ phone: v })}
              />
            </ValidationWrapperV1>
          </WrapLine>
          <Line marginTop={32}>
            <Grid columns={[105, 350]} alignItems="flex-start">
              <Caption usePadding>Комментарий</Caption>
              <Textarea
                data-tid="Description"
                autoResize
                maxLength={500}
                maxRows={10}
                rows={3}
                selectAllOnFocus={false}
                {...inputProps}
                value={additionalInfo || ""}
                onChange={(_, v) => this.setState({ additionalInfo: v })}
              />
            </Grid>
          </Line>
        </ValidationContainer>
      </PersonEditor>
    );
  }

  private onSave = async () => {
    const isValid = this.ValidationContainer && (await this.ValidationContainer.validate());
    if (isValid && this.props.onSave) {
      await this.props.onSave(this.prepareDataToSave());
    }
  };

  private initForm() {
    const { form } = this.props;
    const state: IWorkerEditorState = { position: "" };

    form &&
      each(form, (value: any, key: keyof WorkerDto) => {
        if (key === "phone") {
          value = StringHelper.formatPhoneToInput(value);
        }

        if (value) {
          state[key] = value;
        }
      });

    this.setState({ ...state });
  }

  private prepareDataToSave(): Partial<WorkerDto> {
    const copy = { ...this.state } as Partial<WorkerDto>;

    if (copy.fullName) {
      copy.fullName = StringHelper.removeMoreWhitespaces(copy.fullName);
    }

    if (copy.position) {
      copy.position = StringHelper.removeMoreWhitespaces(copy.position);
    }

    if (copy.additionalInfo) {
      copy.additionalInfo = StringHelper.removeMoreWhitespaces(copy.additionalInfo);
    }

    if (copy.phone) {
      copy.phone = StringHelper.formatPhone(copy.phone);
    }

    return copy;
  }

  private getPositionItems = async (query: string) => {
    if (!this.props.positionsMap) {
      return [query];
    }
    const filtered = this.props.positionsMap.filter(x => x.toLowerCase().includes(query.toLowerCase()));

    return query !== "" ? [query, ...filtered] : filtered;
  };
}

const WrapLine: React.SFC<{ marginBottom?: number }> = props => {
  return (
    <Line marginBottom={props.marginBottom || 20}>
      <Grid columns={[105, 350]}>{props.children}</Grid>
    </Line>
  );
};
