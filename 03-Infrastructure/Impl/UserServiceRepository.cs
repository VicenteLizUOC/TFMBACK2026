using Microsoft.EntityFrameworkCore.Storage;
using TFGBack._02_Domain.Infrastructure.Contracts.Contracts;
using TFGBack._03_Infrastructure.Database;
using TFGBack._03_Infrastructure.Models;

namespace TFGBack._03_Infrastructure.Impl
{
    public class UserServiceRepository : IUserServiceRepository
    {
        private readonly IDatabaseService _databaseService;

        public UserServiceRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }


        public bool registerUserRepository(UserRepository userRepository)
        {

            var databaseResponse = _databaseService.AddUserDb(userRepository);

            return databaseResponse;

        }

        public UserRepository loginRepository(UserRepository userRepository)
        {

            var databaseResponse = _databaseService.loginDb(userRepository);

            return databaseResponse;

        }

        public UserRepository getUserRepository(int userId)
        {
            var databaseResponse = _databaseService.GetSingleUserDb(userId);

            return databaseResponse;
        }

        public bool removeUserRepository(int userId)
        {
            return _databaseService.DeleteUserDb(userId);
        }

        public bool modifyUserRepository(int id, UserRepository userRepository)
        {
            return _databaseService.UpdateUserDb(id, userRepository);
        }

        public bool toggleUserFollow(int followerId, int followedId)
        {
            return _databaseService.toggleUserFollow(followerId, followedId);
        }

        public int getUserFollowersCount(int userId)
        {
            return _databaseService.getUserFollowersCount(userId);
        }

        public int getUserFollowingCount(int userId)
        {
            return _databaseService.getUserFollowingCount(userId);
        }

        public bool isFollowing(int followerId, int followedId)
        {
            return _databaseService.isFollowing(followerId, followedId);
        }

        public List<UserRepository> getUserFollowersList(int userId)
        {
            return _databaseService.getUserFollowersList(userId);
        }

        public List<UserRepository> getUserFollowingList(int userId)
        {
            return _databaseService.getUserFollowingList(userId);
        }
    }
}
