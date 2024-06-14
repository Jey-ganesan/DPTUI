using DPT.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json.Linq;

namespace DPT.MVC.Controllers
{
    public class REQUESTHDRCONTROLLER : Controller
    {
        private readonly ILogger<REQUESTHDRCONTROLLER> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public REQUESTHDRCONTROLLER(ILogger<REQUESTHDRCONTROLLER> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<ActionResult> SaveREQUESTHDR(REQUESTHDR model)
        {
            var client = _httpClientFactory.CreateClient("DPTClient");
            try
            {
                if (model != null)
                {
                    var requestBody = new HttpRequestMessage();
                    if (model.ID != 0)
                    {
                        model.CREATED = DateTime.Now;
                        model.CREATEDBY = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        model.LASTUPDATED = DateTime.Now;
                        model.LASTUPDATEDBY = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        requestBody = new HttpRequestMessage(HttpMethod.Patch, "/api/REQUESTHDR");

                    }
                    else
                    {
                        model.CREATED = DateTime.Now;
                        model.CREATEDBY = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        model.LASTUPDATED = DateTime.Now;
                        model.LASTUPDATEDBY = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        requestBody = new HttpRequestMessage(HttpMethod.Post, "/api/REQUESTHDR");
                    }
                    var serializations = JsonConvert.SerializeObject(model);
                    requestBody.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("applications/json"));
                    requestBody.Content = new StringContent(serializations);
                    requestBody.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await client.SendAsync(requestBody);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<JObject>(content);
                        int requesthdrId = data["requesthdr"]["id"].Value<int>();
                        var successResponse = new { Message = data, StatusCode = 201, Hdrid = requesthdrId };
                        return Json(successResponse);
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        // Handle conflict (HTTP status code 409) by creating a custom JSON response
                        var conflictResponse = new { ErrorMessage = "Error", StatusCode = 409 };
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
            return Json(model);
        }
        public async Task<JsonResult> GetAllREQUESTHDR()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DPTClient");
                var response = await client.GetAsync($"/api/REQUESTHDR");
                var content = await response.Content.ReadAsStringAsync();
                var res = System.Text.Json.JsonSerializer.Deserialize<object>(content);
                return Json(res);
            }
            catch (System.Exception ex)
            {
                return Json(ex);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetRequestPendingForApproval()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var client = _httpClientFactory.CreateClient("DPTClient");
            var response = await client.GetAsync("/api/REQUESTHDR/GetPendingForApproval?userId=" + userId);
            var content = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<object>(content);

            return Json(data);
        }
        public async Task<JsonResult> PostRequestforpayment(List<PaymentRequestDetails> data)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DPTClient");
                var requestBody = new HttpRequestMessage();
                var jsonRequest = JsonConvert.SerializeObject(data);
                requestBody = new HttpRequestMessage(HttpMethod.Post, "/api/REQUESTHDR/PostRequestforpayment");
                requestBody.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("applications/json"));
                requestBody.Content = new StringContent(jsonRequest);
                requestBody.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.SendAsync(requestBody);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var listOfHdrids = JsonConvert.DeserializeObject<List<int>>(responseContent);
                    return Json("Created");
                }
                else
                {
                    return Json("Error occurred while saving");

                }
            }
            catch (System.Exception ex)
            {
                return Json(ex.Message);
            }
        }
        [HttpGet]
        public async Task<JsonResult> ApproveSelectedRequest(int id , string approved, string comments)
        {
            var client = _httpClientFactory.CreateClient("DPTClient");
            var UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var response = await client.GetAsync("/api/REQUESTHDR/UpdateWhenApproved?Id="+ id +"&approved=" + approved +"&comments=" + comments +"&UpdatedBy=" + UserId);
            var content = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<object>(content);
             
            return Json(data);
        }
    }
}
