﻿using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Helpers.CustomModelBinder;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.RequestHelp.Stages.Review;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Controllers
{

    public class RequestHelpController : Controller
    {
        private readonly ILogger<RequestHelpController> _logger;
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;
        private readonly IRequestHelpBuilder _requestHelpBuilder;
        private readonly IAuthService _authService;
        public RequestHelpController(ILogger<RequestHelpController> logger, IRequestService requestService, IGroupService groupService, IRequestHelpBuilder requestHelpBuilder, IAuthService authService)
        {
            _logger = logger;
            _requestService = requestService;
            _groupService = groupService;
            _requestHelpBuilder = requestHelpBuilder;
            _authService = authService;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RequestHelp(
        [ModelBinder(BinderType = typeof(RequestHelpModelBinder))]RequestHelpViewModel requestHelp,
        [ModelBinder(BinderType = typeof(RequestHelpStepsViewModelBinder))] IRequestHelpStageViewModel step,
        CancellationToken cancellationToken)
        {
            try
            {
                requestHelp.Errors = new List<string>();
                requestHelp.Steps[requestHelp.CurrentStepIndex] = step;
                if (requestHelp.Action == "EditRequest")
                {
                    requestHelp.CurrentStepIndex = 0;
                    return View(requestHelp);
                }
                if (requestHelp.Action == "EditDetails")
                {
                    requestHelp.CurrentStepIndex = 1;
                    return View(requestHelp);
                }
                if (requestHelp.Action == "back")
                {
                    requestHelp.CurrentStepIndex--;
                    return View(requestHelp);
                }

                if (!ModelState.IsValid)
                    throw new ValidationException("Model Validation failed");


                if (requestHelp.Action == "next")
                {
                    requestHelp.CurrentStepIndex++;
                    if (step is RequestHelpRequestStageViewModel)
                    {

                        var requestStep = (RequestHelpRequestStageViewModel)step;
                        var detailStage = (RequestHelpDetailStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpDetailStageViewModel).First();

                        detailStage.Type = requestStep.Requestors.Where(x => x.IsSelected).First().Type;
                        detailStage.Questions = await UpdateQuestionsViewModel(detailStage.Questions, requestHelp.RequestHelpFormVariant, RequestHelpFormStage.Detail, (SupportActivities)requestHelp.SelectedSupportActivity());

                        var loggedInUser = await _authService.GetCurrentUser(HttpContext, cancellationToken);
                        if (loggedInUser != null)
                        {
                            switch (detailStage.Type)
                            {
                                case RequestorType.Myself:
                                    if (detailStage.Recipient == null)
                                    {
                                        detailStage.Recipient = new RecipientDetails
                                        {
                                            Firstname = loggedInUser.UserPersonalDetails.FirstName,
                                            Lastname = loggedInUser.UserPersonalDetails.LastName,
                                            AddressLine1 = loggedInUser.UserPersonalDetails.Address.AddressLine1,
                                            AddressLine2 = loggedInUser.UserPersonalDetails.Address.AddressLine2,
                                            AlternatePhoneNumber = loggedInUser.UserPersonalDetails.OtherPhone,
                                            MobileNumber = loggedInUser.UserPersonalDetails.MobilePhone,
                                            Email = loggedInUser.UserPersonalDetails.EmailAddress,
                                            Postcode = loggedInUser.UserPersonalDetails.Address.Postcode,
                                            Town = loggedInUser.UserPersonalDetails.Address.Locality
                                        };
                                    }
                                    break;
                                case RequestorType.Organisation:
                                case RequestorType.OnBehalf:
                                    if (detailStage.Requestor == null)
                                    {
                                        detailStage.Requestor = new RequestorDetails
                                        {
                                            Firstname = loggedInUser.UserPersonalDetails.FirstName,
                                            Lastname = loggedInUser.UserPersonalDetails.LastName,
                                            AlternatePhoneNumber = loggedInUser.UserPersonalDetails.OtherPhone,
                                            MobileNumber = loggedInUser.UserPersonalDetails.MobilePhone,
                                            Email = loggedInUser.UserPersonalDetails.EmailAddress,
                                            Postcode = loggedInUser.UserPersonalDetails.Address.Postcode,
                                        };
                                    }
                                    break;
                            }
                        }
                    }
                    if (step is RequestHelpDetailStageViewModel)
                    {
                        var requestStage = (RequestHelpRequestStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpRequestStageViewModel).First();
                        var detailStage = (RequestHelpDetailStageViewModel)step;
                        var reviewStage = (RequestHelpReviewStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpReviewStageViewModel).First();
                        reviewStage.Recipient = detailStage.Recipient;
                        reviewStage.Requestor = detailStage.Requestor;
                        reviewStage.Task = requestStage.Tasks.Where(x => x.IsSelected).FirstOrDefault();
                        reviewStage.OrganisationName = detailStage.Organisation;
                        reviewStage.TimeRequested = requestStage.Timeframes.Where(X => X.IsSelected).FirstOrDefault();
                        reviewStage.RequestedFor = requestStage.Requestors.Where(x => x.IsSelected).FirstOrDefault();
                        reviewStage.RequestStageQuestions = requestStage.Questions.Questions;
                        reviewStage.DetailsStageQuestions = detailStage.Questions.Questions;
                    }
                }
                if (requestHelp.Action == "finish")
                {
                    var requestStage = (RequestHelpRequestStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpRequestStageViewModel).First();
                    var detailStage = (RequestHelpDetailStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpDetailStageViewModel).First();
                    var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

                    // if they've come through as DIY and there not logged in, throw an error telling them they cant do that
                    if (requestHelp.RequestHelpFormVariant == RequestHelpFormVariant.DIY && user == null)
                    {
                        requestHelp.Errors.Add("To \"Submit & Accept\" a Request, you must be logged in, to submit a normal request, please click on the Request Help link above");
                        throw new ValidationException("User tired to submit DIY Request without being logged in");
                    }

                    var response = await _requestService.LogRequestAsync(requestStage, detailStage, requestHelp.ReferringGroupID, requestHelp.Source, user?.ID ?? 0, cancellationToken);
                    if (response != null)
                    {
                        return RedirectToRoute("request-help/success", new
                        {
                            fulfillable = response.Fulfillable,
                            requestHelpFormVariant = requestHelp.RequestHelpFormVariant,
                            referringGroup = Base64Utils.Base64Encode(requestHelp.ReferringGroupID),
                            source = requestHelp.Source
                        });
                    }
                }
            }
            catch (ValidationException vex)
            {
                _logger.LogError(vex, "a validation error occured in request help form action");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "an error occured in request help form action");
                requestHelp.Errors.Add("Oops! an error occured sumbitting your request, please try again later.");
            }


            return View(requestHelp);
        }


        public async Task<IActionResult> RequestHelp(string referringGroup, string source, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request-help");

            int referringGroupId = DecodeGroupIdOrGetDefault(referringGroup);
            source = ValidateSource(source);

            // Fix to allow existing routing
            if (referringGroup == "v4v")
            {
                referringGroupId = await _groupService.GetGroupIdByKey("ageuklsl", cancellationToken);
            }

            RequestHelpFormVariant requestHelpFormVariant = await _groupService.GetRequestHelpFormVariant(referringGroupId, source);

            if (requestHelpFormVariant == RequestHelpFormVariant.DIY && (!User.Identity.IsAuthenticated))
            {
                string encodedReferringGroupId = Base64Utils.Base64Encode(referringGroupId);
                return Redirect($"/login?ReturnUrl=request-help/{encodedReferringGroupId}/{source}");
            }

            var model = await _requestService.GetRequestHelpSteps(requestHelpFormVariant, referringGroupId, source);
            var requestStage = (RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First();

            SupportActivities? selectedTask = requestStage.Tasks.Where(t => t.IsSelected).FirstOrDefault()?.SupportActivity;
            if (selectedTask != null)
            {
                requestStage.Questions = await UpdateQuestionsViewModel(null, requestHelpFormVariant, RequestHelpFormStage.Request, selectedTask.Value);
            }

            return View(model);
        }

        public IActionResult Success(Fulfillable fulfillable, RequestHelpFormVariant requestHelpFormVariant, string referringGroup, string source)
        {
            source = ValidateSource(source);

            string message;
            string button;

            if (fulfillable == Fulfillable.Accepted_DiyRequest)
            {
                message = @"<p>Your request will now be available in the 'My Accepted Requests' area of your profile.</p>";
                button = "<a href='/account/accepted-requests' class='btn cta large fill mt16 cta--orange'>Done</a>";
            }
            else
            {
                message = requestHelpFormVariant switch
                {
                    RequestHelpFormVariant.FtLOS => @"<p>Your request has been received and we are looking for a volunteer who can help. Someone should get in touch shortly.</p>
                                                      <p>For the Love of Scrubs ask for a small donation of £3 - £4 per face covering to cover the cost of materials and help support their communities. Without donations they aren’t able to continue their good work.</p>
                                                      <p>If you are able to donate, you can do so on their Go Fund Me page <a href='https://www.gofundme.com/f/for-the-love-of-scrubs-face-coverings\' target=\'_blank\'>here</a>.<p>",
                    RequestHelpFormVariant.Ruddington => @"<p>Your request has been received and we're looking for a volunteer who can help, as soon as we find someone we’ll let you know by email. Please be aware that we cannot guarantee help, but we’ll do our best to find a volunteer near you.</p>",
                    _ => @"<p>Your request has been received and we are looking for a volunteer who can help. Someone should get in touch shortly.</p>"
                };

                if (User.Identity.IsAuthenticated)
                {
                    button = $"<a href='/account' class='btn cta large fill mt16 cta--orange'>Done</a>";
                }
                else
                {    
                    message += "<p><strong>Would you be happy to help a neighbour?</strong></p>";
                    message += "<p>Could you help a member of your local community if they needed something? There are lots of different ways you can help, from offering a friendly chat, to picking up groceries or prescriptions, or even sewing a face covering. Please take 5 minutes to sign-up now.</p>";
                    button = $"<a href='/registration/step-one/{referringGroup}/help-request-success' class='btn cta large fill mt16 '>Sign up</a>";
                }

            }

            List<NotificationModel> notifications = new List<NotificationModel> {
                new NotificationModel
                {
                    Title = "Thank you",
                    Subtitle = "Your request has been received",
                    Type = Enums.Account.NotificationType.Success,
                    Message = message,
                    Button = button
                }
            };

            SuccessViewModel vm = new SuccessViewModel
            {
                Notifications = notifications,
                RequestLink = $"/request-help/{referringGroup}/{source}"
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<ActionResult> Questions([FromBody] QuestionRequest request)
        {
            RequestHelpFormVariant requestHelpFormVariant = Enum.Parse<RequestHelpFormVariant>(request.FormVariant);
            RequestHelpFormStage requestHelpFormStage = Enum.Parse<RequestHelpFormStage>(request.FormStage);
            SupportActivities supportActivity = Enum.Parse<SupportActivities>(request.SupportActivity);

            QuestionsViewModel questionsViewModel = new QuestionsViewModel()
            {
                Questions = await _requestHelpBuilder.GetQuestionsForTask(requestHelpFormVariant, requestHelpFormStage, supportActivity)
            };

            questionsViewModel = questionsViewModel.GetQuestionsByLocation(request.Position);

            foreach (var question in questionsViewModel.Questions)
            {
                var matchedAnswer = request.Answers.Where(x => x.Id == question.ID && !string.IsNullOrEmpty(x.Answer)).FirstOrDefault();
                if (matchedAnswer != null)
                {
                    question.Model = matchedAnswer.Answer;
                }
            }

            return PartialView("_Questions", questionsViewModel);
        }

        private async Task<QuestionsViewModel> UpdateQuestionsViewModel(QuestionsViewModel previousQuestionsViewModel, RequestHelpFormVariant requestHelpFormVariant, RequestHelpFormStage requestHelpFormStage, SupportActivities selectedSupportActivity)
        {
            QuestionsViewModel updatedQuestionsViewModel = new QuestionsViewModel()
            {
                Questions = await _requestHelpBuilder.GetQuestionsForTask(requestHelpFormVariant, requestHelpFormStage, selectedSupportActivity)
            };

            if (previousQuestionsViewModel != null)
            {
                foreach (RequestHelpQuestion question in updatedQuestionsViewModel.Questions)
                {
                    var matchedQuestion = previousQuestionsViewModel.Questions.Where(pq => pq.ID == question.ID && !string.IsNullOrEmpty(pq.Model)).FirstOrDefault();
                    if (matchedQuestion != null)
                    {
                        question.Model = matchedQuestion.Model;
                    }
                }
            }

            return updatedQuestionsViewModel;
        }

        public class QuestionRequest
        {
            public string FormVariant { get; set; }
            public string FormStage { get; set; }
            public string SupportActivity { get; set; }
            public string Position { get; set; }
            public List<QuestionAnswer> Answers { get; set; }
            public class QuestionAnswer
            {
                public int Id { get; set; }
                public string Answer { get; set; }
            }
        }

        private int DecodeGroupIdOrGetDefault(string encodedGroupId)
        {
            try
            {
                return Base64Utils.Base64DecodeToInt(encodedGroupId);
            }
            catch
            {
                return -1;
            }
        }

        private string ValidateSource(string source)
        {
            if (source != null && source.All(c => char.IsLetterOrDigit(c) || c == '-'))
            {
                return source;
            }
            else
            {
                return null;
            }
        }
    }
}