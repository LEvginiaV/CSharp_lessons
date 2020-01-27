import CurrencyLabel from "@skbkontur/react-ui/CurrencyLabel";
import memoizeOne from "memoize-one";
import * as React from "react";
import { Guid } from "../../../api/new/dto/Guid";
import { ShopProductDto } from "../../../api/new/dto/WorkOrder/ShopProductDto";
import { NumberHelper } from "../../../common/NumberHelper";
// @ts-ignore
import PocketElasticSearcher from "../../../common/pocket-elasticsearch";
import { NomenclatureCard, ProductCategory, UnitType } from "../../../typings/NomenclatureCard";
import { SpoilerBlock } from "../SpoilerBlock/SpoilerBlock";
import { CardItem, ColumnType, IColumnDescr, TableBlock } from "../TableBlock/TableBlock";
import * as styles from "./WorkOrderProductsBlock.less";

export interface IWorkOrderProductsBlockProps {
  products: ShopProductDto[];
  onHeightUpdated?: () => void;
  servicesTotalSum?: number;
  cards: NomenclatureCard[];
}

interface ExtendedShopProductDto extends Partial<ShopProductDto> {
  totalPrice?: number;
}

export class WorkOrderProductsBlock extends React.Component<
  IWorkOrderProductsBlockProps,
  {
    localProducts: ExtendedShopProductDto[];
    totalSum?: number;
  }
