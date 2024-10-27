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
        private IVarietyRepository _variety = null!;
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
    }
}
