using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public  interface IRegistrationRepository
    {
        Task<RegistrationDTO> GetById(int registrationId);
        Task<IEnumerable<RegistrationDTO>> GetAll();
        Task<bool> Add(RegistrationDTO dto);
        Task<bool> Update(RegistrationDTO dto);
        Task<bool> Delete(int registrationId);
        Task CalculateTotalScoreAllForRegistation(int showId);
        Task Ranking(int showId);
             Task<IEnumerable<RegistrationDTO>> GetRegistrationsByReferee(int userId);
    }
}
