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

            string mockFile = Path.Combine(context.FunctionAppDirectory, "Data", "TrelloReport.json");

            string allText = System.IO.File.ReadAllText(mockFile);
            object jsonObject = JsonConvert.DeserializeObject(allText);
            //return jsonObject;

            return jsonObject != null
                ? (ActionResult)new OkObjectResult(jsonObject)
                : new BadRequestObjectResult("Error reading Trello Report file.");
        }
    }
}
