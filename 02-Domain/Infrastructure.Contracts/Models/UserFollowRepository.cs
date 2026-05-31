using System.ComponentModel.DataAnnotations;

namespace TFGBack._02_Domain.Infrastructure.Contracts.Models
{
    public class UserFollowRepository
    {
        [Key]
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public int FollowedId { get; set; }
    }
}
