﻿using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using HelpMyStreet.Cache;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.UnitTests.Services
{
    class UserServiceTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<ILogger<HelpMyStreetFE.Services.UserService>> _logger;
        private Mock<IMemDistCache<User>> _memDistCache;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _logger = new Mock<ILogger<HelpMyStreetFE.Services.UserService>>();
            _memDistCache = new Mock<IMemDistCache<User>>();
        }

        [Test]
        public void FormatName_EmptyString()
        {
            HelpMyStreetFE.Services.UserService userService = new HelpMyStreetFE.Services.UserService(_userRepository.Object, _logger.Object, _memDistCache.Object);

            Assert.AreEqual("", userService.FormatName(""));
        }

        [Test]
        public void FormatName_ShortName()
        {
            HelpMyStreetFE.Services.UserService userService = new HelpMyStreetFE.Services.UserService(_userRepository.Object, _logger.Object, _memDistCache.Object);

            Assert.AreEqual("A B C O'd", userService.FormatName(" a b C o'd"));
        }

        [Test]
        public void FormatName_MultiWordName()
        {
            HelpMyStreetFE.Services.UserService userService = new HelpMyStreetFE.Services.UserService(_userRepository.Object, _logger.Object, _memDistCache.Object);

            Assert.AreEqual("Martin Seamus MARTY McFly", userService.FormatName("    martin seamus   MARTY mcFly "));
        }
    }
}
