import {sleep} from '../common/PromiseUtils';
import HttpClient from '../common/HttpClient/HttpClient';
import DataGenerator from './balanceReport.fakeDataGenerator';
import DateHelper from "../common/DateHelper";

class RealApi {
  constructor(urlPrefix, shopId) {
    this.urlPrefix = urlPrefix;
    this.shopId = shopId;
  }

  async getData(date) {
    const requestDate = date ? date : this._getDate(new Date());
    return await HttpClient.get(`${this.urlPrefix}/v1/shops/${this.shopId}/balance/${requestDate}`);
  }

  async getTileData(date) {
    const requestDate = date ? this._getDate(date) : this._getDate(new Date());
    return await HttpClient.get(`${this.urlPrefix}/v1/shops/${this.shopId}/balance/${requestDate}`);
  }

  _getDate(date) {
    return DateHelper.localDateToNormalFormat(date);
  }

  async createExcel(data) {
    return await HttpClient.post(`${this.urlPrefix}/v1/excel/balance`, {
      shopId: this.shopId,
      buyPriceTotal: data.buyPriceTotal,
      sellPriceTotal: data.sellPriceTotal,
      filteredProducts: data.filteredProducts,
    });
  }
}

class FakeApi {
  constructor(dataGenerator) {
    if(dataGenerator){
      this._dataGenerator = dataGenerator;
    }
  }

  async getData(date, onlyOneDay) {
    await sleep(500);
    const result = this._dataGenerator
      ? this._dataGenerator.make(onlyOneDay)
      : new DataGenerator().make();
    result.date = date;
    return result;
  }

  async getTileData(date) {
    await sleep(500);
    const result = this._dataGenerator
      ? this._dataGenerator.make()
      : new DataGenerator().make();
    result.date = date;
    return result;
  }

  async createExcel(data) {
    return await HttpClient.post(`http://localhost:14201/marketReportsApi/v1/excel/balance`, data);
  }
}

export default RealApi;
export {FakeApi};
