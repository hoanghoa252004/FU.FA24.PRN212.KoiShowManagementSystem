using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;

namespace BusinessLogicLayer.Interface
{
    public interface IRegistrationService
    {
        Task<bool> Add(RegistrationDTO dto);
        Task<IEnumerable<RegistrationDTO>> Search(string key);
        Task<bool> Update(RegistrationDTO dto);
        Task<IEnumerable<RegistrationDTO>> GetAllRegistrationForReferee(int userId);
        Task<bool> Delete(int registrationId);
        Task<IEnumerable<RegistrationDTO>> GetAllRegistration(int userId);
        Task<IEnumerable<RegistrationDTO>> GetPendingRegistration();
    }
}
