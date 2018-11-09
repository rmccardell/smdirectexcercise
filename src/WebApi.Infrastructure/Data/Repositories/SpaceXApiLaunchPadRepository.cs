using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using WebApi.Core.Contracts.Gateways;
using WebApi.Core.Entities;
using WebApi.Core.Exceptions;

namespace WebApi.Infrastructure.Data.Repositories
{
    /// <summary>
    /// LaunchPad repository based on Space X Api
    /// </summary>
    public class SpaceXLaunchPadApiRespository:ILaunchPadRespository
    {

        private readonly ILogger<SpaceXLaunchPadApiRespository> _logger;
        private readonly RestClient _restClient;
        private readonly SpaceXLauchPadConverter _converter = new SpaceXLauchPadConverter();
        
        public SpaceXLaunchPadApiRespository(ILogger<SpaceXLaunchPadApiRespository> logger)
        {
            _logger = logger;
            _restClient = new RestClient("https://api.spacexdata.com/");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Launch Pad Id</param>
        /// <returns>LaunchPad</returns>
        public Core.Entities.LaunchPad Get(string id)
        {
            LaunchPad launchPad;

            try
            {
              
                var request = new RestRequest("v2/launchPads/{id}");
                request.AddUrlSegment("id", id);

                IRestResponse response = _restClient.Execute(request);

                if (!response.IsSuccessful)
                {
                    //unexpected so we'll throw an exception here 

                    throw new RepositoryException(response.ErrorMessage);

                }

                var content = response.Content; // raw content as string
                launchPad = JsonConvert.DeserializeObject<LaunchPad>(content, _converter);
            }
            catch (Exception ex)
            {
                //log execptions here
                _logger.LogError($"Get LaunchPad request with id {id}.");
                _logger.LogError(ex.Message);

                throw new RepositoryException(ex.Message, ex);
            }

            return launchPad;
        }

        public List<Core.Entities.LaunchPad >GetAll()
        {
            List<LaunchPad> collection;

            try
            {

                var request = new RestRequest("v2/launchPads");

                IRestResponse response = _restClient.Execute(request);

                if (!response.IsSuccessful)
                {
                    //unexpected so we'll throw an exception here 

                    throw new RepositoryException(response.ErrorMessage);

                }

                var content = response.Content; // raw content as string
                collection = JsonConvert.DeserializeObject<List<LaunchPad>>(content, _converter);
            }
            catch (Exception ex)
            {
                //log execptions here
                _logger.LogError($"Get all LaunchPad request.");
                _logger.LogError(ex.Message);

                throw new RepositoryException(ex.Message, ex);
            }


            return collection;
        }
    }

    /// <summary>
    /// Helper class for coverting results from SpaceXApi to the Domain Entity LaunchPad
    /// </summary>
    internal class SpaceXLauchPadConverter : JsonConverter<LaunchPad>
    {
        public override void WriteJson(JsonWriter writer, LaunchPad value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override LaunchPad ReadJson(JsonReader reader, Type objectType, LaunchPad existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            dynamic data = JObject.Load(reader);
            var model = new LaunchPad
            {
                Id = data.id,
                Name = data.full_name,
                Status = data.status
            };

            return model;
        }
    }
}
