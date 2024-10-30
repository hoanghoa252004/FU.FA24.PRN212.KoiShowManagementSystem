using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllReferee();
        Task<bool> CreateReferee(UserDTO dto);
        Task<bool> UpdateUser(UserDTO dto);
        Task<bool> DeleteReferee(int userId);
    }
}
