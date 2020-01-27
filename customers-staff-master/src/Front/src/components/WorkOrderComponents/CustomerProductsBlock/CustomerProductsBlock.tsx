import * as React from "react";
import { CustomerProductDto } from "../../../api/new/dto/WorkOrder/CustomerProductDto";
import { SpoilerBlock } from "../SpoilerBlock/SpoilerBlock";
import { ColumnType, IColumnDescr, TableBlock } from "../TableBlock/TableBlock";
import * as styles from "./CustomerProductsBlock.less";

export interface IWorkOrderProductsBlockProps {
  products: CustomerProductDto[];
  onHeightUpdated?: () => void;
}

export class CustomerProductsBlock extends React.Component<
  IWorkOrderProductsBlockProps,
  {
    localProducts: CustomerProductDto[];
  }
> {
  private SpoilerBlock: SpoilerBlock | null;
  private TableBlock: TableBlock<CustomerProductDto> | null;

  private columns: IProductColumnDescr[] = [
    {
      key: "name",
      align: "left",
      columnType: ColumnType.String,
      placeholder: "Начните вводить название материала",
      activeInLastRow: true,
      maxLength: 100,
      shouldValidate: item => item.quantity.trim() !== "",
      dataTid: "Name",
    },
    {
      key: "quantity",
      name: "Количество",
      width: 120,
      align: "right",
      columnType: ColumnType.String,
      placeholder: "—",
      maxLength: 14,
      shouldValidate: item => item.name.trim() !== "",
      dataTid: "Quantity",
    },
  ];

  constructor(props: IWorkOrderProductsBlockProps, state: {}) {
    super(props, state);

    this.state = {
      localProducts: props.products,
    };
  }

  public async isValid(): Promise<boolean> {
    const localProducts = this.state.localProducts;
    const isError = localProducts.reduce(
      (res, x) => res || !x.name || !x.name.trim() || !x.quantity || !x.quantity.trim(),
      false
    );
    if (isError) {
      this.SpoilerBlock && this.SpoilerBlock.open();
    } else {
      return true;
    }

    return !!this.TableBlock && (await this.TableBlock.validate());
  }

  public getCustomerProducts(): CustomerProductDto[] {
    return this.state.localProducts
      .filter(x => x.name.trim() !== "")
      .map(x => {
        return {
          name: x.name ? x.name.trim() : "",
          quantity: x.quantity.trim() || "",
        };
      });
  }

  public render(): JSX.Element {
    const { onHeightUpdated } = this.props;

    return (
      <SpoilerBlock
        ref={el => (this.SpoilerBlock = el)}
        caption="Материалы заказчика"
        captionElements={this.renderCaption()}
        openChanged={() => onHeightUpdated && onHeightUpdated()}
        data-tid="CustomerProductsBlock"
      >
        <div className={styles.table}>
          <TableBlock<CustomerProductDto>
            ref={el => (this.TableBlock = el)}
            columns={this.columns}
            data={this.state.localProducts}
            onChange={this.onChange}
            onRemove={this.onRemove}
          />
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

  private onRemove = (idx: number) => {
    const localProducts = this.state.localProducts;
    localProducts.splice(idx, 1);
    this.setState({ localProducts });
  };

  private onChange = (idx: number, key: keyof CustomerProductDto, value: string) => {
    const localProducts = this.state.localProducts;
    let product: CustomerProductDto;

    if (idx >= localProducts.length) {
      product = { name: "", quantity: "" };
      localProducts.push(product);
      this.props.onHeightUpdated && this.props.onHeightUpdated();
    } else {
      product = localProducts[idx];
    }

    product[key] = value;
    this.setState({ localProducts });
  };
}

interface IProductColumnDescr extends IColumnDescr<CustomerProductDto> {
  name?: string;
}
