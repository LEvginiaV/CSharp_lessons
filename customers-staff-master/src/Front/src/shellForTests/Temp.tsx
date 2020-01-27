import Button from "@skbkontur/react-ui/components/Button/Button";
import Input from "@skbkontur/react-ui/components/Input/Input";
import * as React from "react";
import { CustomerApi } from "../api/new/CustomerApi";
import { GenderType } from "../models/GenderType";

//tslint:disable

export interface TempState {
  shopId: string;
}

export class Temp extends React.Component<{}, TempState> {
  constructor(props: {}, context: any) {
    super(props, context);
    this.state = {
      shopId: "",
    };
  }

  public render() {
    return (
      <div>
        <Input data-tid="ShopIdInput" onChange={(_, v) => this.setState({ shopId: v })} value={this.state.shopId} />
        <Button data-tid="TryButton" onClick={this.handleTemp}>
          ЖМИ
        </Button>
      </div>
    );
  }

  private handleTemp = async () => {
    const api = new CustomerApi("/customersApi", this.state.shopId);
    let list = await api.readAll();
    console.log("list", list);
    const client = await api.create({
      name: "Федор Фоминых",
      email: "fedorfo@yandex.ru",
      additionalInfo: "Постоянный клиент",
      discount: 4,
      birthday: {
        day: 9,
        month: 6,
      },
      customId: "12345",
      phone: "+79826224186",
      gender: GenderType.Male,
    });
    console.log("client", client);
    list = await api.readAll(0);
    console.log(list);
  };
}
