using DTOs;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IVarietyRepository
    {
        Task<IEnumerable<VarietyDTO>> GetAll();
        Task AddAsync(Variety variety);
        Task UpdateAsync(Variety variety);
        Task DeleteAsync(int VarietyId);
    }
}
