import { RenderErrorMessage, TooltipPosition, ValidationTooltip } from "@skbkontur/react-ui-validations";
import * as React from "react";

export function tooltip(pos: TooltipPosition, dataTid?: string): RenderErrorMessage {
  return (control, hasError, validation) => {
    return (
      <ValidationTooltip
        data-tid={dataTid}
        pos={pos}
        error={hasError}
        render={() => {
          return <span data-tid="ErrorMessage"> {(validation && validation.message) || ""}</span>;
        }}
      >
        {control}
      </ValidationTooltip>
    );
  };
}
