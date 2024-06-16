using DPT.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;

namespace DPT.MVC.Controllers
{
    public class PAYMENTRECEIPTCHARGESCONTROLLER : Controller
    {
        private readonly ILogger<PAYMENTRECEIPTCHARGESCONTROLLER> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public PAYMENTRECEIPTCHARGESCONTROLLER(ILogger<PAYMENTRECEIPTCHARGESCONTROLLER> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        [HttpPost]
        public async Task<ActionResult> SavePAYMENTRECEIPTCHARGES(PAYMENTRECEIPTCHARGES model)
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
                        requestBody = new HttpRequestMessage(HttpMethod.Patch, "/api/PAYMENTRECEIPTCHARGES");

                    }
                    else
                    {
                        model.CREATED = DateTime.Now;
                        model.CREATEDBY = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        model.LASTUPDATED = DateTime.Now;
                        model.LASTUPDATEDBY = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        requestBody = new HttpRequestMessage(HttpMethod.Post, "/api/PAYMENTRECEIPTCHARGES");
                    }
                    var serializations = JsonConvert.SerializeObject(model);
                    requestBody.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("applications/json"));
                    requestBody.Content = new StringContent(serializations);
                    requestBody.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await client.SendAsync(requestBody);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var data = System.Text.Json.JsonSerializer.Deserialize<object>(content);
                        var successResponse = new { Message = data, StatusCode = 201 };
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
        public async Task<JsonResult> GetAllPAYMENTRECEIPTCHARGES()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DPTClient");
                var response = await client.GetAsync($"/api/PAYMENTRECEIPTS");
                var content = await response.Content.ReadAsStringAsync();
                var res = System.Text.Json.JsonSerializer.Deserialize<object>(content);
                return Json(res);
            }
            catch (System.Exception ex)
            {
                return Json(ex);
            }
        }
    }
}
