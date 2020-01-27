export function sleep(timeout: number) {
  return new Promise(accept => setTimeout(accept, timeout));
}
