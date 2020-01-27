import flatten from 'lodash/flatten';
import split from 'lodash/split';
import minBy from 'lodash/minBy';

import getFieldsByPath from './getFieldsByPath.js';

const defaultRegex = /[^\w\dа-яА-ЯёЁ]/i;

export class Tokenizer {
  constructor(splitRegex = defaultRegex)
  {
    this.splitRegex = splitRegex;
  }

  splitIntoTokens(searchString) {
    return split(searchString, this.splitRegex).filter(x=>x).map(x=>x.toLowerCase());
  }
}

export default class PocketElasticSearcher {
  constructor(objects, tokenPaths, exactPaths = [], fullSubstringPaths = [], fieldForSort = null, splitRegex = defaultRegex) {
    this.tokenizer = new Tokenizer(splitRegex);

    this.objectTokens = objects.map(obj => {
      const values = flatten(tokenPaths.map(path => getFieldsByPath(obj, path)));
      const tokens = flatten(values.map(value => this.tokenizer.splitIntoTokens(value)));
      const exactValues = flatten(exactPaths.map(path => getFieldsByPath(obj, path))).filter(x => x).map(x => x.toLowerCase());
      const fullSubstr = flatten(fullSubstringPaths.map(path => getFieldsByPath(obj, path))).filter(x => x).map(x => x.toLowerCase());

      let priority = 1;
      if (fieldForSort)
        priority = getFieldsByPath(obj, fieldForSort)[0];
      return {tokens, exactValues, fullSubstr, obj, priority};
    });
  }

  search(searchString) {
    var searchStringTokens = this.tokenizer.splitIntoTokens(searchString);
    let res = this.objectTokens
      .filter(({tokens, exactValues, fullSubstr}) => searchStringTokens.every(searchStringToken => {
        // noinspection EqualityComparisonWithCoercionJS
        return tokens.some(token => token.startsWith(searchStringToken))
          || exactValues.some(token => token == searchStringToken)
          || fullSubstr.some(token => token.indexOf(searchStringToken) !== -1);
      }))
      .map(({tokens, exactValues, fullSubstr, obj, priority}, idx) => {
        const weight = searchStringTokens.map(searchStringToken => {
          let tokenWeight = 0;
          const partlyEqualTokens = tokens.filter(token => token.startsWith(searchStringToken))
            .concat(fullSubstr.filter(token => token.indexOf(searchStringToken !== -1)));
          if (partlyEqualTokens.length) {
            tokenWeight = searchStringToken.length / minBy(partlyEqualTokens, t => t.length).length;
          } else {
            tokenWeight = 1; // searchStringToken / exactValues === 1 - always
          }
          return tokenWeight;
        }).reduce((sum, weight) => sum + weight, 0);
        return {
          obj,
          priority,
          weight,
          idx,
        };
      });
    res.sort((a, b) => a.priority === b.priority ? a.weight === b.weight ? a.idx - b.idx : b.weight - a.weight : a.priority - b.priority);
    return res.map(x => x.obj);
  }

  getTokenizer(){
    return this.tokenizer;
  }
}
