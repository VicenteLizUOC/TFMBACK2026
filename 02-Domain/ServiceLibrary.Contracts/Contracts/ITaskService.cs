using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;

namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts
{
    public interface ITaskService
    {

        List<TaskDto> getTaskFromBoard(int userId, int boardId);
        bool modifyTask(int userId, int boardId, int taskId, TaskDto task);
        bool addTask(TaskDto task, int boardId, int userId);
        bool removeTask(int userId, int boardId, int taskId);
        Task<string?> getTaskHelp(int userId, int taskId, int boardId);
    }
}
