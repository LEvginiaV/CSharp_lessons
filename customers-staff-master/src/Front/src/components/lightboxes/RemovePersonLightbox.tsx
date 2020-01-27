import * as React from "react";
import { ISimpleLightboxProps, SimpleLightbox } from "./SimpleLightbox";

export interface IRemovePersonLightboxProps extends Partial<ISimpleLightboxProps> {}

export const RemovePersonLightbox: React.SFC<IRemovePersonLightboxProps> = props => {
  return <SimpleLightbox {...props as ISimpleLightboxProps} acceptButtonCaption="Удалить" acceptButtonUse="danger" />;
};
