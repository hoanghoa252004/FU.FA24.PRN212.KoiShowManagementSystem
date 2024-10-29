using BusinessLogicLayer.Interface;
using DataAccessLayer;
using DTOs;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Implementation
{
    public class KoiService : IKoiService
    {
        private static readonly KoiService _instance;
        private readonly Repository _repository;
        private KoiService()
        {
            _repository = Repository.Instance;
        }
        static KoiService()
        {
            _instance = new KoiService();
        }
        public static KoiService Instance => _instance;

        public async Task<bool> CreateKoi(KoiDTO koiDto)
        {
            return await _repository.Koi.CreateKoi(koiDto);
        }

        public async Task<bool> UpdateKoi(KoiDTO koiDto)
        {
            return await _repository.Koi.UpdateKoi(koiDto);
        }

        public async Task<bool> DeleteKoi(int koiId)
        {
            return await _repository.Koi.DeleteKoi(koiId);
        }

        public async Task<KoiDTO?> GetKoiByName(string koiName)
        {
            return await _repository.Koi.GetKoiByName(koiName);
        }

        public async Task<IEnumerable<KoiDTO>> GetAllKoisByUser(int userId)
        {
            return await _repository.Koi.GetAllKoisByUser(userId);

        }

        public async Task<IEnumerable<KoiDTO>> SearchKoiName(string koiName, int userId)
        {
            var result = await _repository.Koi.GetAllKoisByUser(userId);
            if (string.IsNullOrEmpty(koiName))
            {
                return result;
            }
            return result.Where(x => x.Name.Contains(koiName, StringComparison.OrdinalIgnoreCase)).ToList();
        }


    }
}
