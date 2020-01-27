using System.Threading.Tasks;

namespace Market.CustomersAndStaff.Repositories.Interface
{
    public interface ICounterRepository
    {
        Task<int> IncrementAsync(string key, int step = 1);
        Task<int> GetCurrentAsync(string key);
    }
}