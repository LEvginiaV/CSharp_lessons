import BalanceReportApi from './balanceReport.api';

class Api {
  constructor() {
    this.apis = {};
  }

  inject(json) {
    this.apis = {...this.apis, ...json};
  }
}

const apiSingleton = new Api();

export function init(urlPrefix, shopId) {
  apiSingleton.inject({
    balance: new BalanceReportApi(urlPrefix, shopId),
  });
}

export default apiSingleton;
