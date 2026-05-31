using TFGBack._02_Domain.Infrastructure.Contracts.Contracts;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;
using TFGBack._03_Infrastructure.Models;

namespace TFGBack._02_Domain.ServiceLibrary.Impl.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserServiceRepository _userServiceRepository;
        private readonly IBoardService _boardService;

        public UserService(IUserServiceRepository userServiceRepository, IBoardService boardService)
        {
            _userServiceRepository = userServiceRepository;
            _boardService = boardService;
        }

        public async Task<bool> registerUser(UserDto userDto)
        {
            try
            {
                // Ahora usamos await porque el mapeador procesa el stream de la imagen
                var user = await fromUserDtoToRepository(userDto);
                return _userServiceRepository.registerUserRepository(user);
            }
            catch (Exception ex)
            {
                // Es recomendable loguear la excepción: console.WriteLine(ex.Message);
                return false;
            }
        }

        public UserDto login(UserDto userDto)
        {
            try
            {
                // En el login normalmente no enviamos foto, pero usamos el mapeador básico
                // Si el mapeador es async, aquí podrías usar un mapeador simple sin procesamiento de imagen
                var userRepository = new UserRepository
                {
                    Email = userDto.Email,
                    Password = userDto.Password
                };

                var response = _userServiceRepository.loginRepository(userRepository);

                if (response == null) return new UserDto();

                // Al convertir de vuelta al DTO, incluimos la imagen que viene de SQL
                return fromUserRepositoryToDto(response);
            }
            catch (Exception ex)
            {
                return new UserDto();
            }
        }


        //public bool registerUser(UserDto userDto)
        //{
        //    try
        //    {
        //        var user = fromUserDtoToRepository(userDto);
        //        return _userServiceRepository.registerUserRepository(user);
        //    }
        //    catch (Exception ex) 
        //    {
        //        return false;
        //    }
        //}

        //public UserDto login(UserDto userDto)
        //{
        //    try
        //    {
        //        var user = fromUserDtoToRepository(userDto);
        //        var response = _userServiceRepository.loginRepository(user);
        //        return fromUserRepositoryToDto(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new UserDto();
        //    }
        //}

        public UserDto getUser(int userId)
        {
            var user = _userServiceRepository.getUserRepository(userId);
            var dto = fromUserRepositoryToDto(user);

            var followers = _userServiceRepository.getUserFollowersList(userId);
            var following = _userServiceRepository.getUserFollowingList(userId);

            dto.FollowersCount = followers.Count;
            dto.FollowingCount = following.Count;
            dto.Followers = followers.Select(toUserSummaryDto).ToList();
            dto.Following = following.Select(toUserSummaryDto).ToList();

            var myBoards = _boardService.getMyyBoards(userId);
            dto.BoardBestStreaks = myBoards
                .Where(b => b.BestStreak > 0)
                .Select(b => new BoardStreakDto { Id = b.Id, Name = b.Name, BestStreak = b.BestStreak })
                .OrderByDescending(b => b.BestStreak)
                .ToList();

            return dto;
        }

        public UserProfileDto getUserProfile(int targetUserId, int requestingUserId)
        {
            try
            {
                var user = _userServiceRepository.getUserRepository(targetUserId);
                if (user == null || !user.IsAvailable)
                    return new UserProfileDto();

                var profile = new UserProfileDto();
                profile.Id = user.Id;
                profile.Name = user.Name;
                profile.UserType = user.UserType;

                if (user.ProfilePicture != null && user.ProfilePicture.Length > 0)
                    profile.ProfilePictureBase64 = Convert.ToBase64String(user.ProfilePicture);

                profile.FollowersCount = _userServiceRepository.getUserFollowersCount(targetUserId);
                profile.FollowingCount = _userServiceRepository.getUserFollowingCount(targetUserId);
                profile.IsFollowing = _userServiceRepository.isFollowing(requestingUserId, targetUserId);
                profile.PublicBoards = _boardService.getPublicBoardsByUser(targetUserId);

                return profile;
            }
            catch (Exception ex)
            {
                return new UserProfileDto();
            }
        }

        public bool toggleUserFollow(int followerId, int followedId)
        {
            return _userServiceRepository.toggleUserFollow(followerId, followedId);
        }

        public bool removeUser(int userId)
        {
            return _userServiceRepository.removeUserRepository(userId);
        }

        public async Task<bool> modifyUser(int userId, UserDto userDto)
        {
            // Cambiamos a 'await' porque la conversión ahora procesa archivos
            var userRepository = await fromUserDtoToRepository(userDto);
            return _userServiceRepository.modifyUserRepository(userId, userRepository);
        }

        private async Task<UserRepository> fromUserDtoToRepository(UserDto userDto)
        {
            var userRepository = new UserRepository();
            //userRepository.Id = userId; // Asegúrate de asignar el ID
            userRepository.Name = userDto.Name;
            userRepository.Email = userDto.Email;
            userRepository.Password = userDto.Password;
            userRepository.UserType = userDto.UserType;
            userRepository.IsAvailable = true;

            // --- LÓGICA PARA LA IMAGEN ---
            if (userDto.ProfilePicture != null && userDto.ProfilePicture.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Copiamos el stream del archivo al array de bytes
                    await userDto.ProfilePicture.CopyToAsync(memoryStream);
                    userRepository.ProfilePicture = memoryStream.ToArray();
                }
            }

            return userRepository;
        }

        private UserSummaryDto toUserSummaryDto(UserRepository u)
        {
            var dto = new UserSummaryDto { Id = u.Id, Name = u.Name };
            if (u.ProfilePicture != null && u.ProfilePicture.Length > 0)
                dto.ProfilePictureBase64 = Convert.ToBase64String(u.ProfilePicture);
            return dto;
        }

        private UserDto fromUserRepositoryToDto(UserRepository userRepository)
        {
            var userDto = new UserDto();
            userDto.Id = userRepository.Id;
            userDto.Name = userRepository.Name;
            userDto.Email = userRepository.Email;
            userDto.Password = userRepository.Password;
            userDto.UserType = userRepository.UserType;
            userDto.IsAvailable = userRepository.IsAvailable;

            // Convertimos el byte[] de UserRepository a string para UserDto
            if (userRepository.ProfilePicture != null && userRepository.ProfilePicture.Length > 0)
            {
                userDto.ProfilePictureBase64 = Convert.ToBase64String(userRepository.ProfilePicture);
            }

            return userDto;
        }

        //public bool modifyUser(int userId, UserDto userDto)
        //{
        //    var userRepository = fromUserDtoToRepository(userDto);
        //    return _userServiceRepository.modifyUserRepository(userId, userRepository);

        //}


        //private UserRepository fromUserDtoToRepository(UserDto userDto)
        //{
        //    var userRepository = new UserRepository();
        //    userRepository.Name = userDto.Name;
        //    userRepository.Email = userDto.Email;
        //    userRepository.Password = userDto.Password;
        //    userRepository.UserType = userDto.UserType;
        //    userRepository.IsAvailable = true;

        //    return userRepository;

        //}

        //private UserDto fromUserRepositoryToDto(UserRepository userRepository)
        //{
        //    var userDto = new UserDto();
        //    userDto.Id = userRepository.Id;
        //    userDto.Name = userRepository.Name;
        //    userDto.Email = userRepository.Email;
        //    userDto.Password = userRepository.Password;
        //    userDto.UserType = userRepository.UserType;
        //    userDto.IsAvailable = userRepository.IsAvailable;

        //    return userDto;
        //}
    }
}
