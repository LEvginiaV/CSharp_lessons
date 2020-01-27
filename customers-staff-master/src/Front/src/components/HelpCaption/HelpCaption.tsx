import { HelpLite } from "@skbkontur/react-icons";
import Tooltip, { TooltipProps } from "@skbkontur/react-ui/components/Tooltip/Tooltip";
import { PopupPosition } from "@skbkontur/react-ui/Popup";
import * as React from "react";
import { Caption, ICaptionProps } from "../../commonComponents/Caption/Caption";

interface HelpCaptionProps extends ICaptionProps {
  tooltipMessage: string | React.ReactNode;
  tooltipPosition: PopupPosition;
  iconColor?: string;
}

export class HelpCaption extends React.Component<HelpCaptionProps> {
  public render(): JSX.Element {
    const { iconColor, children } = this.props;

    return (
      <Caption {...this.props}>
        <span>{children}</span>
        &nbsp;
        <Tooltip data-tid="HelpTooltip" {...this.bindTooltip()}>
          <span style={{ cursor: "pointer" }}>
            <HelpLite color={iconColor || "#3072C4"} />
          </span>
        </Tooltip>
      </Caption>
    );
  }

  private bindTooltip(): TooltipProps {
    const { tooltipMessage, tooltipPosition } = this.props;

    return {
      trigger: "hover",
      pos: tooltipPosition,
      allowedPositions: [tooltipPosition],
      disableAnimations: false,
      useWrapper: true,
      render: () => tooltipMessage,
    };
  }
}
