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

        public async Task<bool> CreateUser(UserDTO dto, int roleId)
        {
            var result = await _repository.User.CreateUser(dto, roleId);
            if (!result)
            {
                throw new Exception("Failed to create user.");
            }
            return true;
        }


        public async Task<bool> UpdateUser(UserDTO dto)
        {

            //if (dto.RoleId != 2)
            ////{
            ////    throw new ArgumentException("Only referees with RoleID 3 can be updated.");
            ////}
            return await _repository.User.Update(dto);
        }

        public async Task<bool> DeleteReferee(int userId)
        {
            // Delete referee by userId
            return await _repository.User.DeleteReferee(userId);
        }

        public async Task<IEnumerable<UserDTO>> SearchRefereeName(string refereeName)
        {
            var result = await _repository.User.GetAll();

            if (string.IsNullOrEmpty(refereeName))
            {
                return result.Where(x => x.RoleId == 2).ToList(); // Filter only referees if no name is specified
            }

            return  result
                .Where(x => x.Name.Contains(refereeName, StringComparison.OrdinalIgnoreCase) && x.Role!.Equals("Referee", StringComparison.OrdinalIgnoreCase));
                //.ToList();
        }


        public async Task<IEnumerable<UserDTO>> GetAllRefereeToView() => (await _repository.User.GetAll())
                                                                        .Where(u => u.Role!.Equals("Referee", StringComparison.OrdinalIgnoreCase));

    }
}
