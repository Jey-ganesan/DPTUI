using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DPT.MVC.Models;

namespace DPT.MVC.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomersController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public HttpClient HttpClient = new HttpClient();

        public CustomersController(IConfiguration configuration, ILogger<CustomersController> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            //HttpClient = _httpClientFactory.CreateClient("DhlQatarClient");
            HttpClient.BaseAddress = new Uri(_configuration["MySetting:ApiURL"]);
        }

        public async Task<JsonResult> GetCustomers()
        {
            try
            {
                var response = await HttpClient.GetAsync("/api/customers");
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
                return Json(new Models.Customer());
            }
        }

        public async Task<JsonResult> GetCustomersById(int id)
        {
            try
            {
                var response = await HttpClient.GetAsync("/api/customers/" + id);
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
                return Json(new Models.Customer());
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveCustomer(Models.Customer model)
        {
            try
            {
                if (model != null)
                {

                    var requestBody = new HttpRequestMessage();
                    if (model.Id == 0)
                    {
                        model.Id = null;
                    }
                    if (model.Id != 0 && model.Id != null)
                    {

                        model.LastUpdated = DateTime.Now;
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        requestBody = new HttpRequestMessage(HttpMethod.Patch, "/api/customers");
                    }
                    else
                    {
                        model.Created = DateTime.Now;
                        model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        model.LastUpdated = DateTime.Now;
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        requestBody = new HttpRequestMessage(HttpMethod.Post, "/api/customers");
                    }
                    var serializations = JsonConvert.SerializeObject(model);
                    requestBody.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    requestBody.Content = new StringContent(serializations);
                    requestBody.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await HttpClient.SendAsync(requestBody);


                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var successResponse = new { Message = content, StatusCode = 201 };
                        return Json(successResponse);
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        // Handle conflict (HTTP status code 409) by creating a custom JSON response
                        var conflictResponse = new { ErrorMessage = "Customer with same short name already exists.", StatusCode = 409 };
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

        [HttpDelete]
        public async Task<JsonResult> DeleteCustomer(int id)
        {
            try
            {
                var response = await HttpClient.DeleteAsync("/api/customers/" + id);
                var content = await response.Content.ReadAsStringAsync();
                var customersDetails = System.Text.Json.JsonSerializer.Deserialize<object>(content);
                return Json(customersDetails);
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
                return Json(new Models.Customer());
            }
        }

    }
}
