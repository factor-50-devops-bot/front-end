﻿using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class FilterSet
    {
        public IEnumerable<FilterField<SupportActivities>> SupportActivities { get; set; }
        public IEnumerable<FilterField<JobStatuses>> JobStatuses { get; set; }
        public IEnumerable<FilterField<int>> MaxDistanceInMiles { get; set; }
        public IEnumerable<FilterField<int>> DueInNextXDays { get; set; }
    }
}
