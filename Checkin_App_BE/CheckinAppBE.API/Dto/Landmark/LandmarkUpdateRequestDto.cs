namespace Dto.Landmark
{
    public class LandmarkUpdateRequestDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
    }
}