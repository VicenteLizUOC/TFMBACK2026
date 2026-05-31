using System.Linq.Expressions;
using TFGBack._02_Domain.Infrastructure.Contracts.Contracts;
using TFGBack._02_Domain.Infrastructure.Contracts.Models;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;
using TFGBack._03_Infrastructure.Impl;
using TFGBack._03_Infrastructure.Models;

namespace TFGBack._02_Domain.ServiceLibrary.Impl.Impl
{
    public class BoardService : IBoardService
    {
        private readonly IBoardServiceRepository _boardServiceRepository;
        private readonly ITaskServiceRepository _taskServiceRepository;
        private readonly IUserServiceRepository _userServiceRepository;

        public BoardService(IBoardServiceRepository boardServiceRepository, ITaskServiceRepository taskServiceRepository, IUserServiceRepository userServiceRepository)
        {
            _boardServiceRepository = boardServiceRepository;
            _taskServiceRepository = taskServiceRepository;
            _userServiceRepository = userServiceRepository;
        }


        public int addBoard(BoardDto boardDto, int id)
        {
            var boardRepository = new BoardRepository();
            boardRepository.Name = boardDto.Name;
            boardRepository.CreationDate = DateTime.Now;
            boardRepository.StartDate = DateTime.Now.AddDays(-1);
            boardRepository.timeLength = boardDto.timeLength;
            boardRepository.IsAvailable = true;
            boardRepository.Streak = 0;
            boardRepository.IsPublic = boardDto.IsPublic;
            boardRepository.UserId = id;
            boardRepository.Type = boardDto.timeLength == 0 ? "Short" : "Long";
            boardRepository.Description = boardDto.Description;
            boardRepository.Subject = boardDto.Subject;
            boardRepository.CreatorUserId = id;

            return _boardServiceRepository.addBoard(boardRepository);
        }

        public bool followBoard(int userId, int boardId)
        {
            return _boardServiceRepository.followBoard(userId, boardId);

        }

        private const int PageSize = 30;

        public PagedBoardsDto getCommunityBoards(int userId, int page)
        {
            if (page < 1) page = 1;

            var totalBoards = _boardServiceRepository.getCommunityBoardsCount(userId);
            var totalPages = (int)Math.Ceiling((double)totalBoards / PageSize);

            var communityListRepository = _boardServiceRepository.getCommunityBoards(userId, page, PageSize);
            var communityListDto = FromBoardRepositorytoDtoList(communityListRepository);

            var myBoardsRepository = _boardServiceRepository.getMyBoards(userId);
            var myBoardsDto = FromBoardRepositorytoDtoList(myBoardsRepository);

            foreach (var boardDto in communityListDto)
            {
                boardDto.followers = _boardServiceRepository.getFollowersFromBoard(boardDto.Id);

                var creator = _userServiceRepository.getUserRepository(boardDto.CreatorUserId);
                if (creator?.ProfilePicture != null && creator.ProfilePicture.Length > 0)
                    boardDto.CreatorProfilePictureBase64 = Convert.ToBase64String(creator.ProfilePicture);

                if (myBoardsDto.Any(m => m.OriginalBoardId == boardDto.Id))
                    boardDto.IsLiked = true;
            }

            return new PagedBoardsDto
            {
                Boards = communityListDto,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalBoards = totalBoards
            };
        }



        public List<BoardDto> getMyyBoards(int userId)
        {
            var myBoards = _boardServiceRepository.getMyBoards(userId);
            return FromBoardRepositorytoDtoList(myBoards);
        }

        public BoardDto getSingleBoard(int userId, int boardId)
        {
            var board = _boardServiceRepository.getSingleBoard(userId, boardId);
            var boardDto = FromBoardRepositorytoDto(board);

            var creator = _userServiceRepository.getUserRepository(board.CreatorUserId);
            if (creator?.ProfilePicture != null && creator.ProfilePicture.Length > 0)
                boardDto.CreatorProfilePictureBase64 = Convert.ToBase64String(creator.ProfilePicture);

            boardDto.followers = _boardServiceRepository.getFollowersFromBoard(board.OriginalBoardId != 0 ? board.OriginalBoardId : board.Id);

            int leaderboardBoardId = board.OriginalBoardId != 0 ? board.OriginalBoardId : board.Id;
            boardDto.Leaderboard = _boardServiceRepository.getLeaderboard(leaderboardBoardId)
                .Select(e => new LeaderboardEntryDto { UserId = e.UserId, UserName = e.UserName, BestStreak = e.BestStreak })
                .ToList();

            return boardDto;
        }

