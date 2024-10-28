using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IRegistrationRepository
    {
        Task<IEnumerable<RegistrationDTO>> GetRegistrationsByReferee(int userId);
    }
}
