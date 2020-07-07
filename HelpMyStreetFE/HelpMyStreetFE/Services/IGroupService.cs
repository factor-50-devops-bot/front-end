﻿using HelpMyStreet.Contracts.GroupService.Request;
using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Contracts.Shared;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IGroupService
    {
        Task<ResponseWrapper<GetGroupByKeyResponse, GroupServiceErrorCode>> GetGroupByKey(string groupKey);

        Task<ResponseWrapper<GetGroupResponse, GroupServiceErrorCode>> GetGroup(int groupId);

        Task<ResponseWrapper<GetChildGroupsResponse, GroupServiceErrorCode>> GetChildGroups(int groupId);

        Task<ResponseWrapper<PostAssignRoleResponse, GroupServiceErrorCode>> AssignRole(PostAssignRoleRequest postAssignRoleRequest);

        Task<ResponseWrapper<GetRegistrationFormVariantResponse, GroupServiceErrorCode>> GetRegistrationFormVariant(int groupId, string source);

        Task<ResponseWrapper<GetRequestHelpFormVariantResponse, GroupServiceErrorCode>> GetRequestFormVariant(int groupId, string source);

        Task<ResponseWrapper<PostAddUserToDefaultGroupsResponse, GroupServiceErrorCode>> PostAddUserToDefaultGroups(int userId);
    }
}