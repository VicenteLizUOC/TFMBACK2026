using System.Threading.Tasks;
using TFGBack._02_Domain.Infrastructure.Contracts.Contracts;
using TFGBack._02_Domain.Infrastructure.Contracts.Models;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;
using TFGBack._03_Infrastructure.Database;
using TFGBack._03_Infrastructure.Models;

namespace TFGBack._02_Domain.ServiceLibrary.Impl.Impl
{
    public class TaskService : ITaskService
    {
        private readonly ITaskServiceRepository _taskServiceRepository;
        private readonly IGeminiService _geminiService;
        private readonly IUserServiceRepository _userServiceRepository;

        public TaskService(ITaskServiceRepository taskServiceRepository, IGeminiService geminiService, IUserServiceRepository userServiceRepository)
        {
            _taskServiceRepository = taskServiceRepository;
            _geminiService = geminiService;
            _userServiceRepository = userServiceRepository;
        }

        public List<TaskDto> getTaskFromBoard(int userId, int boardId)
        {
            var listDto = FromTaskRepositoryToDtoList(_taskServiceRepository.getTasksFromBoard(userId, boardId));
            return listDto;
        }

        public bool modifyTask(int userId, int boardId, int taskId, TaskDto taskDto)
        {
            return _taskServiceRepository.modifyTask(userId, boardId, taskId, FromTaskDtoToRepository(taskDto));
        }

        public bool removeTask(int userId, int boardId, int taskId)
        {
            return _taskServiceRepository.removeTask(userId, boardId, taskId);
        }

        public bool addTask(TaskDto taskDto, int boardId, int userId)
        {
            var task = FromTaskDtoToRepository(taskDto);
            return _taskServiceRepository.addTask(task, boardId, userId);
        }

        public async Task<string?> getTaskHelp(int userId, int taskId, int boardId)
        {
            var user = _userServiceRepository.getUserRepository(userId);
            if (user == null || user.UserType != "pro")
                return null;

            var tasks = _taskServiceRepository.getTasksFromBoard(userId, boardId);
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null)
                return null;

            return await _geminiService.getTaskHelp(task.Name ?? string.Empty, task.Description ?? string.Empty);
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

        private TaskRepository FromTaskDtoToRepository(TaskDto taskDto)
        {
            var task = new TaskRepository();
            task.Id = taskDto.Id;
            task.Name = taskDto.Name;
            task.Description = taskDto.Description;
            task.CreationDate = taskDto.CreationDate;
            task.IsAvailable = taskDto.IsAvailable;
            task.IsCompleted = taskDto.IsCompleted;
            task.Status = taskDto.Status;
            task.BoardId = taskDto.BoardId;
            task.UserId = taskDto.UserId;

            return task;


        }
    }
}
