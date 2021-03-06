﻿using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Repositories
{
	public interface IRequestHelpRepository
	{
        Task<LogRequestResponse> PostNewRequestForHelpAsync(PostNewRequestForHelpRequest request);
        Task<JobSummary> GetJobSummaryAsync(int jobId);
        Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId, int userId);
        Task<IEnumerable<JobHeader>> GetJobsByFilterAsync(GetJobsByFilterRequest request);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToDoneAsync(int jobId, int createdByUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToOpenAsync(int jobId, int createdByUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToCancelledAsync(int jobId, int createdByUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToInProgressAsync(int jobId, int createdByUserId, int volunteerUserId);
        Task<GetQuestionsByActivtiesResponse> GetQuestionsByActivity(GetQuestionsByActivitiesRequest request);
    }
}


