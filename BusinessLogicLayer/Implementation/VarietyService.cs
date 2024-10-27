using BusinessLogicLayer.Interface;
using DataAccessLayer;
using DataAccessLayer.Implementation;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Implementation
{
    public class VarietyService : IVarietyService
    {
        private static readonly VarietyService _instance;
        private readonly Repository _repository;
        private VarietyService()
        {
            _repository = Repository.Instance;
        }
        static VarietyService()
        {
            _instance = new VarietyService();
        }
        public static VarietyService Instance => _instance;

        public async Task<IEnumerable<VarietyDTO>> GetAll() =>  await _repository.Variety.GetAll();
    }
}
