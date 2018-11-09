using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApi.Api.Filters
{
    /// <summary>
    /// An ActionFilterAttribute that allows for filtering out json results based on 
    /// "filter" query string parameter values. e.g.  filter=name,status will only return the name 
    /// and status properties for the relevant object values
    /// </summary>
    public class ApiResultsFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        public ApiResultsFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ApiResultsFilter>();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var query = context.HttpContext.Request.Query;
            var result = context.Result as OkObjectResult;
            string filter = string.Empty;

            //we will only process results that represent successful api results and have the filter query string parameter
            if (!query.ContainsKey("filter") || result == null)
            {
                return;
            }

            try
            {

                filter = query["filter"];
                var filterValues = filter.Split(",");

                //if there aren't any filters then leave original result set unmodified
                if (!filterValues.Any())
                {
                    return;
                }

                //we need to serialize the IActionResult Value in order to manipulate the JSON
                var responseAsString = JsonConvert.SerializeObject(result.Value);
                string templateString = BuildJsonTemplateString(filterValues);
                var jsonObject = FilterJsonContent(responseAsString, templateString);
                result.Value = jsonObject;

            }
            catch (Exception ex)
            {
                _logger.LogError($"error occurred applying filter: {filter}");
                _logger.LogError(ex.Message);

            }
        }

        private static string CapitalizeFirstLetter(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        //builds json string template based on filter values
        private static string BuildJsonTemplateString(string[] values)
        {
            StringBuilder sb = new StringBuilder("{");

            if (values != null && values.Length > 0)
            {

                for (int i = 0; i < values.Length; i++)
                {
                    var value = values[i].Trim();

                    sb.Append($"'{value}':''");
                    if (i < values.Length - 1)
                    {
                        sb.Append(",");
                    }
                }

            }

            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// Makes use of JSONPath for filtering JSON
        /// adapted from https://www.khalidabuhakmeh.com/transforming-json-using-json-net-and-jsonpath
        /// </summary>
        /// <param name="original"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        private static JToken FilterJsonContent(string original, string template)
        {

            var token = JToken.Parse(original);
            var jsonPathTemplate = JObject.Parse(template);


            //handles json array
            if (token.Type == JTokenType.Array)
            {
                var originalArray = JArray.Parse(original);
                var itemResults = new JArray();

                foreach (var item in originalArray.Children())
                {
                    if (item.Type == JTokenType.Object)
                    {
                        var builtObject = BuildJObjectFromTemplate(jsonPathTemplate, (JObject)item);
                        itemResults.Add(builtObject);
                    }
                }

                return itemResults;
            }

            //handles regular json object
            var originalObject = JObject.Parse(original);
            var result = BuildJObjectFromTemplate(jsonPathTemplate, originalObject);
            return result;

        }

        private static JObject BuildJObjectFromTemplate(JObject templateObject, JObject contentObject)
        {
            JObject jObject = new JObject();

            foreach (var property in templateObject.Properties())
            {
                var value = property.Value?.ToString();

                var path = value?.StartsWith("$") == true
                    ? value
                    : property.Path;

                var selected = contentObject.SelectToken(CapitalizeFirstLetter(path));

                if (selected != null)
                {
                    jObject.Add(property.Name, selected);
                }
            }

            //sort properties in keeping with Json.net
            jObject = new JObject(jObject.Properties().OrderBy(p => p.Name));
            return jObject;
        }
    }
}
