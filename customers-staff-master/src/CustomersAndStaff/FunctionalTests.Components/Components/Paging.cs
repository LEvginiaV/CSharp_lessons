using Kontur.Selone.Properties;
using Kontur.Selone.Selectors.Css;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public class Paging : ComponentBase
    {
        public Paging(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
            ForwardLink = new Link(Container, new CssBy("span").WithAttribute("data-prop-pagenumber", "forward"));
            BackwardLink = new Link(Container, new CssBy("span").WithAttribute("data-prop-pagenumber", "backward"));
        }

        public Link ForwardLink { get; }
        public Link BackwardLink { get; }
        public IProp<int> PagesCount => Prop.Create(() => int.Parse(Container.GetAttribute("data-prop-pagescount")), "Paging.PagesCount");
        public IProp<int> ActivePage => Prop.Create(() => int.Parse(Container.GetAttribute("data-prop-activepage")), "Paging.ActivePage");

        public Link LinkTo(int page)
        {
            return new Link(Container, new CssBy("span").WithAttribute("data-prop-pagenumber", page.ToString()));
        }
    }
}