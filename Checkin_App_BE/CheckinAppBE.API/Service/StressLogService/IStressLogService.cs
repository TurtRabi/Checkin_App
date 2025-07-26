using Common;
using Dto.StressLog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.StressLogService
{
    public interface IStressLogService
    {
        Task<ServiceResult<StressLogResponseDto>> CreateStressLogAsync(Guid userId, StressLogCreateRequestDto requestDto);
        Task<ServiceResult<IEnumerable<StressLogResponseDto>>> GetUserStressLogsAsync(Guid userId, StressLogFilterRequestDto filter);
        Task<ServiceResult<Dictionary<string, double>>> GetAverageStressLevelByPeriodAsync(Guid userId, StressLogFilterRequestDto filter);
    }
}