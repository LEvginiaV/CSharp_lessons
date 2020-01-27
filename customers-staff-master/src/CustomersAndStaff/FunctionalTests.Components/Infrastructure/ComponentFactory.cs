using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Kontur.Selone.Elements;
using Kontur.Selone.Selectors;
using Kontur.Selone.Selectors.Css;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure
{
    public class ComponentFactory : IComponentFactory
    {
        public T CreatePage<T>(IWebDriver searchContext)
            where T : PageBase
        {
            var tid = typeof(T).GetCustomAttribute<TidPageAttribute>()?.Tid;

            if(string.IsNullOrEmpty(tid))
            {
                throw new ArgumentException("TidAttribute is required for Page.");
            }

            return CreateComponent<T>(searchContext, searchContext, tid);
        }

        public T CreateComponent<T>(ISearchContext globalContext, ISearchContext searchContext, string tid)
            where T : ComponentBase
        {
            var selector = CreateCssSelectorByTid(tid);
            return CreateComponent(typeof(T), globalContext, searchContext, selector, null) as T;
        }

        public ElementsCollection<T> CreateCollection<T>(ISearchContext globalContext, ISearchContext searchContext, string tid)
            where T : ComponentBase
        {
            var selector = CreateCssSelectorByTid(tid);
            return CreateCollection<T>(globalContext, searchContext, selector);
        }

        private ElementsCollection<T> CreateCollection<T>(ISearchContext globalContext, ISearchContext searchContext, CssBy cssBy)
            where T : ComponentBase
        {
            T Func(ISearchContext sc, By by, IWebElement we) => CreateComponent<T>(globalContext, sc, by);
            ItemBy ItemByLambda(ByDummy x) => cssBy.FixedByIndex();

            return Activator.CreateInstance(typeof(ElementsCollection<T>),
                                            searchContext,
                                            (ItemByLambda)ItemByLambda,
                                            (Func<ISearchContext, By, IWebElement, T>)Func) as ElementsCollection<T>;
        }

        private T CreateComponent<T>(ISearchContext globalContext, ISearchContext searchContext, By by)
            where T : ComponentBase
        {
            return CreateComponent(typeof(T), globalContext, searchContext, @by, null) as T;
        }

        private object CreateComponent(Type componentType, ISearchContext globalContext, ISearchContext searchContext, By @by, object[] additionalArgs)
        {
            ComponentBase obj;
            if(typeof(PageBase).IsAssignableFrom(componentType))
            {
                obj = Activator.CreateInstance(componentType, searchContext, by, this) as ComponentBase;
            }
            else
            {
                if(IsPortalComponent(componentType))
                    by = new PortalSelector(globalContext, by);

                additionalArgs = additionalArgs ?? new object[0];
                obj = Activator.CreateInstance(componentType, new object[] {searchContext, by}.Concat(additionalArgs).ToArray()) as ComponentBase;
            }

            foreach(var property in componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty).Where(x => x.CanWrite))
            {
                var propertyType = property.PropertyType;
                var cssTidSelector = CreateCssSelectorByTid(property.Name);
                var propertyVal = TryCreate(propertyType, globalContext, obj?.Container, cssTidSelector, property.GetCustomAttributes<AdditionalConstructorArgsAttribute>().SingleOrDefault()?.Args);
                if(propertyVal != null)
                {
                    property.SetValue(obj, propertyVal);
                }
            }

            return obj;
        }

        private object TryCreate(Type type, ISearchContext globalContext, ISearchContext searchContext, CssBy cssBy, object[] additionalArgs)
        {
            if(IsTypeElementsCollection(type))
            {
                return CreateCollection(type.GetGenericArguments().First(), globalContext, searchContext, cssBy);
            }

            if(IsComponent(type))
            {
                return CreateComponent(type, globalContext, searchContext, cssBy, additionalArgs);
            }

            return null;
        }

        private object CreateCollection(Type componentType, ISearchContext globalContext, ISearchContext searchContext,  CssBy cssBy)
        {
            return createCollection.MakeGenericMethod(componentType).Invoke(this, new object[] {globalContext, searchContext, cssBy});
        }

        private static bool IsTypeElementsCollection(Type type)
        {
            if(!type.IsGenericType)
            {
                return false;
            }

            var typeDef = type.GetGenericTypeDefinition();
            var innerType = type.GetGenericArguments().First();
            return (typeDef == typeof(ElementsCollection<>)) && (IsTypeElementsCollection(innerType) || IsComponent(innerType));
        }

        private static bool IsComponent(Type type)
        {
            return typeof(ComponentBase).IsAssignableFrom(type);
        }

        private static bool IsPortalComponent(Type type)
        {
            return typeof(PortalComponentBase).IsAssignableFrom(type);
        }

        private static CssBy CreateCssSelectorByTid(string tid)
        {
            return new CssBy().WithTid(tid);
        }

        private static readonly Expression<Func<ComponentFactory, ISearchContext, ISearchContext, CssBy, ElementsCollection<ComponentBase>>> createCollectionExp
            = (factory, globalContext, searchContext, cssBy) => factory.CreateCollection<ComponentBase>(globalContext, searchContext, cssBy);

        private static readonly MethodInfo createCollection = ((MethodCallExpression)createCollectionExp.Body).Method.GetGenericMethodDefinition();
    }
}