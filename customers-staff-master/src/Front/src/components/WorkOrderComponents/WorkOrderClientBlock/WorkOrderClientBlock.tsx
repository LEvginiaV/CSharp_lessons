import Icon from "@skbkontur/react-icons";
import { ValidationContainer, ValidationWrapperV1 } from "@skbkontur/react-ui-validations";
import Button from "@skbkontur/react-ui/Button";
import ComboBox from "@skbkontur/react-ui/ComboBox";
import Link from "@skbkontur/react-ui/Link";
import MenuHeader from "@skbkontur/react-ui/MenuHeader";
import Textarea from "@skbkontur/react-ui/Textarea";
import memoizeOne from "memoize-one";
import * as React from "react";
import { CustomerDto } from "../../../api/new/dto/CustomerDto";
import { Guid } from "../../../api/new/dto/Guid";
import { Metrics } from "../../../common/MetricsFacade";
import { PhoneInput } from "../../../commonComponents/PhoneInput/PhoneInput";
import { PredicateInput } from "../../../commonComponents/PredicateInput/PredicateInput";
import { CustomerListSearcher, Searcher } from "../../../controls/Searcher";
import { StringHelper } from "../../../helpers/StringHelper";
import { CustomersActionCreator } from "../../../redux/customers";
import { WorkOrderValidationHelper } from "../../../validation/WorkOrderValidationHelper";
import * as styles from "./WorkOrderClientBlock.less";

enum EditState {
  Initial = "Initial",
  Selected = "Selected",
  Edit = "Edit",
}

export interface IWorkOrderClientBlockProps {
  customers: CustomerDto[];
  selectedCustomerId?: Guid;
}

export class WorkOrderClientBlock extends React.Component<
  IWorkOrderClientBlockProps,
  {
    selectedCustomer: CustomerDto;
    editState: EditState;
  }
