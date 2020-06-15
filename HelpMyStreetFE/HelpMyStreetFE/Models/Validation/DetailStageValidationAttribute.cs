﻿using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    sealed public class DetailStageValidationAttribute : ValidationAttribute
    {
        public DetailStageValidationAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            List<string> errors = new List<string>();
            if (value is RequestHelpDetailStageViewModel)
            {
                var vm = (RequestHelpDetailStageViewModel)value;
                    
                switch (vm.Type)
                {
   
                    case HelpMyStreet.Utils.Enums.RequestorType.Myself:
                        ValidateRecipient(vm, errors, true);

                       break;
                    case HelpMyStreet.Utils.Enums.RequestorType.Organisation:
                    case HelpMyStreet.Utils.Enums.RequestorType.OnBehalf:
                        ValidateRecipient(vm, errors, false);
                        ValidateRequestor(vm, errors);
                        break;                                                                                            
                }                    
            }
           
            if(errors.Count > 0)
            {
                ErrorMessage = string.Join(",", errors);
                return false;
            }
            return true;
        }


        private void ValidateRecipient(RequestHelpDetailStageViewModel vm, List<string> errors, bool onlyRecipient)
        {
            if (vm.Recipient == null) errors.Add($"Recipient cannot be null");
            else
            {
                if (string.IsNullOrEmpty(vm.Recipient.Firstname)) errors.Add("Recipients firstname must be supplied");
                if (string.IsNullOrEmpty(vm.Recipient.Lastname)) errors.Add("Recipients lastname must be supplied");
                if (string.IsNullOrEmpty(vm.Recipient.MobileNumber) && string.IsNullOrEmpty(vm.Requestor.AlternatePhoneNumber) && onlyRecipient) errors.Add("Recipients contact number must be supplied");
                if (string.IsNullOrEmpty(vm.Recipient.Email) && onlyRecipient) errors.Add("Recipients email must be supplied");
                if (string.IsNullOrEmpty(vm.Recipient.AddressLine1)) errors.Add("Recipients Address Line 1 must be supplied");
                if (string.IsNullOrEmpty(vm.Recipient.Town)) errors.Add("Recipients Town must be supplied");                
                if (string.IsNullOrEmpty(vm.Recipient.Postcode)) errors.Add("Recipients postcode must be supplied");
            }

        }
        private void ValidateRequestor(RequestHelpDetailStageViewModel vm, List<string> errors)
        {
            if (vm.Requestor == null) errors.Add($"Requestor cannot be null");
            else
            {
                if (string.IsNullOrEmpty(vm.Requestor.Firstname)) errors.Add("Requestors firstname must be supplied");
                if (string.IsNullOrEmpty(vm.Requestor.Lastname)) errors.Add("Requestors lastname must be supplied");
                if (string.IsNullOrEmpty(vm.Requestor.MobileNumber) && string.IsNullOrEmpty(vm.Requestor.AlternatePhoneNumber)) errors.Add("Requestors contact number must be supplied");
                if (string.IsNullOrEmpty(vm.Requestor.Email)) errors.Add("Requestors email must be supplied");
                if (string.IsNullOrEmpty(vm.Requestor.Postcode)) errors.Add("Requestors postcode must be supplied");
            }

        }
    }
}