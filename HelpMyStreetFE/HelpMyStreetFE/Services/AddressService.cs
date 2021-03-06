﻿using HelpMyStreet.Contracts.AddressService.Request;

using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Utils;
using GetPostcodesResponse = HelpMyStreetFE.Models.Reponses.GetPostcodesResponse;

namespace HelpMyStreetFE.Services
{
    public class AddressService : BaseHttpService, IAddressService
    {
        private readonly ILogger<AddressService> _logger;
        private readonly IAddressRepository _addressRepository;
        private readonly IUserRepository _userRepository;

        public AddressService(
            ILogger<AddressService> logger,
            IConfiguration configuration,
            IAddressRepository addressRepository,
            IUserRepository userRepository,
            HttpClient client) : base(client, configuration, "Services:Address")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _addressRepository = addressRepository;
            _userRepository = userRepository;
        }

        public Task<int> GetTotalStreets()
        {
            return Task.Factory.StartNew(() => 1765422);  // TODO: Implement in Address Service
        }

        public async Task<GetPostCodeResponse> CheckPostCode(string postcode)
        {
            postcode = HelpMyStreet.Utils.Utils.PostcodeFormatter.FormatPostcode(postcode);
            var response = await Client.GetAsync($"/api/getpostcode?postcode={postcode}");
            var str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetPostCodeResponse>(str);
        }

        public async Task<List<PostCodeDetail>> GetPostcodeDetailsNearUser(User user)
        {
            var postCodes = await _addressRepository.GetNearbyPostcodes(user.PostalCode);

            var nearby = new List<PostCodeDetail>();

            foreach (var postcode in postCodes.Content.Postcodes)
            {
                var street = string.Concat(postcode.AddressDetails[0].AddressLine1.Where(c => !char.IsNumber(c)));
                var champs = await _userRepository.GetChampionCountByPostcode(postcode.Postcode);
                nearby.Add(new PostCodeDetail { StreetName = street, ChampionCount = champs, Postcode = postcode.Postcode, DistanceInMetres = postcode.DistanceInMetres, FriendlyName = postcode.FriendlyName });
            }

            return nearby;
        }

        public async Task<GetPostCodeCoverageResponse> GetPostcodeCoverage(string postcode)
        {

            postcode = HelpMyStreet.Utils.Utils.PostcodeFormatter.FormatPostcode(postcode);
            GetPostCodeCoverageResponse response = new GetPostCodeCoverageResponse();
            response.PostCodeResponse = await CheckPostCode(postcode);
            if (response.PostCodeResponse.HasContent && response.PostCodeResponse.IsSuccessful)
            {
                response.ChampionCount = await _userRepository.GetChampionCountByPostcode(postcode);
                response.VolunteerCount = await _userRepository.GetVolunteerCountByPostcode(postcode);
            }

            return response;
        }

        public async Task<GetPostcodesResponse> GetFriendlyNames(List<string> postcodes)
        {
            GetPostcodesRequest request = new GetPostcodesRequest
            {
                PostcodeList = new PostcodeList
                {
                    Postcodes = postcodes.Select(x => x).ToList()
                },
                IncludeAddressDetails = false
            };
            return await _addressRepository.GetPostcodes(request);

        }

        public async Task<ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>> GetPostcodeCoordinates(GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest)
        {
            string json = JsonConvert.SerializeObject(getPostcodeCoordinatesRequest);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("/api/GetPostcodeCoordinates", data);
            string str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>>(str);
        }

        public async Task<ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>> GetPostcodeCoordinate(string postcode)
        {
            postcode = PostcodeFormatter.FormatPostcode(postcode);
            GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest = new GetPostcodeCoordinatesRequest()
            {
                Postcodes = new List<string>() { postcode }
            };
            ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode> getPostcodeCoordinatesResponse = await GetPostcodeCoordinates(getPostcodeCoordinatesRequest);

            return getPostcodeCoordinatesResponse;
        }
    }
}
