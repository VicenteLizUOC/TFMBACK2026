using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;

namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts
{
    public interface IBoardService
    {
        BoardDto getSingleBoard(int userId, int boardId);
        List<BoardDto> getMyyBoards(int userId);
        PagedBoardsDto getCommunityBoards(int userId, int page);
        int addBoard(BoardDto boardDto, int id);
        bool followBoard(int userId, int boardId);
        bool modifyBoard(int userId, int boardId, BoardDto boardDto);
        (int newBoardId, int originalBoardId) copyBoard(int userId, int boardId);
        bool removeBoard(int userId, int boardId);
        (DateTime? endDate, int? streak, int? bestStreak) refreshBoard(int userId, int boardId);
        List<BoardDto> getPublicBoardsByUser(int targetUserId);
        PublicBoardDto getPublicBoard(int boardId);
        ExportDataDto getExportData(int userId);
    }
}
