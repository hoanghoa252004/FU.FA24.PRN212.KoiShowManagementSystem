﻿using DataAccessLayer.Interface;
using DTOs;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class ShowDAO : IShowRepository
    {
        private static readonly ShowDAO _instance;
        private ShowDAO() { }
        static ShowDAO()
        {
            _instance = new ShowDAO();
        }
        public static ShowDAO Instance => _instance;

        public async Task<bool> Add(ShowDTO dto)
        {
            bool result = false;
            if (dto != null)
            {
                using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
                {
                    // 1. Add show:
                    Show newShow = new Show()
                    {
                        Title = dto.Title,
                        Description = dto.Description,
                        EntranceFee = (decimal)dto.EntranceFee!,
                        RegisterEndDate = (DateOnly)dto.RegisterEndDate!,
                        RegisterStartDate = (DateOnly)dto.RegisterStartDate!,
                        //Status = dto.Status,
                    };
                    await _context.Shows.AddAsync(newShow);
                    await _context.SaveChangesAsync();
                    // 2. Add show detail:
                    foreach (var item in dto.Varieties!)
                    {
                        var variety = await _context.Varieties.FirstOrDefaultAsync(v => v.Id == item.Id);
                        if (variety != null)
                        {
                            newShow.Varieties.Add(variety);
                        }
                    }
                    // 3. Add criteria:
                    foreach (var item in dto.Criteria!)
                    {
                        await _context.Criteria.AddAsync(new Criterion()
                        {
                            Name = item.Name,
                            Description = item.Description,
                            Percentage = item.Percentage,
                            ShowId = newShow.Id,
                        });
                    }
                    // 4. Add referee chấm show:
                    foreach (var item in dto.Referees!)
                    {
                        User? referee = await _context.Users.SingleOrDefaultAsync(u => u.Id == item.Id);
                        if (referee != null)
                        {
                            await _context.RefereeDetails.AddAsync(new RefereeDetail()
                            {
                                ShowId = newShow.Id,
                                UserId = referee.Id,
                            });
                        }
                    }
                    // 5. Save:
                    await _context.SaveChangesAsync();
                    result = true;
                }
            }
            return result;
        }

        public async Task<ShowDTO> GetById(int showId)
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var show = await _context.Shows
                .Include(s => s.Varieties)
                .Include(s => s.Criteria)
                .SingleOrDefaultAsync(s => s.Id == showId);
                if (show != null)
                {
                    return new ShowDTO()
                    {
                        Id = show.Id,
                        Description = show.Description,
                        EntranceFee = show.EntranceFee,
                        RegisterEndDate = show.RegisterEndDate,
                        RegisterStartDate = show.RegisterStartDate,
                        Status = show.Status,
                        Title = show.Title,
                        Criteria = show.Criteria.Select(cr => new CriterionDTO()
                        {
                            Id = cr.Id,
                            Name = cr.Name,
                            Description = cr.Description,
                            Percentage = cr.Percentage,
                        }),
                        Varieties = show.Varieties.Select(v => new VarietyDTO()
                        {
                            Id = v.Id,
                            Name = v.Name,
                        }),
                    };
                }
                else
                {
                    return null!;
                }
            }
        }

        public async Task<IEnumerable<ShowDTO>> GetAll()
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var result = await _context.Shows
                .Include(s => s.Varieties)
                .Include(s => s.Criteria)
                .Include(s => s.RefereeDetails)
                    .ThenInclude(rd => rd.User)
                        .ThenInclude(u => u.Role)
                .Select(show => new ShowDTO()
                {
                    Id = show.Id,
                    Description = show.Description,
                    EntranceFee = show.EntranceFee,
                    RegisterEndDate = show.RegisterEndDate,
                    RegisterStartDate = show.RegisterStartDate,
                    Status = show.Status,
                    Title = show.Title,
                    Criteria = show.Criteria.Select(cr => new CriterionDTO()
                    {
                        Id = cr.Id,
                        Name = cr.Name,
                        Description = cr.Description,
                        Percentage = cr.Percentage,
                    }),
                    Varieties = show.Varieties.Select(v => new VarietyDTO()
                    {
                        Id = v.Id,
                        Name = v.Name,
                    }),
                    Referees = show.RefereeDetails.Select(rd => new UserDTO()
                    {
                        Id = rd.User.Id,
                        Name = rd.User.Name,
                        Email = rd.User.Email,
                        Phone = rd.User.Phone,
                        Role = rd.User.Role.Title,
                        RoleId = rd.UserId,
                        Status = rd.User.Status
                    })
                }).ToListAsync();
                return result;
            }
        }

        public async Task<bool> Update(ShowDTO dto)
        {
            bool result = false;
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var show = await _context.Shows
                    .Include(s => s.Varieties)
                    .Include(s => s.Criteria)
                    .Include(s => s.RefereeDetails)
                        .ThenInclude(rd => rd.User)
                    .SingleOrDefaultAsync(s => s.Id == dto.Id);
                if(show != null)
                {
                    if (dto.Status != null)
                    {
                        show.Status = dto.Status;
                    }
                    if (dto.Title != null)
                    {
                        show.Title = dto.Title;
                    }
                    if (dto.Description != null)
                    {
                        show.Description = dto.Description;
                    }
                    if (dto.EntranceFee != null)
                    {
                        show.EntranceFee = (decimal)dto.EntranceFee;
                    }
                    if (dto.RegisterStartDate != null)
                    {
                        show.RegisterStartDate = (DateOnly)dto.RegisterStartDate;
                    }
                    if (dto.RegisterEndDate != null)
                    {
                        show.RegisterEndDate = (DateOnly)dto.RegisterEndDate;
                    }
                    // 1. Update Referee:
                    if(dto.Referees != null)
                    {
                        // Xóa những record RefereeDetail cũ của show:
                        foreach (var refereeDetail in _context.RefereeDetails)
                        {
                            if (refereeDetail.ShowId == show.Id)
                            {
                                _context.RefereeDetails.Remove(refereeDetail);
                            }
                        }
                        show.RefereeDetails = null!;
                        // Thêm Referee mới:
                        foreach (var item in dto.Referees)
                        {
                            User? referee = await _context.Users.SingleOrDefaultAsync(u => u.Id == item.Id);
                            if (referee != null)
                            {
                                await _context.RefereeDetails.AddAsync(new RefereeDetail()
                                {
                                    ShowId = dto.Id,
                                    UserId = referee.Id,
                                });
                            }
                        }
                    }
                    // 2. Update Criteria
                    if(dto.Criteria != null)
                    {
                        // Xóa những record Criteria cũ của show:
                        foreach (var criterion in _context.Criteria)
                        {
                            if (criterion.ShowId == show.Id)
                            {
                                _context.Criteria.Remove(criterion);
                            }
                        }
                        show.Criteria = null!;
                        // Thêm criteria mới:
                        foreach (var item in dto.Criteria)
                        {
                            await _context.Criteria.AddAsync(new Criterion()
                            {
                                Name = item.Name,
                                Description = item.Description,
                                Percentage = item.Percentage,
                                ShowId = dto.Id,
                            });
                        }
                    }
                    // 3. Update varieties:
                    if(dto.Varieties != null)
                    {
                        // Xóa varieties cũ:
                        foreach (var varriety in show.Varieties)
                        {
                            varriety.Shows.Remove(show);
                        }
                        // Thêm varieties:
                        foreach (var item in dto.Varieties)
                        {
                            var variety = await _context.Varieties.FirstOrDefaultAsync(v => v.Id == item.Id);
                            if (variety != null)
                            {
                                show.Varieties.Add(variety);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    result = true;
                }
            }
            return result;
        }

        public async Task<bool> Delete(int showId)
        {
            bool result = false;
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var show = await _context.Shows
                .Include(s => s.Varieties)
                .Include(s => s.Criteria)
                .SingleOrDefaultAsync(s => s.Id == showId);
                if (show != null)
                {
                    // TH1: Khi trạng thái là Up Comming, hoặc chưa có record nào liên quan tới:
                    // 1. Xóa record trong (ShowDetail) bảng trung gian:
                    if (show.Status.Equals("UpComing", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        foreach (var varriety in show.Varieties)
                        {
                            varriety.Shows.Remove(show);
                        }
                        // 2. Xóa Criteria liên quan:
                        foreach (var criterion in _context.Criteria)
                        {
                            if (criterion.ShowId == show.Id)
                            {
                                _context.Criteria.Remove(criterion);
                            }
                        }
                        // 3. Xóa Referee Detail liên quan:
                        foreach (var refereeDetail in _context.RefereeDetails)
                        {
                            if (refereeDetail.ShowId == show.Id)
                            {
                                _context.RefereeDetails.Remove(refereeDetail);
                            }
                        }
                        // 4. Xóa Show:
                        _context.Shows.Remove(show);
                        await _context.SaveChangesAsync();
                        result = true;
                    }
                    // TH2: KHi trạng thái là OnGoing, Scoring, Finished và có các record liên quan tới:
                    // => CHuyển trạng thái thành Deleted.
                    else if (show.Status.Equals("Deleted", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        return false;
                    }
                    else
                    {
                        show.Status = "Deleted";
                        await _context.SaveChangesAsync();
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}
