using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTInviteService
    {
        public Task<bool> AcceptInviteAsync(Guid? token, string UserId, int companyId);

        public Task AddNewInviteByAsync(Invite invite);

        public Task<bool> AnyInviteAsync(Guid token, string email, int companyId);

        public Task<Invite> GetInviteAsyc(int inviteId, int companyId);

        public Task<Invite> GetInviteAsync(Guid token, string email, int companyId);

        public Task<bool> ValidateInviteCodeAsync(Guid? token);
    }
}
