﻿using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.RequestHelp.Stages.Review;
using HelpMyStreetFE.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class RequestHelpBuilder : IRequestHelpBuilder
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        public RequestHelpBuilder(IRequestHelpRepository requestHelpRepository)
        {
            _requestHelpRepository = requestHelpRepository;
        }

        public async Task<RequestHelpViewModel> GetSteps(RequestHelpFormVariant requestHelpFormVariant, int referringGroupID, string source)
        {

            var model =  new RequestHelpViewModel
            {
                ReferringGroupID = referringGroupID,
                Source = source,
                RequestHelpFormVariant = requestHelpFormVariant,
                CurrentStepIndex = 0,
                Steps = new List<IRequestHelpStageViewModel>
                {
                    new RequestHelpRequestStageViewModel
                    {
                        PageHeading = GetHelpRequestPageTitle(requestHelpFormVariant),
                        IntoText = GetHelpRequestPageIntroText(requestHelpFormVariant),
                        Tasks = await GetRequestHelpTasks(requestHelpFormVariant),
                        Requestors = new List<RequestorViewModel>
                        {
                            new RequestorViewModel
                            {
                                ID = 1,
                                ColourCode = "orange",
                                Title = "I am requesting help for myself",
                                Text = "I'm the person in need of help",
                                IconDark = "request-myself.svg",
                                IconLight = "request-myself-white.svg",
                                Type = RequestorType.Myself
                            },
                            new RequestorViewModel
                            {
                                ID = 2,
                                ColourCode = "dark-blue",
                                Title = "On behalf of someone else",
                                Text = "I'm looking for help for a relative, neighbour or friend",
                                IconDark = "request-behalf.svg",
                                IconLight = "request-behalf-white.svg",
                                Type = RequestorType.OnBehalf
                            },
                            new RequestorViewModel
                            {
                                ID = 3,
                                ColourCode = "dark-blue",
                                Title = "On behalf of an organisation",
                                Text = "I'm looking for help for an organisation",
                                IconDark = "request-organisation.svg",
                                IconLight = "request-organisation-white.svg",
                                Type = RequestorType.Organisation                                
                            }
                        },
                        Timeframes =  new List<RequestHelpTimeViewModel>
                        {
                            new RequestHelpTimeViewModel{ID = 1, TimeDescription = "Today", Days = 0},
                            new RequestHelpTimeViewModel{ID = 2, TimeDescription = "Within 24 Hours", Days = 1},
                            new RequestHelpTimeViewModel{ID = 3, TimeDescription = "Within a Week", Days = 7},
                            new RequestHelpTimeViewModel{ID = 4, TimeDescription = "When Convenient", Days = 30},
                            new RequestHelpTimeViewModel{ID = 5, TimeDescription = "Other", AllowCustom = true},
                        },
                    },
                    new RequestHelpDetailStageViewModel()
                    {
                        FullRecipientAddressRequired = GetFullRecipientAddressRequired(requestHelpFormVariant)
                    },
                    new RequestHelpReviewStageViewModel(),
                }

            };
            if (requestHelpFormVariant == RequestHelpFormVariant.FtLOS)
            {
                ((RequestHelpRequestStageViewModel)model.Steps.First()).Timeframes.RemoveRange(0, 2);
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.HLP_CommunityConnector)
            {
                ((RequestHelpRequestStageViewModel)model.Steps.First()).Timeframes.RemoveRange(0, 2);
                ((RequestHelpRequestStageViewModel)model.Steps.First()).Timeframes.RemoveRange(2, 1);

                ((RequestHelpRequestStageViewModel)model.Steps.First()).Requestors.RemoveAll(x => x.Type == RequestorType.Organisation);
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.DIY) {
                var requestStep = ((RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First());
                requestStep.Requestors.RemoveAll(x => x.Type ==  RequestorType.Myself);                
            }
            return model;
        }

        private string GetHelpRequestPageTitle(RequestHelpFormVariant requestHelpFormVariant)
        {
            return requestHelpFormVariant switch
            {
                RequestHelpFormVariant.FtLOS => "How can For the Love of Scrubs help?",
                RequestHelpFormVariant.HLP_CommunityConnector => "Get in touch with a Community Connector",
                RequestHelpFormVariant.Ruddington => "Request help from Ruddington Community Response Team",
                _ => "What type of help are you looking for?"
            };
        }

        private string GetHelpRequestPageIntroText(RequestHelpFormVariant requestHelpFormVariant)
        {
            return requestHelpFormVariant switch
            {
                RequestHelpFormVariant.FtLOS => "We have volunteers across the country donating their time and skills to help us beat coronavirus. If you need reusable fabric face coverings, we can help.",
                RequestHelpFormVariant.HLP_CommunityConnector => "If you’re feeling down, anxious or just ‘stuck’ and wanting someone to help you take action to improve your wellbeing, we can put you in touch with a trained volunteer Community Connector. Calls are free, confidential and focused on an issue that you want to make progress on.",
                _ => "People across the country are helping their neighbours and community to stay safe. Whatever you need, we have people who can help."
            };
        }

        private bool GetFullRecipientAddressRequired(RequestHelpFormVariant requestHelpFormVariant)
        {
            return requestHelpFormVariant switch
            {
                RequestHelpFormVariant.HLP_CommunityConnector => false,
                _ => true
            };
        }

        private async Task<List<TasksViewModel>> GetRequestHelpTasks(RequestHelpFormVariant requestHelpFormVariant)
        {
            var tasks = new List<TasksViewModel>();
            if (requestHelpFormVariant == RequestHelpFormVariant.VitalsForVeterans)
            {
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.WellbeingPackage });
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.FtLOS)
            {
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.FaceMask, IsSelected = true });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.HLP_CommunityConnector)
            {
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.CommunityConnector, IsSelected = true });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.Ruddington)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.FaceMask, IsSelected = (requestHelpFormVariant == RequestHelpFormVariant.FaceMasks) },
                    new TasksViewModel { SupportActivity = SupportActivities.CheckingIn },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.Errands },
                    new TasksViewModel { SupportActivity = SupportActivities.MealPreparation },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.DogWalking },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.FaceMask, IsSelected = (requestHelpFormVariant == RequestHelpFormVariant.FaceMasks) },
                    new TasksViewModel { SupportActivity = SupportActivities.CheckingIn },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.Errands },
                    new TasksViewModel { SupportActivity = SupportActivities.MealPreparation },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.HomeworkSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }

            return tasks;
        }

        public async Task<List<RequestHelpQuestion>> GetQuestionsForTask(RequestHelpFormVariant requestHelpFormVariant, RequestHelpFormStage requestHelpFormStage, SupportActivities supportActivity)
        {
            var questions = await _requestHelpRepository.GetQuestionsByActivity(new GetQuestionsByActivitiesRequest
            {
                ActivitesRequest = new ActivitesRequest
                {
                    Activities = new List<SupportActivities>() { supportActivity }
                },
                RequestHelpFormVariantRequest = new RequestHelpFormVariantRequest
                {
                    RequestHelpFormVariant = requestHelpFormVariant
                },
                RequestHelpFormStageRequest = new RequestHelpFormStageRequest
                {
                    RequestHelpFormStage = requestHelpFormStage
                }
            });

            List<RequestHelpQuestion> requestHelpQuestions = questions.SupportActivityQuestions[supportActivity].Select(x => new RequestHelpQuestion
            {
                ID = x.Id,
                InputType = x.Type,
                Label = x.Name,
                Required = x.Required,
                PlaceholderText = x.PlaceholderText,
                SubText = x.SubText,
                Location = x.Location,
                AdditionalData = x.AddtitonalData,
            }).ToList();


            return requestHelpQuestions;
        }

        public RequestPersonalDetails MapRecipient(RequestHelpDetailStageViewModel detailStage)
        {
            return new RequestPersonalDetails
            {
                FirstName = detailStage.Recipient.Firstname,
                LastName = detailStage.Recipient.Lastname,
                MobileNumber = detailStage.Recipient.MobileNumber,
                OtherNumber = detailStage.Recipient.AlternatePhoneNumber,
                EmailAddress = detailStage.Recipient.Email,
                Address = new Address
                {
                    AddressLine1 = detailStage.Recipient.AddressLine1,
                    AddressLine2 = detailStage.Recipient.AddressLine2,
                    Locality = detailStage.Recipient.Town,
                    Postcode = PostcodeFormatter.FormatPostcode(detailStage.Recipient.Postcode),
                }
            };
        }

        public RequestPersonalDetails MapRequestor(RequestHelpDetailStageViewModel detailStage)
        {
            return new RequestPersonalDetails
            {
                FirstName = detailStage.Requestor.Firstname,
                LastName = detailStage.Requestor.Lastname,
                MobileNumber = detailStage.Requestor.MobileNumber,
                OtherNumber = detailStage.Requestor.AlternatePhoneNumber,
                EmailAddress = detailStage.Requestor.Email,
                Address = new Address
                {
                    Postcode = PostcodeFormatter.FormatPostcode(detailStage.Requestor.Postcode),
                }
            };
        }
    }
}
