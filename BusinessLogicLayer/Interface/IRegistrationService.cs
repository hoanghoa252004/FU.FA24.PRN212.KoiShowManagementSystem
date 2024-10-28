﻿using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interface
{
    public interface IRegistrationService
    {
        Task<IEnumerable<RegistrationDTO>> GetAllRegistrationForReferee(int userId);
    }
}
