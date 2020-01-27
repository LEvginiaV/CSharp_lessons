import { Add, Search } from "@skbkontur/react-icons";
import { Loader, Paging } from "@skbkontur/react-ui/components";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Input from "@skbkontur/react-ui/components/Input/Input";
import * as throttle from "lodash/throttle";
import * as React from "react";
import { PERSONS_PER_PAGE } from "../../api/new/Constants";
import { FeatureAppearance } from "../../common/FeatureAppearance";
import { Metrics } from "../../common/MetricsFacade";
import { Caption, CaptionType } from "../../commonComponents/Caption/Caption";
import { FeedbackType } from "../../commonComponents/Feedback/FeedbackType";
import { HelpLink } from "../../commonComponents/HelpLink/HelpLink";
import { Line } from "../../commonComponents/Line/Line";
import { EmptyPage, IEmptyPageProps } from "../EmptyPage/EmptyPage";
import { ContentLayoutInner } from "../Layout/Layout";
import * as styles from "./PersonsList.less";

export interface IPersonsListProps<T> extends IEmptyPageProps {
  heading: string;
  addButtonCaption: string;
  editorComponent: React.ReactElement;
  onAdd: (item?: T) => void;
  dataLength: number;
  onPageChanged: (skip: number, take: number) => void;
  onSearch?: (searchString?: string) => void;
  notFoundCaption: string;
  isEmptyList?: boolean;
  linkCaption: string;
  linkHint: string;
  linkRef: string;
  searchPlaceholder?: string;
  dataLoaded?: boolean;
  loaderCaption?: string;
  wideList?: boolean;
}

export class PersonsList<T> extends React.Component<
  IPersonsListProps<T>,
  { showEditor?: boolean; currentPage?: number; searchString?: string }
> {
  constructor(props: IPersonsListProps<T>, state: {}) {
    super(props, state);
    this.state = { showEditor: false, currentPage: 1, searchString: "" };
  }

  public render(): JSX.Element {
    const {
      addButtonCaption,
      editorComponent,
      dataLength,
      children,
      isEmptyList,
      searchPlaceholder,
      wideList,
      linkCaption,
      linkHint,
      linkRef,
    } = this.props;

    return (
      <ContentLayoutInner>
        {!isEmptyList ? (
          <div className={wideList ? styles.wideWrapper : styles.wrapper}>
            <div className={styles.header}>
              {this.renderHeading()}
              <div className={styles.button}>
                <Button size="medium" data-tid="AddButton" use="primary" onClick={this.showEditor}>
                  <Add /> {addButtonCaption}
                </Button>
              </div>
              <div className={styles.helpLink}>
                <HelpLink caption={linkCaption} onClick={() => window.open(linkRef)} hintText={linkHint} />
              </div>
            </div>
            <div className={styles.search}>
              <Input
                data-tid="SearchInput"
                width="100%"
                value={this.state.searchString}
                leftIcon={<Search />}
                placeholder={searchPlaceholder || "Поиск"}
                onChange={this.onSearch}
              />
            </div>
            {this.renderNotFound()}
            {children}
            {dataLength > PERSONS_PER_PAGE && (
              <Line marginTop={35}>
                <Paging
                  data-tid="Paging"
                  useGlobalListener={false}
                  activePage={this.state.currentPage || 1}
                  onPageChange={this.onPageChange}
                  pagesCount={Math.ceil(dataLength / PERSONS_PER_PAGE)}
                />
              </Line>
            )}
          </div>
        ) : (
          this.renderEmpty()
        )}
        {this.state.showEditor && editorComponent && (
          <editorComponent.type onClose={this.closeEditor} onSave={this.onAdd} {...editorComponent.props} />
        )}
      </ContentLayoutInner>
    );
  }

  private onSearch = (_: any, v?: string) => {
    this.setState(
      { searchString: v },
      throttle(() => this.props.onSearch && this.props.onSearch(this.state.searchString), 200, {
        leading: false,
        trailing: true,
      })
    );
  };

  private onAdd = async (item: T) => {
    try {
      await this.props.onAdd(item);
    } finally {
      this.closeEditor();
    }
  };

  private onPageChange = (currentPage: number) => {
    this.setState({ currentPage });
    this.props.onPageChanged((currentPage - 1) * PERSONS_PER_PAGE, currentPage * PERSONS_PER_PAGE);
  };

  private closeEditor = () => this.setState({ showEditor: false });

  private showEditor = () => {
    Metrics.clientsCreateStart({ where: "list" });
    FeatureAppearance.activate(FeedbackType.CustomersCardsFeedback);
    this.setState({ showEditor: true });
  };

  private renderHeading = () => <Caption type={CaptionType.H1}>{this.props.heading}</Caption>;

  private renderNotFound = (): JSX.Element | null => {
    if (!this.props.isEmptyList && !this.props.dataLength) {
      return <div className={styles.notFoundCaption}>{this.props.notFoundCaption}</div>;
    } else {
      return null;
    }
  };

  private renderEmpty = () => (
    <div>
      {this.renderHeading()}
      {this.props.dataLoaded ? (
        <EmptyPage
          data-tid="EmptyPersonList"
          onAdd={this.showEditor}
          emptyButtonCaption={this.props.addButtonCaption}
          emptyCaption={this.props.emptyCaption}
        />
      ) : (
        <Loader
          data-tid="Loader"
          active={true}
          type="big"
          className={styles.emptyLoader}
          caption={this.props.loaderCaption}
        />
      )}
    </div>
  );
}
