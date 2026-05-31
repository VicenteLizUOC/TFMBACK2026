namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Models
{
    public class BoardCommentDto
    {
        public int Id { get; set; }
        public int BoardId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string? UserProfilePictureBase64 { get; set; }
        public int CreatorBoardUserId { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
