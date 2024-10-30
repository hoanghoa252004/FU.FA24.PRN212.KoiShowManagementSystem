using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IUserRepository
    {
        Task<UserDTO> GetById(int userId);
        Task<IEnumerable<UserDTO>> GetAll();
        Task<bool> Add(UserDTO dto);
        Task<bool> Update(UserDTO dto);
        Task<bool> Delete(int userId);
        Task<bool> CreateReferee(UserDTO dto);
        Task<bool> DeleteReferee(int userId);
    }
}
