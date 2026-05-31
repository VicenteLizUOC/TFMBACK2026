namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Models
{
    public class UserDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public bool? IsAvailable { get; set; }

        public IFormFile? ProfilePicture { get; set; } // Aquí llega el archivo de Angular
        public string? ProfilePictureBase64 { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public List<UserSummaryDto> Followers { get; set; } = new List<UserSummaryDto>();
        public List<UserSummaryDto> Following { get; set; } = new List<UserSummaryDto>();
        public List<BoardStreakDto> BoardBestStreaks { get; set; } = new List<BoardStreakDto>();
    }
}
