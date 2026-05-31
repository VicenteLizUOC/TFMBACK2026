using TFGBack._02_Domain.Infrastructure.Contracts.Models;

namespace TFGBack._02_Domain.Infrastructure.Contracts.Contracts
{
    public interface ICommentServiceRepository
    {
        List<BoardCommentRepository> getComments(int boardId);
        bool addComment(BoardCommentRepository comment);
        bool deleteComment(int commentId, int userId);
    }
}
