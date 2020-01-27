using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

using SkbKontur.Graphite.Client;

namespace Market.CustomersAndStaff.AspNetCore.Core.Middlewares
{
    public class GraphiteSenderMiddleware : IMiddleware
    {
        public GraphiteSenderMiddleware(IStatsDClient statsDClient)
        {
            this.statsDClient = statsDClient.WithScope("Api2");
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await next.Invoke(context);
            }
            catch(Exception)
            {
                WithScope(context)
                    .Increment("ErrorCount", 1);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                WithScope(context)
                    .Timing("Time", stopwatch.Elapsed.Milliseconds);
            }
        }

        private IStatsDClient WithScope(HttpContext context)
        {
            var (controllerName, actionName) = context.GetControllerAndAction();
            var reasonPhrase = ReasonPhrases.GetReasonPhrase(context.Response?.StatusCode ?? 0);
            return statsDClient
                   .WithScope(controllerName ?? "UnknownController")
                   .WithScope(actionName ?? "UnknownAction")
                   .WithScope(reasonPhrase == string.Empty ? "Unknown" : reasonPhrase);
        }

        private readonly IStatsDClient statsDClient;
    }
}