        public bool modifyBoard(int userId, int boardId, BoardDto boardDto)
        {
            return _boardServiceRepository.modifyBoard(userId, boardId, FromBoardDtotoRepository(boardDto));
        }

        public (int newBoardId, int originalBoardId) copyBoard(int userId, int boardId)
        {
            return _boardServiceRepository.copyBoard(userId, boardId);
        }

        public bool removeBoard(int userId, int boardId)
        {
            return _boardServiceRepository.removeBoard(userId, boardId);
        }

        public (DateTime? endDate, int? streak, int? bestStreak) refreshBoard(int userId, int boardId)
        {
            var board = _boardServiceRepository.getSingleBoard(userId, boardId);
            if (board == null || board.Id == 0) return (null, null, null);

            var success = _boardServiceRepository.refreshBoard(userId, boardId);
            if (!success) return (null, null, null);

            var updatedBoard = _boardServiceRepository.getSingleBoard(userId, boardId);
            var newStartDate = DateTime.Now;
            var endDate = board.timeLength == 0
                ? newStartDate.Date.AddDays(1)
                : newStartDate.AddDays(board.timeLength * 7);

            return (endDate, updatedBoard.Streak, updatedBoard.BestStreak);
        }

        public List<BoardDto> getPublicBoardsByUser(int targetUserId)
        {
            var boards = _boardServiceRepository.getPublicBoardsFromUser(targetUserId);
            var boardsDto = FromBoardRepositorytoDtoList(boards);
            foreach (var board in boardsDto)
            {
                board.followers = _boardServiceRepository.getFollowersFromBoard(board.Id);
            }
            return boardsDto;
        }

        public PublicBoardDto getPublicBoard(int boardId)
        {
            try
            {
                var board = _boardServiceRepository.getPublicBoard(boardId);
                if (board.Id == 0)
                    return new PublicBoardDto();

                var dto = new PublicBoardDto();
                dto.Id = board.Id;
                dto.Name = board.Name;
                dto.Description = board.Description;
                dto.Type = board.Type;
                dto.timeLength = board.timeLength;
                dto.CreatorUserId = board.CreatorUserId;
                dto.CreatorUser = board.CreatorUser;
                dto.Subject = board.Subject;
                dto.StartDate = board.StartDate;
                dto.EndDate = board.timeLength == 0
                    ? board.StartDate.Date.AddDays(1)
                    : board.StartDate.AddDays(board.timeLength * 7);
                dto.isTimeOver = DateTime.Now > dto.EndDate;
                dto.followers = _boardServiceRepository.getFollowersFromBoard(board.Id);

                var creator = _userServiceRepository.getUserRepository(board.CreatorUserId);
                if (creator?.ProfilePicture != null && creator.ProfilePicture.Length > 0)
                    dto.CreatorProfilePictureBase64 = Convert.ToBase64String(creator.ProfilePicture);

                var creatorTasks = _taskServiceRepository.getTasksFromBoard(board.CreatorUserId, board.Id);
                dto.Tasks = creatorTasks
                    .Where(t => t.IsAvailable)
                    .Select(t => new PublicTaskDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description
                    })
                    .ToList();

                dto.Leaderboard = _boardServiceRepository.getLeaderboard(board.Id)
                    .Select(e => new LeaderboardEntryDto { UserId = e.UserId, UserName = e.UserName, BestStreak = e.BestStreak })
                    .ToList();

                return dto;
            }
            catch
            {
                return new PublicBoardDto();
            }
        }


