using System;
using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions
{
    public static class PageBaseExtensions
    {
        public static T WaitModal<T>(this PageBase pageBase) where T : ModalBase
        {
            var tid = typeof(T).GetCustomAttributes(typeof(TidModalAttribute), true).Cast<TidModalAttribute>().Select(x => x.Tid).FirstOrDefault();

            if(tid == null)
                throw new Exception($"type {typeof(T).Name} has not TidModalAttribute");

            var component = pageBase.ComponentFactory.CreateComponent<T>(pageBase.WebDriver, pageBase.WebDriver, tid);
            component.IsPresent.Wait().EqualTo(true);
            return component;
        }

        public static T WaitModalClose<T>(this PageBase pageBase) where T : ModalBase
        {
            var tid = typeof(T).GetCustomAttributes(typeof(TidModalAttribute), true).Cast<TidModalAttribute>().Select(x => x.Tid).FirstOrDefault();
            var component = pageBase.ComponentFactory.CreateComponent<T>(pageBase.WebDriver, pageBase.WebDriver, tid);
            component.IsPresent.Wait().EqualTo(false);
            
            return component;
        }
        
        public static T GoToPage<T>(this PageBase pageBase) where T : PageBase
        {
            return pageBase.WebDriver.GoToPage<T>();
        }

        public static T Refresh<T>(this PageBase pageBase) where T : PageBase
        {
            pageBase.WebDriver.Navigate().Refresh();
            return pageBase.WebDriver.GoToPage<T>();
        }
        
    }
}
