using Kontur.Selone.Elements;
using Kontur.Selone.Selectors.XPath;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public class Switcher : ComponentBase
    {
        public Switcher(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
            Buttons = new ElementsCollection<Button>(Container, 
                                                     x => x.XPath(".//")
                                                           .AnyTag()
                                                           .WithAttribute("data-comp-name", "Button")
                                                           .FixedByKey(), 
                                                     (c, s, e) => new Button(c, s));
        }

        public IElementsCollection<Button> Buttons { get; }

        public Button GetButton(int index)
        {
            return new Button(Container, new XPathBy($"(.//*[@data-comp-name='Button'])[{index + 1}]"));
        }

        public Button GetButton(string key)
        {
            return new Button(Container, new XPathBy(".//").AnyTag().WithAttribute("data-key", key));
        }
    }
}