using System;
using System.Threading.Tasks;

namespace Service.AdminService
{
    public interface IAdminService
    {
        Task BanUserAsync(Guid userId);
        Task UnbanUserAsync(Guid userId);
        Task AssignRoleAsync(Guid userId, string roleName);
        Task ApproveLandmarkAsync(Guid landmarkId);
        Task RejectLandmarkAsync(Guid landmarkId);
    }
}