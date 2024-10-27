using DataAccessLayer.Interface;
using DTOs;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class UserDAO : IUserRepository
    {
        private static readonly UserDAO _instance;
        private UserDAO() { }
        static UserDAO()
        {
            _instance = new UserDAO();
        }
        public static UserDAO Instance => _instance;

        public async Task<bool> Add(UserDTO dto)
        {
            bool result = false;
            if (dto != null)
            {
                using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
                {
                    var role = _context.Roles.FirstOrDefaultAsync(r => r.Id == dto.RoleId);
                    if(role != null)
                    {
                        _context.Users.Add(new User()
                        {
                            Name = dto.Name,
                            Password = dto.Password,
                            Email = dto.Email,
                            Phone = dto.Phone,
                            RoleId = role.Id,
                        });
                        await _context.SaveChangesAsync();
                        result = true;
                    }
                }
            }
            return result;
        }

        public async Task<bool> Delete(int userId)
        {
            bool result = false;
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var user =  await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    result = true;
                }
            }
            return result;
        }

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var result = await _context.Users
                    .Include(u => u.Role)
                .Select(user => new UserDTO()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Password = user.Password,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role.Title,
                    RoleId = user.Id,
                    Status = user.Status
                }).ToListAsync();
                return result;
            }
        }

        public async Task<UserDTO> GetById(int userId)
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    return new UserDTO()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Password = user.Password,
                        Email = user.Email,
                        Phone = user.Phone,
                        Role = user.Role.Title,
                        Status = user.Status,
                        RoleId = user.Id,
                    };
                }
                else
                {
                    return null!;
                }
            }
        }

        public Task<bool> Update(UserDTO dto)
        {
            throw new NotImplementedException();
        }

    }
}
