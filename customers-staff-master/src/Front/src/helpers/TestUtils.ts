export class TestUtils {
  public static isTestingMode() {
    return Boolean(getCookie("testingMode"));
  }
}

function getCookie(name: string): string {
  const value = "; " + document.cookie;
  const parts = value.split("; " + name + "=");
  if (parts.length === 2) {
    return parts[1].split(";")[0];
  }
  return "";
}
