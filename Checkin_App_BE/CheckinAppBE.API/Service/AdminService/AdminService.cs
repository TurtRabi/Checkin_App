
using Repository.UWO;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Required for FirstOrDefaultAsync
using Repository.Models;

namespace Service.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task BanUserAsync(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.IsBanned = true;
                _unitOfWork.UserRepository.Update(user); // Corrected: Update is synchronous
                await _unitOfWork.CommitAsync(); // Corrected: Method is CommitAsync
            }
        }

        public async Task UnbanUserAsync(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.IsBanned = false;
                _unitOfWork.UserRepository.Update(user); // Corrected: Update is synchronous
                await _unitOfWork.CommitAsync(); // Corrected: Method is CommitAsync
            }
        }

        public async Task AssignRoleAsync(Guid userId, string roleName)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, includeProperties: "UserRoles"); // Include UserRoles
            var role = await _unitOfWork.RoleRepository.Query().FirstOrDefaultAsync(r => r.RoleName == roleName); // Corrected: RoleName
            if (user != null && role != null)
            {
                // Check if the user already has this role
                if (!user.UserRoles.Any(ur => ur.RoleId == role.Id))
                {
                    user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
                    _unitOfWork.UserRepository.Update(user); // Update user to save the new UserRole
                    await _unitOfWork.CommitAsync();
                }
            }
        }

        public async Task ApproveLandmarkAsync(Guid landmarkId)
        {
            var landmark = await _unitOfWork.LandmarkRepository.GetByIdAsync(landmarkId);
            if (landmark != null)
            {
                landmark.Status = "Approved"; // Corrected: Use the Status property
                _unitOfWork.LandmarkRepository.Update(landmark); // Corrected: Update is synchronous
                await _unitOfWork.CommitAsync(); // Corrected: Method is CommitAsync
            }
        }

        public async Task RejectLandmarkAsync(Guid landmarkId)
        {
            var landmark = await _unitOfWork.LandmarkRepository.GetByIdAsync(landmarkId);
            if (landmark != null)
            {
                _unitOfWork.LandmarkRepository.Delete(landmark); // Corrected: Delete is synchronous
                await _unitOfWork.CommitAsync(); // Corrected: Method is CommitAsync
            }
        }
    }
}
