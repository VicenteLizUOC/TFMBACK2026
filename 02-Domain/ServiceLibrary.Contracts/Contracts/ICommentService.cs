using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;

namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts
{
    public interface ICommentService
    {
        List<BoardCommentDto> getComments(int boardId);
        bool addComment(int boardId, int userId, string content);
        bool deleteComment(int commentId, int userId);
    }
}
