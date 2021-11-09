using System.Collections.Generic;
using System.Net;
using functionsapp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace functionsapp
{
    public static class IronMaiden
    {
        [Function(nameof(IronMaiden))]
        [OpenApiOperation(operationId: nameof(IronMaiden), tags: new[] { nameof(IronMaiden) }, Summary = nameof(IronMaiden), Description = "Iron Maiden", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
        public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
            FunctionContext executionContext, Album album, Album album2)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString($"{album?.Name} - {album.Year}");

            return response;
        }
    }
}
