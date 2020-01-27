import TopBar from "@skbkontur/react-ui/components/TopBar/TopBar";
import { get, set } from "browser-cookies";
import { FeaturesCallbacks, FeaturesData } from "FeaturesData";
import createHashHistory from "history/createHashHistory";
import { NomenclatureCard } from "NomenclatureCard";
import * as React from "react";
import { Redirect, Route, Router, Switch } from "react-router-dom";
import { ApiSingleton } from "../api/new/Api";
import { Guid } from "../api/new/dto/Guid";
import { MarketApiProduct } from "../api/new/dto/MarketApiProduct";
import { FakeCustomerApi } from "../api/new/FakeApis/FakeCustomerApi";
import { FakeWorkerApi } from "../api/new/FakeApis/FakeWorkerApi";
import { App } from "../app/App";
import { AppDataSingleton } from "../app/AppData";
import HttpClient from "../common/HttpClient/HttpClient";
import { FeedbackType } from "../commonComponents/Feedback/FeedbackType";
import "../style.css";
import { Auth, AuthState } from "./Auth";
import { fakeNomenclature } from "./fakeNomenclature";
import { Menu } from "./menu/Menu";
import * as styles from "./Shell.less";

ApiSingleton.injectCustomerApi(new FakeCustomerApi());
ApiSingleton.injectWorkerApi(new FakeWorkerApi());

const history = createHashHistory();

export interface ShellState {
  authSid: string;
  shopId: Guid;
  useFeedback: boolean;
  feedback: FeedbackType[];
  nomenclature: NomenclatureCard[];
}

export class Shell extends React.Component<{}, ShellState> {
  private intervalId?: any;
  constructor(props: {}, context: any) {
    super(props, context);
    this.state = {
      authSid: "",
      shopId: "",
      useFeedback: false,
      feedback: [],
      nomenclature: [],
    };
  }

  public componentDidMount() {
    const authSid = get("auth.sid") || "";
    const shopId = get("shopId") || "";
    this.setState({ authSid, shopId });
    this.configureAppDataSingleton(shopId);
    this.startFetchNomenclature(shopId);
  }

  public render() {
    return (
      <div className={styles.root} data-tid="Shell">
        <TopBar
          userName={this.state.authSid}
          suffix="тесты"
          color="#136DAB"
          onLogout={() => {
            set("auth.sid", "");
            this.setState({ authSid: "", shopId: "" });
            this.stopFetchNomenclature();
          }}
        />
        <div className={styles.shell}>
          <div className={styles.content}>
            <div className={styles.menuContent}>
              <div className={styles.right}>
                {this.state.authSid !== "" && (
                  <Router history={history}>
                    <Switch>
                      <Route
                        path="/customersAndStaff"
                        render={props => (
                          <App
                            {...props}
                            shopId={this.state.shopId || "shopId"}
                            userId={"userId"}
                            cards={this.state.nomenclature}
                            featuresData={this.getFeaturesDataProp()}
                          />
                        )}
                      />
                      <Redirect to="/customersAndStaff" />
                    </Switch>
                  </Router>
                )}
                {this.state.authSid === "" && <Auth data-tid="Auth" onAuth={this.handleAuth} />}
              </div>
              <Menu />
            </div>
          </div>
        </div>
      </div>
    );
  }

  private stopFetchNomenclature() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = undefined;
    }
  }

  private startFetchNomenclature(shopId: string) {
    if (shopId !== "") {
      this.stopFetchNomenclature();
      this.intervalId = setInterval(() => this.fetchNomenclature(shopId), 3000);
      this.fetchNomenclature(shopId);
    } else {
      this.setState({ nomenclature: fakeNomenclature });
    }
  }

  private convertNomenclatureCard(product: MarketApiProduct): NomenclatureCard {
    return {
      id: product.id,
      name: product.name,
      unitType: product.productUnit,
      productCategory: product.productCategory,
      prices: {
        sellPrice: product.pricesInfo.sellPrice,
      },
    };
  }

  private fetchNomenclature = async (shopId: Guid) => {
    const result = (await HttpClient.get(`marketapi/v1.1/shops/${shopId}/products`, {})) as MarketApiProduct[];
    const nomenclature = result.filter(x => !x.isDeleted).map(x => this.convertNomenclatureCard(x));
    if (JSON.stringify(this.state.nomenclature) !== JSON.stringify(nomenclature)) {
      this.setState({ nomenclature });
    }
  };

  private configureAppDataSingleton(shopId: string) {
    AppDataSingleton.configure("/customersAndStaff", shopId);
  }

  private getFeaturesDataProp = (): FeaturesData => {
    if (!this.state.useFeedback) {
      return {
        activeFeatures: [],
        featuresCallbacks: {
          activate: _ => null,
          markAsSeen: _ => null,
          setCompleted: _ => null,
        } as FeaturesCallbacks,
      };
    }

    const { feedback } = this.state;
    return {
      activeFeatures: this.state.feedback,
      featuresCallbacks: {
        setCompleted: (t: FeedbackType) => this.setState({ feedback: feedback.filter(x => x !== t) }),
        activate: (t: FeedbackType) => this.setState({ feedback: [t].concat(feedback) }),
        markAsSeen: (t: FeedbackType) => this.setState({ feedback: feedback.filter(x => x !== t) }),
      } as FeaturesCallbacks,
    };
  };

  private handleAuth = (state: AuthState) => {
    this.configureAppDataSingleton(state.shopId);
    this.setState({ authSid: state.authSid, shopId: state.shopId, useFeedback: state.useFeedback });
    this.startFetchNomenclature(state.shopId);
  };
}
