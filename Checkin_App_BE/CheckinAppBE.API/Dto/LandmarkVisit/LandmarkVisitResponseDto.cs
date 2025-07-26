namespace Dto.LandmarkVisit
{
    public class LandmarkVisitResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid LandmarkId { get; set; }
        public DateTime CheckInTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}