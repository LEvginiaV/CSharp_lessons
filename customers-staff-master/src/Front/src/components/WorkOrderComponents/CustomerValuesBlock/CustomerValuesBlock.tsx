import { ValidationContainer } from "@skbkontur/react-ui-validations";
import Select from "@skbkontur/react-ui/Select";
import * as React from "react";
import { ApplianceCustomerValueDto } from "../../../api/new/dto/WorkOrder/ApplianceCustomerValueDto";
import { CustomerValueListDto } from "../../../api/new/dto/WorkOrder/CustomerValueListDto";
import { CustomerValueTypeDto } from "../../../api/new/dto/WorkOrder/CustomerValueTypeDto";
import { OtherCustomerValueDto } from "../../../api/new/dto/WorkOrder/OtherCustomerValueDto";
import { VehicleCustomerValueDto } from "../../../api/new/dto/WorkOrder/VehicleCustomerValueDto";
import { StringHelper } from "../../../helpers/StringHelper";
import { SpoilerBlock } from "../SpoilerBlock/SpoilerBlock";
import { ApplianceValue } from "./ApplianceValue";
import * as styles from "./CustomerValuesBlock.less";
import { OtherValue } from "./OtherValue";
import { VehicleValue } from "./VehicleValue";

export interface ICustomerValuesBlockProps {
  valueList?: CustomerValueListDto;
  onHeightUpdated?: () => void;
}

export class CustomerValuesBlock extends React.Component<
  ICustomerValuesBlockProps,
  {
    localValueType: CustomerValueTypeDto;
    localVehicle: Partial<VehicleCustomerValueDto>;
    localAppliance: Partial<ApplianceCustomerValueDto>;
    localOther: Partial<OtherCustomerValueDto>;
  }
