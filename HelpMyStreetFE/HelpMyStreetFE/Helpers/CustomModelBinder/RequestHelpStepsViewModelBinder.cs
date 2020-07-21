﻿using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.RequestHelp.Stages.Review;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Helpers.CustomModelBinder
{ 
    public class RequestHelpStepsViewModelBinder : IModelBinder
    {
        private readonly IModelMetadataProvider _provider;
        public RequestHelpStepsViewModelBinder(IModelMetadataProvider provider)
        {
            _provider = provider;
        }
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            var stepTypeValue = bindingContext.ValueProvider.GetValue("StepType");
            var stepType = Type.GetType(stepTypeValue.ToString());
            bindingContext.ModelMetadata = _provider.GetMetadataForType(stepType);
            switch (stepType.Name)
            {
                case nameof(RequestHelpRequestStageViewModel):
                    bindingContext.Result = ModelBindingResult.Success(BindRequestStage(bindingContext));
                    break;
                case nameof(RequestHelpDetailStageViewModel):
                    bindingContext.Result = ModelBindingResult.Success(BuildDetailStage(bindingContext));
                    break;
                case nameof(RequestHelpReviewStageViewModel):
                    bindingContext.Result = ModelBindingResult.Success(BuildReviewStage(bindingContext));
                    break;
            }

            return Task.CompletedTask;
        }

        private RequestHelpReviewStageViewModel BuildReviewStage(ModelBindingContext bindingContext)
        {
            RequestHelpReviewStageViewModel model = JsonConvert.DeserializeObject<RequestHelpReviewStageViewModel>(bindingContext.ValueProvider.GetValue("ReviewStep").FirstValue);
            return model;
        }

        private RequestHelpDetailStageViewModel BuildDetailStage(ModelBindingContext bindingContext)
        {
            RequestHelpDetailStageViewModel model = JsonConvert.DeserializeObject<RequestHelpDetailStageViewModel>(bindingContext.ValueProvider.GetValue("DetailStep").FirstValue);
            var recpientnamePrefix = "currentStep.Recipient.";
            model.Recipient = new RecipientDetails
            {
                Firstname = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.Firstname)).FirstValue,
                Lastname = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.Lastname)).FirstValue ,
                MobileNumber = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.MobileNumber)).FirstValue,
                AlternatePhoneNumber = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.AlternatePhoneNumber)).FirstValue,
                AddressLine1 = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.AddressLine1)).FirstValue ,
                AddressLine2 = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.AddressLine2)).FirstValue ,
                County = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.County)).FirstValue ,
                Postcode = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.Postcode)).FirstValue ,
                Email = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.Email)).FirstValue,
                Town = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.Town)).FirstValue,
            };


            var requestorPrefix = "currentStep.Requestor.";
            model.Requestor = new RequestorDetails
            {
                Firstname = bindingContext.ValueProvider.GetValue(requestorPrefix + nameof(model.Recipient.Firstname)).FirstValue ,
                Lastname = bindingContext.ValueProvider.GetValue(requestorPrefix + nameof(model.Recipient.Lastname)).FirstValue ,
                MobileNumber = bindingContext.ValueProvider.GetValue(requestorPrefix + nameof(model.Recipient.MobileNumber)).FirstValue,
                AlternatePhoneNumber = bindingContext.ValueProvider.GetValue(requestorPrefix + nameof(model.Recipient.AlternatePhoneNumber)).FirstValue,         
                Postcode = bindingContext.ValueProvider.GetValue(requestorPrefix + nameof(model.Recipient.Postcode)).FirstValue,
                Email = bindingContext.ValueProvider.GetValue(requestorPrefix + nameof(model.Recipient.Email)).FirstValue,                     
            };

            model.Organisation = bindingContext.ValueProvider.GetValue("currentStep.Organisation").FirstValue;
            model.CommunicationNeeds = bindingContext.ValueProvider.GetValue("currentStep.CommunicationNeeds").FirstValue;
            model.OtherDetails = bindingContext.ValueProvider.GetValue("currentStep.OtherDetails").FirstValue;

            return model;
        }

        private RequestHelpRequestStageViewModel BindRequestStage(ModelBindingContext bindingContext)
        {
            RequestHelpRequestStageViewModel model = JsonConvert.DeserializeObject<RequestHelpRequestStageViewModel>(bindingContext.ValueProvider.GetValue("RequestStep").FirstValue);

            int selectedTaskId, selectedRequestorId, selectedTimeId = -1;

            int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedTask.Id").FirstValue, out selectedTaskId);
            int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedRequestor.Id").FirstValue, out selectedRequestorId);
            int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedTimeFrame.Id").FirstValue, out selectedTimeId);

            model.Requestors.ForEach(x => x.IsSelected = false);
            var requestor = model.Requestors.Where(x => x.ID == selectedRequestorId).FirstOrDefault();
            if (requestor != null)
            {
                requestor.IsSelected = true;
            }

            model.Tasks.ForEach(x => x.IsSelected = false);
            var task = model.Tasks.Where(x => x.ID == selectedTaskId).FirstOrDefault();
            if (task != null)
            {
                task.IsSelected = true;
            }

            model.Timeframes.ForEach(x => x.IsSelected = false);
            var time = model.Timeframes.Where(x => x.ID == selectedTimeId).FirstOrDefault();
            if (time != null)
            {
                time.IsSelected = true;
                if (time.AllowCustom)
                {
                    int selectedDays = -1;
                    int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedTimeFrame.CustomDays").FirstValue, out selectedDays);
                    time.Days = selectedDays;
                }
            }

            int questionCount = -1;
            int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedTask.QuestionCount").FirstValue, out questionCount);

            for (int i = 0; i < questionCount; i++)
            {
                int questionID = -1;
                int.TryParse(bindingContext.ValueProvider.GetValue($"currentStep.SelectedTask.Questions.[{i}].Id").FirstValue, out questionID);
                var question = task.Questions.Where(x => x.ID == questionID).FirstOrDefault();
                if (question != null)
                {
                    if (question.InputType == QuestionType.LabelOnly)
                    {
                        question.Model = "";
                    }
                    else
                    {
                        question.Model = bindingContext.ValueProvider.GetValue($"currentStep.SelectedTask.Questions.[{i}].Model").FirstValue;
                    }
                }
            }

            bool AgreeToTerms, AgreeToPrivacy = false;            
            bool.TryParse(bindingContext.ValueProvider.GetValue("currentStep." + nameof(model.AgreeToPrivacy)).FirstValue, out AgreeToPrivacy);
            bool.TryParse(bindingContext.ValueProvider.GetValue("currentStep." + nameof(model.AgreeToTerms)).FirstValue, out AgreeToTerms);
            model.AgreeToPrivacy = AgreeToPrivacy;
            model.AgreeToTerms = AgreeToTerms;

            return model;

        }
    }

}
