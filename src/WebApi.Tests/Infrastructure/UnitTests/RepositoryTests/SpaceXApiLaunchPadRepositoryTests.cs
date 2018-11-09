using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebApi.Core.Contracts;
using WebApi.Core.Contracts.Gateways;
using WebApi.Infrastructure.Data.Repositories;

namespace WebApi.Tests.Infrastructure.UnitTests.RepositoryTests
{
    [TestClass]
    public class SpaceXApiLaunchPadRepositoryTests
    {
        private ILaunchPadRespository _launchPadRespository;


        [TestInitialize]
        public void Setup()
        {
            Mock<ILogger<SpaceXLaunchPadApiRespository>> logger = new Mock<ILogger<SpaceXLaunchPadApiRespository>>(); 
            _launchPadRespository = new SpaceXLaunchPadApiRespository(logger.Object);
        }


        [TestMethod]
        public void ShouldBeAbleRetrieveLaunchPads()
        {
            try
            {
                //get all launch pads
                var launchPads = _launchPadRespository.GetAll();
                Assert.IsNotNull(launchPads);

                var launchPad = launchPads.FirstOrDefault();

                if (launchPad != null)
                {
                    //should be able to retrieve launch pad by id
                    var retrievedLaunchPad = _launchPadRespository.Get(launchPad.Id);

                    Assert.IsNotNull(retrievedLaunchPad);
                    Assert.AreEqual(launchPad.Id, retrievedLaunchPad.Id);
                    Assert.AreEqual(launchPad.Name, retrievedLaunchPad.Name);
                }

                //generate random string for launchpad id
                var launchPadId = Path.GetRandomFileName();
                //should be null for non-existent launchpad
                var shouldBeNullLaunchPad = _launchPadRespository.Get(launchPadId);
                Assert.IsNull(shouldBeNullLaunchPad);

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
          



        }
    }
}
