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
            bool result = false;
            // V01: Kiểm tra con koi đó đã thi show này chưa:
            var registrations = await _repository.Registration.GetRegistrationsByShow(dto.ShowId);
            var registeredAlready = registrations.Any(r => r.KoiId == dto.KoiId);
            if(registeredAlready == true)
            {
                throw new Exception("Your Koi's already register for this show !");
            }
            else
            {
                result = await _repository.Registration.Add(dto);
            }
            return result;
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
                                || r.Description!.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                || r.KoiVariety!.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                || r.CreateDate.ToString().Contains(key, StringComparison.OrdinalIgnoreCase) == true);
                return result;
            }
            else
                return null!;
        }

        public async Task<bool> Update(RegistrationDTO dto) => await _repository.Registration.Update(dto);

        public async Task<IEnumerable<RegistrationDTO>> GetAllRegistrationForReferee(int userId)
        {
            return await _repository.Registration.GetRegistrationsByReferee(userId);
        }

        public async Task<bool> Delete(int registrationId)
        {
            bool result = false;
            var registration = await _repository.Registration.GetById(registrationId);
            if(registration != null)
            {
                if (registration.Status!.Equals("Pending", StringComparison.OrdinalIgnoreCase) == true
                        || registration.Status!.Equals("Rejected", StringComparison.OrdinalIgnoreCase) == true)
                {
                    result = await _repository.Registration.Delete(registrationId);
                }
                else
                {
                    throw new Exception("Registration can not be deleted !");
                }
            }
            else
            {
                throw new Exception("Registration does not exist !");
            }
            return result;
        }

        public async Task<IEnumerable<RegistrationDTO>> GetAllRegistration(int userId)
        {
            IEnumerable<RegistrationDTO> result = null!;
            var user = await _repository.User.GetById(userId);
            if(user != null)
            {
                if (user.Role!.Equals("Member", StringComparison.OrdinalIgnoreCase) == true)
                {
                    result =  await _repository.Registration.GetRegistrationsByMember(userId);
                }
                else if (user.Role!.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
                {
                    result =  await _repository.Registration.GetAll();
                }
                else
                {
                    throw new Exception("You do not have permission to use this behavior !");
                }
            }
            else
            {
                throw new Exception("User does not exist !");
            }
            return result;
        }
    }
}
