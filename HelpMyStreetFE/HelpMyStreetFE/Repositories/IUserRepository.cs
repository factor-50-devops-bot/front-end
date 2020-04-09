﻿using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;

namespace HelpMyStreetFE.Repositories
{
    public interface IUserRepository
    {
        Task<int> CreateUser(string email, string authId);
        Task<int> CreateUserStepFour(RegistrationStepFour data);
        Task<int> CreateUserStepThree(RegistrationStepThree data);
        Task<int> CreateUserStepTwo(RegistrationStepTwo data);
        Task<int> GetChampionCountByPostcode(string postcode);
        Task<User> GetUser(int id);
        Task<User> GetUserByAuthId(string authId);
        Task<int> UpdateUser(User user);
    }
}