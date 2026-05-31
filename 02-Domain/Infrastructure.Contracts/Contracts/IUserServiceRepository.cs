using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;
using TFGBack._03_Infrastructure.Models;

namespace TFGBack._02_Domain.Infrastructure.Contracts.Contracts
{
    public interface IUserServiceRepository
    {
        UserRepository getUserRepository(int userId);
        bool registerUserRepository(UserRepository userDto);
        UserRepository loginRepository(UserRepository userDto);
        bool modifyUserRepository(int id, UserRepository userDto);
        bool removeUserRepository(int userId);
        bool toggleUserFollow(int followerId, int followedId);
        int getUserFollowersCount(int userId);
        int getUserFollowingCount(int userId);
        bool isFollowing(int followerId, int followedId);
        List<UserRepository> getUserFollowersList(int userId);
        List<UserRepository> getUserFollowingList(int userId);
    }
}
