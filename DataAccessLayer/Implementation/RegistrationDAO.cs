﻿using DataAccessLayer.Interface;
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
    public class RegistrationDAO : IRegistrationRepository
    {
        private static readonly RegistrationDAO _instance;
        private RegistrationDAO() { }
        static RegistrationDAO()
        {
            _instance = new RegistrationDAO();
        }
        public static RegistrationDAO Instance => _instance;

        public async Task<bool> Add(RegistrationDTO dto)
        {
            bool result = false;
            if (dto != null)
            {
                using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
                {
                    var koi = await _context.Kois.FirstOrDefaultAsync(k => k.Id == dto.KoiId);
                    if (koi != null)
                    {
                        _context.Registrations.Add(new Registration()
                        {
                            CreateDate = DateOnly.FromDateTime(DateTime.Now),
                            KoiId = (int)dto.KoiId!,
                            Description = dto.Description!,
                            Size = (int)dto.Size!,
                            ShowId = dto.ShowId,
                            Image01 = dto.Image01!,
                            Image02 = dto.Image02!,
                            Image03 = dto.Image03!,
                        });
                        await _context.SaveChangesAsync();
                        result = true;
                    }
                }
            }
            return result;
        }

        public Task<bool> Delete(int registrationId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RegistrationDTO>> GetAll()
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var result = await _context.Registrations
                    .Include(r => r.Koi)
                        .ThenInclude(k => k.Variety)
                    .Include(r => r.Show)
                .Select(registration => new RegistrationDTO()
                {
                    CreateDate = registration.CreateDate,
                    KoiId = registration.KoiId,
                    Description = registration.Description,
                    Size = registration.Size,
                    ShowId = registration.ShowId,
                    ShowName = registration.Show.Title,
                    Image01 = registration.Image01,
                    Image02 = registration.Image01,
                    Image03 = registration.Image01,
                    IsBestVote = registration.IsBestVote,
                    Note = registration.Note,
                    Rank = registration.Rank,
                    Status = registration.Status,
                    TotalScore = registration.TotalScore,
                    KoiName = registration.Koi.Name,
                    Id = registration.Id,
                    KoiVariety = registration.Koi.Variety.Name,
                }).ToListAsync();
                return result;
            }
        }

        public Task<RegistrationDTO> GetById(int registrationId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(RegistrationDTO dto)
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                bool result = false;
                var registration = _context.Registrations.FirstOrDefault(r => r.Id == dto.Id);
                if(registration != null)
                {
                    if (dto.Note != null)
                    {
                        registration.Note = dto.Note;
                    }
                    if(dto.KoiId != null)
                    {
                        registration.KoiId = (int)dto.KoiId;
                    }
                    if(dto.Size != null)
                    {
                        registration.Size = (int)dto.Size;
                    }
                    if (dto.Description != null)
                    {
                        registration.Description = dto.Description;
                    }
                    if (dto.Status != null)
                    {
                        registration.Status = dto.Status;
                    }
                    if (dto.Image01 != null)
                    {
                        registration.Image01 = dto.Image01;
                    }
                    if (dto.Image02 != null)
                    {
                        registration.Image01 = dto.Image02;
                    }
                    if (dto.Image03 != null)
                    {
                        registration.Image01 = dto.Image03;
                    }
                    await _context.SaveChangesAsync();
                    result = true;
                }
                return result;
            }
        }

        public async Task CalculateTotalScoreAllForRegistation(int showId)
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var registrations = await _context.Registrations
                    .Where(r => r.ShowId == showId && r.Status.Equals("Accepted") && r.TotalScore == null)
                    .Include(r => r.Scores)
                        .ThenInclude(score => score.Criteria)
                        .ToListAsync();
                foreach(var registration in registrations)
                {
                    decimal totalScore = _context.Scores
                        .Where(s => s.RegistrationId == registration.Id)
                        .GroupBy(s => s.CriteriaId)
                        .Select(group => new // IGroup: bao gồm Key ( tiêu chí gom ) và IEnumerable<kiểu dữ liệu của các records>
                        {
                            CriteriaId = group.Key,
                            WeightedAverage = group.Average(s => s.Score1) * group.First().Criteria.Percentage / 100,
                        })
                        .Sum(x => x.WeightedAverage);
                    registration.TotalScore = totalScore;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task Ranking(int showId)
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var registrations = await _context.Registrations
                    .Where(r => r.ShowId == showId && r.Status.Equals("Accepted") && r.TotalScore != null)
                    .Include(r => r.Scores)
                        .ThenInclude(score => score.Criteria)
                    .Select(r => new
                    {
                        Registration = r,
                        ScoreList = _context.Scores
                            .Where(s => s.RegistrationId == r.Id)
                            .GroupBy(s => s.CriteriaId)
                            .Select(group => new // IGroup: bao gồm Key ( tiêu chí gom ) và IEnumerable<kiểu dữ liệu của các records>.
                            {
                                CriteriaId = group.Key,
                                TotalScoreByCriteria = group.Average(s => s.Score1) * group.First().Criteria.Percentage / 100,
                                Percentage = group.First().Criteria.Percentage,
                            })
                            .OrderByDescending(x => x.Percentage).ToList(), // Chuyển về dạng list để xác định index,
                                                                            // sort tiêu chí trọng số lớn lên ưu tiên
                    })
                    .OrderByDescending(x => x.ScoreList.Sum(sl => sl.TotalScoreByCriteria))
                    .ThenByDescending(x => x.ScoreList[1])
                    .ThenByDescending(x => x.ScoreList[1])
                    .ThenByDescending(x => x.ScoreList[1])
                    .ToListAsync();
            }
        }
    }
}
