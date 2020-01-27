using System;
using System.Threading.Tasks;

using Market.Api.Client;
using Market.CustomersAndStaff.OnlineApi.Controllers;
using Market.CustomersAndStaff.OnlineApi.Filters.Extensions;
using Market.CustomersAndStaff.Repositories.Interface;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Market.CustomersAndStaff.OnlineApi.Filters
{
    public class ShopInjectorFilter : ActionFilterAttribute
    {
        public ShopInjectorFilter(IMarketApiClient marketApiClient, IPublicLinkRepository publicLinkRepository)
        {
            this.marketApiClient = marketApiClient;
            this.publicLinkRepository = publicLinkRepository;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context.Controller is IShopController controller)
            {
                var link = context.RouteData.GetParam("link");
                if(link == null)
                {
                    throw new Exception("Shop controller must have link in the route");
                }

                var publicLink = await publicLinkRepository.ReadByPublicLinkAsync(link);

                if(publicLink == null || !publicLink.IsActive)
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }

                var shop = await marketApiClient.Shops.Get(publicLink.ShopId);
                controller.Shop = shop;
            }

            await next();
        }

        private readonly IMarketApiClient marketApiClient;
        private readonly IPublicLinkRepository publicLinkRepository;
    }
}