using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using HttpTriggerAttribute = Microsoft.Azure.Functions.Worker.HttpTriggerAttribute;
using AuthorizationLevel = Microsoft.Azure.Functions.Worker.AuthorizationLevel;
using Simaira.Function.API.Swagger.Models;
using Swashbuckle.AspNetCore.Annotations;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Authorization;

namespace Simaira.Function.API.Swagger
{
    public class ReportController
    {
        private readonly ILogger<ReportController> _logger;

        public ReportController(ILogger<ReportController> logger)
        {
            _logger = logger;
        }


        // https://medium.com/@yuka1984/open-api-swagger-and-swagger-ui-on-azure-functions-v2-c-a4a460b34b55

        /// <summary>
        /// Add Formulation Archive Report
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("AddFormulationArchiveReportAsync")]
        [SwaggerOperation("AddFormulationArchiveReportAsync")]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(ResponseModel), description: "signed user email account")]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: null, description: "wrong email or password")]
        public async Task<IActionResult> AddFormulationArchiveReportAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "FormulationArchiveReport")]
        [RequestBodyType(typeof(RequestModel), "Contains Club Name used for searching purposes")]HttpRequest request)
        {
            _logger.LogInformation("This is an Http trigger function to test Swagger.");
            var result = await request.ReadFromJsonAsync<RequestModel>();

            string responseMessage = "This is an Http trigger function to test Swagger.";

            return new OkObjectResult(responseMessage);
        }



        /// <summary>
        /// Get Formulation Archive Report
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("GetFormulationArchiveReportsWithSearchParametersAsync")]
        [SwaggerOperation("GetFormulationArchiveReportsWithSearchParametersAsync")]
        [QueryStringParameter("Domains", "this is email", DataType = typeof(string), Required = true)]
        [QueryStringParameter("Product Code", "this is name", DataType = typeof(string), Required = false)]        
        [QueryStringParameter("Acoat", "this is phone", DataType = typeof(string), Required = false)]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(ResponseModel), description: "Get all reports")]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: null, description: "No report exist")]
        public async Task<IActionResult> GetFormulationArchiveReportsWithSearchParametersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "FormulationArchiveReport")] HttpRequest request)
        {
            _logger.LogInformation("This is an Http trigger function to test Swagger.");

            return new OkObjectResult($"Name: {request.Query["name"]}, email: {request.Query["email"]}, phone: {request.Query["phone"]}");
        }



        /// <summary>
        /// Get Formulation Archive Report by Id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("GetFormulationArchiveReportByRequestIdAsync")]
        [SwaggerOperation("GetFormulationArchiveReportByRequestIdAsync")]
        [QueryStringParameter("requestId", "this is unique report request Id", DataType = typeof(string), Required = true)]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(ResponseModel), description: "get requested report")]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: null, description: "report not exist")]
        public async Task<IActionResult> GetFormulationArchiveReportByRequestIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "FormulationArchiveReport/{requestId}")] HttpRequest request)
        {
            _logger.LogInformation("This is an Http trigger function to test Swagger.");

            return new OkObjectResult($"Name: {request.Query["requestId"]}");
        }


        // https://stackoverflow.com/questions/62158746/need-help-setting-my-default-api-route-on-swashbuckle-azurefunctions

        [Function("MaterialReportAsync")]
        /// <summary>
        /// Add Material Report
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[ProducesResponseType(typeof(RequestBodyModel), (int)HttpStatusCode.OK)]s       
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]       
        
        public async Task<IActionResult> MaterialReportAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = null)] HttpRequest request)
        {
            _logger.LogInformation("This is an Http trigger function to test Swagger.");

            string responseMessage = "This is an Http trigger function to test Swagger.";

            return new OkObjectResult(responseMessage);
        }
    }
}
