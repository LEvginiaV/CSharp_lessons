import { ValidationContainer, ValidationWrapperV1 } from "@skbkontur/react-ui-validations";
import DatePicker from "@skbkontur/react-ui/DatePicker";
import Input from "@skbkontur/react-ui/Input";
import * as React from "react";
import { DateTime } from "../../../api/new/dto/DateTime";
import { Guid } from "../../../api/new/dto/Guid";
import { WorkerDto } from "../../../api/new/dto/WorkerDto";
import { formatDatePickerDateToIso, formatIsoDateToDatePicker } from "../../../common/DateHelper";
import { Caption } from "../../../commonComponents/Caption/Caption";
import { Grid } from "../../../commonComponents/Grid/Grid";
import { Line } from "../../../commonComponents/Line/Line";
import { PhoneInput } from "../../../commonComponents/PhoneInput/PhoneInput";
import { StringHelper } from "../../../helpers/StringHelper";
import { WorkOrderValidationHelper } from "../../../validation/WorkOrderValidationHelper";
import { WorkerSelectorDropdown } from "../WorkerSelectorDropdown/WorkerSelectorDropdown";
import * as styles from "./WorkOrderInfoBlock.less";

export interface IWorkOrderInfoBlockProps {
  phone?: string;
  workers: WorkerDto[];
  receptionWorkerId?: Guid;
  warrantyNumber?: string;
  completionDatePlanned?: DateTime;
  completionDateFact?: DateTime;
  receptionDate?: DateTime;
}

export class WorkOrderInfoBlock extends React.Component<
  IWorkOrderInfoBlockProps,
  {
    localPhone?: string;
    localReceptionWorkerId?: Guid;
    localWarrantyNumber?: string;
    localCompletionDatePlanned?: string;
    localCompletionDateFact?: string;
  }
> {
  private ValidationContainer: ValidationContainer | null;

  constructor(props: IWorkOrderInfoBlockProps, state: {}) {
    super(props, state);

    this.state = {
      localPhone: props.phone,
      localReceptionWorkerId: props.receptionWorkerId,
      localWarrantyNumber: props.warrantyNumber,
      localCompletionDatePlanned: formatIsoDateToDatePicker(props.completionDatePlanned),
      localCompletionDateFact: formatIsoDateToDatePicker(props.completionDateFact),
    };
  }

  public getPhone(): string | undefined {
    return this.state.localPhone;
  }

  public getReceptionWorkerId(): Guid | undefined {
    return this.state.localReceptionWorkerId;
  }

  public getWarrantyNumber(): string | undefined {
    return this.state.localWarrantyNumber && this.state.localWarrantyNumber.trim();
  }

  public getCompletionDatePlanned(): DateTime {
    const completionDatePlanned = formatDatePickerDateToIso(this.state.localCompletionDatePlanned);
    if (!completionDatePlanned) {
      throw new Error("completion date is null");
    }

    return completionDatePlanned;
  }

  public getCompletionDateFact(): DateTime | undefined {
    return formatDatePickerDateToIso(this.state.localCompletionDateFact);
  }

  public async isValid(): Promise<boolean> {
    return !!this.ValidationContainer && (await this.ValidationContainer.validate());
  }

  public render(): JSX.Element {
    const { workers, receptionDate } = this.props;
    const {
      localPhone,
      localReceptionWorkerId,
      localWarrantyNumber,
      localCompletionDateFact,
      localCompletionDatePlanned,
    } = this.state;

    return (
      <div className={styles.root}>
        <ValidationContainer ref={el => (this.ValidationContainer = el)}>
          <WrapLine>
            <Caption>Телефон обратной связи</Caption>
            <ValidationWrapperV1 {...WorkOrderValidationHelper.bindPhoneValidation(localPhone)}>
              <PhoneInput
                value={StringHelper.formatPhoneToInput(localPhone)}
                width={149}
                onChange={(_, v) => this.setState({ localPhone: StringHelper.formatPhone(v) })}
                data-tid="Phone"
              />
            </ValidationWrapperV1>
          </WrapLine>
          {workers && !!workers.length && (
            <WrapLine>
              <Caption>ФИО приемщика</Caption>
              <WorkerSelectorDropdown
                onChange={workerId => this.setState({ localReceptionWorkerId: workerId })}
                width={315}
                selectedWorkerId={localReceptionWorkerId}
                workers={workers}
                caption={""}
                data-tid="Worker"
              />
            </WrapLine>
          )}
          <WrapLine>
            <Caption>№ гарантийного талона</Caption>
            <Input
              value={localWarrantyNumber || ""}
              maxLength={20}
              width={149}
              onChange={(_, v) => this.setState({ localWarrantyNumber: v })}
              data-tid="WarrantyNumber"
            />
          </WrapLine>
          <WrapLine marginBottom={1}>
            <Caption>Дата выполнения заказа</Caption>
            <div className={styles.dates}>
              <div>
                <ValidationWrapperV1
                  {...WorkOrderValidationHelper.bindCompletionDateValidation(
                    localCompletionDatePlanned,
                    receptionDate,
                    "CompletionDatePlannedValidation"
                  )}
                >
                  <DatePicker
                    value={localCompletionDatePlanned || null}
                    onChange={(_, v) => this.setState({ localCompletionDatePlanned: v })}
                    data-tid="CompletionDatePlanned"
                  />
                </ValidationWrapperV1>
                <div className={styles.tip}>План</div>
              </div>
              <div>
                <ValidationWrapperV1
                  {...WorkOrderValidationHelper.bindCompletionDateValidation(
                    localCompletionDateFact,
                    receptionDate,
                    "CompletionDateFactValidation",
                    false
                  )}
                >
                  <DatePicker
                    value={localCompletionDateFact || null}
                    onChange={(_, v) => this.setState({ localCompletionDateFact: v })}
                    data-tid="CompletionDateFact"
                  />
                </ValidationWrapperV1>
                <div className={styles.tip}>Факт</div>
              </div>
            </div>
          </WrapLine>
        </ValidationContainer>
      </div>
    );
  }
}

const WrapLine: React.SFC<{ marginBottom?: number }> = props => {
  return (
    <Line marginBottom={props.marginBottom || 18}>
      <Grid columns={[225]} alignItems={"baseline"}>
        {props.children}
      </Grid>
    </Line>
  );
};
