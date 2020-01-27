import * as React from "react";
import * as styles from "./Layout.less";

export const Layout: React.SFC = props => <div className={styles.layout}>{props.children}</div>;

export const NavigationLayout: React.SFC = props => <div className={styles.nav}>{props.children}</div>;

export const ContentLayout: React.SFC = props => <div className={styles.content}>{props.children}</div>;

export const ContentLayoutInner: React.SFC = props => <div className={styles.contentInner}>{props.children}</div>;

export const ContentLayoutFullScreen: React.SFC = props => (
  <div className={styles.contentFullScreen}>{props.children}</div>
);
