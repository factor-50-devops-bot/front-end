﻿using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;

namespace HelpMyStreetFE.Services
{
    public interface IUserService
    {
        Task<int> CreateUser(string email, string authId);
        Task<int> UpdateUser(User user);
    }
}