using Common;
using Dto.Role;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.RoleService
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponseDto>> GetAllRolesAsync(RoleFilterRequestDto filter);
        Task<RoleResponseDto?> GetRoleByIdAsync(Guid roleId);
        Task<ServiceResult> CreateRoleAsync(RoleCreateRequestDto request);
        Task<ServiceResult> UpdateRoleAsync(Guid roleId, RoleUpdateRequestDto request);
        Task<ServiceResult> DeleteRoleAsync(Guid roleId);
    }
}
