using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers
{
    [TidModal("WorkerEditor")]
    public class WorkerEditorModal : ModalBase
    {
        public WorkerEditorModal(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Input Name { get; set; }
        public ComboBox Position { get; set; }
        public Input Phone { get; set; }
        public TextArea Description { get; set; }

        public Button AcceptButton { get; set; }
        public Button CancelButton { get; set; }
    }
}