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
        Task<bool> CreateUser(UserDTO dto, int roleId);
        Task<bool> UpdateUser(UserDTO dto);
        Task<bool> DeleteReferee(int userId);

        Task<IEnumerable<UserDTO>> SearchRefereeName(string refereeName);
        Task<IEnumerable<UserDTO>> GetAllRefereeToView();
    }
}
