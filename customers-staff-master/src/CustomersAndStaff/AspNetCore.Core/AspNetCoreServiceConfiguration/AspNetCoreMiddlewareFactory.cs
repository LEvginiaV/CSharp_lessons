using System;

using GroboContainer.Core;

using Microsoft.AspNetCore.Http;

namespace Market.CustomersAndStaff.AspNetCore.Core.AspNetCoreServiceConfiguration
{
    public class AspNetCoreMiddlewareFactory : IMiddlewareFactory
    {
        public AspNetCoreMiddlewareFactory(IContainer container)
        {
            this.container = container;
        }

        public IMiddleware Create(Type middlewareType)
        {
            return container.Get(middlewareType) as IMiddleware;
        }

        public void Release(IMiddleware middleware)
        {
        }

        private readonly IContainer container;
    }
}