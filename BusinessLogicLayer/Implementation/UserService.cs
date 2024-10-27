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
    }
}
