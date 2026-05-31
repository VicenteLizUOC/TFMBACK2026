using System.Net.Mail;
using System.Threading.Tasks;
using TFGBack._02_Domain.Infrastructure.Contracts.Models;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;
using TFGBack._03_Infrastructure.Data;
using TFGBack._03_Infrastructure.Models;

namespace TFGBack._03_Infrastructure.Database
{
    public class DatabaseService : IDatabaseService
    {
        private readonly DataContext _dataContext;

        public DatabaseService( DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public bool AddUserDb(UserRepository user)
        {
            try
            {
                var mailAddress = new MailAddress(user.Email);


                var existingUserEmail = _dataContext.Users.Where(x => x.Email == user.Email).FirstOrDefault();
                var existingUserName = _dataContext.Users.Where(x => x.Name == user.Name).FirstOrDefault();

                if (existingUserEmail != null || existingUserName != null)
                {
                    return false;
                }
                else
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    user.UserType = "free";
                    _dataContext.Users.Add(user);
                    _dataContext.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public UserRepository loginDb(UserRepository user)
        {
            try
            {
                var singleUserRepository = _dataContext.Users?.Where(e => e.Email == user.Email).FirstOrDefault();
                if (singleUserRepository == null)
                {
                    return new UserRepository();
                }
                else
                {
                    return singleUserRepository;
                }
            }
            catch
            {
                return new UserRepository();
            }
        }
        public UserRepository GetSingleUserDb(int id)
        {
            try
            {
                var singleUserRepository = _dataContext.Users?.Where(e => e.Id == id).FirstOrDefault();
                if (singleUserRepository == null)
                {
                    return new UserRepository();
                }
                else
                {
                    return singleUserRepository;
                }
            }
            catch
            {
                return new UserRepository();
            }
        }
        public bool DeleteUserDb(int id)
        {
            try
            {
                var userRepository = _dataContext.Users.Where(e => e.Id == id).FirstOrDefault();
                if (userRepository == null)
                {
                    return false;
                }
                else
                {
                    userRepository.IsAvailable = false;
                    _dataContext.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateUserDb(int id, UserRepository user)
        {
            try
            {
                var updateUserRepository = _dataContext.Users?.Where(e => e.Id == id).FirstOrDefault();

                if (updateUserRepository == null)
                {
                    return false;
                }
                else
                {
                    updateUserRepository.Name = user.Name;
                    //updateUserRepository.Password = user.Password;
                    updateUserRepository.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    updateUserRepository.UserType = user.UserType;

                    // --- NUEVA LÍNEA: Guardar el mapa de bits ---
                    // Solo lo actualizamos si el usuario ha enviado una foto nueva
                    if (user.ProfilePicture != null)
                    {
                        updateUserRepository.ProfilePicture = user.ProfilePicture;
                    }

                    var updateBoardList = _dataContext.Boards?.Where(e => e.CreatorUserId == updateUserRepository.Id).ToList();

                    foreach (var board in updateBoardList)
                    {
                        board.CreatorUser = user.Name;
                    }


                    _dataContext.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        //UserFollows
        public bool toggleUserFollow(int followerId, int followedId)
        {
            try
            {
                if (followerId == followedId) return false;

                var existing = _dataContext.UserFollows
                    .FirstOrDefault(f => f.FollowerId == followerId && f.FollowedId == followedId);

                if (existing != null)
                {
                    _dataContext.UserFollows.Remove(existing);
                }
                else
                {
                    _dataContext.UserFollows.Add(new UserFollowRepository
                    {
                        FollowerId = followerId,
                        FollowedId = followedId
                    });
                }

                _dataContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int getUserFollowersCount(int userId)
        {
            try
            {
                return _dataContext.UserFollows.Count(f => f.FollowedId == userId);
            }
            catch { return 0; }
        }

        public int getUserFollowingCount(int userId)
        {
            try
            {
                return _dataContext.UserFollows.Count(f => f.FollowerId == userId);
            }
            catch { return 0; }
        }

        public bool isFollowing(int followerId, int followedId)
        {
            try
            {
                return _dataContext.UserFollows
                    .Any(f => f.FollowerId == followerId && f.FollowedId == followedId);
            }
            catch { return false; }
        }

        public List<UserRepository> getUserFollowersList(int userId)
        {
            try
            {
                var followerIds = _dataContext.UserFollows
                    .Where(f => f.FollowedId == userId)
                    .Select(f => f.FollowerId)
                    .ToList();

                return _dataContext.Users
                    .Where(u => followerIds.Contains(u.Id) && u.IsAvailable)
                    .ToList();
            }
            catch { return new List<UserRepository>(); }
        }

        public List<UserRepository> getUserFollowingList(int userId)
        {
            try
            {
                var followingIds = _dataContext.UserFollows
                    .Where(f => f.FollowerId == userId)
                    .Select(f => f.FollowedId)
                    .ToList();

                return _dataContext.Users
                    .Where(u => followingIds.Contains(u.Id) && u.IsAvailable)
                    .ToList();
            }
            catch { return new List<UserRepository>(); }
        }

        //Boards
        public BoardRepository getSingleBoard(int userId, int boardId)
        {
            try
            {
                var singleBoardRepository = _dataContext.Boards?.Where(e => e.Id == boardId && e.UserId == userId).FirstOrDefault();
                if (singleBoardRepository == null)
                {
                    return new BoardRepository();
                }
                else
                {
                    return singleBoardRepository;
                }
            }
            catch
            {
                return new BoardRepository();
            }
        }
        public List<BoardRepository> getMyBoards(int userId)
        {
            try
            {
                var boardsRepository = _dataContext.Boards?.Where(e => e.IsAvailable == true && e.UserId == userId).ToList();
                if (boardsRepository.Count < 1 || boardsRepository == null)
                {
                    return new List<BoardRepository>();
                }
                else
                {
                    return boardsRepository;
                }
            }
            catch
            {
                return new List<BoardRepository>();
            }
        }
        public List<BoardRepository> getCommunityBoards(int userId, int page, int pageSize)
        {
            try
            {
                return _dataContext.Boards
                    .Where(e => e.IsAvailable == true &&
                                e.IsPublic == true &&
                                e.UserId == e.CreatorUserId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            catch
            {
                return new List<BoardRepository>();
            }
        }

        public int getCommunityBoardsCount(int userId)
        {
            try
            {
                return _dataContext.Boards
                    .Count(e => e.IsAvailable == true &&
                                e.IsPublic == true &&
                                e.UserId == e.CreatorUserId);
            }
            catch
            {
                return 0;
            }
        }

        public BoardRepository getPublicBoard(int boardId)
        {
            try
            {
                var board = _dataContext.Boards
                    .FirstOrDefault(e => e.Id == boardId && e.IsPublic == true && e.IsAvailable == true);

                return board ?? new BoardRepository();
            }
            catch
            {
                return new BoardRepository();
            }
        }

        public List<BoardRepository> getPublicBoardsFromUser(int targetUserId)
        {
            try
            {
                var boards = _dataContext.Boards
                    .Where(e => e.CreatorUserId == targetUserId &&
                                e.UserId == targetUserId &&
                                e.IsPublic == true &&
                                e.IsAvailable == true)
                    .ToList();

                return boards ?? new List<BoardRepository>();
            }
            catch
            {
                return new List<BoardRepository>();
            }
        }

        public int getFollowersFromBoard(int boardId)
        {
            try
            {
                return _dataContext.Boards.Where(e => e.OriginalBoardId == boardId && e.IsAvailable == true).ToList().Count;
            }
            catch
            {
                return 0;
            }
        }

        public List<(int UserId, string UserName, int BestStreak)> getLeaderboard(int originalBoardId)
        {
            try
            {
                return (from b in _dataContext.Boards
                        join u in _dataContext.Users on b.UserId equals u.Id into userGroup
                        from u in userGroup.DefaultIfEmpty()
                        where b.OriginalBoardId == originalBoardId && !b.IsModified && b.timeLength == 0
                              && _dataContext.Boards.Any(ob => ob.Id == originalBoardId && ob.IsPublic)
                        orderby b.BestStreak descending
                        select new { b.UserId, UserName = u != null ? u.Name : b.CreatorUser, b.BestStreak })
                       .Take(3)
                       .ToList()
                       .Select(e => (e.UserId, e.UserName, e.BestStreak))
                       .ToList();
            }
            catch
            {
                return new List<(int, string, int)>();
            }
        }
        public List<(string UserName, int BestStreak)> getFollowersData(int boardId)
        {
            try
            {
                return (from b in _dataContext.Boards
                        join u in _dataContext.Users on b.UserId equals u.Id into userGroup
                        from u in userGroup.DefaultIfEmpty()
                        where b.OriginalBoardId == boardId && b.IsAvailable
                        orderby b.BestStreak descending
                        select new { UserName = u != null ? u.Name : b.CreatorUser, b.BestStreak })
                       .ToList()
                       .Select(e => (e.UserName, e.BestStreak))
                       .ToList();
            }
            catch
            {
                return new List<(string, int)>();
            }
        }

        public int addBoard(BoardRepository boardRepository)
        {
            try
            {
                var creator = _dataContext.Users.Where(e => boardRepository.CreatorUserId == e.Id).FirstOrDefault();
                if (creator == null) return 0;

                if (boardRepository.IsPublic && creator.UserType != "pro")
                    return 0;

                boardRepository.CreatorUser = creator.Name;
                boardRepository.Streak = 0;
                boardRepository.BestStreak = 0;
                _dataContext.Boards.Add(boardRepository);
                _dataContext.SaveChanges();

                boardRepository.OriginalBoardId = boardRepository.Id;
                _dataContext.SaveChanges();

                return boardRepository.Id;
            }
            catch
            {
                return 0;
            }
        }
        public bool followBoard(int userId,int boardId)
        {
            try
            {
                var followedBoard = _dataContext.Boards?.Where(e => e.OriginalBoardId == boardId && e.UserId == userId).FirstOrDefault();

                var singleBoardRepository = _dataContext.Boards?.Where(e => e.Id == boardId).FirstOrDefault();
                if (singleBoardRepository == null || singleBoardRepository.CreatorUserId == userId)
                {
                    return false;
                }
                else
                {
                    if(followedBoard != null) // Ya lo seguimos
                    {
                        followedBoard.IsAvailable = !followedBoard.IsAvailable;
                        _dataContext.SaveChanges();

                        var tasks = _dataContext.Tasks.Where(e => e.BoardId == followedBoard.Id).ToList();
                        _dataContext.Tasks.RemoveRange(tasks);
                        followedBoard.Streak = 0;
                        followedBoard.BestStreak = 0;
                        _dataContext.SaveChanges();

                        if(followedBoard.IsAvailable == true)
                        {
                            followedBoard.IsModified = false;
                            var oldtasks = _dataContext.Tasks.Where(e => e.BoardId == singleBoardRepository.OriginalBoardId && e.IsAvailable == true).ToList();
                            var newTaskList = new List<TaskRepository>();
                            foreach (var task in oldtasks)
                            {
                                var newTask = new TaskRepository();
                                newTask.Name = task.Name;
                                newTask.Description = task.Description;
                                newTask.CreationDate = task.CreationDate;
                                newTask.IsAvailable = true;
                                newTask.Status = "toDo";
                                newTask.BoardId = followedBoard.Id;
                                newTask.IsCompleted = false;
                                newTask.UserId = userId;

                                newTaskList.Add(newTask);
                            }

                            _dataContext.Tasks.AddRange(newTaskList);
                            _dataContext.SaveChanges();
                        }
                        return true;
                        
                    }
                    else
                    {
                        //add board
                        var newBoard = singleBoardRepository;
                        newBoard.Id = 0;
                        newBoard.UserId = userId;
                        newBoard.Streak = 0;
                        newBoard.BestStreak = 0;
                        newBoard.IsModified = false;
                        newBoard.IsPublic = false;
                        newBoard.IsAvailable = true;
                        newBoard.StartDate = DateTime.Now.AddDays(-1);
                        newBoard.CreationDate = DateTime.Now;
                        newBoard.OriginalBoardName = singleBoardRepository.Name;
                        _dataContext.Boards.Add(newBoard);
                        _dataContext.SaveChanges();


                        //add tasks
                        var tasks = _dataContext.Tasks.Where(e => e.BoardId == singleBoardRepository.OriginalBoardId && e.IsAvailable == true).ToList();
                        var newTaskList = new List<TaskRepository>();
                        foreach (var task in tasks)
                        {
                            var newTask = new TaskRepository();
                            newTask.Name = task.Name;
                            newTask.Description = task.Description;
                            newTask.CreationDate = task.CreationDate;
                            newTask.IsAvailable = true;
                            newTask.Status = "toDo";
                            newTask.BoardId = newBoard.Id;
                            newTask.IsCompleted = false;
                            newTask.UserId = userId;

                            newTaskList.Add(newTask);
                        }

                        _dataContext.Tasks.AddRange(newTaskList);
                        _dataContext.SaveChanges();

                        return true;

                    }
                }
            }
            catch
            {
                return false;
            }
        }
        public bool modifyBoard(int userId, int boardId, BoardRepository boardRepository)
        {
            try
            {
                var updateBoardRepository = _dataContext.Boards?.Where(e => e.Id == boardId).FirstOrDefault();

                if (updateBoardRepository == null || updateBoardRepository.CreatorUserId != userId)
                {
                    return false;
                }
                else
                {
                    if (boardRepository.IsPublic)
                    {
                        var user = _dataContext.Users?.FirstOrDefault(u => u.Id == userId);
                        if (user == null || user.UserType != "pro")
                            boardRepository.IsPublic = false;
                    }

                    updateBoardRepository.Name = boardRepository.Name;
                    updateBoardRepository.Description = boardRepository.Description;
                    updateBoardRepository.IsPublic = boardRepository.IsPublic;
                    //updateBoardRepository.timeLength = boardRepository.timeLength;
                    //updateBoardRepository.Type = boardRepository.timeLength == 0 ? "Short" : "Long";
                    _dataContext.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public (int newBoardId, int originalBoardId) copyBoard(int userId, int boardId)
        {
            try
            {
                var followedBoard = _dataContext.Boards.FirstOrDefault(e => e.Id == boardId && e.UserId == userId);
                if (followedBoard == null) return (0, 0);

                var userTasks = _dataContext.Tasks
                    .Where(e => e.BoardId == boardId && e.UserId == userId && e.IsAvailable == true)
                    .ToList();

                var creator = _dataContext.Users.FirstOrDefault(u => u.Id == userId);

                var newBoard = new BoardRepository
                {
                    Name = followedBoard.Name + " (copy)",
                    OriginalBoardName = followedBoard.Name,
                    Description = followedBoard.Description,
                    timeLength = followedBoard.timeLength,
                    Subject = followedBoard.Subject,
                    IsPublic = false,
                    UserId = userId,
                    CreatorUserId = userId,
                    CreatorUser = creator?.Name ?? string.Empty,
                    CreationDate = DateTime.Now,
                    StartDate = DateTime.Now.AddDays(-1),
                    IsAvailable = true,
                    Streak = 0,
                    BestStreak = 0,
                    IsModified = false,
                    Type = followedBoard.timeLength == 0 ? "Short" : "Long"
                };

                _dataContext.Boards.Add(newBoard);
                _dataContext.SaveChanges();

                newBoard.OriginalBoardId = newBoard.Id;

                foreach (var task in userTasks)
                {
                    _dataContext.Tasks.Add(new TaskRepository
                    {
                        Name = task.Name,
                        Description = task.Description,
                        CreationDate = DateTime.Now,
                        IsAvailable = true,
                        IsCompleted = false,
                        Status = "toDo",
                        BoardId = newBoard.Id,
                        UserId = userId
                    });
                }

                _dataContext.SaveChanges();
                return (newBoard.Id, followedBoard.OriginalBoardId);
            }
            catch
            {
                return (0, 0);
            }
        }

        public bool removeBoard(int userId, int boardId)
        {
            try
            {
                var updateBoardRepository = _dataContext.Boards?.Where(e => e.Id == boardId && e.UserId == userId).FirstOrDefault();
                var tasks = _dataContext.Tasks?.Where(e => e.BoardId == boardId).ToList();

                if (updateBoardRepository == null)
                {
                    return false;
                }
                else
                {
                    foreach (var task in tasks)
                    {
                        task.IsAvailable = false;
                    }
                    updateBoardRepository.IsAvailable = !updateBoardRepository.IsAvailable;

                    _dataContext.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool refreshBoard(int userId, int boardId)
        {
            try
            {
                var updateBoardRepository = _dataContext.Boards.FirstOrDefault(e => e.UserId == userId && e.Id == boardId);

                if (updateBoardRepository == null)
                {
                    return false;
                }

                var tasks = _dataContext.Tasks.Where(e => e.BoardId == updateBoardRepository.Id).ToList();

                bool allTasksDone = tasks.All(task => task.Status == "done");

                if (updateBoardRepository.timeLength == 0)
                {
                    // Short board — lógica de streak
                    bool missedCycle = DateTime.Today > updateBoardRepository.StartDate.Date.AddDays(1);

                    if (tasks.Count == 0 || !allTasksDone)
                    {
                        updateBoardRepository.Streak = 0;
                    }
                    else if (missedCycle)
                    {
                        updateBoardRepository.Streak = 1;
                        if (updateBoardRepository.Streak > updateBoardRepository.BestStreak)
                            updateBoardRepository.BestStreak = updateBoardRepository.Streak;
                    }
                    else
                    {
                        updateBoardRepository.Streak += 1;
                        if (updateBoardRepository.Streak > updateBoardRepository.BestStreak)
                            updateBoardRepository.BestStreak = updateBoardRepository.Streak;
                    }
                }
                else
                {
                    // Long board — registrar fecha de completado y días restantes
                    if (allTasksDone && tasks.Count > 0)
                    {
                        updateBoardRepository.CompletedDate = DateTime.Now;
                    }
                    else
                    {
                        updateBoardRepository.CompletedDate = null;
                    }
                }

                updateBoardRepository.StartDate = DateTime.Now;

                foreach (var item in tasks)
                {
                    item.IsCompleted = false;
                    item.Status = "toDo";
                }

                _dataContext.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }



        //Comments
        public List<BoardCommentRepository> getComments(int boardId)
        {
            try
            {
                return _dataContext.BoardComments
                    .Where(c => c.BoardId == boardId && c.IsAvailable)
                    .OrderByDescending(c => c.CreationDate)
                    .ToList();
            }
            catch { return new List<BoardCommentRepository>(); }
        }

        public bool addComment(BoardCommentRepository comment)
        {
            try
            {
                var board = _dataContext.Boards.FirstOrDefault(b => b.Id == comment.BoardId && b.IsPublic && b.IsAvailable);
                if (board == null) return false;

                comment.CreatorBoardUserId = board.CreatorUserId;
                comment.CreationDate = DateTime.Now;
                comment.IsAvailable = true;
                _dataContext.BoardComments.Add(comment);
                _dataContext.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public bool deleteComment(int commentId, int userId)
        {
            try
            {
                var comment = _dataContext.BoardComments.FirstOrDefault(c => c.Id == commentId && c.IsAvailable);
                if (comment == null) return false;

                var board = _dataContext.Boards.FirstOrDefault(b => b.Id == comment.BoardId);
                bool isAuthor = comment.UserId == userId;
                bool isBoardCreator = board != null && board.CreatorUserId == userId;

                if (!isAuthor && !isBoardCreator) return false;

                comment.IsAvailable = false;
                _dataContext.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        //Tasks
        public List<TaskRepository> getTasksFromBoard(int userId, int boardId)
        {
            try
            {
                var tasksRepository = _dataContext.Tasks?.Where(e => e.UserId == userId && e.BoardId == boardId && e.IsAvailable == true).ToList();
                if (tasksRepository.Count < 1 || tasksRepository == null)
                {
                    return new List<TaskRepository>();
                }
                else
                {
                    return tasksRepository;
                }
            }
            catch
            {
                return new List<TaskRepository>();
            }
        }
        public bool removeTask(int userId, int boardId, int taskId)
        {
            try
            {
                var board = _dataContext.Boards?.Where(e => e.UserId == userId && e.Id == boardId).FirstOrDefault();
                var updateTaskRepository = _dataContext.Tasks?.Where(e => e.Id == taskId && e.BoardId == boardId).FirstOrDefault();

                if (updateTaskRepository == null || updateTaskRepository.UserId != userId || board == null)
                {
                    return false;
                }
                else
                {

                    _dataContext.Tasks.Remove(updateTaskRepository);

                    if (board.UserId != board.CreatorUserId)
                        board.IsModified = true;

                    _dataContext.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool modifyTask(int userId, int taskId, int boardId, TaskRepository taskRepository)
        {
            try
            {
                var updateTaskRepository = _dataContext.Tasks?.Where(e => e.Id == taskId).FirstOrDefault();
                if (updateTaskRepository == null || updateTaskRepository.UserId != userId)
                {
                    return false;
                }
                else
                {
                    bool structuralChange = updateTaskRepository.Name != taskRepository.Name ||
                                           updateTaskRepository.Description != taskRepository.Description;

                    updateTaskRepository.Name = taskRepository.Name;
                    updateTaskRepository.Description = taskRepository.Description;
                    updateTaskRepository.IsCompleted = taskRepository.IsCompleted;
                    updateTaskRepository.Status = taskRepository.Status;

                    if (structuralChange)
                    {
                        var board = _dataContext.Boards.FirstOrDefault(b => b.Id == boardId);
                        if (board != null && board.UserId != board.CreatorUserId)
                            board.IsModified = true;
                    }

                    _dataContext.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool addTask(TaskRepository taskRepository, int boardId, int userId)
        {
            try
            {
                var board = _dataContext.Boards?.Where(e => e.UserId == userId && e.Id == boardId).FirstOrDefault();
                if (board != null)
                {
                    taskRepository.IsCompleted = false;
                    taskRepository.UserId = userId;
                    taskRepository.BoardId = boardId;
                    taskRepository.Status = "toDo";
                    _dataContext.Tasks.Add(taskRepository);

                    if (board.UserId != board.CreatorUserId)
                        board.IsModified = true;

                    _dataContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
