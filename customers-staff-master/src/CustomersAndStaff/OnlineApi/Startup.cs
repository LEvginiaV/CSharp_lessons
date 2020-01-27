using System;
using System.Reflection;

using GroboContainer.Core;

using Market.CustomersAndStaff.AspNetCore.Core.Configuration;
using Market.CustomersAndStaff.AspNetCore.Core.Middlewares;
using Market.CustomersAndStaff.OnlineApi.Filters;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Market.CustomersAndStaff.OnlineApi
{
    public class Startup : IStartup
    {
        public Startup(IContainer container)
        {
            this.container = container;
            webApiSettings = container.Get<IWebApiSettings>();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore(options =>
                    {
                        options.Filters.Add<ShopInjectorFilter>();
                    })
                .AddApplicationPart(Assembly.GetEntryAssembly())
                .AddJsonOptions(opt =>
                    {
                        opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        opt.SerializerSettings.Converters = new JsonConverter[]
                            {
                                new StringEnumConverter(true)
                            };
                    })
                .AddJsonFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);

            services
                .AddSingleton(container)
                .AddSingleton(container.Get<IControllerActivator>())
                .AddSingleton(container.Get<IMiddlewareFactory>());

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app)
        {
            if(webApiSettings.IsDevelopment)
                app.UseDeveloperExceptionPage();

            if(webApiSettings.Prefix != null)
            {
                app.UsePathBase(PathString.FromUriComponent(webApiSettings.Prefix));
                app.Use(async (context, func) =>
                    {
                        if(context.Request.PathBase != webApiSettings.Prefix)
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                        else
                            await func();
                    });
            }

            app.UseMiddleware<GraphiteSenderMiddleware>();
            app.UseMvc();
        }

        private readonly IContainer container;
        private readonly IWebApiSettings webApiSettings;
    }
}