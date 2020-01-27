import { HelpBook } from "@skbkontur/react-icons";
import Hint from "@skbkontur/react-ui/Hint";
import Link from "@skbkontur/react-ui/Link";
import * as React from "react";

export interface IHelpLinkProps {
  caption: string;
  hintText: string;
  onClick: () => void;
}

export class HelpLink extends React.Component<IHelpLinkProps, { hovered: boolean }> {
  constructor(props: IHelpLinkProps, state: {}) {
    super(props, state);

    this.state = {
      hovered: false,
    };
  }

  public render(): React.ReactNode {
    const { caption, hintText, onClick } = this.props;

    return (
      <Link
        onClick={onClick}
        onMouseEnter={() => this.setState({ hovered: true })}
        onMouseLeave={() => this.setState({ hovered: false })}
      >
        <Hint text={hintText} pos="top" opened={this.state.hovered} manual>
          <HelpBook />
        </Hint>
        {" " + caption}
      </Link>
    );
  }
}
