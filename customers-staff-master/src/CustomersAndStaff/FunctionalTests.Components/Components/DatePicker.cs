using System;
using System.Text;

using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;
using Kontur.Selone.Selectors.Css;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public class DatePicker : ComponentBase
    {
        public DatePicker(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
            span = Container.SearchElement(new CssBy("label>span"));
        }

        public IProp<string> Text => Prop.Create(() => Container.Text, "DatePicker.Text");

        public IProp<bool> Error => Prop.Create(() => span.GetAttribute("data-prop-error") == "true", "DatePicker.Error");
        
        public IProp<bool> Warning => Prop.Create(() => span.GetAttribute("data-prop-warning") == "true", "DatePicker.Warning");

        public IProp<DateTime> Value => Prop.Create(() => DateTime.ParseExact(Container.Text, "dd.MM.yyyy", null), "DatePicker.Value");
        
        public void Click()
        {
            span.Click();
        }

        public void SetValue(DateTime date)
        {
            SetRawValue(date.ToString("dd.MM.yyyy"));
        }

        public void ResetValue(DateTime date)
        {
            ResetRawValue(date.ToString("dd.MM.yyyy"));
        }

        public void SetRawValue(string value)
        {
            Clear();
            EmulateSendKeys(span, value);
        }

        public void Clear()
        {
            Click();
            span.SendKeys(Keys.Backspace + Keys.Backspace + Keys.Backspace + Keys.Backspace + Keys.Backspace);
        }

        public void ResetRawValue(string value)
        {
            Clear();
            SetRawValue(value);
        }
        
        private static void EmulateSendKeys(IWebElement webElement, string s)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("var createKeydownEvent = function(char) {");
            stringBuilder.AppendLine("    return new KeyboardEvent('keydown', {");
            stringBuilder.AppendLine("        key: char,");
            stringBuilder.AppendLine("        bubbles: true,");
            stringBuilder.AppendLine("    });");
            stringBuilder.AppendLine("};");
            stringBuilder.AppendLine("var input = '" + s + "'");
            stringBuilder.AppendLine("for (var i = 0; i < input.length; ++i) {");
            stringBuilder.AppendLine("    var char = input[i]");
            stringBuilder.AppendLine("    var event = createKeydownEvent(char);");
            stringBuilder.AppendLine("    x.dispatchEvent(event);");
            stringBuilder.AppendLine("}");
            var js = stringBuilder.ToString();
            webElement.ExecuteJs(js);
        }

        private readonly IWebElement span;
    }
}