> {
  private SpoilerBlock: SpoilerBlock | null;
  private TableBlock: TableBlock<ExtendedShopProductDto> | null;

  private columns: IProductColumnDescr[] = [
    {
      key: "productId",
      align: "left",
      columnType: ColumnType.Card,
      placeholder: "Начните вводить название материала",
      activeInLastRow: true,
      additionalInfo: {
        search: query => this.searchProduct(query),
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
        getUnitType: item => (item && item.productId ? this.getUnitType(item.productId) : UnitType.Piece),
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
  ];

  private searcher = memoizeOne((cards: NomenclatureCard[]) => {
    const searcher = new PocketElasticSearcher(
      cards.filter(
        x =>
          x.productCategory === ProductCategory.NonAlcoholic ||
          x.productCategory === ProductCategory.Unknown ||
          x.productCategory === ProductCategory.Tobacco ||
          x.productCategory === ProductCategory.AnotherExcise
      ),
      ["name"]
    );
    return (query: string) => searcher.search(query);
  });

  constructor(props: IWorkOrderProductsBlockProps, state: {}) {
    super(props, state);

    const localProducts: ExtendedShopProductDto[] = props.products;
    localProducts.forEach(x => this.computeTotalPrice(x));
    const totalSum = this.computeTotalSum(localProducts);

    this.state = {
      localProducts,
      totalSum,
    };
  }

  public async isValid(): Promise<boolean> {
    const localProducts = this.state.localProducts;
    const isError = localProducts.reduce((res, x) => res || !x.price || !x.quantity, false);
    if (isError) {
      this.SpoilerBlock && this.SpoilerBlock.open();
    } else {
      return true;
    }

    return !!this.TableBlock && (await this.TableBlock.validate());
  }

  public getShopProducts(): ShopProductDto[] {
    return this.state.localProducts.map(x => {
      return {
        productId: x.productId || "",
        quantity: x.quantity || 0,
        price: x.price,
      };
    });
  }

  public render(): JSX.Element {
    const { onHeightUpdated } = this.props;
    return (
      <SpoilerBlock
        ref={el => (this.SpoilerBlock = el)}
        caption="Материалы исполнителя"
        captionElements={this.renderCaption()}
        openChanged={() => onHeightUpdated && onHeightUpdated()}
        data-tid="ProductsBlock"
      >
        <div>
          <div className={styles.table}>
            <TableBlock<ExtendedShopProductDto>
              ref={el => (this.TableBlock = el)}
              columns={this.columns}
              data={this.state.localProducts}
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

  private renderCaption(): JSX.Element {
    return (
      <div className={styles.row}>
        {this.columns.slice(1).map((x, i) => (
          <div className={styles.item} style={{ width: x.width, textAlign: x.align }} key={i}>
            {x.name}
          </div>
        ))}
      </div>
    );
  }

  private renderTotalSum(): JSX.Element | undefined {
    const { totalSum } = this.state;
    const { servicesTotalSum } = this.props;
    return totalSum ? (
      <div className={styles.totalSum}>
        <div className={styles.sumForProducts}>
          Стоимость материалов
          <span className={styles.price}>
            <CurrencyLabel value={totalSum} data-tid="TotalSum" />
          </span>
        </div>
        {servicesTotalSum && (
          <div className={styles.sumForServicesAndProducts}>
            Итого за работы и материалы
            <span className={styles.price}>
              <CurrencyLabel
                value={NumberHelper.round(totalSum + servicesTotalSum)}
                data-tid="SumForServicesAndProducts"
              />
            </span>
          </div>
        )}
      </div>
    ) : (
      undefined
    );
  }

  private renderItem(id: Guid): JSX.Element {
    const card = this.props.cards.find(x => x.id === id);
    return <span>{card ? card.name : "?"}</span>;
  }

  private getSearcher(): (query: string) => NomenclatureCard[] {
    return this.searcher(this.props.cards);
  }

  private searchProduct(query: string): CardItem[] {
    return this.getSearcher()(query)
      .map(x => ({ id: x.id, label: x.name }))
      .slice(0, 10);
  }

  private getUnitType: (id: Guid) => UnitType | undefined = id => {
    const card = this.props.cards.find(x => x.id === id);
    return card ? card.unitType : undefined;
  };

  private onRemove = (idx: number) => {
    const localProducts = this.state.localProducts;
    localProducts.splice(idx, 1);
    const totalSum = this.computeTotalSum(localProducts);
    this.setState({ localProducts, totalSum });
  };

  private onChange = (idx: number, key: keyof ExtendedShopProductDto, value?: string) => {
    const localProducts = this.state.localProducts;
    let product: ExtendedShopProductDto;

    if (idx >= localProducts.length) {
      if (key !== "productId" || !value) {
        throw new Error("unexpected data");
      }
      const card = this.props.cards.find(x => x.id === value);
      product = { productId: value, price: card ? card.prices.sellPrice || undefined : undefined };
      localProducts.push(product);
      this.props.onHeightUpdated && this.props.onHeightUpdated();
    } else {
      product = localProducts[idx];
    }

    product[key] = value;
    this.setState({ localProducts });
  };

  private onChangeNumber = (idx: number, key: keyof ExtendedShopProductDto, value?: number) => {
    const localProducts = this.state.localProducts;
    const service = localProducts[idx];
    service[key] = value;
    this.computeTotalPrice(service);
    const totalSum = this.computeTotalSum(localProducts);

    this.setState({ localProducts, totalSum });
  };

  private computeTotalPrice(product: ExtendedShopProductDto) {
    if (product.price && product.quantity) {
      const totalPrice = NumberHelper.round(product.price * product.quantity);
      product.totalPrice = totalPrice;
    } else {
      product.totalPrice = undefined;
    }
  }

  private computeTotalSum(localProducts: ExtendedShopProductDto[]): number | undefined {
    let totalSum =
      localProducts && localProducts.length
        ? localProducts.map(x => x.totalPrice).reduce((sum, x) => (sum ? sum + (x || 0) : x))
        : undefined;

    totalSum = totalSum && NumberHelper.round(totalSum);

    if (!this.state) {
      return totalSum;
    }
    if ((!!totalSum && !this.state.totalSum) || (!totalSum && !!this.state.totalSum)) {
      this.props.onHeightUpdated && this.props.onHeightUpdated();
    }

    return totalSum;
  }
}

interface IProductColumnDescr extends IColumnDescr<ExtendedShopProductDto> {
  name?: string;
}
