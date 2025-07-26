namespace Dto.User
{
    public class UserFilterRequestDto
    {
        public string? Keyword { get; set; }
        public Guid? RoleId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
