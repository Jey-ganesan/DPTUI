using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using DPT.MVC.Models;
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
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;
            _configuration = configuration;
            HttpClient.BaseAddress = new Uri(_configuration["MySetting:ApiURL"]);
            connection = _configuration.GetConnectionString("DPT.ConnectionString");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ActionResult LoadPage(string pageName, int? id = 0, string mode = "", int? pageId = 0)
        {
            try
            {
                //var token = HttpContext.Session.GetString("Token");

                //if (string.IsNullOrEmpty(token))
                //{
                //    return PartialView(false);
                //}
                //if (pageId != 0 && pageId != null)
                //{
                //    var tmp = permission.Where(p => p.menuId == pageId).FirstOrDefault();
                //    ViewBag.access = new Access();
                //    if (tmp != null)
                //    {
                //        ViewBag.access = new Access()
                //        {
                //            add = tmp.add,
                //            edit = tmp.edit,
                //            view = tmp.view,
                //            delete = tmp.delete
                //        };
                //    }
                //}
                //else
                //{
                ViewBag.access = new Access();
                //}

                ViewBag.paramID = id;
                ViewBag.url = _configuration["MySetting:ApiURL"];
                ViewBag.mode = mode;
                ViewBag.pageId = pageId;
                //var username = HttpContext.Session.GetString("DisplayName");
                //ViewBag.userRole = HttpContext.Session.GetString("UserRole");
                //ViewBag.username = username;

                return PartialView(pageName);
            }
            catch (Exception e)
            {
                var displayName = HttpContext.Session.GetString("DisplayName");

                _logger.LogError(e, "An error occurred in {ControllerName}/{ActionName} for {DisplayName} Exception is {message}.", ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName, displayName, e.Message);
                throw;
            }
        }

        public async Task<JsonResult> GetGridData(int id, string from, string to, string status)
        {
            try
            {
                string param = string.Empty;
                if (!string.IsNullOrEmpty(from))
                {
                    param += $"&from={Uri.EscapeDataString(from)}";
                }
                if (!string.IsNullOrEmpty(to))
                {
                    param += $"&to={Uri.EscapeDataString(to)}";
                }
                if (!string.IsNullOrEmpty(status))
                {
                    param += $"&status={Uri.EscapeDataString(status)}";
                }
                var client = _httpClientFactory.CreateClient("DPTClient");
                var response = await client.GetAsync("/api/masters/gridmaster?id=" + id);
                var content = await response.Content.ReadAsStringAsync();
                var res = System.Text.Json.JsonSerializer.Deserialize<List<object>>(content);
                return Json(res);
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
                return Json(false);
            }
        }

        
    }
}
