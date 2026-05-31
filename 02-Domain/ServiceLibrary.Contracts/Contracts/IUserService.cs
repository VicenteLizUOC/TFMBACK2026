using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;

namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts
{
    public interface IUserService
    {
        UserDto getUser(int userId);
        UserProfileDto getUserProfile(int targetUserId, int requestingUserId);
        bool toggleUserFollow(int followerId, int followedId);
        bool removeUser(int userId);
        Task<bool> registerUser(UserDto userDto);
        UserDto login(UserDto userDto);
        Task<bool> modifyUser(int userId, UserDto userDto);
    }
}