> {
  private ValidationContainer: ValidationContainer | null;
  private SpoilerBlock: SpoilerBlock | null;

  constructor(props: ICustomerValuesBlockProps, state: {}) {
    super(props, state);

    const localValueType = props.valueList ? props.valueList.customerValueType : CustomerValueTypeDto.Vehicle;
    const localVehicle =
      props.valueList && localValueType === CustomerValueTypeDto.Vehicle
        ? (props.valueList.customerValues[0] as VehicleCustomerValueDto) || {}
        : {};
    const localAppliance =
      props.valueList && localValueType === CustomerValueTypeDto.Appliances
        ? (props.valueList.customerValues[0] as ApplianceCustomerValueDto) || {}
        : {};
    const localOther =
      props.valueList && localValueType === CustomerValueTypeDto.Other
        ? (props.valueList.customerValues[0] as OtherCustomerValueDto) || {}
        : {};

    this.state = {
      localValueType,
      localVehicle,
      localAppliance,
      localOther,
    };
  }

  public getCustomerValues(): CustomerValueListDto {
    return {
      customerValueType: this.state.localValueType,
      customerValues: this.getValuesList(),
    };
  }

  public async isValid(): Promise<boolean> {
    if (this.isValidInternal()) {
      return true;
    }

    this.SpoilerBlock && this.SpoilerBlock.open();

    return !!this.ValidationContainer && (await this.ValidationContainer.validate());
  }

  public render(): React.ReactNode {
    const { onHeightUpdated } = this.props;

    return (
      <SpoilerBlock
        ref={el => (this.SpoilerBlock = el)}
        caption="Принятые от клиента ценности"
        captionElements={this.renderDropdown()}
        openChanged={() => {
          onHeightUpdated && onHeightUpdated();
        }}
        data-tid="CustomerValuesBlock"
      >
        <ValidationContainer ref={el => (this.ValidationContainer = el)}>{this.renderValue()}</ValidationContainer>
      </SpoilerBlock>
    );
  }

  private renderValue(): JSX.Element {
    switch (this.state.localValueType) {
      case CustomerValueTypeDto.Appliances:
        return (
          <ApplianceValue value={this.state.localAppliance} onChange={v => this.setState({ localAppliance: v })} />
        );
      case CustomerValueTypeDto.Vehicle:
        return <VehicleValue value={this.state.localVehicle} onChange={v => this.setState({ localVehicle: v })} />;
      case CustomerValueTypeDto.Other:
        return <OtherValue value={this.state.localOther} onChange={v => this.setState({ localOther: v })} />;
    }

    throw new Error("Unknown value type");
  }

  private renderDropdown(): JSX.Element {
    return (
      <div onClick={e => e.stopPropagation()} className={styles.typeSelector}>
        <Select<CustomerValueTypeDto>
          width={151}
          items={[CustomerValueTypeDto.Appliances, CustomerValueTypeDto.Vehicle, CustomerValueTypeDto.Other]}
          value={this.state.localValueType}
          renderItem={StringHelper.formatCustomerValueType}
          renderValue={StringHelper.formatCustomerValueType}
          onChange={(_, v) => this.setState({ localValueType: v })}
          data-tid="TypeSelector"
        />
      </div>
    );
  }

  private getValuesList(): ApplianceCustomerValueDto[] | VehicleCustomerValueDto[] | OtherCustomerValueDto[] {
    switch (this.state.localValueType) {
      case CustomerValueTypeDto.Vehicle:
        const localVehicle = this.state.localVehicle;
        return this.isModelEmpty(localVehicle)
          ? []
          : [
              {
                vin: localVehicle.vin || "",
                brand: localVehicle.brand ? localVehicle.brand.trim() : "",
                model: localVehicle.model ? localVehicle.model.trim() : "",
                registerSign: localVehicle.registerSign || "",
                year: localVehicle.year,
                bodyNumber: localVehicle.bodyNumber || "",
                engineNumber: localVehicle.engineNumber || "",
                additionalInfo: localVehicle.additionalInfo ? localVehicle.additionalInfo.trim() : "",
              },
            ];
      case CustomerValueTypeDto.Appliances:
        const localAppliance = this.state.localAppliance;
        return this.isModelEmpty(localAppliance)
          ? []
          : [
              {
                name: localAppliance.name ? localAppliance.name.trim() : "",
                brand: localAppliance.brand ? localAppliance.brand.trim() : "",
                model: localAppliance.model ? localAppliance.model.trim() : "",
                number: localAppliance.number ? localAppliance.number.trim() : "",
                year: localAppliance.year,
                additionalInfo: localAppliance.additionalInfo ? localAppliance.additionalInfo.trim() : "",
              },
            ];
      case CustomerValueTypeDto.Other:
        const localOther = this.state.localOther;
        return this.isModelEmpty(localOther)
          ? []
          : [{ additionalInfo: localOther.additionalInfo ? localOther.additionalInfo.trim() : "" }];
    }

    throw new Error("Unknown value type");
  }

  private isValidInternal(): boolean {
    const nowYear = new Date().getFullYear();
    switch (this.state.localValueType) {
      case CustomerValueTypeDto.Vehicle:
        const localVehicle = this.state.localVehicle;
        return (
          this.isModelEmpty(localVehicle) ||
          (!!localVehicle.vin &&
            localVehicle.vin.length === 17 &&
            !!localVehicle.registerSign &&
            (!localVehicle.year || (localVehicle.year >= 1900 && localVehicle.year <= nowYear)))
        );
      case CustomerValueTypeDto.Appliances:
        const localAppliance = this.state.localAppliance;
        return (
          this.isModelEmpty(localAppliance) ||
          (!!localAppliance.name &&
            (!localAppliance.year || (localAppliance.year >= 1900 && localAppliance.year <= nowYear)))
        );
      case CustomerValueTypeDto.Other:
        return true;
    }

    throw new Error("Unknown value type");
  }

  private isModelEmpty(model: any) {
    for (const key in model) {
      if (!!model[key]) {
        return false;
      }
    }

    return true;
  }
}
