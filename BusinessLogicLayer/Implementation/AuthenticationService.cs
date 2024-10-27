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
    public class AuthenticationService : IAuthenticationService
    {
        private static readonly AuthenticationService _instance;
        private readonly Repository _repository;
        private AuthenticationService()
        {
            _repository = Repository.Instance;
        }
        static AuthenticationService()
        {
            _instance = new AuthenticationService();
        }
        public static AuthenticationService Instance => _instance;
        public async Task<UserDTO> Login(string email, string password)
        {
            var users = await _repository.User.GetAll();
            var result = users
                .SingleOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) 
                                        && u.Password.Equals(password, StringComparison.OrdinalIgnoreCase));
            if(result != null)
            {
                if(result.Status == false)
                {
                    throw new Exception("Your account's banned already!");
                }
                else
                {
                    return result;
                }
            }
            else
            {
                throw new Exception("Incorrect Email or Password !");
            }
        }

        public Task<bool> SignUp(UserDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
