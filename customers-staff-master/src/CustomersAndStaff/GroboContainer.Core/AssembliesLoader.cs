using System.Linq;
using System.Reflection;

namespace Market.CustomersAndStaff.GroboContainer.Core
{
    public static class AssembliesLoader
    {
        public static Assembly[] Load()
        {
            return AssembliesLoaderBase.Load(new[] {"Alko", "Catalogue", "GrobExp", "CashboxBackend", "Market"}).ToArray();
        }
    }
}