import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import * as classnames from "classnames";
import * as H from "history";
import memoizeOne from "memoize-one";
import * as React from "react";
import { Link } from "react-router-dom";
import { INITIAL_PAGING_OPTIONS, PERSONS_PER_PAGE } from "../../../api/new/Constants";
import { CustomerDto } from "../../../api/new/dto/CustomerDto";
import { AppDataSingleton } from "../../../app/AppData";
import { FeatureAppearance } from "../../../common/FeatureAppearance";
import { Metrics } from "../../../common/MetricsFacade";
import { Caption, CaptionType } from "../../../commonComponents/Caption/Caption";
import { Feedback } from "../../../commonComponents/Feedback/Feedback";
import { FeedbackType } from "../../../commonComponents/Feedback/FeedbackType";
import { Grid } from "../../../commonComponents/Grid/Grid";
import { OnBoardingComponent } from "../../../commonComponents/OnBoardingComponent/OnBoardingComponent";
import { CustomerListSearcher, Searcher } from "../../../controls/Searcher";
import { PagingHelper } from "../../../helpers/PagingHelper";
import { StringHelper } from "../../../helpers/StringHelper";
import { ListType } from "../../../models/ListType";
import { ContentLayoutInner } from "../../Layout/Layout";
import { IPersonsListProps, PersonsList } from "../../PersonsList/PersonsList";
import * as styles from "../../PersonsList/PersonsList.less";
import { CustomerEditor } from "../CustomerEditor/CustomerEditor";
import { CustomersBoard1, CustomersBoard2, CustomersBoard3 } from "./imgs";

export interface ICustomersListProps {
  data: CustomerDto[] | null;
  itemPathPrefix: string;
  onAddItem: (customer: CustomerDto) => Promise<string>;
  history: H.History;
  hideCustomersOnBoard: boolean | null;
  updateSetting: () => void;
}

export class CustomersListView extends React.Component<
  ICustomersListProps,
  {
    skip?: number;
    take?: number;
    searchString?: string;
    dataLoaded?: boolean;
    isLoading?: boolean;
    waitingNewPersonGuid?: boolean;
    isAdding: boolean;
  }
