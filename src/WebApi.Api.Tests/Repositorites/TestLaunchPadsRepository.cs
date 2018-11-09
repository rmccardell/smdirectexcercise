using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApi.Core.Contracts.Gateways;
using WebApi.Core.Entities;

namespace WebApi.Api.Tests.Repositorites
{
    public class TestLaunchPadsRepository:ILaunchPadRespository
    {
        private readonly List<LaunchPad> _launchPads;

        public TestLaunchPadsRepository(List<LaunchPad> data)
        {
            _launchPads = data;
        }

        public LaunchPad Get(string id)
        {
            return _launchPads.FirstOrDefault(lp => lp.Id == id);
        }

        public List<LaunchPad> GetAll()
        {
            return _launchPads;
        }
    }
}
