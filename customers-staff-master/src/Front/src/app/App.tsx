import RenderContainer from "@skbkontur/react-ui/components/RenderContainer/RenderContainer";
import * as React from "react";
import { Provider } from "react-redux";
import { Store } from "redux";
import { FeatureAppearance } from "../common/FeatureAppearance";
import { Metrics } from "../common/MetricsFacade";
import { ContentLayout, Layout } from "../components/Layout/Layout";
import { Navigation } from "../components/Navigation/Navigation";
import { MainRouting } from "../components/Routing/Routing";
import { Wrapper } from "../components/Wrapper/Wrapper";
import { createRootStore } from "../redux/createRootStore";
import { NomenclatureActionCreator } from "../redux/nomenclature";
import { RootState } from "../redux/rootReducer";
import { FeaturesData } from "../typings/FeaturesData";
import { NomenclatureCard } from "../typings/NomenclatureCard";

interface AppProps {
  userId: string;
  shopId: string;
  cards: NomenclatureCard[];
  featuresData: FeaturesData;
}

// хак, чтобы прокрутить rootId для этого модуля
for (let i = 0; i < 100; i++) {
  // @ts-ignore
  RenderContainer.getRootId();
}

// note https://redux.js.org/recipes/isolating-redux-sub-apps
export class App extends React.Component<AppProps, {}> {
  private store: Store<RootState>;

  constructor(props: any) {
    super(props);
    this.store = createRootStore();
  }

  public componentWillMount() {
    const { shopId, userId, featuresData } = this.props;
    Metrics.init(shopId, this.useConsoleForMetrics());
    FeatureAppearance.set(featuresData, userId, shopId);
  }

  public componentDidMount() {
    this.store.dispatch(NomenclatureActionCreator.setNomenclature(this.props.cards));
    Metrics.enterMain();
  }

  public componentWillReceiveProps(nextProps: Readonly<AppProps>): void {
    Metrics.init(this.props.shopId, this.useConsoleForMetrics());
    if (nextProps.cards !== this.props.cards) {
      this.store.dispatch(NomenclatureActionCreator.setNomenclature(nextProps.cards));
    }
    if (nextProps.featuresData.activeFeatures !== this.props.featuresData.activeFeatures) {
      FeatureAppearance.set(nextProps.featuresData, nextProps.userId, nextProps.shopId);
    }
  }

  public render() {
    return (
      <Provider store={this.store}>
        <Wrapper>
          <Layout data-tid="MainPage">
            <Navigation />
            <ContentLayout>
              <MainRouting {...this.props} />
            </ContentLayout>
          </Layout>
        </Wrapper>
      </Provider>
    );
  }

  private useConsoleForMetrics(): boolean {
    const NODE_ENV = process && process.env && process.env.NODE_ENV;
    return NODE_ENV === "development";
  }
}
