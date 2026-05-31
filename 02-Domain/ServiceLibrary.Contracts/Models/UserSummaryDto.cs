namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Models
{
    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ProfilePictureBase64 { get; set; }
    }
}
