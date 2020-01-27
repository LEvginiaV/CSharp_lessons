import { ButtonUse } from "@skbkontur/react-ui/Button";
import { Spinner } from "@skbkontur/react-ui/components";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Modal from "@skbkontur/react-ui/components/Modal/Modal";
import * as React from "react";

export interface ISimpleLightboxProps {
  header: string;
  body: string | React.ReactNode;
  acceptButtonCaption: string;
  onAccept: () => void;
  onClose: () => void;
  width?: number;
  acceptButtonUse?: ButtonUse;
  disableActionButtons: boolean;
}

export const SimpleLightbox: React.SFC<ISimpleLightboxProps> = props => {
  return (
    <Modal ignoreBackgroundClick onClose={props.onClose} width={props.width || 500}>
      <Modal.Header sticky>{props.header}</Modal.Header>
      <Modal.Body>
        <div>{props.body}</div>
      </Modal.Body>
      <Modal.Footer sticky panel>
        <Gapped gap={7}>
          <Button
            size="medium"
            width={120}
            data-tid="Accept"
            onClick={props.onAccept}
            disabled={props.disableActionButtons}
            use={props.acceptButtonUse || "primary"}
          >
            {props.disableActionButtons ? <Spinner dimmed type="mini" caption="" /> : props.acceptButtonCaption}
          </Button>
          <Button
            size="medium"
            width={120}
            data-tid="Cancel"
            onClick={props.onClose}
            disabled={props.disableActionButtons}
          >
            Отменить
          </Button>
        </Gapped>
      </Modal.Footer>
    </Modal>
  );
};
