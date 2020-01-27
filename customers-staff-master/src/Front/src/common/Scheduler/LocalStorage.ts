class LocalStorage<T> {
  public set(key: string, value: T) {
    localStorage.setItem(key, JSON.stringify(value));
  }

  public get(key: string) {
    try {
      return JSON.parse(localStorage.getItem(key) || "");
    } catch (e) {
      return null;
    }
  }
}

const localStorageWrapper = new LocalStorage();
export { localStorageWrapper };
