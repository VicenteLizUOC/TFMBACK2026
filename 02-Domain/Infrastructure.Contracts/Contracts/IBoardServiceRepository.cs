using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;
using TFGBack._03_Infrastructure.Models;

namespace TFGBack._02_Domain.Infrastructure.Contracts.Contracts
{
    public interface IBoardServiceRepository
    {
        BoardRepository getSingleBoard(int userId, int boardId);
        List<BoardRepository> getMyBoards(int userId);
        List<BoardRepository> getCommunityBoards(int userId, int page, int pageSize);
        int getCommunityBoardsCount(int userId);
        int addBoard(BoardRepository boardRepository);
        bool followBoard(int userId, int boardId);
        bool modifyBoard(int boardId, int userId, BoardRepository boardRepository);
        (int newBoardId, int originalBoardId) copyBoard(int userId, int boardId);
        bool removeBoard(int userId, int boardId);
        bool refreshBoard(int userId, int boardId);
        int getFollowersFromBoard(int boardId);
        List<BoardRepository> getPublicBoardsFromUser(int targetUserId);
        BoardRepository getPublicBoard(int boardId);
        List<(int UserId, string UserName, int BestStreak)> getLeaderboard(int originalBoardId);
        List<(string UserName, int BestStreak)> getFollowersData(int boardId);
    }
}
