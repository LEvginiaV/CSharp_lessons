export default HttpClient;

declare class HttpClient {
  public static get(url: string, query: any): Promise<any>;
  public static post(url: string, body: any): Promise<any>;
  public static put(url: string, body: any): Promise<any>;
  public static delete(url: string, query: any): Promise<any>;
}
