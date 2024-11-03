using BusinessLogicLayer.Interface;
using DataAccessLayer;
using DataAccessLayer.Implementation;
using DataAccessLayer.Interface;
using DTOs;
using Entities;
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

        public async Task<IEnumerable<VarietyDTO>> SearchVarietyByName(string name)
        {
            var allVarieties = await _repository.Variety.GetAll();
            if (string.IsNullOrEmpty(name))
            {
                return allVarieties;
            }

            return allVarieties.Where(v => v.Name!.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task AddVariety(VarietyDTO variety)
        {
            await _repository.Variety.AddAsync(new Variety
            {
                Name = variety.Name,
                Status = variety.Status
            });
        }

        public async Task UpdateVariety(VarietyDTO variety)
        {
            var entity = new Variety
            {
                Id = variety.Id,
                Name = variety.Name,
                Status = variety.Status
            };
            await _repository.Variety.UpdateAsync(entity);
        }

        public async Task DeleteVariety(int varietyId)
        {
            await _repository.Variety.DeleteAsync(varietyId);
        }
    }
}
