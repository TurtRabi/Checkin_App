using Common;
using Dto.Badge;
using Repository.Models;
using Repository.UWO;
using Service.NotificationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.BadgeService
{
    public class BadgeService : IBadgeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGoNotificationClientService _goNotificationClientService;

        public BadgeService(IUnitOfWork unitOfWork, IGoNotificationClientService goNotificationClientService)
        {
            _unitOfWork = unitOfWork;
            _goNotificationClientService = goNotificationClientService;
        }

        public async Task<ServiceResult<IEnumerable<BadgeResponseDto>>> GetAllBadgesAsync()
        {
            var badges = await _unitOfWork.BadgeRepository.GetAllAsync();
            var badgeDtos = badges.Select(b => new BadgeResponseDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                ImageUrl = b.ImageUrl,
                RequiredCheckins = b.RequiredCheckins,
                PointsAwarded = b.PointsAwarded
            });
            return ServiceResult<IEnumerable<BadgeResponseDto>>.Success(badgeDtos);
        }

        public async Task<ServiceResult<BadgeResponseDto>> GetBadgeByIdAsync(Guid id)
        {
            var badge = await _unitOfWork.BadgeRepository.GetByIdAsync(id);
            if (badge == null)
            {
                return ServiceResult<BadgeResponseDto>.Fail("Badge not found.");
            }
            var badgeDto = new BadgeResponseDto
            {
                Id = badge.Id,
                Name = badge.Name,
                Description = badge.Description,
                ImageUrl = badge.ImageUrl,
                RequiredCheckins = badge.RequiredCheckins,
                PointsAwarded = badge.PointsAwarded
            };
            return ServiceResult<BadgeResponseDto>.Success(badgeDto);
        }

        public async Task<ServiceResult<IEnumerable<UserBadgeResponseDto>>> GetUserBadgesAsync(Guid userId)
        {
            var userBadges = await _unitOfWork.UserBadgeRepository.GetByConditionAsync(ub => ub.UserId == userId, includeProperties: "Badge");
            var userBadgeDtos = userBadges.Select(ub => new UserBadgeResponseDto
            {
                Id = ub.Id,
                UserId = ub.UserId,
                BadgeId = ub.BadgeId,
                EarnedAt = ub.EarnedAt,
                Badge = new BadgeResponseDto
                {
                    Id = ub.Badge.Id,
                    Name = ub.Badge.Name,
                    Description = ub.Badge.Description,
                    ImageUrl = ub.Badge.ImageUrl,
                    RequiredCheckins = ub.Badge.RequiredCheckins,
                    PointsAwarded = ub.Badge.PointsAwarded
                }
            });
            return ServiceResult<IEnumerable<UserBadgeResponseDto>>.Success(userBadgeDtos);
        }

        public async Task<ServiceResult<UserBadgeResponseDto>> AwardUserBadgeAsync(Guid userId, Guid badgeId)
        {
            var existingUserBadge = await _unitOfWork.UserBadgeRepository.GetSingleByConditionAsync(ub => ub.UserId == userId && ub.BadgeId == badgeId);
            if (existingUserBadge != null)
            {
                return ServiceResult<UserBadgeResponseDto>.Fail("User already has this badge.");
            }

            var badge = await _unitOfWork.BadgeRepository.GetByIdAsync(badgeId);
            if (badge == null)
            {
                return ServiceResult<UserBadgeResponseDto>.Fail("Badge not found.");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult<UserBadgeResponseDto>.Fail("User not found.");
            }

            var userBadge = new UserBadge
            {
                UserId = userId,
                BadgeId = badgeId,
                EarnedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserBadgeRepository.AddAsync(userBadge);

            user.Points += badge.PointsAwarded;
            _unitOfWork.UserRepository.Update(user);

            await _unitOfWork.CommitAsync();

            // Send notification for badge awarded
            var notificationMessage = $"Chúc mừng {user.DisplayName}! Bạn đã nhận được huy hiệu '{badge.Name}' và nhận được {badge.PointsAwarded} điểm.";
            var result = await _goNotificationClientService.SendNotificationToGOServiceAsync($"user.{user.Id}", notificationMessage);
            if (!result.IsSuccess)
            {
                // Xử lý lỗi nếu gửi thông báo thất bại (ví dụ: log lỗi)
                Console.WriteLine($"Lỗi gửi thông báo huy hiệu cho user {user.Id}: {result.Message}");
            }

            var userBadgeDto = new UserBadgeResponseDto
            {
                Id = userBadge.Id,
                UserId = userBadge.UserId,
                BadgeId = userBadge.BadgeId,
                EarnedAt = userBadge.EarnedAt,
                Badge = new BadgeResponseDto
                {
                    Id = badge.Id,
                    Name = badge.Name,
                    Description = badge.Description,
                    ImageUrl = badge.ImageUrl,
                    RequiredCheckins = badge.RequiredCheckins,
                    PointsAwarded = badge.PointsAwarded
                }
            };

            return ServiceResult<UserBadgeResponseDto>.Success(userBadgeDto);
        }

        public async Task<ServiceResult<BadgeResponseDto>> CreateBadgeAsync(BadgeCreateRequestDto badgeDto)
        {
            var badge = new Badge
            {
                Id = Guid.NewGuid(),
                Name = badgeDto.Name,
                Description = badgeDto.Description,
                ImageUrl = badgeDto.ImageUrl,
                RequiredCheckins = badgeDto.RequiredCheckins,
                PointsAwarded = badgeDto.PointsAwarded
            };

            await _unitOfWork.BadgeRepository.AddAsync(badge);
            await _unitOfWork.CommitAsync();

            var createdBadgeDto = new BadgeResponseDto
            {
                Id = badge.Id,
                Name = badge.Name,
                Description = badge.Description,
                ImageUrl = badge.ImageUrl,
                RequiredCheckins = badge.RequiredCheckins,
                PointsAwarded = badge.PointsAwarded
            };
            return ServiceResult<BadgeResponseDto>.Success(createdBadgeDto);
        }

        public async Task<ServiceResult<BadgeResponseDto>> UpdateBadgeAsync(BadgeUpdateRequestDto badgeDto)
        {
            var badge = await _unitOfWork.BadgeRepository.GetByIdAsync(badgeDto.Id);
            if (badge == null)
            {
                return ServiceResult<BadgeResponseDto>.Fail("Badge not found.");
            }

            badge.Name = badgeDto.Name;
            badge.Description = badgeDto.Description;
            badge.ImageUrl = badgeDto.ImageUrl;
            badge.RequiredCheckins = badgeDto.RequiredCheckins;
            badge.PointsAwarded = badgeDto.PointsAwarded;

            _unitOfWork.BadgeRepository.Update(badge);
            await _unitOfWork.CommitAsync();

            var updatedBadgeDto = new BadgeResponseDto
            {
                Id = badge.Id,
                Name = badge.Name,
                Description = badge.Description,
                ImageUrl = badge.ImageUrl,
                RequiredCheckins = badge.RequiredCheckins,
                PointsAwarded = badge.PointsAwarded
            };
            return ServiceResult<BadgeResponseDto>.Success(updatedBadgeDto);
        }

        public async Task<ServiceResult<bool>> DeleteBadgeAsync(Guid id)
        {
            var badge = await _unitOfWork.BadgeRepository.GetByIdAsync(id);
            if (badge == null)
            {
                return ServiceResult<bool>.Fail("Badge not found.");
            }

            _unitOfWork.BadgeRepository.Delete(badge);
            await _unitOfWork.CommitAsync();
            return ServiceResult<bool>.Success(true);
        }
    }
}