using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interface
{
    public interface IVarietyService
    {
        Task<IEnumerable<VarietyDTO>> GetAll();
        Task AddVariety(VarietyDTO variety);
        Task UpdateVariety(VarietyDTO variety);
        Task DeleteVariety(int varietyId);
        Task<IEnumerable<VarietyDTO>> SearchVarietyByName(string name);
    }
}
