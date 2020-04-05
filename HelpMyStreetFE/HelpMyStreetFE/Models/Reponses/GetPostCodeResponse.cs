﻿using HelpMyStreet.Utils.Models;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Reponses
{
    public class GetPostCodeResponseContent
    {
        public List<Address> AddressDetails { get; set; }
    }

    public class GetPostCodeResponse
    {
        public bool HasContent { get; set; }
        public bool IsSuccessful { get; set; }
        public string PostCode { get; set; }
        public GetPostCodeResponseContent content { get; set; }
    }
}
