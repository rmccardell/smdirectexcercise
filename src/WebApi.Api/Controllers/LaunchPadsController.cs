using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Api.Filters;
using WebApi.Core.Contracts.Gateways;
using WebApi.Core.Entities;

namespace WebApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaunchPadsController : ControllerBase
    {

        private readonly ILogger<LaunchPadsController> _logger;
        private readonly ILaunchPadRespository _launchPadRepository;
        public LaunchPadsController(ILogger<LaunchPadsController> logger, ILaunchPadRespository launchPadRepository)
        {
            _logger = logger;
            _launchPadRepository = launchPadRepository;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiResultsFilter))]
        [ProducesResponseType(typeof(IEnumerable<LaunchPad>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 500)]
        public IActionResult Get(string filter)
        {
            try
            {
                var launchPads = _launchPadRepository.GetAll();

                if (launchPads == null)
                    return NoContent();

                return Ok(launchPads);

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(ApiResultsFilter))]
        [ProducesResponseType(typeof(LaunchPad), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 500)]
        public IActionResult Get(string id, string filter)
        {
            try
            {
                if (id == null)
                    return BadRequest();

                var launchPad = _launchPadRepository.Get(id);

                if (launchPad == null)
                    return NoContent();

                return Ok(launchPad);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

        }
    }
}