using TFGBack._02_Domain.Infrastructure.Contracts.Models;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;

namespace TFGBack._02_Domain.Infrastructure.Contracts.Contracts
{
    public interface ITaskServiceRepository
    {

        List<TaskRepository> getTasksFromBoard(int userId, int boardId);
        bool modifyTask(int userId, int boardId, int taskId, TaskRepository taskDto);
        bool removeTask(int userId, int boardId, int taskId);
        bool addTask(TaskRepository task, int boardId, int userIdS);
    }
}
