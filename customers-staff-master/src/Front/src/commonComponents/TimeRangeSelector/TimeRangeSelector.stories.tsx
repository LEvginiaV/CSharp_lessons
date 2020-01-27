import { ValidationContainer } from "@skbkontur/react-ui-validations";
import Input from "@skbkontur/react-ui/Input";
import { storiesOf } from "@storybook/react";
import * as React from "react";
import { TimeRangeSelector } from "./TimeRangeSelector";
import { TimeRangeSelectorValidationHelper } from "./TimeRangeSelectorValidationHelper";

storiesOf("TimeRangeSelector", module)
  .add("InputWithMaskBug", () => {
    return <Input mask={"99:99"} value={"23:00"} />;
  })
  .add("Simple", () => {
    return <TimeRangeWrapper />;
  });

interface State {
  start: string;
  end: string;
}

class TimeRangeWrapper extends React.Component<{}, State> {
  constructor(props: Readonly<{}>) {
    super(props);
    this.state = {
      start: "11:00",
      end: "18:00",
    };
  }

  public render() {
    const { start, end } = this.state;
    return (
      <div style={{ width: 200, height: 200, padding: 100 }}>
        <ValidationContainer>
          <TimeRangeSelector
            startTime={this.state.start}
            endTime={this.state.end}
            autocompleteProviderStart={time => Promise.resolve(time ? [time] : [])}
            autocompleteProviderEnd={time => Promise.resolve(time ? [time] : [])}
            onChange={(startTime, endTime) => this.setState({ start: startTime, end: endTime })}
            startValidationFunc={() => TimeRangeSelectorValidationHelper.bindStartTimeValidation(start, end)}
            endValidationFunc={() => TimeRangeSelectorValidationHelper.bindEndTimeValidation(start, end)}
          />
        </ValidationContainer>
      </div>
    );
  }
}
