import { Spinner } from "@skbkontur/react-ui/components";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import SidePage from "@skbkontur/react-ui/components/SidePage/SidePage";
import * as React from "react";
import { Line } from "../../commonComponents/Line/Line";

interface IPersonEditorProps {
  acceptButtonCaption: string;
  heading: string;
  onSave: () => void;
  onClose: () => void;
  disableActionButtons: boolean;
}

export class PersonEditor extends React.Component<IPersonEditorProps, {}> {
  constructor(props: IPersonEditorProps, state: {}) {
    super(props, state);
  }

  public render(): JSX.Element {
    const { disableActionButtons, heading, onClose, children, acceptButtonCaption } = this.props;

    return (
      <SidePage width={800} blockBackground onClose={() => !disableActionButtons && onClose()}>
        <SidePage.Header>{heading}</SidePage.Header>
        <SidePage.Body>
          <SidePage.Container>
            <Line marginBottom={100}>{children}</Line>
          </SidePage.Container>
        </SidePage.Body>
        <SidePage.Footer panel>
          <Gapped gap={7}>
            <Button
              width={120}
              size="medium"
              data-tid="AcceptButton"
              use="primary"
              disabled={disableActionButtons}
              onClick={this.props.onSave}
            >
              {disableActionButtons ? <Spinner dimmed type="mini" caption="" /> : acceptButtonCaption}
            </Button>
            <Button width={120} size="medium" data-tid="CancelButton" onClick={onClose} disabled={disableActionButtons}>
              Отменить
            </Button>
          </Gapped>
        </SidePage.Footer>
      </SidePage>
    );
  }
}
