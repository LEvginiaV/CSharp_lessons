import CurrencyLabel from "@skbkontur/react-ui/CurrencyLabel";
import memoizeOne from "memoize-one";
import * as React from "react";
import { Guid } from "../../../api/new/dto/Guid";
import { WorkerDto } from "../../../api/new/dto/WorkerDto";
import { ShopServiceDto } from "../../../api/new/dto/WorkOrder/ShopServiceDto";
import { NumberHelper } from "../../../common/NumberHelper";
// @ts-ignore
import PocketElasticSearcher from "../../../common/pocket-elasticsearch";
import { NomenclatureCard, ProductCategory } from "../../../typings/NomenclatureCard";
import { SpoilerBlock } from "../SpoilerBlock/SpoilerBlock";
import { CardItem, ColumnType, IColumnDescr, TableBlock } from "../TableBlock/TableBlock";
import * as styles from "./WorkOrderServicesBlock.less";

export interface IWorkOrderServicesBlockProps {
  services: ShopServiceDto[];
  workers: WorkerDto[];
  onHeightUpdated?: () => void;
  onTotalSumUpdated: (totalSum?: number) => void;
  cards: NomenclatureCard[];
}

interface ExtendedShopServiceDto extends Partial<ShopServiceDto> {
  totalPrice?: number;
}

export class WorkOrderServicesBlock extends React.Component<
  IWorkOrderServicesBlockProps,
  {
    localServices: ExtendedShopServiceDto[];
    totalSum?: number;
  }
