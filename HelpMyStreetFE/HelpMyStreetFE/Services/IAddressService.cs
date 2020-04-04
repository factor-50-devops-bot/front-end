﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IAddressService
    {
        Task<PostCodeResponse> CheckPostCode(string postCode);
        Task<int> GetPostCodesCovered();
        Task<int> GetStreetChampions();
        Task<int> GetStreetsCovered();
        Task<int> GetStreetsRemaining();
    }
}