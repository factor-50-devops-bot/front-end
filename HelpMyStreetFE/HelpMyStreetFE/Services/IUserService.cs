﻿using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;

namespace HelpMyStreetFE.Services
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(string email, string authId);
        Task<int> UpdateUserAsync(User user);
        Task<User> GetUserAsync(int id);
        Task CreateUserStepTwoAsync(int id, string postCode, string firstName, string lastName, string addressLine1, string addressLine2, string addressLine3, string locality, string mobile, string otherPhone, System.DateTime dob);
        Task CreateUserStepThreeAsync(int id, System.Collections.Generic.List<HelpMyStreet.Utils.Enums.SupportActivities> activities, float supportRadius, bool supportContact, bool medical);
        Task CreateUserStepFourAsync(int id, bool roleUnderstood, System.Collections.Generic.List<string> postcodes);
    }
}