> {
  private SpoilerBlock: SpoilerBlock | null;
  private TableBlock: TableBlock<ExtendedShopServiceDto> | null;

  private columns: IServiceColumnDescr[] = [
    {
      key: "productId",
      align: "left",
      columnType: ColumnType.Card,
      placeholder: "Начните вводить название услуги",
      activeInLastRow: true,
      additionalInfo: {
        search: query => this.searchService(query),
        render: id => this.renderItem(id),
      },
      dataTid: "CardName",
    },
    {
      key: "quantity",
      name: "Количество",
      width: 120,
      align: "right",
      columnType: ColumnType.Quantity,
      autofocus: true,
      placeholder: "—",
      additionalInfo: {
        getUnitType: () => undefined,
      },
      dataTid: "Quantity",
    },
    {
      key: "price",
      name: "Цена, ₽",
      width: 120,
      align: "right",
      columnType: ColumnType.Price,
      placeholder: "—",
      dataTid: "Price",
    },
    {
      key: "totalPrice",
      name: "Сумма, ₽",
      width: 120,
      align: "right",
      columnType: ColumnType.PriceLabel,
      placeholder: "—",
      dataTid: "TotalPrice",
    },
    {
      key: "workerId",
      name: "Исполнитель",
      width: 315,
      align: "left",
      columnType: ColumnType.Worker,
      placeholder: "—",
      dataTid: "Worker",
    },
  ];

  private searcher = memoizeOne((cards: NomenclatureCard[]) => {
    const searcher = new PocketElasticSearcher(cards.filter(x => x.productCategory === ProductCategory.Service), [
      "name",
    ]);
    return (query: string) => searcher.search(query);
  });

  constructor(props: IWorkOrderServicesBlockProps, state: {}) {
    super(props, state);

    const localServices: ExtendedShopServiceDto[] = props.services;
    localServices.forEach(x => this.computeTotalPrice(x));
    const totalSum = this.computeTotalSum(localServices);

    this.state = {
      localServices,
      totalSum,
    };
  }

  public async isValid(): Promise<boolean> {
    const localServices = this.state.localServices;
    const isError = localServices.reduce((res, x) => res || !x.price || !x.quantity, false);
    if (isError) {
      this.SpoilerBlock && this.SpoilerBlock.open();
    } else {
      return true;
    }

    return !!this.TableBlock && (await this.TableBlock.validate());
  }

  public componentDidMount(): void {
    this.props.onTotalSumUpdated(this.state.totalSum);
  }

  public getShopServices(): ShopServiceDto[] {
    return this.state.localServices.map(x => {
      return {
        productId: x.productId || "",
        quantity: x.quantity || 0,
        price: x.price,
        workerId: x.workerId,
      };
    });
  }

  public render(): JSX.Element {
    const hasWorkers = !!this.props.workers.length;
    const { onHeightUpdated } = this.props;

    return (
      <SpoilerBlock
        ref={el => (this.SpoilerBlock = el)}
        caption="Работы и услуги"
        captionElements={this.renderCaption(hasWorkers)}
        openChanged={() => onHeightUpdated && onHeightUpdated()}
        data-tid="ServicesBlock"
      >
        <div>
          <div className={hasWorkers ? styles.table : styles.tableWithoutWorkers}>
            <TableBlock<ExtendedShopServiceDto>
              ref={el => (this.TableBlock = el)}
              columns={hasWorkers ? this.columns : this.columns.slice(0, this.columns.length - 1)}
              data={this.state.localServices}
              workers={this.props.workers}
              onChange={this.onChange}
              onChangeNumber={this.onChangeNumber}
              onRemove={this.onRemove}
            />
          </div>
          {this.renderTotalSum()}
        </div>
      </SpoilerBlock>
    );
  }

  private getSearcher(): (query: string) => NomenclatureCard[] {
    return this.searcher(this.props.cards);
  }

  private renderCaption(hasWorkers: boolean): JSX.Element {
    return (
      <div className={styles.row}>
        {this.columns.slice(1).map((x, i) => (
          <div className={styles.item} style={{ width: x.width, textAlign: x.align }} key={i}>
            {hasWorkers || i < this.columns.length - 2 ? x.name : ""}
          </div>
        ))}
      </div>
    );
  }

  private renderTotalSum(): JSX.Element | undefined {
    const { totalSum } = this.state;
    return totalSum ? (
      <div className={styles.totalSum}>
        Стоимость работ
        <span className={styles.price}>
          <CurrencyLabel value={totalSum} data-tid="TotalSum" />
        </span>
      </div>
    ) : (
      undefined
    );
  }

  private renderItem(id: Guid): JSX.Element {
    const card = this.props.cards.find(x => x.id === id);
    return <span>{card ? card.name : "?"}</span>;
  }

  private searchService(query: string): CardItem[] {
    return this.getSearcher()(query)
      .map(x => ({ id: x.id, label: x.name }))
      .slice(0, 10);
  }

  private onRemove = (idx: number) => {
    const localServices = this.state.localServices;
    localServices.splice(idx, 1);
    const totalSum = this.computeTotalSum(localServices);
    this.setState({ localServices, totalSum });
  };

  private onChange = (idx: number, key: keyof ExtendedShopServiceDto, value?: string) => {
    const localServices = this.state.localServices;

    let service: ExtendedShopServiceDto;

    if (idx >= localServices.length) {
      if (key !== "productId" || !value) {
        throw new Error("unexpected data");
      }
      const card = this.props.cards.find(x => x.id === value);
      service = { productId: value, price: card ? card.prices.sellPrice || undefined : undefined };
      localServices.push(service);
      this.props.onHeightUpdated && this.props.onHeightUpdated();
    } else {
      service = localServices[idx];
    }

    service[key] = value;
    this.setState({ localServices });
  };

  private onChangeNumber = (idx: number, key: keyof ExtendedShopServiceDto, value?: number) => {
    const localServices = this.state.localServices;
    const service = localServices[idx];
    service[key] = value;
    this.computeTotalPrice(service);
    const totalSum = this.computeTotalSum(localServices);

    this.setState({ localServices, totalSum });
  };

  private computeTotalPrice(service: ExtendedShopServiceDto) {
    if (service.price && service.quantity) {
      service.totalPrice = NumberHelper.round(service.price * service.quantity);
    } else {
      service.totalPrice = undefined;
    }
  }

  private computeTotalSum(localServices: ExtendedShopServiceDto[]): number | undefined {
    let totalSum =
      localServices && localServices.length
        ? localServices.map(x => x.totalPrice).reduce((sum, x) => (sum ? sum + (x || 0) : x))
        : undefined;

    totalSum = totalSum && NumberHelper.round(totalSum);

    if (!this.state) {
      return totalSum;
    }

    if ((!!totalSum && !this.state.totalSum) || (!totalSum && !!this.state.totalSum)) {
      this.props.onHeightUpdated && this.props.onHeightUpdated();
    }
    if (totalSum !== this.state.totalSum) {
      this.props.onTotalSumUpdated(totalSum);
    }

    return totalSum;
  }
}

interface IServiceColumnDescr extends IColumnDescr<ExtendedShopServiceDto> {
  name?: string;
}
