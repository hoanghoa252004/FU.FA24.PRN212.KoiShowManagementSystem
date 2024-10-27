using DTOs;
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
    }
}
