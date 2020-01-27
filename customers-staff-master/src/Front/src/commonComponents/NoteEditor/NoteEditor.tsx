import { Button, Gapped, Spinner, Textarea } from "@skbkontur/react-ui/components";
import { TextareaProps } from "@skbkontur/react-ui/Textarea";
import * as React from "react";
import { StringHelper } from "../../helpers/StringHelper";
import { Line } from "../Line/Line";

interface INoteEditorProps extends TextareaProps {
  note: string;
  onSave: (note: string) => void;
  disableActionButtons: boolean;
}

export class NoteEditor extends React.Component<INoteEditorProps, { localNote: string | null }> {
  constructor(props: INoteEditorProps, state: any) {
    super(props, state);
    this.state = { localNote: null };
  }

  public componentDidMount() {
    this.setState({ localNote: null });
  }

  public componentDidUpdate() {
    if (this.state.localNote !== null && this.state.localNote === this.props.note) {
      this.setState({ localNote: null });
    }
  }

  public render(): JSX.Element {
    const { disableActionButtons } = this.props;
    const localNote = this.state.localNote !== null ? this.state.localNote : this.props.note;
    return (
      <div>
        <Textarea {...this.getTextareaProps()} value={localNote} onChange={this.onChange} autoResize />
        {this.state.localNote !== null && (
          <Line marginTop={12}>
            <Gapped gap={7}>
              <Button use="primary" width={112} disabled={disableActionButtons} onClick={this.onSave}>
                {disableActionButtons ? <Spinner dimmed type="mini" caption="" /> : "Сохранить"}
              </Button>
              <Button width={94} disabled={disableActionButtons} onClick={() => this.setState({ localNote: null })}>
                Отменить
              </Button>
            </Gapped>
          </Line>
        )}
      </div>
    );
  }

  private onChange = (_: any, value: string) => {
    value = value ? StringHelper.removeMoreWhitespaces(value, false) : value;
    this.setState({ localNote: value });
  };

  private getTextareaProps(): TextareaProps {
    const props = { ...this.props };
    delete props.note;
    delete props.onSave;
    delete props.disableActionButtons;
    return props;
  }

  private onSave = () => this.state.localNote !== null && this.props.onSave(this.state.localNote);
}
