import { ValidationWrapperV1 } from "@skbkontur/react-ui-validations";
import Textarea from "@skbkontur/react-ui/Textarea";
import * as React from "react";
import { VehicleCustomerValueDto } from "../../../api/new/dto/WorkOrder/VehicleCustomerValueDto";
import { PredicateInput } from "../../../commonComponents/PredicateInput/PredicateInput";
import { StringHelper } from "../../../helpers/StringHelper";
import { WorkOrderValidationHelper } from "../../../validation/WorkOrderValidationHelper";
import * as styles from "./VehicleValue.less";

export interface IVehicleValueProps {
  value: Partial<VehicleCustomerValueDto>;
  onChange: (vehicle: Partial<VehicleCustomerValueDto>) => void;
}

export class VehicleValue extends React.Component<IVehicleValueProps> {
  constructor(props: IVehicleValueProps, state: {}) {
    super(props, state);
  }

  public render(): React.ReactNode {
    const localVehicle = this.props.value;

    return (
      <div className={styles.root} data-tid="VehicleValue">
        <div className={styles.leftBlock}>
          <WrapLine caption="Марка">{this.renderInput("brand", 40, undefined, false, 402)}</WrapLine>
          <WrapLine caption="Модель">{this.renderInput("model", 100, undefined, false, 402)}</WrapLine>
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
          <WrapLine caption="Гос. номер">
            <ValidationWrapperV1
              {...WorkOrderValidationHelper.bindStringNonEmptyValidation(
                localVehicle.registerSign,
                "RegisterSignValidation"
              )}
            >
              {this.renderInput("registerSign", 15, /^[0-9A-ZАВЕКМНОРСТУХ]*$/, true)}
            </ValidationWrapperV1>
          </WrapLine>
          <WrapLine caption="VIN">
            <ValidationWrapperV1
              {...WorkOrderValidationHelper.bindStringLengthValidation(localVehicle.vin, 17, "VinValidation")}
            >
              {this.renderInput("vin", 17, /^[0-9A-Z]*$/)}
            </ValidationWrapperV1>
          </WrapLine>
          <WrapLine caption="Номер кузова">{this.renderInput("bodyNumber", 17, /^[0-9A-Z]*$/)}</WrapLine>
          <WrapLine caption="Номер двигателя">{this.renderInput("engineNumber", 17, /^[0-9A-Z]*$/)}</WrapLine>
          <WrapLine caption="Год выпуска">
            <ValidationWrapperV1 {...WorkOrderValidationHelper.bindYearValidation(localVehicle.year)}>
              {this.renderInput("year", 4, /^[0-9]*$/, false, 55)}
            </ValidationWrapperV1>
          </WrapLine>
        </div>
      </div>
    );
  }

  private renderInput(
    key: keyof VehicleCustomerValueDto,
    maxLength?: number,
    regex?: RegExp,
    upperCase: boolean = false,
    width: number = 150
  ): JSX.Element {
    const localVehicle = this.props.value;
    // @ts-ignore
    const value = localVehicle[key] ? localVehicle[key].toString() : undefined;

    return (
      <PredicateInput
        predicate={v => (regex ? regex.test(v.toUpperCase()) : true)}
        postProcess={upperCase ? v => v.toUpperCase() : undefined}
        width={width}
        maxLength={maxLength}
        onChange={(_, v) => this.onChange(key, v)}
        value={value || ""}
        data-tid={StringHelper.capitalizeFirstLetter(key)}
      />
    );
  }

  private onChange = (key: keyof VehicleCustomerValueDto, value: string) => {
    const localVehicle = this.props.value;
    localVehicle[key] = value;
    this.props.onChange && this.props.onChange(localVehicle);
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
