using BusinessLogicLayer.Interface;
using DataAccessLayer;
using DataAccessLayer.Implementation;
using DTOs;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Implementation
{
    public class ShowService : IShowService
    {
        private static readonly ShowService _instance;
        private readonly Repository _repository;
        private ShowService()
        {
            _repository = Repository.Instance;
        }
        static ShowService()
        {
            _instance = new ShowService();
        }
        public static ShowService Instance => _instance;
        public async Task<bool> Add(ShowDTO dto)
        {
            bool result = await _repository.Show.Add(dto);
            return result;
        }

        public async Task<bool> Delete(int showId)
        {
            bool result = await _repository.Show.Delete(showId);
            return result;
        }

        public async Task<IEnumerable<ShowDTO>> Search(string key)
        {
            var shows = await _repository.Show.GetAll();
            if (shows.Any() == true)
            {
                var result = shows.Where(s =>
                                s.Title.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                || s.Description.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                || s.RegisterStartDate.ToString().Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                || s.EntranceFee.ToString().Contains(key, StringComparison.OrdinalIgnoreCase) == true);
                return result;
            }
            else
                return null!;
        }

        public async Task<bool> Update(ShowDTO dto)
        {
            return false;
        }

        public async Task<IEnumerable<RegistrationDTO>> ReviewScore(int showId)
        {
            await _repository.Registration.CalculateTotalScoreAllForRegistation(showId);
            return null!;
        }
    }
}
