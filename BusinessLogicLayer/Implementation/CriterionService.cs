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
    public class CriterionService : ICrierionService
    {
        private static readonly CriterionService _instance;
        private readonly Repository _repository;
        private CriterionService()
        {
            _repository = Repository.Instance;
        }
        static CriterionService()
        {
            _instance = new CriterionService();
        }
        public static CriterionService Instance => _instance;

        public async Task<IEnumerable<CriterionDTO>> GetAllCriterionByShow(int ShowId)
        {
            return await _repository.Criterion.GetCriterionByShow(ShowId);
        }
    }
}
