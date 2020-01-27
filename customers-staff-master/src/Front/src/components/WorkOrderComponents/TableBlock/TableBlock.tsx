import Icon from "@skbkontur/react-icons";
import { ValidationContainer, ValidationWrapperV1 } from "@skbkontur/react-ui-validations";
import ComboBox from "@skbkontur/react-ui/ComboBox";
import CurrencyInput from "@skbkontur/react-ui/CurrencyInput";
import CurrencyLabel from "@skbkontur/react-ui/CurrencyLabel";
import Hint from "@skbkontur/react-ui/Hint";
import Input from "@skbkontur/react-ui/Input";
import Link from "@skbkontur/react-ui/Link";
import * as React from "react";
import { Guid } from "../../../api/new/dto/Guid";
import { WorkerDto } from "../../../api/new/dto/WorkerDto";
import { StringHelper } from "../../../helpers/StringHelper";
import { UnitType } from "../../../typings/NomenclatureCard";
import { WorkOrderValidationHelper } from "../../../validation/WorkOrderValidationHelper";
import { WorkerSelectorDropdown } from "../WorkerSelectorDropdown/WorkerSelectorDropdown";
import { Placeholder } from "./Placeholder";
import * as styles from "./TableBlock.less";

export enum ColumnType {
  Card = "Card",
  String = "String",
  Quantity = "Quantity",
  Price = "Price",
  PriceLabel = "PriceLabel",
  Worker = "Worker",
}

export interface CardItem {
  id: Guid;
  label: string;
}

export interface ICardInfo {
  search: (text: string) => CardItem[];
  render: (id: Guid) => JSX.Element;
}

export interface IQuantityInfo<T> {
  getUnitType: (item?: T) => UnitType | undefined;
}

export interface IColumnDescr<T> {
  key: keyof T;
  width?: number;
  align: "left" | "right";
  columnType: ColumnType;
  placeholder?: string;
  additionalInfo?: ICardInfo | IQuantityInfo<T>;
  activeInLastRow?: boolean;
  maxLength?: number;
  autofocus?: boolean;
  shouldValidate?: (item: T) => boolean;
  dataTid?: string;
}

export interface ITableBlockProps<T> {
  columns: Array<IColumnDescr<T>>;
  data: T[];
  workers?: WorkerDto[];
  onChange: (idx: number, field: keyof T, value?: string) => void;
  onChangeNumber?: (idx: number, field: keyof T, value?: number) => void;
  onRemove: (idx: number) => void;
}

export class TableBlock<T> extends React.Component<ITableBlockProps<T>> {
  private ValidationContainer: ValidationContainer | null;
  private Mounted: boolean = false;

  public componentDidMount(): void {
    this.Mounted = true;
  }

  public componentWillUnmount(): void {
    this.Mounted = false;
  }

  public async validate(): Promise<boolean> {
    return !!this.ValidationContainer && (await this.ValidationContainer.validate());
  }

  public render(): JSX.Element {
    const { data } = this.props;

    return (
      <ValidationContainer ref={el => (this.ValidationContainer = el)}>
        <div>{[...data, undefined].map((x, i) => this.renderItem(i, x))}</div>
      </ValidationContainer>
    );
  }

  private renderItem(idx: number, item?: T): JSX.Element {
    const { columns } = this.props;
    return (
      <div key={idx} className={styles.rowWrapper} data-tid="RowItem">
        <div className={styles.row}>
          <div className={styles.index}>{idx + 1}</div>
          {columns.map((column, i) => (
            <div
              className={styles.item}
              style={{
                flexGrow: column.width ? undefined : 1,
                flexShrink: column.width ? undefined : 1,
                flexBasis: column.width ? undefined : 0,
                alignContent: column.align,
              }}
              key={i}
            >
              {this.renderField(idx, column, item)}
            </div>
          ))}
          <div className={styles.item}>
            {item && (
              <Hint text="удалить">
                <Link onClick={() => this.props.onRemove(idx)} use="grayed" data-tid="RemoveLink">
                  <Icon name="Trash" />
                </Link>
              </Hint>
            )}
          </div>
        </div>
      </div>
    );
  }

