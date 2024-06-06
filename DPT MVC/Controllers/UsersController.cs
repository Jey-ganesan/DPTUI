using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using DPT.MVC.Models;

namespace DPT.MVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public UsersController(ILogger<UsersController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<JsonResult> GetUserById(int id)
        {
            var client = _httpClientFactory.CreateClient("DPTClient");
            var response = await client.GetAsync("/api/users/" + id);
            var content = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<object>(content);
            return Json(data);
        }

        [HttpPost]
        public async Task<JsonResult> SaveUser(Users model)
        {
            var client = _httpClientFactory.CreateClient("DPTClient");
            try
            {
                if (model != null)
                {
                    var requestBody = new HttpRequestMessage();
                    if (model.id != 0)
                    {

                        //model.LastUpdated = DateTime.Now;
                        //model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        requestBody = new HttpRequestMessage(HttpMethod.Patch, "/api/users");
                    }
                    else
                    {
                        //model.Created = DateTime.Now;
                        //model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        //model.LastUpdated = DateTime.Now;
                        //model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        requestBody = new HttpRequestMessage(HttpMethod.Post, "/api/users");
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
        public async Task<JsonResult> CheckUser(int id, string Email)
        {
            var client = _httpClientFactory.CreateClient("DPTClient");
            var response = await client.GetAsync($"/api/users/CheckUser?id={id}&Email={Email}");
            var content = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<bool>(content);

            return Json(data);
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteUser(int id)
        {
            var client = _httpClientFactory.CreateClient("DPTClient");
            var response = await client.GetAsync("/api/users/" + id);
            var content = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<bool>(content);

            return Json(data);
        }
    }
}
