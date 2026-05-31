using TFGBack._02_Domain.Infrastructure.Contracts.Contracts;
using TFGBack._03_Infrastructure.Database;
using TFGBack._03_Infrastructure.Models;

namespace TFGBack._03_Infrastructure.Impl
{
    public class BoardServiceRepository : IBoardServiceRepository
    {
        private readonly IDatabaseService _databaseService;

        public BoardServiceRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public int addBoard(BoardRepository boardRepository)
        {
            return _databaseService.addBoard(boardRepository);
        }

        public bool followBoard(int userId, int boardId)
        {
            return _databaseService.followBoard(userId, boardId);

        }

        public List<BoardRepository> getCommunityBoards(int userId, int page, int pageSize)
        {
            return _databaseService.getCommunityBoards(userId, page, pageSize);
        }

        public int getCommunityBoardsCount(int userId)
        {
            return _databaseService.getCommunityBoardsCount(userId);
        }

        public List<BoardRepository> getMyBoards(int userId)
        {
            return _databaseService.getMyBoards(userId);
        }

        public BoardRepository getSingleBoard(int userId, int boardId)
        {
            return _databaseService.getSingleBoard(userId, boardId);
        }

        public bool modifyBoard(int userId, int boardId, BoardRepository boardRepository)
        {
            return _databaseService.modifyBoard(userId, boardId, boardRepository);
        }

        public (int newBoardId, int originalBoardId) copyBoard(int userId, int boardId)
        {
            return _databaseService.copyBoard(userId, boardId);
        }

        public bool removeBoard(int userId, int boardId)
        {
            return _databaseService.removeBoard(userId, boardId);
        }

        public bool refreshBoard(int userId, int boardId)
        {
            return _databaseService.refreshBoard(userId, boardId);
        }

        public int getFollowersFromBoard(int boardId)
        {
            return _databaseService.getFollowersFromBoard(boardId);
        }

        public List<BoardRepository> getPublicBoardsFromUser(int targetUserId)
        {
            return _databaseService.getPublicBoardsFromUser(targetUserId);
        }

        public BoardRepository getPublicBoard(int boardId)
        {
            return _databaseService.getPublicBoard(boardId);
        }

        public List<(int UserId, string UserName, int BestStreak)> getLeaderboard(int originalBoardId)
        {
            return _databaseService.getLeaderboard(originalBoardId);
        }

        public List<(string UserName, int BestStreak)> getFollowersData(int boardId)
        {
            return _databaseService.getFollowersData(boardId);
        }
    }
}
