using System.Collections.Generic;
using WebApi.Core.Entities;

namespace WebApi.Core.Contracts.Gateways
{
    /// <summary>
    /// Defines signatures for LaunchPadRepositories
    /// </summary>
    public interface ILaunchPadRespository
    {
        LaunchPad Get(string id);
        List<LaunchPad> GetAll();
    }
}
