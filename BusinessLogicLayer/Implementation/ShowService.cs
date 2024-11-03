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
            var show = await _repository.Show.GetById(showId);
            if(show != null)
            {
                if (show.Status.ToLower().Equals("Upcoming"))
                {
                    bool result = await _repository.Show.Delete(showId);
                    return result;
                }
                else
                {
                    throw new Exception("This show can not be deleted !");
                }
            }
            else
            {
                throw new Exception("Show does not exist !");
            }
        }

        public async Task<IEnumerable<ShowDTO>> Search(int userId, string key)
        {
            //
            var user = await _repository.User.GetById(userId);
            if (user != null)
            {
                if (user.Role!.Equals("Member", StringComparison.OrdinalIgnoreCase) == true)
                {

                    var shows = (await _repository.Show.GetAll())
                                    .Where(s => !s.Status.Equals("UpComing", StringComparison.OrdinalIgnoreCase) == true);
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
                else if (user.Role!.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
                {
                    var shows = (await _repository.Show.GetAll());
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
                else
                {
                    throw new Exception("You do not have permission to use this behavior !");
                }
            }
            else
            {
                throw new Exception("User does not exist !");
            }
            //
            
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

        public async Task<IEnumerable<ShowDTO>> GetAll(int userId)
        {
            var user = await _repository.User.GetById(userId);
            if (user != null)
            {
                var result = await _repository.Show.GetAll();
                if (user.Role!.Equals("Member", StringComparison.OrdinalIgnoreCase) == true
                    || user.Role!.Equals("Referee", StringComparison.OrdinalIgnoreCase) == true)
                {
                    result = result.Where(s => !s.Status.Equals("UpComing", StringComparison.OrdinalIgnoreCase) == true)
                        .OrderByDescending(s => s.RegisterStartDate).ToList();
                }
                else if(user.Role!.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
                {
                    result = result.OrderByDescending(s => s.RegisterStartDate).ToList();
                }
                return result;
            }
            else
            {
                throw new Exception("User does not exist !");
            }
        }
    }
}
