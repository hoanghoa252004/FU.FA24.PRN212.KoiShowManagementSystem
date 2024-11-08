using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interface
{
    public interface IShowService
    {
        Task<IEnumerable<ShowDTO>> Search(int userId, string key);
        Task<bool> Add(ShowDTO dto);
        Task<bool> Update(ShowDTO dto);
        Task<bool> Delete(int showId);
        Task<IEnumerable<RegistrationDTO>> ReviewScore(int showId);
        Task<IEnumerable<ShowDTO>> GetAll(int userId);
        Task<bool> AnnouceResult(int showId);
        Task<IEnumerable<RegistrationDTO>> GetAllKoiParticipants(int showId);
    }
}
