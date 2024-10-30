using BusinessLogicLayer.Interface;
using DataAccessLayer;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Implementation
{
    public class UserService : IUserService
    {
        private static readonly UserService _instance;
        private readonly Repository _repository;
        private UserService()
        {
            _repository = Repository.Instance;
        }
        static UserService()
        {
            _instance = new UserService();
        }
        public static UserService Instance => _instance;
        public async Task<IEnumerable<UserDTO>> GetAllReferee() => (await _repository.User.GetAll())
                                                                        .Where(u => u.Role!.Equals("Referee", StringComparison.OrdinalIgnoreCase)
                                                                                    && u.Status == true);

        public async Task<bool> CreateReferee(UserDTO dto)
        {

            dto.RoleId = 3;
            return await _repository.User.CreateReferee(dto);
        }

        public async Task<bool> UpdateUser(UserDTO dto)
        {
            // Ensure RoleId is 3 for updating referees only
            if (dto.RoleId != 3)
            {
                throw new ArgumentException("Only referees with RoleID 3 can be updated.");
            }
            return await _repository.User.Update(dto);
        }

        public async Task<bool> DeleteReferee(int userId)
        {
            // Delete referee by userId
            return await _repository.User.DeleteReferee(userId);
        }

    }
}
