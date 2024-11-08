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
                if (show.Status.Equals("Upcoming", StringComparison.OrdinalIgnoreCase) == true)
                {
                    bool result = await _repository.Show.Delete(showId);
                    return result;
                }
                else
                {
                    throw new Exception("This show can not be deleted  as it is "+ show.Status +"!");
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
                                        || s.RegisterStartDate.ToString()!.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                        || s.EntranceFee.ToString()!.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                        || s.Status.Contains(key, StringComparison.OrdinalIgnoreCase) == true);
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
                                        || s.RegisterStartDate.ToString()!.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                        || s.EntranceFee.ToString()!.Contains(key, StringComparison.OrdinalIgnoreCase) == true
                                        || s.Status.Contains(key, StringComparison.OrdinalIgnoreCase) == true);
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
            bool result = false;
            if(dto == null )
            {
                throw new Exception("Show does not exist !");
            }
            else
            {
                if (dto.Title == null
                    && dto.Description == null
                    && dto.EntranceFee == null
                    && dto.RegisterStartDate == null
                    && dto.RegisterEndDate == null
                    && dto.Criteria == null
                    && dto.Referees == null
                    && dto.Varieties == null
                    && dto.Status == null)
                {
                    throw new Exception("Nothing to update !");
                }
                else
                {
                    if(dto.Status.Equals("OnGoing", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        bool checks = (await _repository.Show.GetAll()).Any(s => s.Status.Equals("OnGoing", StringComparison.OrdinalIgnoreCase) == true
                                                                                || s.Status.Equals("Scoring", StringComparison.OrdinalIgnoreCase) == true);
                        if(checks == true)
                        {
                            throw new Exception("Can not publish this show as there is an ongoing show at this time !");
                        }
                        else
                        {
                            result = await _repository.Show.Update(dto);
                        }
                    }
                    else
                    {
                        result = await _repository.Show.Update(dto);
                    }
                }
            }
            return result;
        }

        public async Task<IEnumerable<RegistrationDTO>> ReviewScore(int showId)
        {
            bool isScoreCompletely = await _repository.Score.IsScoreCompletely(showId);
            if(isScoreCompletely == true)
            {
                // Kiểm tra thử xem show chấm điểm chưa:
                IEnumerable<RegistrationDTO> registrationsInShow = await _repository.Registration.GetRegistrationsByShow(showId);
                if(registrationsInShow != null && registrationsInShow.Any() == true)
                {
                    RegistrationDTO registration = registrationsInShow.FirstOrDefault()!;
                    if (registration.Rank == null && registration.TotalScore == null)
                    {
                        await _repository.Registration.CalculateTotalScoreAllForRegistation(showId);
                        await _repository.Registration.Ranking(showId);
                        registrationsInShow = await _repository.Registration.GetRegistrationsByShow(showId);
                    }
                    return registrationsInShow.OrderBy(r => r.Rank);
                }
                else
                {
                    throw new Exception("The show has not contained any registration yet");
                }
            }
            else
            {
                throw new Exception("The registrations's show has not completely scored yet");
            }
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

        public async Task<bool> AnnouceResult(int showId)
        {
            bool updateShow = await _repository.Show.Update(new ShowDTO()
            {
                Id = showId,
                Status = "Finished",
            });
            bool updateRegistration  = await _repository.Registration.UpdateStatusAllRegistrationByShow(showId, "Scored");
            if (updateShow == true && updateRegistration == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<RegistrationDTO>> GetAllKoiParticipants(int showId)
        {
            var result = (await _repository.Registration.GetRegistrationsByShow(showId))
                                    .Where(r => r.Status!.Equals("Accepted", StringComparison.OrdinalIgnoreCase) == true
                                            || r.Status!.Equals("Scored", StringComparison.OrdinalIgnoreCase) == true);
            return result;
        }
    }
}
