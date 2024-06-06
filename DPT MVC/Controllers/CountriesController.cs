using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using ExcelDataReader.Log;
using Microsoft.Extensions.Logging;
using DPT.MVC.Models;

namespace ARAP.Controllers
{
    public class CountriesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CountriesController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public HttpClient HttpClient = new HttpClient();
        string connection = string.Empty;


        public CountriesController(IConfiguration configuration, ILogger<CountriesController> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            //HttpClient = _httpClientFactory.CreateClient("DhlQatarClient");
            HttpClient.BaseAddress = new Uri(_configuration["MySetting:ApiURL"]);
            connection = _configuration.GetConnectionString("DPT.ConnectionString");

        }

        [HttpPost]
        public async Task<JsonResult> SaveCountry(Country model)
        {
            try
            {
                if (model != null)
                {
                    var requestBody = new HttpRequestMessage();
                    if (model.Id != 0)
                    {
                        model.LastUpdated = DateTime.Now;
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        model.LastUpdatedByName = HttpContext.Session.GetString("DisplayName");
                        requestBody = new HttpRequestMessage(HttpMethod.Patch, "/api/countries");
                    }
                    else
                    {

                        model.Created = DateTime.Now;
                        model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        model.CreatedByName = HttpContext.Session.GetString("DisplayName");
                        model.LastUpdated = DateTime.Now;
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                        model.LastUpdatedByName = HttpContext.Session.GetString("DisplayName");
                        requestBody = new HttpRequestMessage(HttpMethod.Post, "/api/countries");
                    }
                    var serializations = JsonConvert.SerializeObject(model);
                    requestBody.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("applications/json"));
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
        public async Task<JsonResult> GetCountries()
        {
            try
            {
                var response = await HttpClient.GetAsync("/api/countries");
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
        public async Task<JsonResult> GetCountryById(int id)
        {
            try
            {
                var response = await HttpClient.GetAsync("/api/countries/" + id);
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
        [HttpDelete]
        public async Task<JsonResult> DeleteCountry(int id)
        {
            try
            {
                var response = await HttpClient.DeleteAsync("/api/countries/" + id);
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

    }
}
