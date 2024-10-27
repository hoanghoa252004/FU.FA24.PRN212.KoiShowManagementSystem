using DataAccessLayer.Interface;
using DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class VarietyDAO : IVarietyRepository
    {
        private static readonly VarietyDAO _instance;
        private VarietyDAO() { }
        static VarietyDAO()
        {
            _instance = new VarietyDAO();
        }
        public static VarietyDAO Instance => _instance;
        public async Task<IEnumerable<VarietyDTO>> GetAll()
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var result =  await _context.Varieties.Select(v => new VarietyDTO()
                {
                    Id = v.Id,
                    Name = v.Name,
                    Status = v.Status,
                }).ToListAsync();
                return result;
            }
        }
    }
}
