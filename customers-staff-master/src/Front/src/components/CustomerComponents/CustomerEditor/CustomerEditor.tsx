import { ValidationContainer, ValidationWrapperV1 } from "@skbkontur/react-ui-validations";
import Input from "@skbkontur/react-ui/components/Input/Input";
import Textarea from "@skbkontur/react-ui/components/Textarea/Textarea";
import { Override } from "@skbkontur/react-ui/typings/utility-types";
import * as each from "lodash/each";
import * as React from "react";
import { BirthdayDto } from "../../../api/new/dto/BirthdayDto";
import { CustomerDto } from "../../../api/new/dto/CustomerDto";
import { Guid } from "../../../api/new/dto/Guid";
import { BirthdayControl, IBirthday } from "../../../commonComponents/BirthdayControl/BirthdayControl";
import { Caption, CaptionType } from "../../../commonComponents/Caption/Caption";
import { DiscountInput } from "../../../commonComponents/DiscountInput/DiscountInput";
import { Grid } from "../../../commonComponents/Grid/Grid";
import { Line } from "../../../commonComponents/Line/Line";
import { NameInput } from "../../../commonComponents/NameInput/NameInput";
import { PhoneInput } from "../../../commonComponents/PhoneInput/PhoneInput";
import { StringHelper } from "../../../helpers/StringHelper";
import { GenderType } from "../../../models/GenderType";
import { CustomerInfoValidationHelper } from "../../../validation/CustomerInfoValidationHelper";
import { ValidationHelper } from "../../../validation/ValidationHelper";
import { GenderSelector } from "../../GenderSelector/GenderSelector";
import { HelpCaption } from "../../HelpCaption/HelpCaption";
import { PersonEditor } from "../../PersonEditor/PersonEditor";

interface ICustomerEditorProps {
  heading: string;
  acceptButtonCaption?: string;
  form?: Partial<CustomerDto>;
  onSave?: (customer: Partial<CustomerDto>) => void;
  onClose?: () => void;
  disableActionButtons: boolean;
}

interface ICustomerEditorState extends Override<CustomerDto, { discount?: string; id?: Guid; birthday?: IBirthday }> {}

export class CustomerEditor extends React.Component<ICustomerEditorProps, ICustomerEditorState> {
  private ValidationContainer: ValidationContainer | null;
  private NameInput: NameInput | null;

  constructor(props: ICustomerEditorProps, state: ICustomerEditorState) {
    super(props, state);
    this.state = {};
    this.NameInput = null;
  }

  public componentDidMount() {
    this.initForm();
    this.NameInput && this.NameInput.focus();
  }

