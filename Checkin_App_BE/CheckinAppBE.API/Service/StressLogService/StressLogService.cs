using AutoMapper;
using Common;
using Dto.StressLog;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Repository.UWO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.StressLogService
{
    public class StressLogService : IStressLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StressLogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<StressLogResponseDto>> CreateStressLogAsync(Guid userId, StressLogCreateRequestDto requestDto)
        {
            var stressLog = _mapper.Map<StressLog>(requestDto);
            stressLog.UserId = userId;
            stressLog.LogTime = DateTime.UtcNow;

            await _unitOfWork.StressLogRepository.AddAsync(stressLog);
            await _unitOfWork.CommitAsync();

            return ServiceResult<StressLogResponseDto>.Success(_mapper.Map<StressLogResponseDto>(stressLog));
        }

        public async Task<ServiceResult<IEnumerable<StressLogResponseDto>>> GetUserStressLogsAsync(Guid userId, StressLogFilterRequestDto filter)
        {
            var query = _unitOfWork.StressLogRepository.Query().Where(sl => sl.UserId == userId);

            if (filter.StartDate.HasValue)
            {
                query = query.Where(sl => sl.LogTime >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(sl => sl.LogTime <= filter.EndDate.Value);
            }

            var stressLogs = await query.OrderByDescending(sl => sl.LogTime).ToListAsync();
            var stressLogDtos = _mapper.Map<IEnumerable<StressLogResponseDto>>(stressLogs);

            return ServiceResult<IEnumerable<StressLogResponseDto>>.Success(stressLogDtos);
        }

        public async Task<ServiceResult<Dictionary<string, double>>> GetAverageStressLevelByPeriodAsync(Guid userId, StressLogFilterRequestDto filter)
        {
            var query = _unitOfWork.StressLogRepository.Query().Where(sl => sl.UserId == userId);

            if (filter.StartDate.HasValue)
            {
                query = query.Where(sl => sl.LogTime >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(sl => sl.LogTime <= filter.EndDate.Value);
            }

            var stressLogs = await query.ToListAsync();

            if (!stressLogs.Any())
            {
                return ServiceResult<Dictionary<string, double>>.Success(new Dictionary<string, double>());
            }

            Dictionary<string, double> result = new Dictionary<string, double>();

            if (filter.Period?.ToLower() == "week")
            {
                result = stressLogs.GroupBy(sl => GetIsoWeekOfYear(sl.LogTime))
                                   .ToDictionary(g => $"Week {g.Key} of {g.First().LogTime.Year}", g => g.Average(sl => sl.StressLevel));
            }
            else if (filter.Period?.ToLower() == "month")
            {
                result = stressLogs.GroupBy(sl => sl.LogTime.ToString("yyyy-MM"))
                                   .ToDictionary(g => g.Key, g => g.Average(sl => sl.StressLevel));
            }
            else // Default to daily if no period or invalid period is specified
            {
                result = stressLogs.GroupBy(sl => sl.LogTime.Date)
                                   .ToDictionary(g => g.Key.ToString("yyyy-MM-dd"), g => g.Average(sl => sl.StressLevel));
            }

            return ServiceResult<Dictionary<string, double>>.Success(result);
        }

        private static int GetIsoWeekOfYear(DateTime time)
        {
            // Seriously, Microsoft? No built-in ISO week number?
            // This is a common way to get ISO week number
            DayOfWeek day = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time,
                                                               System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                                                               DayOfWeek.Monday);
        }
    }
}
