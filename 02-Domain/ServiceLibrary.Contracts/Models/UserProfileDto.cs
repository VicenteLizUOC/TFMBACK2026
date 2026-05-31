namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Models
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserType { get; set; }
        public string? ProfilePictureBase64 { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowing { get; set; }
        public List<BoardDto> PublicBoards { get; set; } = new List<BoardDto>();
    }
}
