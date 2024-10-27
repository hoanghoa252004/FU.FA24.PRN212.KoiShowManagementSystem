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
    }
}
