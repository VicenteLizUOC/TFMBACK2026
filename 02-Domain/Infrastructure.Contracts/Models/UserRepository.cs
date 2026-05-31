using System.ComponentModel.DataAnnotations;

namespace TFGBack._03_Infrastructure.Models
{
    public class UserRepository
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public bool IsAvailable { get; set; }

        public byte[]? ProfilePicture { get; set; }
    }
}
