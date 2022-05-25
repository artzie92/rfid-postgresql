using RestEase;
using WorkTimeMonitor.DTO.Commands;

namespace WorkTimeMonitor.DTO
{
    public interface IWorkTimeMonitorRestClient
    {
        [Post("api/cards/history")]
        Task CreateCardHistory([Body] CreateCardHistoryCommand command);
    }
}