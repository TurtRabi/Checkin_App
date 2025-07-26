using Common;
using Dto.Role;
using Repository.UWO;
using Repository.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Service.RoleService
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IUnitOfWork unitOfWork, ILogger<RoleService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<RoleResponseDto>> GetAllRolesAsync(RoleFilterRequestDto filter)
        {
            var query = _unitOfWork.Role.Query();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(r => r.RoleName.Contains(filter.Keyword));
            }

            var roles = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return roles.Select(r => new RoleResponseDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName
            });
        }

        public async Task<RoleResponseDto?> GetRoleByIdAsync(Guid roleId)
        {
            var role = await _unitOfWork.Role.GetFirstOrDefaultAsync(r => r.RoleId == roleId);
            if (role == null)
            {
                return null; // Or throw a specific exception
            }

            return new RoleResponseDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            };
        }

        public async Task<ServiceResult> CreateRoleAsync(RoleCreateRequestDto request)
        {
            if (string.IsNullOrEmpty(request.RoleName))
            {
                return ServiceResult.Failed("HB40016", "Tên vai trò không được để trống.");
            }

            var existingRole = await _unitOfWork.Role.GetFirstOrDefaultAsync(r => r.RoleName == request.RoleName);
            if (existingRole != null)
            {
                return ServiceResult.Failed("HB40017", "Tên vai trò đã tồn tại.");
            }

            var newRole = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = request.RoleName
            };

            await _unitOfWork.Role.AddAsync(newRole);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Vai trò đã được tạo thành công.");
        }

        public async Task<ServiceResult> UpdateRoleAsync(Guid roleId, RoleUpdateRequestDto request)
        {
            var role = await _unitOfWork.Role.GetFirstOrDefaultAsync(r => r.RoleId == roleId);
            if (role == null)
            {
                return ServiceResult.Failed("HB40408", "Vai trò không tồn tại.");
            }

            if (string.IsNullOrEmpty(request.RoleName))
            {
                return ServiceResult.Failed("HB40018", "Tên vai trò không được để trống.");
            }

            var existingRole = await _unitOfWork.Role.GetFirstOrDefaultAsync(r => r.RoleName == request.RoleName && r.RoleId != roleId);
            if (existingRole != null)
            {
                return ServiceResult.Failed("HB40019", "Tên vai trò đã được sử dụng bởi vai trò khác.");
            }

            role.RoleName = request.RoleName;
            _unitOfWork.Role.Update(role);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Vai trò đã được cập nhật thành công.");
        }

        public async Task<ServiceResult> DeleteRoleAsync(Guid roleId)
        {
            var role = await _unitOfWork.Role.GetFirstOrDefaultAsync(r => r.RoleId == roleId);
            if (role == null)
            {
                return ServiceResult.Failed("HB40409", "Vai trò không tồn tại.");
            }

            // Check if any users are assigned to this role
            var usersInRole = await _unitOfWork.UserRole.FindAsync(ur => ur.RoleId == roleId);
            if (usersInRole.Any())
            {
                return ServiceResult.Failed("HB40020", "Không thể xóa vai trò vì có người dùng đang được gán vai trò này.");
            }

            await _unitOfWork.Role.DeleteAsync(role);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Vai trò đã được xóa thành công.");
        }
    }
}
