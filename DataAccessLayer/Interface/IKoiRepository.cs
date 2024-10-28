using DTOs;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IKoiRepository
    {
        Task<bool> CreateKoi(KoiDTO koiDto);
        Task<bool> UpdateKoi(KoiDTO koiDto);
        Task<bool> DeleteKoi(int koiId);
        Task<KoiDTO?> GetKoiByName(string koiname);
        Task<IEnumerable<KoiDTO>> GetAllKoisByUser(int userId);
    }
}
