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

        public async Task<IEnumerable<ShowDTO>> GetAll()
        {
            var result = await _repository.Show.GetAll();
            return result;
        }

        public async Task<ShowDTO> GetById(int showId)
        {
            var result = await _repository.Show.GetById(showId);
            return result;
        }

        public async Task<bool> Update(ShowDTO dto)
        {
            return false;
        }
    }
}
