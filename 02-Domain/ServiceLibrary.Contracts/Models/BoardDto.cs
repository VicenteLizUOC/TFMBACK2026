namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Models
{
    public class BoardDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime StartDate { get; set; }
        public int timeLength { get; set; }
        public DateTime EndDate { get; set; }
        public bool isTimeOver { get; set; }
        public bool IsAvailable { get; set; }
        public int Streak { get; set; }
        public int BestStreak { get; set; }
        public bool IsPublic { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public int CreatorUserId { get; set; }
        public string CreatorUser { get; set; }
        public int OriginalBoardId { get; set; }
        public string? OriginalBoardName { get; set; }
        public string? Subject { get; set; }
        public bool IsModified { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? DaysRemaining { get; set; }
        public List<TaskDto>? Tasks { get; set; }
        public bool? IsLiked { get; set; }
        public bool isLockedForToday { get; set; }
        public int? followers { get; set; }
        public string? CreatorProfilePictureBase64 { get; set; }
        public List<LeaderboardEntryDto>? Leaderboard { get; set; }
    }
}
