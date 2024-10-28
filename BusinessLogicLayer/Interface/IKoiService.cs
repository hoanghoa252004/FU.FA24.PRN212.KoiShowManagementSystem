using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interface
{
    public interface IKoiService
    {
        Task<bool> CreateKoi(KoiDTO koiDto);
        Task<bool> UpdateKoi(KoiDTO koiDto);
        Task<bool> DeleteKoi(int koiId);
        Task<KoiDTO?> GetKoiByName(string koiName);
        Task<IEnumerable<KoiDTO>> GetAllKoisByUser(int userId);
        Task<IEnumerable<KoiDTO>> SearchKoiName(string koiName, int userId);

    }
}
