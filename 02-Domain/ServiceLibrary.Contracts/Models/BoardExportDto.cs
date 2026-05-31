namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Models
{
    public class ExportDataDto
    {
        public UserExportDto User { get; set; }
        public BoardStatsExportDto Stats { get; set; }
        public List<BoardExportDto> PublicBoards { get; set; }
    }

    public class UserExportDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string AccountType { get; set; }
    }

    public class BoardStatsExportDto
    {
        public int TotalBoards { get; set; }
        public int PublicBoards { get; set; }
        public int PrivateBoards { get; set; }
        public int FollowedBoards { get; set; }
    }

    public class BoardExportDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }
        public string CreatedAt { get; set; }
        public int TotalFollowers { get; set; }
        public List<FollowerExportDto> Followers { get; set; }
        public List<TaskExportDto> Tasks { get; set; }
    }

    public class FollowerExportDto
    {
        public string UserName { get; set; }
        public int BestStreak { get; set; }
    }

    public class TaskExportDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