> {
  private getSearcher: (data: CustomerDto[]) => Searcher<CustomerDto> = memoizeOne(CustomerListSearcher);

  constructor(props: ICustomersListProps, state: {}) {
    super(props, state);
    this.state = { ...INITIAL_PAGING_OPTIONS, searchString: "", isAdding: false };
  }

  public render(): JSX.Element {
    if (this.props.hideCustomersOnBoard === false) {
      return this.renderOnBoard();
    }

    const filteredData = this.filterBySearchString();
    const data = this.sliceByPaging(filteredData);

    return (
      <div>
        {FeatureAppearance.shouldShow(FeedbackType.CustomersCardsFeedback) && (
          <Feedback {...FeatureAppearance.getProps(FeedbackType.CustomersCardsFeedback)} />
        )}
        <PersonsList<CustomerDto> {...this.bindPersonList(filteredData)} data-tid="CustomerList">
          {this.renderTable(data, filteredData && filteredData.length)}
        </PersonsList>
      </div>
    );
  }

  private onAdd = async (customer: CustomerDto) => {
    try {
      this.setState({ isAdding: true });
      const id = await this.props.onAddItem(customer);
      this.showAddToast(id);

      Metrics.clientsCreateSuccess({ ...Metrics.variablesFromCustomer(customer, "list"), cardid: id });
    } catch (e) {
      Toast.push("Ошибка при сохранении");
    } finally {
      this.setState({ isAdding: false });
    }
  };

  private showAddToast(guid: string) {
    const to = `${AppDataSingleton.prefix}/${ListType.Customers}/${guid}`;

    Toast.push("Клиент добавлен", {
      label: "Показать",
      handler: () => {
        this.props.history.push(to);
        Toast.close();
      },
    });
  }

  private renderOnBoard() {
    return (
      <ContentLayoutInner>
        <Caption type={CaptionType.H1}>Клиенты</Caption>
        <div style={{ height: 40 }} />
        <OnBoardingComponent
          onFinish={this.props.updateSetting}
          startText="Начать работу с базой клиентов"
          pages={[
            {
              headerTextLines: [
                "Ведите клиентскую базу, чтобы делать персональные скидки",
                "и быстро записывать постоянных клиентов в заказ или календарь",
              ],
              imageUrl: CustomersBoard1,
              footerTextLines: [
                "Клиенту можно дать индивидуальную скидку.",
                "Мы планируем развивать систему лояльности.",
              ],
            },
            {
              headerTextLines: [
                "Ведите клиентскую базу, чтобы делать персональные скидки",
                "и быстро записывать постоянных клиентов в заказ или календарь",
              ],
              imageUrl: CustomersBoard2,
              footerTextLines: [
                "В новом приложении Контур.Касса можно указывать постоянных",
                "клиентов, скидка применится автоматически",
              ],
            },
            {
              headerTextLines: [
                "Ведите клиентскую базу, чтобы делать персональные скидки",
                "и быстро записывать постоянных клиентов в заказ или календарь",
              ],
              imageUrl: CustomersBoard3,
              footerTextLines: [
                "Быстрая запись клиентов в календарь или заказ-наряд.",
                "Позже можно будет следить за историей посещений",
              ],
            },
          ]}
        />
      </ContentLayoutInner>
    );
  }

  private renderTable(data: CustomerDto[], filteredDataLength: number = 0): JSX.Element | null {
    if (!data || !data.length) {
      return null;
    }

    return (
      <div>
        <div className={styles.tableHading}>
          <Grid columns={this.getGridColumns()}>
            <Caption type={CaptionType.GraySmall}>
              {StringHelper.getNumberCase(filteredDataLength, "клиент", "клиента", "клиентов")}
            </Caption>
            {this.isCustomCardsUsing() ? <Caption type={CaptionType.GraySmall}>Карта</Caption> : null}
            <Caption type={CaptionType.GraySmall}>Телефон</Caption>
            <Caption type={CaptionType.GraySmall} align="right">
              Скидка
            </Caption>
          </Grid>
        </div>
        <div>{data.map(this.renderItem)}</div>
      </div>
    );
  }

  private renderItem = (item: CustomerDto, i: number): JSX.Element | null => {
    if (!item) {
      return null;
    }

    const nameClassNames = classnames(styles.name, !item.name && styles.nameDimmed);

    return (
      <Link
        data-tid="CustomerItem"
        to={`${AppDataSingleton.prefix}/${this.props.itemPathPrefix}/${item.id}`}
        key={i}
        className={styles.item}
      >
        <div className={styles.itemInner}>
          <Grid columns={this.getGridColumns()}>
            <div data-tid="Name" className={nameClassNames}>
              {item.name || "Имя не указано"}
            </div>
            {this.isCustomCardsUsing() ? (
              <Caption data-tid="CustomId" type={!item.customId ? CaptionType.Gray : undefined}>
                {item.customId || "не указана"}
              </Caption>
            ) : null}
            <Caption data-tid="Phone" type={!item.phone ? CaptionType.Gray : undefined}>
              {StringHelper.formatPhoneString(item.phone) || "не указан"}
            </Caption>
            <Caption data-tid="Discount" type={!item.discount ? CaptionType.Gray : undefined} align="right">
              {StringHelper.printDiscountString(item.discount)}
            </Caption>
          </Grid>
        </div>
      </Link>
    );
  };

  private filterBySearchString(): CustomerDto[] {
    if (!this.props.data) {
      return [];
    }

    return this.state.searchString
      ? this.getSearcher(this.props.data).search(this.state.searchString)
      : this.props.data;
  }

  private sliceByPaging(data?: CustomerDto[]): CustomerDto[] {
    return data ? PagingHelper.getSlice(data, PERSONS_PER_PAGE, this.state.skip, this.state.take) : [];
  }

  private getGridColumns = () => (this.isCustomCardsUsing() ? [210, 130, 140, 50] : [330, 0, 145, 55]);

  private isCustomCardsUsing = () => this.props.data && this.props.data.some((i: CustomerDto) => !!(i && i.customId));

  private onSearch = (searchString?: string) => this.setState({ searchString });

  private bindPersonList(filteredData: CustomerDto[]): IPersonsListProps<CustomerDto> {
    const isEmptyList = this.props.hideCustomersOnBoard === null || !this.props.data || !this.props.data.length;
    const editorComponent = (
      <CustomerEditor
        disableActionButtons={this.state.isAdding}
        heading="Новый клиент"
        acceptButtonCaption="Добавить"
      />
    );
    const emptyCaption = (
      <span>
        Добавьте клиентов, чтобы работать
        <br /> с календарем записи и скидками
      </span>
    );

    // TODO: think about more simple and friendly using of loading

    return {
      heading: "Клиенты",
      addButtonCaption: "Новый клиент",
      notFoundCaption: "Клиенты не найдены",
      searchPlaceholder: "Введите имя, телефон или номер карты",
      loaderCaption: "Загружаем список клиентов",
      isEmptyList,
      editorComponent,
      emptyCaption,
      onAdd: this.onAdd,
      dataLength: filteredData && filteredData.length,
      onSearch: this.onSearch,
      onPageChanged: (skip: number, take: number) => this.setState({ skip, take }),
      dataLoaded: this.props.hideCustomersOnBoard !== null && !!this.props.data,
      wideList: true,
      linkCaption: "Работа с базой клиентов",
      linkHint: "Где и как использовать базу клиентов",
      linkRef: "https://egais.userecho.com/knowledge-bases/2/articles/5209-kak-vesti-bazu-klientov",
    };
  }
}
