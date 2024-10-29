using BusinessLogicLayer.Interface;
using DataAccessLayer;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Implementation
{
    public class RegistrationService : IRegistrationService
    {
        private static readonly RegistrationService _instance;
        private readonly Repository _repository;
        private RegistrationService()
        {
            _repository = Repository.Instance;
        }
        static RegistrationService()
        {
            _instance = new RegistrationService();
        }
        public static RegistrationService Instance => _instance;
        public async Task<bool> Add(RegistrationDTO dto)
        {
            return await _repository.Registration.Add(dto);
        }

        public async Task<IEnumerable<RegistrationDTO>> Search(string key)
        {
            var registrations = await _repository.Registration.GetAll();
            if (registrations.Any() == true)
            {
                var result = registrations.Where(r =>
                                r.KoiName!.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                || r.ShowName!.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                || r.Rank!.ToString()!.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                || r.Description.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                || r.KoiVariety!.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                || r.CreateDate.ToString().Contains(key, StringComparison.OrdinalIgnoreCase) == true);
                return result;
            }
            else
                return null!;
        }

        public async Task<bool> Update(RegistrationDTO dto) => await _repository.Registration.Update(dto);
    }
}
