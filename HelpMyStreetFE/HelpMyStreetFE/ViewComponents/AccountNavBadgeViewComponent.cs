﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.ViewComponents
{
    public class AccountNavBadgeViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;

        public AccountNavBadgeViewComponent(IRequestService requestService, IGroupService groupService)
        {
            _requestService = requestService;
            _groupService = groupService;
        }

        public async Task<IViewComponentResult> InvokeAsync(User user, MenuPage menuPage, string groupKey, string cssClass, CancellationToken cancellationToken)
        {
            var viewModel = new AccountNavBadgeViewModel()
            {
                CssClass = cssClass,
                MenuPage = menuPage,
                GroupKey = groupKey,
                Count = await GetCount(user, menuPage, groupKey, cancellationToken)
            };

            return View("AccountNavBadge", viewModel);
        }

        public async Task<int> GetCount(User user, MenuPage menuPage, string groupKey, CancellationToken cancellationToken)
        {
            if (menuPage == MenuPage.GroupRequests)
            {
                if (!await _groupService.GetUserHasRole(user.ID, groupKey, GroupRoles.TaskAdmin, cancellationToken))
                {
                    return 0;
                }
            }

            IEnumerable<JobHeader> jobs = menuPage switch
            {
                MenuPage.GroupRequests
                    => (await _requestService.GetGroupRequestsAsync(groupKey, false, cancellationToken))?.Where(j => j.JobStatus == JobStatuses.Open || j.JobStatus == JobStatuses.InProgress),
                MenuPage.AcceptedRequests
                    => (await _requestService.GetJobsForUserAsync(user.ID, false, cancellationToken))?.Where(j => j.JobStatus == JobStatuses.InProgress),
                MenuPage.CompletedRequests
                    => (await _requestService.GetJobsForUserAsync(user.ID, false, cancellationToken))?.Where(j => j.JobStatus == JobStatuses.Done),
                MenuPage.OpenRequests
                    => (await _requestService.GetOpenJobsAsync(user, false, cancellationToken)),
                _ => null
            };

            return jobs != null ? jobs.Count() : 0;
        }
    }
}
