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
    public class KoiDAO : IKoiRepository
    {
        private static readonly KoiDAO _instance;
        private KoiDAO() { }
        static KoiDAO()
        {
            _instance = new KoiDAO();
        }
        public static KoiDAO Instance => _instance;


        public async Task<bool> CreateKoi(KoiDTO koiDto)
        {
            using (var context = new Prn212ProjectKoiShowManagementContext())
            {
                Koi koi = new Koi
                {
                    Name = koiDto.Name,
                    Description = koiDto.Description,
                    Size = koiDto.Size,
                    Image = koiDto.Image,
                    UserId = koiDto.UserId,
                    VarietyId = koiDto.VarietyId,
                    Status = koiDto.Status
                };
                await context.Kois.AddAsync(koi);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateKoi(KoiDTO koiDto)
        {
            using (var context = new Prn212ProjectKoiShowManagementContext())
            {
                Koi koi = await context.Kois.FindAsync(koiDto.Id);
                if (koi != null)
                {
                    koi.Name = koiDto.Name;
                    koi.Description = koiDto.Description;
                    koi.Size = koiDto.Size;
                    koi.Image = koiDto.Image;
                    koi.UserId = koiDto.UserId;
                    koi.VarietyId = koiDto.VarietyId;
                    koi.Status = koiDto.Status;
                    context.Kois.Update(koi);
                    await context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        }


        public async Task<bool> DeleteKoi(int koiId)
        {
            using (var context = new Prn212ProjectKoiShowManagementContext())
            {
                var koi = await context.Kois.Include(k => k.Registrations)
                                            .FirstOrDefaultAsync(k => k.Id == koiId);
                if (koi != null)
                {
                    if (koi.Registrations != null && koi.Registrations.Any())
                    {
                        // Nếu Koi đã đăng ký cuộc thi, chỉ thay đổi Status thành 0
                        koi.Status = false;
                        context.Kois.Update(koi);
                    }
                    else
                    {
                        // Nếu Koi chưa đăng ký cuộc thi nào, xóa khỏi database
                        context.Kois.Remove(koi);
                    }
                    await context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        }


        public async Task<KoiDTO?> GetKoiByName(string koiname)
        {
            using (var context = new Prn212ProjectKoiShowManagementContext())
            {
                var koi = await context.Kois.FirstOrDefaultAsync(k => k.Name == koiname);
                if (koi != null)
                {
                    return new KoiDTO
                    {
                        Id = koi.Id,
                        Name = koi.Name,
                        Description = koi.Description,
                        Size = koi.Size,
                        Image = koi.Image,
                        UserId = koi.UserId,
                        VarietyId = koi.VarietyId,
                        Status = koi.Status
                    };
                }
                return null;
            }
        }

        public async Task<IEnumerable<KoiDTO>> GetAllKoisByUser(int userId)
        {
            using (var context = new Prn212ProjectKoiShowManagementContext())
            {
                return await context.Kois
                    .Where(k => k.UserId == userId)
                    .Include(k => k.Variety)
                    .Select(koi => new KoiDTO
                    {
                        Id = koi.Id,
                        Name = koi.Name,
                        Description = koi.Description,
                        Size = koi.Size,
                        Image = koi.Image,
                        UserId = koi.UserId,
                        VarietyId = koi.VarietyId,
                        VarietyName = koi.Variety.Name!,
                        Status = koi.Status
                    }).ToListAsync();
            }
        }
    }
}