        public ExportDataDto getExportData(int userId)
        {
            var allMyBoards = _boardServiceRepository.getMyBoards(userId);
            var publicBoards = allMyBoards.Where(b => b.IsPublic && b.UserId == b.CreatorUserId && b.IsAvailable).ToList();
            var privateBoards = allMyBoards.Where(b => !b.IsPublic && b.UserId == b.CreatorUserId && b.IsAvailable).ToList();
            var followedBoards = allMyBoards.Where(b => b.UserId != b.CreatorUserId && b.IsAvailable).ToList();

            var user = _userServiceRepository.getUserRepository(userId);

            return new ExportDataDto
            {
                User = new UserExportDto
                {
                    Name = user?.Name ?? string.Empty,
                    Email = user?.Email ?? string.Empty,
                    AccountType = user?.UserType ?? string.Empty
                },
                Stats = new BoardStatsExportDto
                {
                    TotalBoards = allMyBoards.Count(b => b.IsAvailable),
                    PublicBoards = publicBoards.Count,
                    PrivateBoards = privateBoards.Count,
                    FollowedBoards = followedBoards.Count
                },
                PublicBoards = publicBoards.Select(board => new BoardExportDto
                {
                    Name = board.Name,
                    Description = board.Description ?? string.Empty,
                    Subject = board.Subject ?? string.Empty,
                    Type = board.Type,
                    CreatedAt = board.CreationDate.ToString("yyyy-MM-dd"),
                    TotalFollowers = _boardServiceRepository.getFollowersFromBoard(board.Id),
                    Followers = _boardServiceRepository.getFollowersData(board.Id)
                        .Select(f => new FollowerExportDto { UserName = f.UserName, BestStreak = f.BestStreak })
                        .ToList(),
                    Tasks = _taskServiceRepository.getTasksFromBoard(userId, board.Id)
                        .Where(t => t.IsAvailable)
                        .Select(t => new TaskExportDto { Name = t.Name, Description = t.Description ?? string.Empty })
                        .ToList()
                }).ToList()
            };
        }

        //Mappers
        private List<BoardDto> FromBoardRepositorytoDtoList(List<BoardRepository> list)
        {
            var listDto = new List<BoardDto>();
            foreach (var board in list)
            {
                var boardTaskList = FromTaskRepositoryToDtoList(_taskServiceRepository.getTasksFromBoard(board.UserId, board.Id));
                foreach (var task in boardTaskList)
                {
                    if (task.UserId != board.CreatorUserId)
                    {
                        task.Status = "toDo";
                        task.IsAvailable = true;
                        task.IsCompleted = true;
                    }
                }

                var boardDto = new BoardDto();
                boardDto.Id = board.Id;
                boardDto.Name = board.Name;
                boardDto.CreationDate = board.CreationDate;
                boardDto.StartDate = board.StartDate;
                boardDto.timeLength = board.timeLength;

                if (board.timeLength == 0)
                {
                    boardDto.EndDate = board.StartDate.Date.AddDays(1);
                }
                else
                {
                    boardDto.EndDate = board.StartDate.AddDays(board.timeLength * 7);
                }

                boardDto.isTimeOver = DateTime.Now > boardDto.EndDate;
                boardDto.isLockedForToday = board.timeLength == 0
                    && board.StartDate.Date == DateTime.Today
                    && boardDto.EndDate > DateTime.Now;

                boardDto.IsAvailable = board.IsAvailable;
                bool shortMissedCycle = board.timeLength == 0 && DateTime.Today > board.StartDate.Date.AddDays(1);
                boardDto.Streak = shortMissedCycle ? 0 : board.Streak;
                boardDto.BestStreak = board.BestStreak;
                boardDto.IsPublic = board.IsPublic;
                boardDto.UserId = board.UserId;
                boardDto.Type = board.Type;
                boardDto.Description = board.Description;
                boardDto.CreatorUserId = board.CreatorUserId;
                boardDto.CreatorUser = board.CreatorUser;
                boardDto.OriginalBoardId = board.OriginalBoardId;
                boardDto.OriginalBoardName = board.OriginalBoardName;
                boardDto.Subject = board.Subject;
                boardDto.IsModified = board.IsModified;
                boardDto.CompletedDate = board.CompletedDate;
                boardDto.DaysRemaining = board.timeLength > 0
                    ? Math.Max(0, (int)(boardDto.EndDate - DateTime.Now).TotalDays)
                    : null;
                boardDto.Tasks = boardTaskList;

                listDto.Add(boardDto);
            }
            return listDto;
        }


