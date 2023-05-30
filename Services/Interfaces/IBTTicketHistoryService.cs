using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTTicketHistoryService
    {
        Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string UserId);

        Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId);

        Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId);
    }
}
