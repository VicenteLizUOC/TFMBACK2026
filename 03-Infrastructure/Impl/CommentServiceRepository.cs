using TFGBack._02_Domain.Infrastructure.Contracts.Contracts;
using TFGBack._02_Domain.Infrastructure.Contracts.Models;
using TFGBack._03_Infrastructure.Database;

namespace TFGBack._03_Infrastructure.Impl
{
    public class CommentServiceRepository : ICommentServiceRepository
    {
        private readonly IDatabaseService _databaseService;

        public CommentServiceRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<BoardCommentRepository> getComments(int boardId)
        {
            return _databaseService.getComments(boardId);
        }

        public bool addComment(BoardCommentRepository comment)
        {
            return _databaseService.addComment(comment);
        }

        public bool deleteComment(int commentId, int userId)
        {
            return _databaseService.deleteComment(commentId, userId);
        }
    }
}
