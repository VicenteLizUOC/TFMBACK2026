namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Models
{
    public class PagedBoardsDto
    {
        public List<BoardDto> Boards { get; set; } = new List<BoardDto>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalBoards { get; set; }
    }
}
