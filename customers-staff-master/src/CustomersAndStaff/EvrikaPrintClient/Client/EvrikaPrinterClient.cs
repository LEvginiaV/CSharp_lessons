using System;
using System.Threading.Tasks;

using Kontur.Clusterclient.Core;
using Kontur.Clusterclient.Core.Model;
using Kontur.Clusterclient.Core.Ordering;
using Kontur.Clusterclient.Core.Strategies;
using Kontur.Clusterclient.Core.Strategies.DelayProviders;
using Kontur.Clusterclient.Core.Topology;
using Kontur.Clusterclient.Core.Transforms;
using Kontur.Clusterclient.Transport.Http;
using Kontur.Logging;

using Market.CustomersAndStaff.EvrikaPrintClient.Models;
using Market.CustomersAndStaff.Portal.Core;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Portal.Auth;
using Portal.Common;

namespace Market.CustomersAndStaff.EvrikaPrintClient.Client
{
    public class EvrikaPrinterClient : IEvrikaPrinterClient
    {
        public EvrikaPrinterClient(IEvrikaPrinterClientSettings settings, IMasterPortalSettings masterPortalSettings, IPortalSettings portalSettings)
        {
            var provider = new ApiKeyAuthenticationProvider(portalSettings.ApiKey);
            var authClient = new AuthClient(new ClientSettings(provider, new FakeLog(), TimeSpan.FromSeconds(15), false),
                                            new TopologyProvider(settings.TestAuthUrls.Length > 0 ? settings.TestAuthUrls : portalSettings.AuthUrls));

            var result = authClient.AuthByPassAsync2(new AuthByPassRequest(masterPortalSettings.MasterLogin, masterPortalSettings.MasterPassword)).Result;
            result.EnsureSuccess();
            var authSid = result.Response.Sid;
            Console.WriteLine($"AuthSid: {authSid}");

            clusterClient = new ClusterClient(null, config =>
                {
                    var repeatReplicasCount = settings.RepeatReplicasCount;
                    config.ClusterProvider = new FixedClusterProvider(settings.Host);
                    config.SetupKonturHttpTransport();
                    config.RepeatReplicas(repeatReplicasCount);
                    config.ReplicaOrdering = new AsIsReplicaOrdering();
                    config.DefaultRequestStrategy = new ForkingRequestStrategy(new EqualDelaysProvider(repeatReplicasCount), repeatReplicasCount);
                    config.AddRequestTransform(new DefaultHeadersTransform(new[]
                        {
                            new Header("Authorization", $"auth.sid {authSid}"),
                            new Header("X-Kontur-Apikey", $"{settings.PortalApiKey}"),
                            new Header(HeaderNames.Accept, "application/json"),
                        }));
                    config.DefaultTimeout = settings.Timeout;
                });
        }

        public async Task<string> CreatePrintTaskAsync(PrintTask task)
        {
            var uri = new RequestUrlBuilder()
                      .AppendToPath(printerApiVersion)
                      .AppendToPath("Print")
                      .Build();
            var request = Request.Post(uri)
                                 .WithContentTypeHeader("application/json")
                                 .WithContent(JsonConvert.SerializeObject(task, new JsonSerializerSettings
                                     {
                                         Converters = new JsonConverter[] {new StringEnumConverter()}
                                     }));
            var result = await clusterClient.SendAsync(request).ConfigureAwait(false);
            var response = result.Response;
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<string>(response.Content.ToString());
        }

        public async Task<PrintTaskInfo> GetTaskInfoAsync(string taskId)
        {
            var uri = new RequestUrlBuilder()
                      .AppendToPath(printerApiVersion)
                      .AppendToPath("Task")
                      .AppendToPath("Status")
                      .AppendToQuery("taskId", taskId)
                      .Build();
            var result = await clusterClient.SendAsync(Request.Get(uri)).ConfigureAwait(false);
            var response = result.Response;
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<PrintTaskInfo>(response.Content.ToString());
        }

        public async Task<byte[]> GetTaskResultAsync(string taskId)
        {
            var uri = new RequestUrlBuilder()
                      .AppendToPath(printerApiVersion)
                      .AppendToPath("Task")
                      .AppendToPath("Result")
                      .AppendToQuery("taskId", taskId)
                      .Build();
            var result = await clusterClient.SendAsync(Request.Get(uri)).ConfigureAwait(false);
            var response = result.Response;
            response.EnsureSuccessStatusCode();

            var stringResult = JsonConvert.DeserializeObject<string>(System.Text.Encoding.UTF8.GetString(response.Content.ToArray()));
            return Convert.FromBase64String(stringResult);
        }

        private readonly IClusterClient clusterClient;

        private const string printerApiVersion = "v2";
    }
}