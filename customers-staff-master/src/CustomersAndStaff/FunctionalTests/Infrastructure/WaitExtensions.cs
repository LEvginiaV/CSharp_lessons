using System;

using FluentAssertions;

using Kontur.RetryableAssertions.Extensions;
using Kontur.Selone.Elements;
using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

namespace Market.CustomersAndStaff.FunctionalTests.Infrastructure
{
    public static class WaitExtensions
    {
        public static void WaitPresence(this ComponentBase component, bool isPresent = true,  string componentDescription = null)
        {
            component.IsPresent.Wait($"Ожидаем наличие компонента {component.GetCssSelector()} {componentDescription}").EqualTo(isPresent);
        }
        
        public static void WaitAbsence(this ComponentBase component, bool isPresent = false, string componentDescription = null)
        {
            component.IsPresent.Wait($"Ожидаем отсутствие компонента {component.GetCssSelector()} {componentDescription}").EqualTo(isPresent);
        }

        public static void WaitText(this Label label, string text, string errorMessage = null)
        {
            label.Text.Wait(errorMessage).EqualTo(text);
        }
        
        public static void WaitTextContains(this Label label, string text, string errorMessage = null)
        {
            label.Text.Wait(errorMessage).Contains(text);
        }
        
        public static void WaitText(this Input input, string text, string errorMessage = null)
        {
            input.Text.Wait(errorMessage).EqualTo(text);
        }
        
        public static void WaitText(this DatePicker datePicker, string text, string errorMessage = null)
        {
            datePicker.Text.Wait(errorMessage).EqualTo(text);
        }

        public static void WaitError(this DatePicker datePicker, bool error, string errorMessage = null)
        {
            datePicker.Error.Wait(errorMessage).EqualTo(error);
        }
        
        public static void WaitWarning(this DatePicker datePicker, bool warning, string errorMessage = null)
        {
            datePicker.Warning.Wait(errorMessage).EqualTo(warning);
        }

        public static void WaitCount<T>(this ElementsCollection<T> collection, int count, string collectionName = "") where T : ComponentBase
        {
            var colName = string.IsNullOrEmpty(collectionName) ? "коллекции" : collectionName; 
            collection.Count.Wait($"ожидаем {count} элементов в '{colName}'").EqualTo(count);
        }
        
        public static void WaitAll<T>(this ElementsCollection<T> collection, Func<T, IProp<bool>> transform) where T : ComponentBase
        {
            collection.Wait().All(transform);
        }
        
        public static void WaitBool(this IProp<bool> prop, bool value, string errorMessage = null)
        {
            prop.Wait(errorMessage).EqualTo(value);
        }
    }
}
