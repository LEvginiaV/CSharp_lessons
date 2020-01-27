export const repeatArray = (arr, n) => {
  let result = arr;
  for (let i = 0; i < n; i++) {
    result = result.concat(arr);
  }
  return result;
};
