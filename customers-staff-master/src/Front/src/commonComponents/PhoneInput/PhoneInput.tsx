import Input from "@skbkontur/react-ui/components/Input/Input";
import { InputProps } from "@skbkontur/react-ui/Input";
import * as React from "react";
import * as styles from "./PhoneInput.less";

export const PhoneInput: React.StatelessComponent<InputProps> = props => {
  return <Input {...props} mask="999 999-99-99" prefix={<span className={styles.prefix}>+7</span>} />;
};
