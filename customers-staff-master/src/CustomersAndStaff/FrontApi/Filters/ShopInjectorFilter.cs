using System;
using System.Threading.Tasks;

using Market.Api.Client;
using Market.CustomersAndStaff.FrontApi.Controllers;
using Market.CustomersAndStaff.FrontApi.Filters.Extensions;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Market.CustomersAndStaff.FrontApi.Filters
{
    public class ShopInjectorFilter : ActionFilterAttribute
    {
        public ShopInjectorFilter(IMarketApiClient marketApiClient)
        {
            this.marketApiClient = marketApiClient;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context.Controller is IShopController controller)
            {
                var shopId = context.RouteData.GetGuidFromRouteDataParameter("shopId");
                if(shopId == null)
                {
                    throw new Exception("Shop controller must have shopId in the route");
                }

                var shop = await marketApiClient.Shops.Get(shopId.Value);
                controller.Shop = shop;
            }

            await next();
        }

        private readonly IMarketApiClient marketApiClient;
    }
}