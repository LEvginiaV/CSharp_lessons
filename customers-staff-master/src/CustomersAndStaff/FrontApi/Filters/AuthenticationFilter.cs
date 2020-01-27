using System;
using System.Linq;
using System.Threading.Tasks;

using Market.Api.Client;
using Market.Api.Models.ResourceGroups;
using Market.CustomersAndStaff.FrontApi.Controllers;
using Market.CustomersAndStaff.FrontApi.Filters.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

using Portal.Auth;
using Portal.Requisites;

namespace Market.CustomersAndStaff.FrontApi.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        public AuthenticationFilter(IAuthClient authClient, IRequisitesClient requisitesClient, IMarketApiClient marketApiClient)
        {
            this.authClient = authClient;
            this.requisitesClient = requisitesClient;
            this.marketApiClient = marketApiClient;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!(context.Controller is IShopController) && !(context.Controller is IUserController))
            {
                throw new Exception("Controller must implement IShopController or IUserController");
            }

            var portalSid = context.HttpContext.Request.GetPortalSid();

            if(string.IsNullOrEmpty(portalSid))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var sessionStateResult = await authClient.GetSessionStateAsync(portalSid);
            if(!sessionStateResult.IsSuccessful())
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var sessionState = sessionStateResult.Response;
            var userResult = await requisitesClient.Users.GetAsync(sessionState.UserId, sessionId : sessionState.SessionId);
            if(!userResult.IsSuccessful())
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var user = userResult.Response;

            if(context.Controller is IUserController userController)
            {
                userController.AuthUser = user;
            }

            if(context.Controller is IShopController shopController)
            {
                var resourceGroup = await marketApiClient.ResourceGroupsClient.Get(user.Id.ToString());

                if(resourceGroup <= ResourceGroups.UserResourse)
                {
                    if(!user.Requisites.TryGetValue("scope/egais/partyids", out var organizationIdsString))
                    {
                        context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    var organizationIds = organizationIdsString.ParsePortalGuidsSet();

                    if(organizationIds.Length == 0 || organizationIds.All(x => x != shopController.Shop.OrganizationId))
                    {
                        context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                }
            }

            await next();
        }

        private readonly IAuthClient authClient;
        private readonly IRequisitesClient requisitesClient;
        private readonly IMarketApiClient marketApiClient;
    }
}