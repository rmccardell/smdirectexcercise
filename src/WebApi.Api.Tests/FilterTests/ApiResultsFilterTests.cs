using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Api.Controllers;
using WebApi.Api.Filters;
using WebApi.Api.Tests.Helpers;
using WebApi.Core.Entities;
using Xunit;

namespace WebApi.Api.Tests.FilterTests
{
    public class ApiResultsFilterTests
    {

        private readonly List<LaunchPad> _data;

        public ApiResultsFilterTests()
        {
            var data = TestDataHelper.LoadTestData("LaunchPadsData.json");
            _data = JsonConvert.DeserializeObject<List<LaunchPad>>(data);
        }


        [Fact]
        public void ShouldBeAbleToFilterResultsWithQueryParameterFilterName_Name()
        {

            Mock<LoggerFactory> loggerFactory = new Mock<LoggerFactory>();


            var actionExecutedContext =
                BuildMockActionExecutedContext(new Dictionary<string, StringValues> {{"filter", "name"}},
                    new OkObjectResult(_data));

            var resultsFilter = new ApiResultsFilter(loggerFactory.Object);
            resultsFilter.OnActionExecuted(actionExecutedContext);
            
            var result = actionExecutedContext.Result as OkObjectResult;
            var jsonArray = JArray.Parse(JsonConvert.SerializeObject(result.Value));


            Assert.True(jsonArray.HasValues);

            foreach (var jToken in jsonArray.Children())
            {
                var jsonObject = (JObject) jToken;
                Assert.Contains(jsonObject.Properties().ToList(), p => p.Name == "name");
                Assert.Single(jsonObject.Properties().ToList());
            }
           


        }

        [Fact]
        public void ShouldBeAbleToFilterResultsWithQueryParameterFilterName_Status()
        {

            Mock<LoggerFactory> loggerFactory = new Mock<LoggerFactory>();


            var actionExecutedContext =
                BuildMockActionExecutedContext(new Dictionary<string, StringValues> { { "filter", "status" } },
                    new OkObjectResult(_data));

            var resultsFilter = new ApiResultsFilter(loggerFactory.Object);
            resultsFilter.OnActionExecuted(actionExecutedContext);

            var result = actionExecutedContext.Result as OkObjectResult;
            var jsonArray = JArray.Parse(JsonConvert.SerializeObject(result.Value));


            Assert.True(jsonArray.HasValues);

            foreach (var jToken in jsonArray.Children())
            {
                var jsonObject = (JObject)jToken;
                Assert.Contains(jsonObject.Properties().ToList(), p => p.Name == "status");
                Assert.Single(jsonObject.Properties().ToList());
            }



        }

        [Fact]
        public void ShouldBeAbleToFilterResultsWithQueryParameterFilterName_Id()
        {

            Mock<LoggerFactory> loggerFactory = new Mock<LoggerFactory>();


            var actionExecutedContext =
                BuildMockActionExecutedContext(new Dictionary<string, StringValues> { { "filter", "id" } },
                    new OkObjectResult(_data));

            var resultsFilter = new ApiResultsFilter(loggerFactory.Object);
            resultsFilter.OnActionExecuted(actionExecutedContext);

            var result = actionExecutedContext.Result as OkObjectResult;
            var jsonArray = JArray.Parse(JsonConvert.SerializeObject(result.Value));


            Assert.True(jsonArray.HasValues);

            foreach (var jToken in jsonArray.Children())
            {
                var jsonObject = (JObject)jToken;
                Assert.Contains(jsonObject.Properties().ToList(), p => p.Name == "id");
                Assert.Single(jsonObject.Properties().ToList());
            }



        }

        [Fact]
        public void ShouldBeAbleToFilterResultsWithQueryParameterFilterNames_Name_Status_Id()
        {

            Mock<LoggerFactory> loggerFactory = new Mock<LoggerFactory>();


            var actionExecutedContext =
                BuildMockActionExecutedContext(new Dictionary<string, StringValues> { { "filter", "name,status,id" } },
                    new OkObjectResult(_data));

            var resultsFilter = new ApiResultsFilter(loggerFactory.Object);
            resultsFilter.OnActionExecuted(actionExecutedContext);

            var result = actionExecutedContext.Result as OkObjectResult;
            var jsonArray = JArray.Parse(JsonConvert.SerializeObject(result.Value));


            Assert.True(jsonArray.HasValues);

            foreach (var jToken in jsonArray.Children())
            {
                var jsonObject = (JObject)jToken;
                Assert.Contains(jsonObject.Properties().ToList(), p => p.Name == "id");
                Assert.Contains(jsonObject.Properties().ToList(), p => p.Name == "name");
                Assert.Contains(jsonObject.Properties().ToList(), p => p.Name == "status");
                Assert.Equal(3, jsonObject.Properties().ToList().Count);
            }



        }

        [Fact]
        public void ShouldReturnEmptyResultSetWithFilterNamesOfPropertiesNotInResultSet()
        {

            Mock<LoggerFactory> loggerFactory = new Mock<LoggerFactory>();


            var actionExecutedContext =
                BuildMockActionExecutedContext(new Dictionary<string, StringValues> { { "filter", "non" } },
                    new OkObjectResult(_data));

            var resultsFilter = new ApiResultsFilter(loggerFactory.Object);
            resultsFilter.OnActionExecuted(actionExecutedContext);

            var result = actionExecutedContext.Result as OkObjectResult;
            var jsonArray = JArray.Parse(JsonConvert.SerializeObject(result.Value));


            Assert.True(jsonArray.HasValues);

            foreach (var jToken in jsonArray.Children())
            {
                var jsonObject = (JObject)jToken;
                Assert.Empty(jsonObject.Properties().ToList());
            }



        }

        private ActionExecutedContext BuildMockActionExecutedContext<TResult>(Dictionary<string, StringValues> queryParmeters, TResult result) where TResult: ObjectResult
        {
            var modelState = new ModelStateDictionary();
            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();

            mockRequest.Setup(request => request.Query).Returns(new QueryCollection(queryParmeters));
            mockHttpContext.Setup(context => context.Request).Returns(mockRequest.Object);

            var actionContext = new ActionContext(
                mockHttpContext.Object,
                new Mock<RouteData>().Object,
                new Mock<ActionDescriptor>().Object,
                modelState
            );

            var controller = new LaunchPadsController(null, null);

            var actionExecutingContext =
                new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), controller)
                {
                    Result = new OkObjectResult(_data)
                };


            return actionExecutingContext;
        }
    }
}
