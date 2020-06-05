﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Email
{
    public class RequestSettings
    {
        public int OpenRequestsRadius { get; set; }
        public int RequestsSessionExpiryInMinutes { get; set; }
    }
}
