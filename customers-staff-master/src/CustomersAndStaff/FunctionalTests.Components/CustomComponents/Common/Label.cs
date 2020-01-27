using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common
{
    public class Label<T> : Label
    {
        public Label(ISearchContext searchContext, By @by, IPropTransformation<T> transformation)
            : base(searchContext, @by)
        {
            this.transformation = transformation;
        }

        public IProp<T> Value => Text.Transform(transformation);

        private readonly IPropTransformation<T> transformation;
    }
}