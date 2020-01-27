import { ValidationWrapperV1 } from "@skbkontur/react-ui-validations";
import Textarea from "@skbkontur/react-ui/Textarea";
import * as React from "react";
import { ApplianceCustomerValueDto } from "../../../api/new/dto/WorkOrder/ApplianceCustomerValueDto";
import { PredicateInput } from "../../../commonComponents/PredicateInput/PredicateInput";
import { StringHelper } from "../../../helpers/StringHelper";
import { WorkOrderValidationHelper } from "../../../validation/WorkOrderValidationHelper";
import * as styles from "./ApplianceValue.less";

export interface IApplianceValueProps {
  value: Partial<ApplianceCustomerValueDto>;
  onChange: (vehicle: Partial<ApplianceCustomerValueDto>) => void;
}

export class ApplianceValue extends React.Component<IApplianceValueProps> {
  constructor(props: IApplianceValueProps, state: {}) {
    super(props, state);
  }

  public render(): React.ReactNode {
    const localVehicle = this.props.value;

    return (
      <div className={styles.root} data-tid="ApplianceValue">
        <div className={styles.leftBlock}>
          <WrapLine caption="Наименование">
            <ValidationWrapperV1
              {...WorkOrderValidationHelper.bindStringNonEmptyValidation(localVehicle.name, "NameValidation")}
            >
              {this.renderInput("name", 100, 402)}
            </ValidationWrapperV1>
          </WrapLine>
          <WrapLine caption="Номер">{this.renderInput("number", 100, 402)}</WrapLine>
          <div className={styles.commentCaption}>Описание дефектов / Комментарий</div>
          <div className={styles.comment}>
            <Textarea
              placeholder="Заявленный дефект, внешний вид, комплектность, прочие комментарии"
              width={522}
              maxLength={500}
              resize={"none"}
              maxRows={0}
              rows={4}
              value={this.props.value.additionalInfo}
              onChange={(_, v) => this.onChange("additionalInfo", v)}
              data-tid="Comment"
            />
          </div>
        </div>
        <div className={styles.rightBlock}>
          <WrapLine caption="Марка">{this.renderInput("brand", 40)}</WrapLine>
          <WrapLine caption="Модель">{this.renderInput("model", 100)}</WrapLine>
          <WrapLine caption="Год выпуска">
            <ValidationWrapperV1 {...WorkOrderValidationHelper.bindYearValidation(localVehicle.year)}>
              {this.renderInput("year", 4, 55, /^[0-9]*$/)}
            </ValidationWrapperV1>
          </WrapLine>
        </div>
      </div>
    );
  }

  private renderInput(
    key: keyof ApplianceCustomerValueDto,
    maxLength?: number,
    width: number = 150,
    regex?: RegExp
  ): JSX.Element {
    const localVehicle = this.props.value;
    // @ts-ignore
    const value = localVehicle[key] ? localVehicle[key].toString() : undefined;

    return (
      <PredicateInput
        predicate={v => (regex ? regex.test(v) : true)}
        width={width}
        maxLength={maxLength}
        onChange={(_, v) => this.onChange(key, v)}
        value={value || ""}
        data-tid={StringHelper.capitalizeFirstLetter(key)}
      />
    );
  }

  private onChange = (key: keyof ApplianceCustomerValueDto, value: string) => {
    const localAppliance = this.props.value;
    localAppliance[key] = value;
    this.props.onChange && this.props.onChange(localAppliance);
  };
}

const WrapLine: React.SFC<{ caption: string }> = props => {
  return (
    <div className={styles.line}>
      <div className={styles.caption}>{props.caption}</div>
      <div className={styles.field}>{props.children}</div>
    </div>
  );
};
