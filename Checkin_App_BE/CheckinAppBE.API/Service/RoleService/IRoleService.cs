using Common;
using Dto.Role;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.RoleService
{
    public interface IRoleService
    {
        Task<ServiceResult<IEnumerable<RoleResponseDto>>> GetAllRolesAsync(RoleFilterRequestDto filter);
        Task<ServiceResult<RoleResponseDto>> GetRoleByIdAsync(Guid roleId);
        Task<ServiceResult> CreateRoleAsync(RoleCreateRequestDto request);
        Task<ServiceResult> UpdateRoleAsync(Guid roleId, RoleUpdateRequestDto request);
        Task<ServiceResult> DeleteRoleAsync(Guid roleId);
    }
}