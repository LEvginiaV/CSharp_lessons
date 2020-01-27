import Textarea from "@skbkontur/react-ui/Textarea";
import * as React from "react";
import { OtherCustomerValueDto } from "../../../api/new/dto/WorkOrder/OtherCustomerValueDto";
import * as styles from "./OtherValue.less";

export interface IOtherValueProps {
  value: Partial<OtherCustomerValueDto>;
  onChange: (vehicle: Partial<OtherCustomerValueDto>) => void;
}

export class OtherValue extends React.Component<IOtherValueProps> {
  constructor(props: IOtherValueProps, state: {}) {
    super(props, state);
  }

  public render(): React.ReactNode {
    const additionalInfo = this.props.value.additionalInfo;

    return (
      <div className={styles.root} data-tid="OtherValue">
        <div className={styles.commentCaption}>Описание принятой ценности</div>
        <div className={styles.comment}>
          <Textarea
            placeholder="Заявленный дефект, внешний вид, комплектность, прочие комментарии"
            width={522}
            maxLength={500}
            resize={"none"}
            maxRows={0}
            rows={4}
            value={additionalInfo}
            onChange={(_, v) => this.onChange("additionalInfo", v)}
            data-tid="Comment"
          />
        </div>
      </div>
    );
  }

  private onChange = (key: keyof OtherCustomerValueDto, value: string) => {
    const localVehicle = this.props.value;
    localVehicle[key] = value;
    this.props.onChange && this.props.onChange(localVehicle);
  };
}
