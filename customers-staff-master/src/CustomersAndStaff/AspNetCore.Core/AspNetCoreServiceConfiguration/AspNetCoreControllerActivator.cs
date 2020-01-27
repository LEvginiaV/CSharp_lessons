using GroboContainer.Core;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Market.CustomersAndStaff.AspNetCore.Core.AspNetCoreServiceConfiguration
{
    public class AspNetCoreControllerActivator : IControllerActivator
    {
        public AspNetCoreControllerActivator(IContainer container)
        {
            this.container = container;
        }

        public object Create(ControllerContext context)
        {
            return container.Create(context.ActionDescriptor.ControllerTypeInfo.AsType());
        }

        public void Release(ControllerContext context, object controller)
        {
        }

        private readonly IContainer container;
    }
}