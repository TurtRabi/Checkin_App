using System;
using Dto.User;

namespace Dto.Treasure
{
    public class UserTreasureResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TreasureId { get; set; }
        public DateTime CollectedAt { get; set; }
        public Guid? VisitId { get; set; }
        public TreasureResponseDto Treasure { get; set; }
        public UserResponseDto User { get; set; }
    }
}