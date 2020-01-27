import Button from "@skbkontur/react-ui/components/Button/Button";
import Checkbox from "@skbkontur/react-ui/components/Checkbox/Checkbox";
import Input from "@skbkontur/react-ui/components/Input/Input";
import Toast from "@skbkontur/react-ui/Toast";
import { get, set } from "browser-cookies";
import * as React from "react";
import { CustomerApi } from "../api/new/CustomerApi";
import { Guid } from "../api/new/dto/Guid";
import * as styles from "./Auth.less";

export interface AuthProps {
  onAuth: (state: AuthState) => any;
}

export interface AuthState {
  authSid: string;
  shopId: Guid;
  useRealApi: boolean;
  useFeedback: boolean;
}

export class Auth extends React.Component<AuthProps, AuthState> {
  constructor(props: AuthProps, context: any) {
    super(props, context);
    const auth = this.getAuthParams();
    const authSid = get("auth.sid") || "";
    const shopId = get("shopId") || "";
    if (auth.authSid !== "" && auth.shopId !== "" && auth.shopId !== shopId && auth.authSid !== authSid) {
      set("auth.sid", "");
      set("shopId", "");
    }
    this.state = {
      shopId: auth.shopId,
      authSid: auth.authSid,
      useRealApi: true,
      useFeedback: false,
    };
  }

  public render() {
    return (
      <div className={styles.container}>
        <table>
          <tbody>
            <tr>
              <td className={styles.label}>Api</td>
              <td>
                <Checkbox
                  data-tid={"UseRealApiCheckbox"}
                  checked={this.state.useRealApi}
                  onChange={(_, v) => {
                    this.setState({ useRealApi: v });
                  }}
                >
                  Использовать настоящее API
                </Checkbox>
              </td>
            </tr>
            <tr>
              <td className={styles.label}>Feedback</td>
              <td>
                <Checkbox
                  data-tid={"UseFeedbackCheckbox"}
                  checked={this.state.useFeedback}
                  onChange={(_, v) => {
                    this.setState({ useFeedback: v });
                  }}
                >
                  Показывать панельки Feedback
                </Checkbox>
              </td>
            </tr>
            {this.state.useRealApi && (
              <tr>
                <td className={styles.label}>ShopId</td>
                <td>
                  <Input
                    width={300}
                    data-tid="ShopIdInput"
                    value={this.state.shopId}
                    onChange={(_, v) => this.setState({ shopId: v })}
                  />
                </td>
              </tr>
            )}
            {this.state.useRealApi && (
              <tr>
                <td className={styles.label}>auth.sid</td>
                <td>
                  <Input
                    width={600}
                    data-tid="AuthSidInput"
                    value={this.state.authSid}
                    onChange={(_, v) => this.setState({ authSid: v })}
                  />
                </td>
              </tr>
            )}
            <tr>
              <td />
              <td>
                <Button data-tid="SendButton" use="primary" onClick={this._handleSubmit}>
                  Готово
                </Button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    );
  }

  private _handleSubmit = async () => {
    const authSid = this.state.useRealApi ? this.state.authSid : "FAKE_API_SID";
    const shopId = this.state.useRealApi ? this.state.shopId : "";
    set("auth.sid", authSid);
    set("shopId", shopId);
    if (this.state.useRealApi) {
      const api = new CustomerApi("/customersApi", shopId);
      try {
        await api.readAll();
      } catch (e) {
        Toast.push("Что-то пошло не так:\r" + e);
        set("auth.sid", "");
        set("shopId", "");
        throw e;
      }
    }

    this.props.onAuth({
      ...this.state,
      authSid,
      shopId,
    });
  };

  private getAuthParams = (): { authSid: string; shopId: string } => {
    const search = window.location.search;
    if (!search) {
      return { shopId: "", authSid: "" };
    }

    const getParam = (queryParts: string[], paramName: string): string => {
      const goodParts = queryParts.filter(part => part.startsWith(paramName));
      return goodParts.length === 0 ? "" : goodParts[0].split("=")[1];
    };

    // tslint:disable-next-line
    eval('window.history.replaceState({}, document.title, "/")');
    const parts = search.substring(1).split("&");
    return { shopId: getParam(parts, "shopId"), authSid: getParam(parts, "authSid") };
  };
}
