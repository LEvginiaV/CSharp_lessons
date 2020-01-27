using System.Threading.Tasks;

using Market.CustomersAndStaff.EvrikaPrintClient.Models;

namespace Market.CustomersAndStaff.EvrikaPrintClient.Client
{
    public interface IEvrikaPrinterClient
    {
        Task<string> CreatePrintTaskAsync(PrintTask task);
        Task<PrintTaskInfo> GetTaskInfoAsync(string taskId);
        Task<byte[]> GetTaskResultAsync(string taskId);
    }
}