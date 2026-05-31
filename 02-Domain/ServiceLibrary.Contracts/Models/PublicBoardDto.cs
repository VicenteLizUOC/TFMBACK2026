namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Models
{
    public class PublicBoardDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int timeLength { get; set; }
        public int? followers { get; set; }
        public int CreatorUserId { get; set; }
        public string CreatorUser { get; set; }
        public string? Subject { get; set; }
        public string? CreatorProfilePictureBase64 { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool isTimeOver { get; set; }
        public List<PublicTaskDto> Tasks { get; set; } = new List<PublicTaskDto>();
        public List<LeaderboardEntryDto>? Leaderboard { get; set; }
    }

    public class PublicTaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
