using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interface
{
    public interface IAuthenticationService
    {
        Task<UserDTO> Login(string email, string password);

        Task<bool> SignUp(UserDTO dto);
    }
}
