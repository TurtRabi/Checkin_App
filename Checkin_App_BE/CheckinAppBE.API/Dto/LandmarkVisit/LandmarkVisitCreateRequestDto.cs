namespace Dto.LandmarkVisit
{
    public class LandmarkVisitCreateRequestDto
    {
        public Guid LandmarkId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}