using DataAccessLayer.Implementation;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class Repository
    {
        // SINGLETON:
        private static readonly Repository _instance;
        private Repository() { }
        static Repository()
        {
            _instance = new Repository();
        }
        public static Repository Instance => _instance;

        // FIELDS:-----------------------------------------
        private IShowRepository _show = null!;
        private IUserRepository _user = null!;
        private IKoiRepository _koi = null!;
        private IVarietyRepository _variety = null!;
        private ICriterionRepository _criterion = null!;
        private IRegistrationRepository _registration = null!;
        private IScoreRepository _score = null!;

        // PROPERTIES:-------------------------------------
        public IShowRepository Show
        {
            get
            {
                if (this._show == null)
                {
                    this._show = ShowDAO.Instance;
                }
                return this._show;
            }
        }
        public IUserRepository User
        {
            get
            {
                if (this._user == null)
                {
                    this._user = UserDAO.Instance;
                }
                return this._user;
            }
        }

        public IKoiRepository Koi
        {
            get
            {
                if (this._koi == null)
                {
                    this._koi = KoiDAO.Instance;
                }
                return this._koi;
            }
        }

        public IVarietyRepository Variety
        {
            get
            {
                if (this._variety == null)
                {
                    this._variety = VarietyDAO.Instance;
                }
                return this._variety;
            }
        }

        public ICriterionRepository Criterion
        {
            get
            {
                if (this._criterion == null)
                {
                    this._criterion = CriterionDAO.Instance;
                }
                return this._criterion;
            }
        }

        public IRegistrationRepository Registration
        {
            get
            {
                if (this._registration == null)
                {
                    this._registration = RegistrationDAO.Instance;
                }
                return this._registration;
            }
        }

        public IScoreRepository Score
        {
            get
            {
                if (this._score == null)
                {
                    this._score = ScoreDAO.Instance;
                }
                return this._score;
            }
        }
    }
}
