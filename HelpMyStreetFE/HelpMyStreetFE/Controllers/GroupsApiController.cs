﻿using System;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HelpMyStreetFE.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupsApiController : Controller
    {
        private readonly ILogger<GroupsApiController> _logger;
        private readonly IGroupService _groupService;
        private readonly IAuthService _authService;

        public GroupsApiController(ILogger<GroupsApiController> logger, IGroupService groupService, IAuthService authService)
        {
            _logger = logger;
            _groupService = groupService;
            _authService = authService;
        }

        [AuthorizeAttributeNoRedirect]
        [HttpGet("join-group")]
        public async Task<IActionResult> JoinGroup(string g, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
                int groupId = Base64Utils.Base64DecodeToInt(g);

                var result = await _groupService.PostAssignRole(user.ID, groupId, GroupRoles.Member, user.ID, cancellationToken);

                return result switch
                {
                    GroupPermissionOutcome.Success => StatusCode((int)HttpStatusCode.OK),
                    GroupPermissionOutcome.Unauthorized => StatusCode((int)HttpStatusCode.Unauthorized),
                    _ => throw new ArgumentException(message: $"Unexpected GroupPermissionOutcome value: {result}", paramName: nameof(result))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in JoinGroup", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [AuthorizeAttributeNoRedirect]
        [HttpGet("leave-group")]
        public async Task<IActionResult> LeaveGroup(string g, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
                int groupId = Base64Utils.Base64DecodeToInt(g);

                var result = await _groupService.PostRevokeRole(user.ID, groupId, GroupRoles.Member, user.ID, cancellationToken);

                return result switch
                {
                    GroupPermissionOutcome.Success => StatusCode((int)HttpStatusCode.OK),
                    GroupPermissionOutcome.Unauthorized => StatusCode((int)HttpStatusCode.Unauthorized),
                    _ => throw new ArgumentException(message: $"Unexpected GroupPermissionOutcome value: {result}", paramName: nameof(result))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in LeaveGroup", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
