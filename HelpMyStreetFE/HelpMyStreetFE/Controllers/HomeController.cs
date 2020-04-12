﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;

namespace HelpMyStreetFE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAddressService _addressService;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IAddressService addressService, IConfiguration configuration)
        {
            _logger = logger;
            _addressService = addressService;
            _configuration = configuration;
        }


        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Get home");

            var reqs = new List<Task<int>>
            {
                _addressService.GetStreetChampions(),
                _addressService.GetStreetsCovered(),
                _addressService.GetStreetsRemaining(),
                _addressService.GetPostCodesCovered()
            };

            await Task.WhenAll(reqs);

            var model = new HomeViewModel
            {
                NumStreetChampions = reqs[0].Result,
                NumStreetsCovered = reqs[1].Result,
                NumStreetsRemaining = reqs[2].Result,
                PostCodesCovered = reqs[3].Result,
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
