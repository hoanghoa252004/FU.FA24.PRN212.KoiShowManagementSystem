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
    }
}
