using TFGBack._02_Domain.Infrastructure.Contracts.Contracts;
using TFGBack._02_Domain.Infrastructure.Contracts.Models;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;
using TFGBack._03_Infrastructure.Database;

namespace TFGBack._03_Infrastructure.Impl
{
    public class TaskServiceRepository : ITaskServiceRepository
    {
        private readonly IDatabaseService _databaseService;

        public TaskServiceRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<TaskRepository> getTasksFromBoard(int userId, int boardId)
        {
            return _databaseService.getTasksFromBoard(userId, boardId);
        }

        public bool modifyTask(int userId, int boardId, int taskId, TaskRepository task)
        {
            return _databaseService.modifyTask(userId, taskId, boardId, task);
        }

        public bool removeTask(int userId, int boardId, int taskId)
        {
            return _databaseService.removeTask(userId, boardId, taskId);
        }

        public bool addTask(TaskRepository task, int boardId, int userId)
        {
            return _databaseService.addTask(task, boardId, userId);
        }

    }
}
