import { Loader } from "@skbkontur/react-ui/components";
import Link from "@skbkontur/react-ui/components/Link/Link";

import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import * as React from "react";
import { RouteChildrenProps } from "react-router";
import { CustomerDto } from "../../../api/new/dto/CustomerDto";
import { Guid } from "../../../api/new/dto/Guid";
import { Metrics } from "../../../common/MetricsFacade";
import { Caption, CaptionType } from "../../../commonComponents/Caption/Caption";
import { Grid } from "../../../commonComponents/Grid/Grid";
import { Line } from "../../../commonComponents/Line/Line";
import { NoteEditor } from "../../../commonComponents/NoteEditor/NoteEditor";
import { StringHelper } from "../../../helpers/StringHelper";
import { HelpCaption } from "../../HelpCaption/HelpCaption";
import { ContentLayoutFullScreen } from "../../Layout/Layout";
import { PersonHeader } from "../../PersonHeader/PersonHeader";
import { CustomerEditor } from "../CustomerEditor/CustomerEditor";
import * as styles from "./Customer.less";

export interface ICustomerProps extends CustomerDto, RouteChildrenProps<{ id: string }> {
  onSave: (id: Guid, customer: Partial<CustomerDto>) => Promise<void>;
  onSaveComment: (id: Guid, comment: string) => Promise<void>;
  onBack: () => void;
  version: number;
}

export class CustomerView extends React.Component<
  ICustomerProps,
  { showEditor?: boolean; isSaving: boolean; isSavingComment: boolean }
> {
  constructor(props: ICustomerProps, state: {}) {
    super(props, state);
    this.state = { showEditor: false, isSaving: false, isSavingComment: false };
  }

  public render(): JSX.Element {
    return (
      <ContentLayoutFullScreen data-tid="CustomerPage">
        {this.props.version ? this.renderContent() : this.renderLoader()}
      </ContentLayoutFullScreen>
    );
  }

  private onSave = async (customer: Partial<CustomerDto>) => {
    try {
      this.setState({ isSaving: true });
      await this.props.onSave(this.props.id, customer);
      Metrics.clientsEditSuccess({
        ...Metrics.variablesFromCustomer(customer, "list"),
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
      Metrics.clientsComment({
        cardid: this.props.id,
        value: comment,
      });
    } catch (e) {
      Toast.push("Ошибка при сохранении");
    } finally {
      this.setState({ isSavingComment: false });
    }
  };

  private onCloseEditor = () => this.setState({ showEditor: false });
  private onOpenEditor = () => {
    Metrics.clientsEditStart({ where: "list", cardid: this.props.id });
    this.setState({ showEditor: true });
  };

  private renderContent(): JSX.Element {
    const { name, gender, phone, email, additionalInfo, birthday, customId, discount } = this.props;
    const groupMargin = 40;

    return (
      <div>
        {this.state.showEditor && (
          <CustomerEditor
            disableActionButtons={this.state.isSaving}
            onSave={this.onSave}
            onClose={this.onCloseEditor}
            heading="Редактирование карточки клиента"
            form={{ gender, phone, name, additionalInfo, birthday, customId, discount, email }}
          />
        )}
        <PersonHeader
          data-tid="PersonHeader"
          fullName={name || ""}
          description={phone ? StringHelper.formatPhoneString(phone) : "Телефон не указан"}
          genderType={gender}
          dimmedDescription={!phone}
          useGender
          onBack={this.props.onBack}
          onEdit={this.onOpenEditor}
        />
        <div className={styles.wrapper}>
          <WrapLine valueDataTid="Discount" value={StringHelper.printDiscountString(discount)} caption="Скидка" />
          {this.renderCustomId(groupMargin)}
          {StringHelper.getBirthdayString(birthday) && (
            <WrapLine
              valueDataTid="Birthday"
              value={StringHelper.getBirthdayString(birthday)}
              caption="День рождения"
            />
          )}
          {email && (
            <WrapLine
              value={
                <Link data-tid="Email" href={`mailto:${email}`}>
                  {email}
                </Link>
              }
              caption="Эл. почта"
            />
          )}
          <Line marginBottom={20}>
            <Grid columns={[125, 300]} alignItems="flex-start">
              <Caption type={CaptionType.Gray} usePadding>
                Комментарий
              </Caption>
              <NoteEditor
                data-tid="AdditionalInfo"
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
          </Line>
        </div>
      </div>
    );
  }

  private renderCustomId(groupMargin: number): JSX.Element {
    const help = (
      <span>
        Если у вас уже есть скидочные карты,
        <br /> введите номер или штрихкод, чтобы
        <br /> быстро выбирать клиента на кассе
      </span>
    );

    return (
      <WrapLine
        valueDataTid="CustomId"
        value={this.props.customId}
        caption={
          <HelpCaption tooltipPosition="top left" tooltipMessage={help}>
            Номер карты
          </HelpCaption>
        }
        marginBottom={groupMargin}
      />
    );
  }

  private renderLoader(): JSX.Element {
    return <Loader className={styles.loader} active={true} type="big" />;
  }
}

const WrapLine: React.SFC<{
  value?: string | number | JSX.Element | null;
  caption?: string | JSX.Element;
  marginBottom?: number;
  valueDataTid?: string;
}> = props => {
  const { value, valueDataTid, caption, marginBottom } = props;
  return value && caption ? (
    <Line marginBottom={marginBottom || 20}>
      <Grid columns={[125, 300]}>
        <Caption type={CaptionType.Gray}>{caption}</Caption>
        <div data-tid={valueDataTid}>{value}</div>
      </Grid>
    </Line>
  ) : null;
};
