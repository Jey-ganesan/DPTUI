using ARAP.Controllers;
using DPT.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json.Linq;

namespace DPT.MVC.Controllers
{
    public class PAYMENTHDRCONTROLLER : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PAYMENTHDRCONTROLLER> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public HttpClient HttpClient = new HttpClient();
        string connection = string.Empty;


        public PAYMENTHDRCONTROLLER(IConfiguration configuration, ILogger<PAYMENTHDRCONTROLLER> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            //HttpClient = _httpClientFactory.CreateClient("DhlQatarClient");
            HttpClient.BaseAddress = new Uri(_configuration["MySetting:ApiURL"]);
            connection = _configuration.GetConnectionString("DPT.ConnectionString");

        }

        [HttpPost]
        public async Task<JsonResult> SavePAYMENTHDR(PAYMENTHDR model)
        {
            try
            {
                if (model != null)
                {
                    var requestBody = new HttpRequestMessage();
                    if (model.ID != 0)
                    {
                        model.LASTUPDATED = DateTime.Now;
                        model.LASTUPDATEDBY = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        requestBody = new HttpRequestMessage(HttpMethod.Patch, "/api/PAYMENTHDR");
                    }
                    else
                    {

                        model.CREATED = DateTime.Now;
                        model.CREATEDBY = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        model.LASTUPDATED = DateTime.Now;
                        model.LASTUPDATEDBY = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        requestBody = new HttpRequestMessage(HttpMethod.Post, "/api/PAYMENTHDR");
                    }
                    var serializations = JsonConvert.SerializeObject(model);
                    requestBody.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("applications/json"));
                    requestBody.Content = new StringContent(serializations);
                    requestBody.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await HttpClient.SendAsync(requestBody);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<JObject>(content);
                        int requesthdrId = data["paymenthdr"]["id"].Value<int>();
                        var successResponse = new { Message = data, StatusCode = 201, Hdrid = requesthdrId };
                        return Json(successResponse);
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        // Handle conflict (HTTP status code 409) by creating a custom JSON response
                        var conflictResponse = new { ErrorMessage = "Country with same short name already exists.", StatusCode = 409 };
                        return Json(conflictResponse);
                    }
                }
            }
            catch (Exception e)
            {
                // Get the controller and action names
                var controllerName = ControllerContext.ActionDescriptor.ControllerName;
                var actionName = ControllerContext.ActionDescriptor.ActionName;

                // Get the DisplayName from the session
                var displayName = HttpContext.Session.GetString("DisplayName");

                // Log the error along with the controller, action, and DisplayName
                _logger.LogError(e, "An error occurred in {ControllerName}/{ActionName} for {DisplayName}.", controllerName, actionName, displayName);
            }

            return Json("");
        }
        public async Task<JsonResult> GetAllPAYMENTHDR()
        {
            try
            {
                var response = await HttpClient.GetAsync("/api/PAYMENTHDR");
                var content = await response.Content.ReadAsStringAsync();
                var currenciesDetails = System.Text.Json.JsonSerializer.Deserialize<object>(content);
                return Json(currenciesDetails);
            }
            catch (Exception e)
            {
                // Get the controller and action names
                var controllerName = ControllerContext.ActionDescriptor.ControllerName;
                var actionName = ControllerContext.ActionDescriptor.ActionName;

                // Get the DisplayName from the session
                var displayName = HttpContext.Session.GetString("DisplayName");

                // Log the error along with the controller, action, and DisplayName
                _logger.LogError(e, "An error occurred in {ControllerName}/{ActionName} for {DisplayName}.", controllerName, actionName, displayName);
                return Json(new Country());
            }
        }
        public async Task<JsonResult> GetRequestnos()
        {
            try
            {
                var response = await HttpClient.GetAsync("/api/PAYMENTHDR/GetRequestnos");
                var content = await response.Content.ReadAsStringAsync();
                var currenciesDetails = System.Text.Json.JsonSerializer.Deserialize<object>(content);
                return Json(currenciesDetails);
            }
            catch (Exception e)
            {
                // Get the controller and action names
                var controllerName = ControllerContext.ActionDescriptor.ControllerName;
                var actionName = ControllerContext.ActionDescriptor.ActionName;

                // Get the DisplayName from the session
                var displayName = HttpContext.Session.GetString("DisplayName");

                // Log the error along with the controller, action, and DisplayName
                _logger.LogError(e, "An error occurred in {ControllerName}/{ActionName} for {DisplayName}.", controllerName, actionName, displayName);
                return Json(new Country());
            }
        }
        public async Task<JsonResult> GetdetailsByRequestid(int Hdrid)
        {
            var client = _httpClientFactory.CreateClient("DPTClient");
            var response = await client.GetAsync("/api/PAYMENTHDR/GetdetailsByRequestid?Hdrid=" + Hdrid);
            var content = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<object>(content);

            return Json(data);
        }
        [HttpGet]
        public async Task<JsonResult> GetAllPaymentExceptionForApprove()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var client = _httpClientFactory.CreateClient("DPTClient");
            var response = await client.GetAsync("/api/PAYMENTHDR/GetAllPaymentExceptionForApprove?UserId="+userId);
            var content = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<object>(content);

            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> ExceptionalApproveSelectedPayments(int id, string approved, string comments)
        {
            var client = _httpClientFactory.CreateClient("DPTClient");
            var UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var response = await client.GetAsync("/api/PAYMENTHDR/UpdateWhenExceptionalPaymentIsApproved?Id=" + id + "&approved=" + approved + "&comments=" + comments + "&UpdatedBy=" + UserId);
            var content = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<object>(content);

            return Json(data);
        }
    }
}
