using System;

namespace Dto.Treasure
{
    public class OpenTreasureResponseDto
    {
        public Guid UserTreasureId { get; set; }
        public string TreasureName { get; set; }
        public int AwardedCoin { get; set; }
        public int AwardedExperiencePoints { get; set; }
        public string Message { get; set; }
    }
}