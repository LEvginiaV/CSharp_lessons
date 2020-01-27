import Button from "@skbkontur/react-ui/components/Button/Button";
import ComboBox from "@skbkontur/react-ui/components/ComboBox/ComboBox";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import MenuHeader from "@skbkontur/react-ui/MenuHeader";
import memoizeOne from "memoize-one";
import * as React from "react";
import { CustomerDto } from "../../api/new/dto/CustomerDto";
import { CustomerInfoDto } from "../../api/new/dto/CustomerInfoDto";
import { Metrics } from "../../common/MetricsFacade";
import { DiscountInput } from "../../commonComponents/DiscountInput/DiscountInput";
import { FormattedPhone } from "../../commonComponents/FormattedPhone/FormattedPhone";
import { NameInput } from "../../commonComponents/NameInput/NameInput";
import { PhoneInput } from "../../commonComponents/PhoneInput/PhoneInput";
import { CustomerListSearcher, Searcher } from "../../controls/Searcher";
import { StringHelper } from "../../helpers/StringHelper";
import { CalendarModalErrorInfo } from "./CalendarHelper";
import * as styles from "./CustomerSelector.less";

interface Props {
  value: Nullable<CustomerDto | CustomerInfoDto>;
  newCustomerDiscount?: string;
  errorName: Nullable<CalendarModalErrorInfo>;
  errorPhone: Nullable<CalendarModalErrorInfo>;
  customers: CustomerDto[];
  onChange: (customer: Nullable<CustomerDto | CustomerInfoDto>, newCustomerDiscount?: string) => void;
  onSave: () => Promise<void>;
}
interface State {
  query: string;
  expand: boolean;
}

export class CustomerSelector extends React.Component<Props, State> {
  public state: Readonly<State> = {
    query: "",
    expand: false,
  };

  private getSearcher: (data: CustomerDto[]) => Searcher<CustomerDto> = memoizeOne(CustomerListSearcher);

  public render() {
    const { value } = this.props;
    return (
      <div>
        {value == null && this.renderCombobox()}
        {value && (value.hasOwnProperty("id") ? this.renderView() : this.renderAddForm())}
      </div>
    );
  }

  private renderView() {
    const { value } = this.props;
    if (!value) {
      return;
    }
    return (
      <div className={styles.view} onClick={() => this.setState({ expand: true })}>
        <div className={styles.name} data-tid="CustomerViewNameLabel">
          {value.name}
        </div>
        <div className={styles.line2}>
          {value.phone && (
            <div className={styles.phone} data-tid="CustomerViewPhoneLabel">
              <FormattedPhone value={value.phone} />
            </div>
          )}
          <div>{this.renderDiscount(value.discount)}</div>
        </div>
        <div className={styles.comment}>{value.additionalInfo}</div>

        <div className={styles.kebabIcon}>
          <Kebab size="large" data-tid="CustomerKebab">
            <MenuItem onClick={this.handleClear}>Выбрать другого клиента</MenuItem>
          </Kebab>
        </div>
      </div>
    );
  }

  private renderDiscount(discount: number | undefined): JSX.Element {
    if (discount === undefined || !Number.isFinite(discount)) {
      return <span />;
    }
    return (
      <span>
        скидка&nbsp;<span data-tid="CustomerViewDiscountLabel">{StringHelper.formatDiscountString(discount)}</span>%
      </span>
    );
  }

  private renderAddForm() {
    const customer = this.props.value;
    if (!customer) {
      return null;
    }
    return (
      <div className={styles.addForm}>
        <div className={styles.line1}>
          <NameInput
            data-tid="AddCustomerFormNameInput"
            error={!!this.props.errorName}
            width="100%"
            placeholder="Как зовут клиента"
            value={customer.name}
            onChange={(_, v) => this.handleChangeCustomerInfo({ name: v })}
            maxLength={120}
          />
        </div>
        <div className={styles.line2}>
          <div className={styles.field}>
            <div className={styles.label}>Телефон</div>
            <PhoneInput
              data-tid="AddCustomerFormPhoneInput"
              error={!!this.props.errorPhone}
              width={145}
              value={StringHelper.formatPhoneToInput(customer.phone)}
              onChange={(_, v) => this.handleChangeCustomerInfo({ phone: StringHelper.formatPhone(v) })}
            />
          </div>
          <div className={styles.field}>
            <div className={styles.label}>Скидка</div>
            <DiscountInput
              data-tid="DiscountInput"
              width={60}
              value={this.props.newCustomerDiscount}
              onChange={(_, v) => this.handleChangeDiscount(v)}
            />{" "}
            %
          </div>
          {this.props.errorPhone && <div className={styles.phoneError}>{this.props.errorPhone.text}</div>}
        </div>
        <div className={styles.kebabIcon}>
          <Kebab size="large" data-tid="CustomerEditorKebab">
            <MenuItem onClick={this.props.onSave}>Сохранить</MenuItem>
            <MenuItem onClick={this.handleClear}>Отменить</MenuItem>
          </Kebab>
        </div>
      </div>
    );
  }

  private renderCombobox() {
    return (
      <ComboBox
        data-tid="CustomerComboBox"
        getItems={this.getItems}
        onChange={(_, item) => this.props.onChange(item)}
        onUnexpectedInput={this.handleClear}
        placeholder="Введите имя или телефон"
        value={this.props.value}
        renderItem={this.renderComboboxItem}
        renderValue={item => item.name}
        valueToString={item => item.name || ""}
        maxLength={120}
        width="100%"
      />
    );
  }

  private renderComboboxItem = (item: CustomerDto) => {
    return (
      <div className={styles.comboboxItem}>
        <div className={styles.name}>{item.name}</div>
        <div className={styles.phone}>
          <FormattedPhone value={item.phone} />
        </div>
      </div>
    );
  };

  private renderAddNewItem = (): JSX.Element => (
    <MenuHeader>
      <Button use="link" onClick={this.onAddItem}>
        <span className={styles.comboboxNotFound}>+ Добавить{this.state.query ? ` "${this.state.query}"` : ""}</span>
      </Button>
    </MenuHeader>
  );

  private handleChangeDiscount = (discountValue?: string) => {
    this.props.onChange(this.props.value, discountValue);
  };

  private handleChangeCustomerInfo = (part: Partial<CustomerInfoDto>) => {
    this.props.onChange({
      ...this.props.value,
      ...part,
    });
  };

  private handleClear = () => {
    this.props.onChange(null);
  };

  private onAddItem = () => {
    const { query } = this.state;
    let name = query;
    let phone = "";
    if (/^\+?([0-9 \-\)\(]+)$/.test(query)) {
      phone = query
        .replace(/[ \-\)\(]/g, "")
        .replace(/^(\+7|8)(.*)$/, "$2")
        .substring(0, 10);
      name = "";
    }

    this.props.onChange({
      additionalInfo: "",
      name,
      phone: phone ? "7" + phone : "",
    });
    Metrics.clientsCreateStart({ where: "calendar" });
  };

  private getItems = (query: string): Promise<any[]> => {
    this.setState({ query });
    if (!query) {
      return Promise.resolve(this.props.customers);
    }
    const searchResult = this.getSearcher(this.props.customers).search(query);
    const result = [];
    result.push(this.renderAddNewItem());
    searchResult.forEach(x => result.push(x));
    return Promise.resolve(result);
  };
}
