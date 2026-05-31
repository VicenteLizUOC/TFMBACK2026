using TFGBack._02_Domain.Infrastructure.Contracts.Models;
using TFGBack._03_Infrastructure.Models;

namespace TFGBack._03_Infrastructure.Database
{
    public interface IDatabaseService
    {
        //User
        bool AddUserDb(UserRepository user);
        UserRepository loginDb(UserRepository user);
        bool DeleteUserDb(int id);
        bool UpdateUserDb(int id, UserRepository user);
        UserRepository GetSingleUserDb(int id);

        //UserFollows
        bool toggleUserFollow(int followerId, int followedId);
        int getUserFollowersCount(int userId);
        int getUserFollowingCount(int userId);
        bool isFollowing(int followerId, int followedId);
        List<UserRepository> getUserFollowersList(int userId);
        List<UserRepository> getUserFollowingList(int userId);

        //Boards
        BoardRepository getSingleBoard(int userId, int boardId);
        List<BoardRepository> getMyBoards(int userId);
        List<BoardRepository> getCommunityBoards(int userId, int page, int pageSize);
        int getCommunityBoardsCount(int userId);
        List<BoardRepository> getPublicBoardsFromUser(int targetUserId);
        BoardRepository getPublicBoard(int boardId);
        int addBoard(BoardRepository boardRepository);
        bool followBoard(int userId, int boardId);
        bool modifyBoard(int boardId, int userId, BoardRepository boardRepository);
        (int newBoardId, int originalBoardId) copyBoard(int userId, int boardId);
        bool removeBoard(int userId,int boardId);
        bool refreshBoard(int userId, int boardId);
        int getFollowersFromBoard(int boardId);
        List<(int UserId, string UserName, int BestStreak)> getLeaderboard(int originalBoardId);
        List<(string UserName, int BestStreak)> getFollowersData(int boardId);

        //Comments
        List<BoardCommentRepository> getComments(int boardId);
        bool addComment(BoardCommentRepository comment);
        bool deleteComment(int commentId, int userId);

        //Tasks
        List<TaskRepository> getTasksFromBoard(int userId, int boardId);
        bool addTask(TaskRepository taskRepository, int boardId, int userId);
        bool modifyTask(int userId, int taskId, int boardId, TaskRepository taskRepository);
        bool removeTask(int userId, int boardId, int taskId);

    }
}
