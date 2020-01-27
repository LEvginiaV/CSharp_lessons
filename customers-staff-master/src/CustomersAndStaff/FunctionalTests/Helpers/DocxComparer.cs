using System;
using System.IO;
using System.Linq;

using GroupDocs.Comparison;
using GroupDocs.Comparison.Common.Changes;
using GroupDocs.Comparison.Common.ComparisonSettings;

using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

using MoreLinq;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers
{
    public static class DocxComparer
    {
        public static void CompareDocxFiles(string value, string expected)
        {
            WaiterHelper.WaitUntil(() => File.Exists(value));
            WaiterHelper.WaitUntil(() => File.Exists(expected));

            var comparer = new Comparer();
            var compareResult = comparer.Compare(expected, value, new ComparisonSettings());

            var images = compareResult.GetImages();
            var changes = compareResult.GetChanges();

            if (changes.Length > 0)
            {
                images.ForEach((x, i) => x.SaveImage($"diff_{i}"));
                Assert.Fail(string.Join("\n", changes.Select(x => $"{x.Type}: {x.Text} {FormatStyleChangeInfo(x.StyleChanges)}")));
            }
        }

        private static string FormatStyleChangeInfo(StyleChangeInfo[] infos)
        {
            if (infos == null || infos.Length == 0)
            {
                return "";
            }

            return $"({string.Join("; ", infos.Select(x => $"{x.ChangedProperty}: {x.OldValue} -> {x.NewValue}"))})";
        }
    }
}