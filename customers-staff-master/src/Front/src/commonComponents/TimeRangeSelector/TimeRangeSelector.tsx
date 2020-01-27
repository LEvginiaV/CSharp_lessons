import { ValidationWrapperV1 } from "@skbkontur/react-ui-validations";
import { Autocomplete, Gapped } from "@skbkontur/react-ui/components";
import * as React from "react";
import { AutocompleteValidationWrapper } from "./AutocompleteValidationWrapper";
import * as styles from "./TimeRangeSelector.less";
import { ValidationResult } from "./TimeRangeSelectorValidationHelper";

export interface ITimeRangeSelectorProps {
  startTime: string;
  endTime: string;
  autocompleteProviderStart: (time: string) => Promise<string[]>;
  autocompleteProviderEnd: (time: string) => Promise<string[]>;
  onChange: (start: string, end: string) => void;

  startValidationFunc: () => ValidationResult;
  endValidationFunc: () => ValidationResult;
}

export interface ITimeRangeSelectorState {
  ok: string;
}

export class TimeRangeSelector extends React.Component<ITimeRangeSelectorProps, ITimeRangeSelectorState> {
  public FirstAutocompleteValidationWrapper: AutocompleteValidationWrapper | null;
  constructor(props: ITimeRangeSelectorProps) {
    super(props);
  }

  public render(): JSX.Element {
    const { startTime, endTime, autocompleteProviderStart, autocompleteProviderEnd, onChange } = this.props;
    const { startValidationFunc, endValidationFunc } = this.props;

    const commonProps = {
      width: 60,
      mask: "99:99",
      disablePortal: false,
      hasShadow: true,
      preventWindowScroll: false,
      renderItem: (s: string) => <span>{s}</span>,
      alwaysShowMask: true,
    };

    return (
      <div className={styles.root}>
        <Gapped gap={8}>
          <ValidationWrapperV1 {...startValidationFunc()}>
            <AutocompleteValidationWrapper ref={el => (this.FirstAutocompleteValidationWrapper = el)}>
              <Autocomplete
                {...commonProps}
                data-tid="StartTime"
                value={startTime}
                onChange={(_, v) => onChange(v, this.props.endTime)}
                source={autocompleteProviderStart}
                menuMaxHeight={155}
                size={"small"}
                menuAlign={"left"}
                suffix={""}
                error={false}
              />
            </AutocompleteValidationWrapper>
          </ValidationWrapperV1>
          <span className={styles.dash}>—</span>
          <ValidationWrapperV1 {...endValidationFunc()}>
            <AutocompleteValidationWrapper>
              <Autocomplete
                {...commonProps}
                data-tid="EndTime"
                value={endTime}
                onChange={(_, v) => onChange(this.props.startTime, v)}
                source={autocompleteProviderEnd}
                menuMaxHeight={155}
                size={"small"}
                menuAlign={"left"}
              />
            </AutocompleteValidationWrapper>
          </ValidationWrapperV1>
        </Gapped>
      </div>
    );
  }
}
