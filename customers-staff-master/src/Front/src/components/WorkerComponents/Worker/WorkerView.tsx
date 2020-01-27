import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import * as React from "react";
import { RouteChildrenProps } from "react-router";
import { Guid } from "../../../api/new/dto/Guid";
import { WorkerDto } from "../../../api/new/dto/WorkerDto";
import { Metrics } from "../../../common/MetricsFacade";
import { Caption, CaptionType } from "../../../commonComponents/Caption/Caption";
import { Grid } from "../../../commonComponents/Grid/Grid";
import { Line } from "../../../commonComponents/Line/Line";
import { NoteEditor } from "../../../commonComponents/NoteEditor/NoteEditor";
import { StringHelper } from "../../../helpers/StringHelper";
import { HelpCaption } from "../../HelpCaption/HelpCaption";
import { ContentLayoutFullScreen } from "../../Layout/Layout";
import { IRemovePersonLightboxProps, RemovePersonLightbox } from "../../lightboxes/RemovePersonLightbox";
import { PersonHeader } from "../../PersonHeader/PersonHeader";
import { WorkerEditor } from "../WorkerEditor/WorkerEditor";
import * as styles from "./Worker.less";

// TODO: id from router must be guid
export interface IWorkerProps extends WorkerDto, RouteChildrenProps<{ id: string }> {
  onSave: (id: Guid, worker: Partial<WorkerDto>) => Promise<void>;
  onSaveComment: (id: Guid, comment: string) => Promise<void>;
  onRemove: (guid: Guid) => Promise<void>;
  onBack: () => void;
  positionsMap?: string[];
  version: number;
}

export class WorkerView extends React.Component<
  IWorkerProps,
  {
    showEditor?: boolean;
    showRemoveLightbox?: boolean;
    isSaving: boolean;
    isRemoving: boolean;
    isSavingComment: boolean;
  }
> {
  constructor(props: IWorkerProps, state: {}) {
    super(props, state);
    this.state = {
      showEditor: false,
      showRemoveLightbox: false,
      isSaving: false,
      isRemoving: false,
      isSavingComment: false,
    };
  }

  public render(): JSX.Element {
    return (
      <ContentLayoutFullScreen data-tid="WorkerPage">
        {this.props.version ? this.renderContent() : this.renderLoader()}
      </ContentLayoutFullScreen>
    );
  }

  public onRemove = async () => {
    try {
      this.setState({ isRemoving: true });
      await this.props.onRemove(this.props.id);
    } catch (e) {
      Toast.push("Ошибка при удалении");
    } finally {
      this.setState({ isRemoving: false });
      this.props.onBack();
    }
  };

  private onSave = async (worker: Partial<WorkerDto>) => {
    try {
      this.setState({ isSaving: true });
      await this.props.onSave(this.props.id, worker);
      Metrics.staffEditSuccess({
        ...Metrics.variablesFromWorker(worker),
        cardid: this.props.id,
      });
    } catch (e) {
      Toast.push("Ошибка при сохранении");
    } finally {
      this.onCloseEditor();
      this.setState({ isSaving: false });
    }
  };

  private onSaveComment = async (comment: string) => {
    try {
      this.setState({ isSavingComment: true });
      await this.props.onSaveComment(this.props.id, comment);
    } catch (e) {
      Toast.push("Ошибка при сохранении");
    } finally {
      this.setState({ isSavingComment: false });
    }
  };

  private onCloseEditor = () => this.setState({ showEditor: false });
  private onOpenEditor = () => {
    Metrics.staffEditStart({ cardid: this.props.id });
    this.setState({ showEditor: true });
  };

  private renderRemoveLightbox(): JSX.Element | null {
    const { fullName, position } = this.props;

    const body = (
      <div>
        <Caption>{fullName}</Caption>
        {position && <Caption type={CaptionType.Gray}>{position}</Caption>}
        <Caption usePadding>
          Карточку нельзя будет восстановить. В отчетах и документах история по сотруднику сохранится.
        </Caption>
      </div>
    );

    const props: IRemovePersonLightboxProps = {
      header: "Удаление сотрудника",
      body,
      onAccept: this.onRemove,
      onClose: () => this.setState({ showRemoveLightbox: false }),
      disableActionButtons: this.state.isRemoving,
    };

    if (this.state.showRemoveLightbox) {
      return <RemovePersonLightbox data-tid="RemoveWorkerModal" {...props} />;
    } else {
      return null;
    }
  }

  private renderContent(): JSX.Element {
    const { fullName, code, additionalInfo, position, phone, positionsMap } = this.props;
    const columns = [135, 300];
    const bindLine = { marginBottom: 20 };
    const workerCodeTooltipMessage = (
      <div>
        <Caption>Номер сотрудника в Маркете. </Caption>
        <Caption>Используйте для быстрого поиска на кассе.</Caption>
      </div>
    );

    return (
      <div>
        {this.state.showEditor && (
          <WorkerEditor
            disableActionButtons={this.state.isSaving}
            positionsMap={positionsMap}
            onSave={this.onSave}
            onClose={this.onCloseEditor}
            heading="Редактирование карточки сотрудника"
            form={{ code, phone, fullName, additionalInfo, position }}
          />
        )}
        {this.renderRemoveLightbox()}
        <PersonHeader
          data-tid="PersonHeader"
          fullName={fullName}
          dimmedDescription={!position}
          description={position || "Должность не указана"}
          onBack={this.props.onBack}
          onEdit={this.onOpenEditor}
          onRemove={() => this.setState({ showRemoveLightbox: true })}
        />
        <div className={styles.wrapper}>
          <Line {...bindLine}>
            <Grid columns={columns}>
              <HelpCaption
                data-tid="CodeHelpCaption"
                type={CaptionType.Gray}
                tooltipMessage={workerCodeTooltipMessage}
                tooltipPosition="top left"
              >
                Код сотрудника
              </HelpCaption>
              <div data-tid="Code">{code}</div>
            </Grid>
          </Line>
          <Line {...bindLine}>
            <Grid columns={columns}>
              <Caption type={CaptionType.Gray}>Телефон</Caption>
              <div data-tid="Phone">
                {StringHelper.isNullOrEmpty(phone) ? "не указан" : StringHelper.formatPhoneString(phone)}
              </div>
            </Grid>
          </Line>
          <Grid columns={columns} alignItems="flex-start">
            <Caption type={CaptionType.Gray} usePadding>
              Комментарий
            </Caption>
            <NoteEditor
              data-tid="Description"
              disableActionButtons={this.state.isSavingComment}
              width={350}
              maxLength={500}
              maxRows={10}
              rows={3}
              selectAllOnFocus={false}
              note={additionalInfo || ""}
              onSave={this.onSaveComment}
            />
          </Grid>
        </div>
      </div>
    );
  }

  private renderLoader(): JSX.Element {
    return <Loader className={styles.loader} active={true} type="big" />;
  }
}