  public render(): JSX.Element {
    const { name, birthday, phone, gender, email, discount, customId, additionalInfo } = this.state;
    const inputProps = { width: "100%" };

    return (
      <PersonEditor
        data-tid="CustomerEditor"
        acceptButtonCaption={this.props.acceptButtonCaption || "Сохранить"}
        heading={this.props.heading}
        onSave={this.onSave}
        onClose={() => this.props.onClose && this.props.onClose()}
        disableActionButtons={this.props.disableActionButtons}
      >
        <ValidationContainer ref={el => (this.ValidationContainer = el)}>
          <WrapLine>
            <Caption>Имя</Caption>
            <ValidationWrapperV1 {...CustomerInfoValidationHelper.bindCustomerNameValidation(name, phone, customId)}>
              <NameInput
                data-tid="Name"
                {...inputProps}
                maxLength={120}
                placeholder="Как зовут клиента"
                value={name || ""}
                onChange={(_, v) => this.setState({ name: v })}
                ref={el => (this.NameInput = el)}
              />
            </ValidationWrapperV1>
          </WrapLine>
          <WrapLine marginBottom={40}>
            <Caption>Телефон</Caption>
            <ValidationWrapperV1 {...CustomerInfoValidationHelper.bindCustomerPhoneValidation(phone, name, customId)}>
              <PhoneInput
                data-tid="Phone"
                width={160}
                value={phone || ""}
                onChange={(_, v) => this.setState({ phone: v })}
              />
            </ValidationWrapperV1>
          </WrapLine>
          <WrapLine>
            <Caption>Скидка</Caption>
            <span>
              <DiscountInput
                data-tid="Discount"
                width={60}
                value={discount}
                onChange={(_, v) => this.setState({ discount: v })}
              />
              &nbsp;%
            </span>
          </WrapLine>
          <WrapLine marginBottom={40}>
            <HelpCaption
              tooltipPosition="top left"
              tooltipMessage={
                <span>
                  Если у вас уже есть скидочные карты,
                  <br /> введите номер или штрихкод, чтобы
                  <br /> быстро выбирать клиента на кассе
                </span>
              }
            >
              Номер карты
            </HelpCaption>
            <ValidationWrapperV1
              {...CustomerInfoValidationHelper.bindCustomerCustomIdValidation(customId, name, phone)}
            >
              <Input
                data-tid="CustomId"
                placeholder="Если есть"
                maxLength={16}
                width={160}
                value={customId || ""}
                onChange={(_, v) => this.setState({ customId: v })}
              />
            </ValidationWrapperV1>
          </WrapLine>
          <Line marginBottom={20}>
            <Caption usePadding type={CaptionType.H3}>
              Дополнительная информация
            </Caption>
          </Line>
          <WrapLine>
            <Caption>Пол</Caption>
            <GenderSelector
              data-tid="GenderSelector"
              onSelect={(newGender: GenderType) => this.setState({ gender: newGender })}
              selected={gender}
            />
          </WrapLine>
          <WrapLine>
            <Caption>День рождения</Caption>
            <BirthdayControl
              data-tid="Birthday"
              birthday={birthday}
              onChange={(newBirthday: IBirthday) => this.setState({ birthday: newBirthday })}
              dayValidationProps={CustomerInfoValidationHelper.bindBirthdayDayValidation(birthday)}
              monthValidationProps={CustomerInfoValidationHelper.bindBirthdayMonthValidation(birthday)}
              yearValidationProps={CustomerInfoValidationHelper.bindBirthdayYearValidation(birthday)}
            />
          </WrapLine>
          <WrapLine>
            <Caption>Эл. почта</Caption>
            <ValidationWrapperV1 {...ValidationHelper.bindEmailValidation(email)}>
              <Input
                data-tid="Email"
                {...inputProps}
                maxLength={100}
                value={email || ""}
                onChange={(_, v) => this.setState({ email: v })}
              />
            </ValidationWrapperV1>
          </WrapLine>
          <Line marginBottom={15}>
            <Grid columns={[120, 350]} alignItems="flex-start">
              <Caption usePadding>Комментарий</Caption>
              <Textarea
                data-tid="AdditionalInfo"
                autoResize
                maxLength={500}
                maxRows={10}
                rows={3}
                selectAllOnFocus={false}
                {...inputProps}
                value={additionalInfo || ""}
                onChange={(_, v) => this.setState({ additionalInfo: v })}
              />
            </Grid>
          </Line>
        </ValidationContainer>
      </PersonEditor>
    );
  }

  private initForm() {
    const { form } = this.props;
    const state: ICustomerEditorState = {};

    form &&
      each(form, (value: any, key: keyof CustomerDto) => {
        if (key === "phone") {
          value = StringHelper.formatPhoneToInput(value);
        }

        if (key === "birthday") {
          value = this.prepareBirthday(value);
        }

        if (value || (key === "discount" && value === 0)) {
          state[key] = value;
        }
      });

    this.setState({ ...state });
  }

  private onSave = async () => {
    const isValidate = this.ValidationContainer && (await this.ValidationContainer.validate());
    if (isValidate && this.props.onSave) {
      await this.props.onSave(this.prepareDataToSave());
    }
  };

  private prepareDataToSave(): Partial<CustomerDto> {
    const copy = { ...this.state } as Partial<CustomerDto>;

    if (copy.name) {
      copy.name = StringHelper.removeMoreWhitespaces(copy.name);
    }

    if (copy.additionalInfo) {
      copy.additionalInfo = StringHelper.removeMoreWhitespaces(copy.additionalInfo);
    }

    if (copy.phone) {
      copy.phone = StringHelper.formatPhone(copy.phone);
    }

    if (copy.discount) {
      copy.discount = StringHelper.formatDiscountNumber(copy.discount);
    }

    if (copy.birthday) {
      if (!copy.birthday.day) {
        copy.birthday = undefined;
      } else if (!copy.birthday.year) {
        copy.birthday.year = undefined;
      }
    }

    return copy;
  }

  private prepareBirthday(birthday: BirthdayDto): IBirthday {
    const result: IBirthday = {};
    if (birthday) {
      result.day = birthday.day;
      result.month = birthday.month;
      result.year = birthday.year;
    }
    return result;
  }
}

const WrapLine: React.SFC<{ marginBottom?: number }> = props => {
  return (
    <Line marginBottom={props.marginBottom || 15}>
      <Grid columns={[120, 350]}>{props.children}</Grid>
    </Line>
  );
};