  private renderField(idx: number, column: IColumnDescr<T>, item?: T): JSX.Element {
    const { onChange } = this.props;
    const fieldProps = {
      width: column.width || "100%",
      align: column.align,
      placeholder: column.placeholder,
      selectAllOnFocus: true,
      borderless: true,
      maxLength: column.maxLength,
    };

    if (!item && !column.activeInLastRow) {
      return (
        <Placeholder
          {...fieldProps}
          value={column.placeholder}
          marginLeft={column.columnType === ColumnType.Worker ? 1 : undefined}
        />
      );
    }

    switch (column.columnType) {
      case ColumnType.String:
        const str = this.getStringValue(item, column.key);
        return (
          <ValidationWrapperV1
            {...WorkOrderValidationHelper.bindStringNonEmptyValidation(
              item && (!column.shouldValidate || column.shouldValidate(item)) ? str : "-",
              column.dataTid + "Validation"
            )}
          >
            <Input
              {...fieldProps}
              value={str || ""}
              borderless
              onChange={(_, v) => onChange(idx, column.key, v)}
              data-tid={column.dataTid}
            />
          </ValidationWrapperV1>
        );
      case ColumnType.Worker:
        return (
          <WorkerSelectorDropdown
            {...fieldProps}
            workers={this.props.workers || []}
            selectedWorkerId={this.getStringValue(item, column.key)}
            caption={""}
            onChange={workerId => {
              onChange(idx, column.key, workerId);
            }}
            data-tid={column.dataTid}
          />
        );
      case ColumnType.Card:
        const cardInfo = column.additionalInfo as ICardInfo;
        const productId = this.getStringValue(item, column.key);
        return productId ? (
          <div className={styles.cardItem} data-tid={column.dataTid}>
            {cardInfo.render(productId)}
          </div>
        ) : (
          <ComboBox<CardItem>
            {...fieldProps}
            getItems={text => Promise.resolve(cardInfo.search(text))}
            onUnexpectedInput={() => null}
            drawArrow={false}
            onChange={(_, v) => {
              v && onChange(idx, column.key, v.id);
            }}
            data-tid={column.dataTid + "Selector"}
          />
        );
      case ColumnType.Quantity:
        const qInfo = column.additionalInfo as IQuantityInfo<T>;
        const unitType = qInfo.getUnitType(item);
        const digits = !!unitType && unitType !== UnitType.Piece ? 3 : 0;
        const numValue = this.getNumberValue(item, column.key);
        return this.renderCurrencyInput(idx, column, numValue, digits, StringHelper.formatUnitType(unitType));
      case ColumnType.Price:
        return this.renderCurrencyInput(idx, column, this.getNumberValue(item, column.key), 2);
      case ColumnType.PriceLabel:
        const num = this.getNumberValue(item, column.key);
        return num ? (
          <div className={styles.priceLabel} style={{ ...fieldProps, textAlign: column.align }}>
            <CurrencyLabel {...fieldProps} value={num} fractionDigits={2} data-tid={column.dataTid} />
          </div>
        ) : (
          <Placeholder {...fieldProps} value={column.placeholder} data-tid={column.dataTid} />
        );
    }
  }

  private getNumberValue(item: T | undefined, key: keyof T): number | undefined {
    return !!item && typeof item[key] === "number" ? Number(item[key]) : undefined;
  }

  private getStringValue(item: T | undefined, key: keyof T): string | undefined {
    return !!item && typeof item[key] === "string" ? String(item[key]) : undefined;
  }

  private renderCurrencyInput(idx: number, column: IColumnDescr<T>, value?: number, digits?: number, suffix?: string) {
    const fieldProps = {
      width: column.width || "100%",
      align: column.align,
      placeholder: column.placeholder,
      selectAllOnFocus: true,
      borderless: true,
      autoFocus: this.Mounted && column.autofocus,
    };

    return (
      <ValidationWrapperV1 {...WorkOrderValidationHelper.bindNumberValidation(value, column.dataTid + "Validation")}>
        <CurrencyInput
          {...fieldProps}
          value={value}
          maxLength={13 + (digits ? digits + 1 : 0)}
          onChange={(_, v) => this.onChangeNumber(idx, column.key, v)}
          fractionDigits={digits || 0}
          suffix={suffix ? <span className={styles.unitType}>{suffix}</span> : undefined}
          data-tid={column.dataTid}
        />
      </ValidationWrapperV1>
    );
  }

  private onChangeNumber = (idx: number, key: keyof T, v: number | null | undefined) => {
    const { onChangeNumber } = this.props;
    onChangeNumber && onChangeNumber(idx, key, v !== null ? v : undefined);
  };
}
