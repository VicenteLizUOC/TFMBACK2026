using System.ComponentModel.DataAnnotations;

namespace TFGBack._03_Infrastructure.Models
{
    public class BoardRepository

    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime StartDate { get; set; }
        public int timeLength { get; set; }
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
    }
}
