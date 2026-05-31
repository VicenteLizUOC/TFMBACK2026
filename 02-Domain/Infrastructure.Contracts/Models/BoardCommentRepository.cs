using System.ComponentModel.DataAnnotations;

namespace TFGBack._02_Domain.Infrastructure.Contracts.Models
{
    public class BoardCommentRepository
    {
        [Key]
        public int Id { get; set; }
        public int BoardId { get; set; }
        public int UserId { get; set; }
        public int CreatorBoardUserId { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsAvailable { get; set; }
    }
}
