using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Api.Controllers;
using WebApi.Api.Tests.Helpers;
using WebApi.Api.Tests.Repositorites;
using WebApi.Core.Entities;
using Xunit;

namespace WebApi.Api.Tests.Controllers
{
    public class LaunchPadsControllerTests
    {
        private readonly LaunchPadsController _launchPadsController;
        private readonly List<LaunchPad> _launchPads;

        public LaunchPadsControllerTests()
        {
            var testFile = TestDataHelper.LoadTestData("LaunchPadsData.json");
            _launchPads  = JsonConvert.DeserializeObject<List<LaunchPad>>(testFile);
            var launchPadsRepository = new TestLaunchPadsRepository(_launchPads);

            Mock<ILogger<LaunchPadsController>> logger = new Mock<ILogger<LaunchPadsController>>();
            _launchPadsController = new LaunchPadsController(logger.Object, launchPadsRepository);

        }

        [Fact]
        public void CanRetrieveAllLaunchPads()
        {
            var results = _launchPadsController.Get(null);

            var okResult = results as OkObjectResult;
            Assert.NotNull(okResult);

            var retrievedLaunchPads = (List<LaunchPad>)okResult.Value;

            Assert.NotNull(retrievedLaunchPads);

            Assert.Equal(_launchPads, retrievedLaunchPads);
           
        }

        [Fact]
        public void CanRetrieveLaunchPadsById()
        {
            foreach (var launchPad in _launchPads)
            {
                var result = _launchPadsController.Get(launchPad.Id, null);
                var okResult = result as OkObjectResult;
                Assert.NotNull(okResult);

                var retrievedLaunchPad = (LaunchPad)okResult.Value;

                Assert.NotNull(retrievedLaunchPad);

                Assert.Same(launchPad, retrievedLaunchPad);
            }
        }

        [Fact]
        public void ShouldReturnNoContentForNonExistingLaunchPad()
        {
            var result = _launchPadsController.Get("gooop-334343", null);
            Assert.IsType<NoContentResult>(result);

        }

        [Fact]
        public void ShouldReturnBadRequestForMissingIdParameter()
        {
            var result = _launchPadsController.Get(null, null);
            Assert.IsType<BadRequestResult>(result);

        }

      
    }
}
