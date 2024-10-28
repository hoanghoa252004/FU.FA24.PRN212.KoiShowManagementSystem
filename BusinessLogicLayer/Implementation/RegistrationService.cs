using BusinessLogicLayer.Interface;
using DataAccessLayer;
using DTOs;
using Microsoft.EntityFrameworkCore.Internal;
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

        public async Task<IEnumerable<RegistrationDTO>> GetAllRegistrationForReferee(int userId)
        {
            return await _repository.Registration.GetRegistrationsByReferee(userId);
        }
    }
}
