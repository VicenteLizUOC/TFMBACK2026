using System.ComponentModel.DataAnnotations;

namespace TFGBack._02_Domain.Infrastructure.Contracts.Models
{
    public class TaskRepository
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsCompleted { get; set; }
        public string Status { get; set; }
        public int BoardId { get; set; }
        public int UserId { get; set; }
    }
}
