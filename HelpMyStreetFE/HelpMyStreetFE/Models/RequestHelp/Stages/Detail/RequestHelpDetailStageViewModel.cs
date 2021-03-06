﻿using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.Enum;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.Validation;
using System.Collections.Generic;


namespace HelpMyStreetFE.Models.RequestHelp.Stages.Detail
{
    [DetailStageValidation]
    public class RequestHelpDetailStageViewModel : IRequestHelpStageViewModel
    {
        public string Organisation { get; set; }
        public RequestorDetails Requestor { get; set; }
        public RecipientDetails Recipient { get; set; } 
        public string TemplateName { get; set; } = "RequestHelpDetailStageViewModel";
        public RequestorType Type { get; set; }
        public QuestionsViewModel Questions { get; set; }
        public bool FullRecipientAddressRequired { get; set; }
    }    
}

