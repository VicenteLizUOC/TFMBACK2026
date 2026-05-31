using TFGBack._02_Domain.Infrastructure.Contracts.Contracts;
using TFGBack._02_Domain.Infrastructure.Contracts.Models;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;

namespace TFGBack._02_Domain.ServiceLibrary.Impl.Impl
{
    public class CommentService : ICommentService
    {
        private readonly ICommentServiceRepository _commentServiceRepository;
        private readonly IUserServiceRepository _userServiceRepository;

        public CommentService(ICommentServiceRepository commentServiceRepository, IUserServiceRepository userServiceRepository)
        {
            _commentServiceRepository = commentServiceRepository;
            _userServiceRepository = userServiceRepository;
        }

        public List<BoardCommentDto> getComments(int boardId)
        {
            var comments = _commentServiceRepository.getComments(boardId);
            return comments.Select(c =>
            {
                var user = _userServiceRepository.getUserRepository(c.UserId);
                var dto = new BoardCommentDto
                {
                    Id = c.Id,
                    BoardId = c.BoardId,
                    UserId = c.UserId,
                    UserName = user?.Name ?? "Unknown",
                    UserType = user?.UserType ?? "free",
                    CreatorBoardUserId = c.CreatorBoardUserId,
                    Content = c.Content,
                    CreationDate = c.CreationDate
                };

                if (user?.ProfilePicture != null && user.ProfilePicture.Length > 0)
                    dto.UserProfilePictureBase64 = Convert.ToBase64String(user.ProfilePicture);

                return dto;
            }).ToList();
        }

        public bool addComment(int boardId, int userId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return false;

            var comment = new BoardCommentRepository
            {
                BoardId = boardId,
                UserId = userId,
                Content = content
            };

            return _commentServiceRepository.addComment(comment);
        }

        public bool deleteComment(int commentId, int userId)
        {
            return _commentServiceRepository.deleteComment(commentId, userId);
        }
    }
}
