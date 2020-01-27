import { Delete } from "@skbkontur/react-icons";
import * as React from "react";
import * as styles from "./Feedback.less";
import { FeedbackType } from "./FeedbackType";

export interface FeedbackProps {
  userId: string;
  shopId: string;
  type: FeedbackType;
  onClose: (name: string) => void;
  onClick: (name: string) => void;
}

export class Feedback extends React.Component<FeedbackProps, {}> {
  public render(): JSX.Element {
    const { type, onClose } = this.props;
    const { description, url } = this._getMeta();

    return (
      <div className={styles.root}>
        <div className={styles.description} onClick={() => this._onClick(url)}>
          {description}&nbsp;
          <div className={styles.link}>Ответьте на опрос</div>
        </div>
        <div className={styles.closeIcon}>
          <Delete color={"white"} size={20} onClick={() => onClose(type.toString())} />
        </div>
      </div>
    );
  }

  private _onClick = (url: string): void => {
    const { type, onClick } = this.props;
    onClick(type);
    window.open(url, "_blank");
  };

  private _getMeta = () => {
    const { userId, shopId } = this.props;

    switch (this.props.type) {
      case FeedbackType.WorkersCardsFeedback:
        return {
          description: "Помогите сделать карточку сотрудника лучше.",
          url: `https://kontur.typeform.com/to/RWY6PO?userid=${userId}&salespointid=${shopId}`,
        };
      case FeedbackType.CustomersCardsFeedback:
        return {
          description: "Помогите сделать карточку клиента лучше.",
          url: `https://kontur.typeform.com/to/KdFkYn?userid=${userId}&salespointid=${shopId}`,
        };
      case FeedbackType.CalendarsFeedback:
        return {
          description: "Помогите сделать календарь лучше.",
          url: `https://kontur.typeform.com/to/Ge7CfB?userid=${userId}&salespointid=${shopId}`,
        };
      case FeedbackType.WorkersSchedulesFeedback:
        return {
          description: "Помогите сделать график работы лучше.",
          url: `https://kontur.typeform.com/to/Q3OSxr?userid=${userId}&salespointid=${shopId}`,
        };
      case FeedbackType.WorkOrdersFeedback:
        return {
          description: "Помогите сделать заказ-наряд лучше.",
          url: `https://kontur.typeform.com/to/X5hVep?userid=${userId}&salespointid=${shopId}`,
        };
    }
  };
}
