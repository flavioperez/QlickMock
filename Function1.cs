using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Microsoft.Extensions.Configuration;

namespace QlickMock
{
    public static class Function1
    {
        [FunctionName("GetMockedTrelloReport")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("GetMockedTrelloReport called.");

            string jsonFile = GetFileNameWithExtensionIfApply(req.Query["jsonFile"]);

            if (string.IsNullOrEmpty(jsonFile))
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                jsonFile = jsonFile ?? data?.report;
            }

            if (string.IsNullOrEmpty(jsonFile))
            {
                log.LogInformation("Returning bad request: invalid json report name.");
                return new BadRequestObjectResult("Please pass [jsonFile] value on the query string or in the request body");
            }

            string mockFile = string.Empty;
            string fileContent = string.Empty; 
            try
            {
                mockFile = Path.Combine(context.FunctionAppDirectory, "Data", jsonFile);
                fileContent = System.IO.File.ReadAllText(mockFile);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"ERROR: [{mockFile}] not found.");
            }

            object jsonObject = null;

            try
            {
                jsonObject = JsonConvert.DeserializeObject(fileContent);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult($"ERROR: File content is not a invalid json structure.");
            }

            return (ActionResult)new OkObjectResult(jsonObject);
        }

        private static string GetFileNameWithExtensionIfApply(string fileName)
        {
            string withExt = fileName;

            if (!string.IsNullOrEmpty(fileName))
            {
                if (!withExt.Trim().ToLower().EndsWith(".json"))
                {
                    withExt = withExt + ".json";
                }
            }
            return withExt;
        }
    }
}
