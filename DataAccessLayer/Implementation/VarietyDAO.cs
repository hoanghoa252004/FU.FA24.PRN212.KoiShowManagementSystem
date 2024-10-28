using DataAccessLayer.Interface;
using DTOs;
using Entities;
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


        public async Task<IEnumerable<VarietyDTO>> GetVariety()
        {
            using (var context = new Prn212ProjectKoiShowManagementContext())
            {
                var result = await context.Varieties.Select( v => new VarietyDTO
                {
                    Id = v.Id,
                    Name = v.Name,
                    Status = v.Status,
                }).ToListAsync();
                return result;
            }
        }

        public async Task AddAsync(Variety variety)
        {
            using (var context = new Prn212ProjectKoiShowManagementContext())
            {
                await context.Varieties.AddAsync(variety);
                await context.SaveChangesAsync();
            }
        }


        public async Task UpdateAsync(Variety variety)
        {
            using (var context = new Prn212ProjectKoiShowManagementContext())
            {
                context.Varieties.Update(variety);
                await context.SaveChangesAsync();
            }
        }


        public async Task DeleteAsync(int VarietyId)
        {
            using (var context = new Prn212ProjectKoiShowManagementContext())
            {
                var variety = await context.Varieties.FindAsync(VarietyId);
                if (variety != null)
                {
                    context.Varieties.Remove(variety);
                    await context.SaveChangesAsync();
                }
            }
        }


    }
}
