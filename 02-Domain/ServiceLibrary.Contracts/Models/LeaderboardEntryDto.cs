namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Models
{
    public class LeaderboardEntryDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int BestStreak { get; set; }
    }
}
