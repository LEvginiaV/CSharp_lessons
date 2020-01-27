import isArray from 'lodash/isArray';
import flatten from 'lodash/flatten';
import toString from 'lodash/toString';

export default function getFieldsByPath(obj, path = "") {
  return getFieldsByPathArray(obj, path.split('.').filter(x=>x))
}

function getFieldsByPathArray(obj, pathArray) {
  if (!obj)
    return [];
  if (!pathArray.length) {
    if(isArray(obj)){
      return obj.map(x=>toString(x));
    }
    return [toString(obj)];
  }
  const [cur, ...rest] = pathArray;
  if (isArray(obj)) {
    return flatten(obj.map(x => getFieldsByPathArray(x[cur], rest)));
  }
  return getFieldsByPathArray(obj[cur], rest);
}

