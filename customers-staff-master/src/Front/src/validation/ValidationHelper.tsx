import { tooltip } from "@skbkontur/react-ui-validations";
import { ValidationWrapperPartialProps } from "./CustomerInfoValidationHelper";

export class ValidationHelper {
  public static bindRequiredField(
    value?: string | null,
    message?: string | JSX.Element | null
  ): ValidationWrapperPartialProps {
    if (!value) {
      return {
        validationInfo: {
          message: message || "",
          type: "submit",
        },
        renderMessage: tooltip("right middle"),
      };
    } else {
      return this.validInfo;
    }
  }

  public static bindSimplePhoneValidation(phone?: string): ValidationWrapperPartialProps {
    const formatted = phone && phone.replace(/( |-)/g, "");

    if (formatted && formatted.length !== 10) {
      return {
        renderMessage: tooltip("right middle"),
        validationInfo: {
          type: "lostfocus",
          message: "В номере должно быть 11 цифр",
        },
      };
    } else {
      return this.validInfo;
    }
  }

  public static bindEmailValidation(email?: string): ValidationWrapperPartialProps {
    const isValid = email && this.emailRegex.test(email.toLowerCase());

    if (email && !isValid) {
      return {
        renderMessage: tooltip("right middle"),
        validationInfo: {
          type: "lostfocus",
          message: "Неверный адрес почты",
        },
      };
    } else {
      return this.validInfo;
    }
  }

  private static readonly emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

  private static readonly validInfo = {
    validationInfo: null,
  };
}