        private BoardDto FromBoardRepositorytoDto(BoardRepository board)
        {
            var boardDto = new BoardDto();
            boardDto.Id = board.Id;
            boardDto.Name = board.Name;
            boardDto.CreationDate = board.CreationDate;
            boardDto.StartDate = board.StartDate;
            boardDto.timeLength = board.timeLength;

            if (board.timeLength == 0)
            {
                boardDto.EndDate = board.StartDate.Date.AddDays(1);
            }
            else
            {
                boardDto.EndDate = board.StartDate.AddDays(board.timeLength * 7);
            }

            boardDto.isTimeOver = DateTime.Now > boardDto.EndDate;
            boardDto.isLockedForToday = board.timeLength == 0
                && board.StartDate.Date == DateTime.Today
                && boardDto.EndDate > DateTime.Now;

            boardDto.IsAvailable = board.IsAvailable;
            bool shortMissedCycle = board.timeLength == 0 && DateTime.Today > board.StartDate.Date.AddDays(1);
            boardDto.Streak = shortMissedCycle ? 0 : board.Streak;
            boardDto.BestStreak = board.BestStreak;
            boardDto.IsPublic = board.IsPublic;
            boardDto.UserId = board.UserId;
            boardDto.Type = board.Type;
            boardDto.Description = board.Description;
            boardDto.Subject = board.Subject;
            boardDto.CreatorUserId = board.CreatorUserId;
            boardDto.CreatorUser = board.CreatorUser;
            boardDto.OriginalBoardId = board.OriginalBoardId;
            boardDto.OriginalBoardName = board.OriginalBoardName;
            boardDto.IsModified = board.IsModified;
            boardDto.CompletedDate = board.CompletedDate;
            boardDto.DaysRemaining = board.timeLength > 0
                ? Math.Max(0, (int)(boardDto.EndDate - DateTime.Now).TotalDays)
                : null;

            return boardDto;
        }


        private BoardRepository FromBoardDtotoRepository(BoardDto board)
        {
            try
            {
                var boardRepository = new BoardRepository();
                boardRepository.Id = board.Id;
                boardRepository.Name = board.Name;
                boardRepository.CreationDate = board.CreationDate;
                boardRepository.StartDate = board.StartDate;
                boardRepository.timeLength = board.timeLength;
                boardRepository.IsAvailable = board.IsAvailable;
                boardRepository.Streak = board.Streak;
                boardRepository.BestStreak = board.BestStreak;
                boardRepository.IsPublic = board.IsPublic;
                boardRepository.Type = board.timeLength == 0 ? "Short" : "Long";
                boardRepository.Description = board.Description;
                boardRepository.CreatorUserId = board.UserId;
                boardRepository.CreatorUserId = board.CreatorUserId;
                boardRepository.CreatorUser = board.CreatorUser;
                boardRepository.OriginalBoardId = board.OriginalBoardId;
                boardRepository.OriginalBoardName = board.OriginalBoardName;
                boardRepository.Subject = board.Subject;
                boardRepository.IsModified = board.IsModified;

                return boardRepository;
            }

            catch
            {
                return new BoardRepository();
            }
        }

        private List<TaskDto> FromTaskRepositoryToDtoList(List<TaskRepository> list)
        {
            var listDto = new List<TaskDto>();
            foreach (var task in list)
            {
                var taskDto = new TaskDto();
                taskDto.Id = task.Id;
                taskDto.Name = task.Name;
                taskDto.Description = task.Description;
                taskDto.CreationDate = task.CreationDate;
                taskDto.IsAvailable = task.IsAvailable;
                taskDto.IsCompleted = task.IsCompleted;
                taskDto.Status = task.Status;
                taskDto.BoardId = task.BoardId;
                taskDto.UserId = task.UserId;

                listDto.Add(taskDto);
            }
            return listDto;

        }

    }
}
