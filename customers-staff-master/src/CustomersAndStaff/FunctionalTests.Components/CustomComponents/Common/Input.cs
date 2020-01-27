using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common
{
    public class Input<T> : Input
    {
        public Input(ISearchContext searchContext, By @by, IPropTransformation<T> transformation)
            : base(searchContext, @by)
        {
            this.transformation = transformation;
        }

        public IProp<T> Value => Text.Transform(transformation);

        public void SetValue(T value)
        {
            SetRawValue(transformation.Serialize(value));
        }

        public void ResetValue(T value)
        {
            ResetRawValue(transformation.Serialize(value));
        }

        private readonly IPropTransformation<T> transformation;
    }
}