> {
  private ValidationContainer: ValidationContainer | null;
  private ClientCombobox: ComboBox<CustomerDto> | null;
  private getSearcher: (data: CustomerDto[]) => Searcher<CustomerDto> = memoizeOne(CustomerListSearcher);

  constructor(props: IWorkOrderClientBlockProps, state: {}) {
    super(props, state);

    const selectedCustomer =
      props.customers && props.selectedCustomerId
        ? props.customers.find(x => x.id === props.selectedCustomerId)
        : undefined;

    this.state = {
      selectedCustomer: selectedCustomer || { id: props.selectedCustomerId || "" },
      editState: props.selectedCustomerId ? EditState.Selected : EditState.Initial,
    };
  }

  public async isValid(): Promise<boolean> {
    return (
      this.state.editState === EditState.Selected ||
      (!!this.ValidationContainer && (await this.ValidationContainer.validate()))
    );
  }

  public focusClient() {
    this.ClientCombobox && this.ClientCombobox.focus();
  }

  public componentDidUpdate(prevProps: Readonly<IWorkOrderClientBlockProps>): void {
    if (this.state.editState !== EditState.Initial && prevProps.customers !== this.props.customers) {
      const currentCustomer = this.state.selectedCustomer;
      const selectedCustomer = this.props.customers.find(x => x.id === currentCustomer.id);

      if (!selectedCustomer) {
        return;
      }

      if (this.state.editState === EditState.Selected) {
        this.setState({ selectedCustomer });
      }

      if (this.state.editState === EditState.Edit) {
        this.setState({
          selectedCustomer: { ...selectedCustomer, name: currentCustomer.name, phone: currentCustomer.phone },
        });
      }
    }
  }

  public async saveCustomerAndGetId(): Promise<Guid> {
    const { editState, selectedCustomer } = this.state;

    if (editState === EditState.Selected) {
      return selectedCustomer.id;
    }

    if (editState === EditState.Initial) {
      const customerId = await CustomersActionCreator.create(selectedCustomer);
      const customer = this.props.customers.find(x => x.id === customerId);
      customer && this.setState({ selectedCustomer: customer, editState: EditState.Selected });
      Metrics.clientsCreateSuccess({
        ...Metrics.variablesFromCustomer(selectedCustomer, "workorder"),
        cardid: customerId,
      });
      return customerId;
    }

    if (editState === EditState.Edit) {
      await CustomersActionCreator.update(selectedCustomer.id, selectedCustomer);
      const customer = this.props.customers.find(x => x.id === selectedCustomer.id);
      customer && this.setState({ selectedCustomer: customer, editState: EditState.Selected });
      Metrics.clientsEditSuccess(Metrics.variablesFromCustomer(selectedCustomer, "workorder"));
      return selectedCustomer.id;
    }

    throw new Error("Unknown edit state");
  }

  public render(): React.ReactNode {
    const { selectedCustomer, editState } = this.state;

    if (editState === EditState.Selected) {
      return this.renderSelectedState();
    }

    return (
      <div className={styles.root}>
        <ValidationContainer ref={el => (this.ValidationContainer = el)}>
          <div className={styles.row}>
            <div className={styles.caption}>Клиент</div>
            <div>
              {editState === EditState.Initial && (
                <ValidationWrapperV1 {...WorkOrderValidationHelper.bindNameValidation(selectedCustomer.name)}>
                  <ComboBox<CustomerDto>
                    ref={el => (this.ClientCombobox = el)}
                    width={402}
                    maxLength={120}
                    placeholder="Как зовут клиента"
                    getItems={this.getItems}
                    renderItem={this.renderItem}
                    valueToString={x => x.name || ""}
                    renderValue={x => x.name || ""}
                    value={selectedCustomer}
                    itemToValue={x => x.name || ""}
                    onUnexpectedInput={x => ({ id: "", name: x })}
                    onChange={(_, x) =>
                      this.setState({ selectedCustomer: x, editState: x.id ? EditState.Selected : EditState.Initial })
                    }
                    data-tid="NameSelector"
                  />
                </ValidationWrapperV1>
              )}
              {editState === EditState.Edit && (
                <ValidationWrapperV1 {...WorkOrderValidationHelper.bindStringNonEmptyValidation(selectedCustomer.name)}>
                  <PredicateInput
                    width={402}
                    maxLength={120}
                    predicate={StringHelper.isNameValid}
                    value={selectedCustomer.name || ""}
                    placeholder="Как зовут клиента"
                    onChange={(_, v) => this.setState({ selectedCustomer: { ...selectedCustomer, name: v } })}
                    data-tid="NameInput"
                  />
                </ValidationWrapperV1>
              )}
            </div>
          </div>
          <div className={styles.row}>
            <div className={styles.caption}>Телефон</div>
            <div>
              <ValidationWrapperV1 {...WorkOrderValidationHelper.bindPhoneValidation(selectedCustomer.phone)}>
                <PhoneInput
                  width={149}
                  value={StringHelper.formatPhoneToInput(selectedCustomer.phone)}
                  onChange={(_, v) =>
                    this.setState({ selectedCustomer: { ...selectedCustomer, phone: StringHelper.formatPhone(v) } })
                  }
                  data-tid="PhoneInput"
                />
              </ValidationWrapperV1>
            </div>
          </div>
          <div className={styles.row}>
            <div className={styles.caption}>Комментарий</div>
            <div>
              <Textarea
                width={402}
                maxRows={0}
                rows={4}
                maxLength={500}
                resize={"none"}
                value={selectedCustomer.additionalInfo || ""}
                onChange={(_, v) => this.setState({ selectedCustomer: { ...selectedCustomer, additionalInfo: v } })}
                placeholder="Укажите адрес, паспортные данные или другой комментарий"
                data-tid="Comment"
              />
            </div>
          </div>
          {editState === EditState.Edit && (
            <div className={styles.row}>
              <div className={styles.caption} />
              <Link onClick={this.onUndo} data-tid="CancelEdit">
                <Icon name="Undo" /> Отменить редактирование
              </Link>
            </div>
          )}
        </ValidationContainer>
      </div>
    );
  }

  private renderSelectedState(): JSX.Element {
    const { selectedCustomer } = this.state;

    return (
      <div className={styles.root}>
        <div className={styles.row}>
          <div className={styles.caption}>Клиент</div>
          <div className={styles.value}>
            {!!selectedCustomer.name && (
              <div className={styles.selectedRow} data-tid="SelectedName">
                {selectedCustomer.name}
              </div>
            )}
            {!!selectedCustomer.phone && (
              <div className={styles.selectedRow} data-tid="SelectedPhone">
                {StringHelper.formatPhoneString(selectedCustomer.phone)}
              </div>
            )}
            {!!selectedCustomer.additionalInfo && (
              <div className={styles.selectedRow} data-tid="SelectedInfo">
                {selectedCustomer.additionalInfo}
              </div>
            )}
            <div className={styles.selectRowLink}>
              <div className={styles.link}>
                <Link
                  onClick={() => {
                    Metrics.clientsEditStart({ where: "workorder", cardid: this.state.selectedCustomer.id });
                    this.setState({ editState: EditState.Edit });
                  }}
                  data-tid="ChangeData"
                >
                  <Icon name="Edit" /> Изменить данные
                </Link>
              </div>
              <div>
                <Link
                  onClick={() => this.setState({ editState: EditState.Initial, selectedCustomer: { id: "" } })}
                  data-tid="SelectAnother"
                >
                  <Icon name="ArrowParallelHorizontal" /> Выбрать другого
                </Link>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  private onUndo = () => {
    const { selectedCustomer } = this.state;
    const customer = this.props.customers.find(x => x.id === selectedCustomer.id);
    this.setState({ selectedCustomer: customer || { id: selectedCustomer.id }, editState: EditState.Selected });
  };

  private closeClientCombobox = () => {
    if (this.ClientCombobox) {
      this.ClientCombobox.blur();
    }
  };

  private renderAddNewItem = (query: string): JSX.Element => (
    <MenuHeader>
      <Button use="link" onClick={this.closeClientCombobox}>
        <span className={styles.addNewItemLink}>+ Добавить{query ? ` "${query}"` : ""}</span>
      </Button>
    </MenuHeader>
  );

  private getItems: (query: string) => Promise<any[]> = async query => {
    if (!query) {
      return this.props.customers;
    }
    const searchResult = this.getSearcher(this.props.customers).search(query);
    const result = [];
    result.push(this.renderAddNewItem(query));
    searchResult.forEach(x => result.push(x));
    return result;
  };

  private renderItem(item: CustomerDto): JSX.Element {
    return (
      <div className={styles.searchRow}>
        <div className={styles.name} data-tid="Name">
          {item.name}
        </div>
        <div className={styles.phone} data-tid="Phone">
          {!!item.phone && StringHelper.formatPhoneString(item.phone)}
          {!item.phone && <span className={styles.noPhone}>Нет телефона</span>}
        </div>
      </div>
    );
  }
}
