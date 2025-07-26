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

        public async Task<ServiceResult<IEnumerable<RoleResponseDto>>> GetAllRolesAsync(RoleFilterRequestDto filter)
        {
            var query = _unitOfWork.RoleRepository.Query();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(r => r.RoleName.Contains(filter.Keyword));
            }

            var roles = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return ServiceResult<IEnumerable<RoleResponseDto>>.Success(roles.Select(r => new RoleResponseDto
            {
                RoleId = r.Id,
                RoleName = r.RoleName
            }));
        }

        public async Task<ServiceResult<RoleResponseDto>> GetRoleByIdAsync(Guid roleId)
        {
            var role = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
            {
                return ServiceResult<RoleResponseDto>.Fail("Vai trò không tồn tại.", 404);
            }

            return ServiceResult<RoleResponseDto>.Success(new RoleResponseDto
            {
                RoleId = role.Id,
                RoleName = role.RoleName
            });
        }

        public async Task<ServiceResult> CreateRoleAsync(RoleCreateRequestDto request)
        {
            if (string.IsNullOrEmpty(request.RoleName))
            {
                return ServiceResult.Fail("Tên vai trò không được để trống.", 400);
            }

            var existingRole = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(r => r.RoleName == request.RoleName);
            if (existingRole != null)
            {
                return ServiceResult.Fail("Tên vai trò đã tồn tại.", 409);
            }

            var newRole = new Role
            {
                Id = Guid.NewGuid(),
                RoleName = request.RoleName
            };

            await _unitOfWork.RoleRepository.AddAsync(newRole);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Vai trò đã được tạo thành công.");
        }

        public async Task<ServiceResult> UpdateRoleAsync(Guid roleId, RoleUpdateRequestDto request)
        {
            var role = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
            {
                return ServiceResult.Fail("Vai trò không tồn tại.", 404);
            }

            if (string.IsNullOrEmpty(request.RoleName))
            {
                return ServiceResult.Fail("Tên vai trò không được để trống.", 400);
            }

            var existingRole = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(r => r.RoleName == request.RoleName && r.Id != roleId);
            if (existingRole != null)
            {
                return ServiceResult.Fail("Tên vai trò đã được sử dụng bởi vai trò khác.", 409);
            }

            role.RoleName = request.RoleName;
            _unitOfWork.RoleRepository.Update(role);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Vai trò đã được cập nhật thành công.");
        }

        public async Task<ServiceResult> DeleteRoleAsync(Guid roleId)
        {
            var role = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
            {
                return ServiceResult.Fail("Vai trò không tồn tại.", 404);
            }

            // Check if any users are assigned to this role
            var usersInRole = await _unitOfWork.UserRoleRepository.FindAsync(ur => ur.RoleId == roleId);
            if (usersInRole.Any())
            {
                return ServiceResult.Fail("Không thể xóa vai trò vì có người dùng đang được gán vai trò này.", 400);
            }

            _unitOfWork.RoleRepository.Delete(role);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Vai trò đã được xóa thành công.");
        }
    }
}