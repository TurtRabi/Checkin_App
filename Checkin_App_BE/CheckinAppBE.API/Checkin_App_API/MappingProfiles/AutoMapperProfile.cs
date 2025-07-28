using AutoMapper;
using Dto.RewardCard;
using Dto.StressLog;
using Dto.Treasure;
using Dto.User;
using Repository.Models;

namespace Checkin_App_API.MappingProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Treasure Mappings
            CreateMap<Treasure, TreasureResponseDto>().ReverseMap();
            CreateMap<TreasureCreateRequestDto, Treasure>();
            CreateMap<TreasureUpdateRequestDto, Treasure>();

            // UserTreasure Mappings
            CreateMap<UserTreasure, UserTreasureResponseDto>().ReverseMap();

            // User Mappings
            CreateMap<User, UserResponseDto>().ReverseMap();
            CreateMap<UserUpdateRequestDto, User>();

            // StressLog Mappings
            CreateMap<StressLog, StressLogResponseDto>().ReverseMap();
            CreateMap<StressLogCreateRequestDto, StressLog>();

            // RewardCard Mappings
            CreateMap<RewardCard, RewardCardResponseDto>().ReverseMap();

            // UserRewardCard Mappings
            CreateMap<UserRewardCard, UserRewardCardResponseDto>().ReverseMap();
        }
    }
}