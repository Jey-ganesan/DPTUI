using DPT_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security;

namespace DPT.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _httpClientFactory;
        public HttpClient HttpClient = new HttpClient();
        public static string DashboardType = string.Empty;  
        string connection = string.Empty;

        public HomeController(ILogger<HomeController> logger   ,  IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;
            _configuration = configuration;
            HttpClient.BaseAddress = new Uri(_configuration["MySetting:ApiURL"]);
            connection = _configuration.GetConnectionString("DPT.ConnectionString");
        }

        [HttpPost]
        public async Task<IActionResult> Login(Users model)
        {
            try
            {
                if (model != null)
                {
                    var serializations = JsonConvert.SerializeObject(model);
                    var requestBody = new HttpRequestMessage();
                    requestBody = new HttpRequestMessage(HttpMethod.Post, "/api/token");
                    requestBody.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("applications/json"));
                    requestBody.Content = new StringContent(serializations);
                    requestBody.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var reponse = await HttpClient.SendAsync(requestBody);
                    if (reponse.IsSuccessStatusCode)
                    {
                        var token = await reponse.Content.ReadAsStringAsync();

                        var response = await HttpClient.GetAsync("/api/users/getbyemail/" + model.email);
                        var content = await response.Content.ReadAsStringAsync();
                        var userDetails = System.Text.Json.JsonSerializer.Deserialize<Users>(content);
                        //DashboardType = userDetails.dashboardType;

                        HttpContext.Session.SetString("Token", token);
                        HttpContext.Session.SetString("UserInfo", content);
                        HttpContext.Session.SetString("UserId", userDetails.id.ToString());
                        HttpContext.Session.SetString("Email", model.email);
                   //     HttpContext.Session.SetString("TransPre", userDetails.transPrefix);
                        HttpContext.Session.SetString("DisplayName", userDetails.displayName);
                        HttpContext.Session.SetString("UserRole", userDetails.userRole);
                        //ViewBag.ReportData = GetReport();
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Index");

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
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                {
                    return View("Login");
                }

                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                return View();


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
                throw;
            }
        }


        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
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
                throw;
            }
            return RedirectToAction("Index");
        }

    }
}
