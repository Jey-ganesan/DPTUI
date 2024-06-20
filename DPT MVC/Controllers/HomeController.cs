using DPT.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace DPT.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _httpClientFactory;
        public HttpClient HttpClient = new HttpClient();
        public static string DashboardType = string.Empty;
        public static List<MenuInfo> menus = new List<MenuInfo>();


        public HomeController(ILogger<HomeController> logger, IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;
            _configuration = configuration;
            HttpClient.BaseAddress = new Uri(_configuration["MySetting:ApiURL"]);
        }

        [HttpPost]
        public async Task<IActionResult> Login(Users model)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DPTClient");

                if (model != null)
                {
                    var serializations = JsonConvert.SerializeObject(model);
                    var requestBody = new HttpRequestMessage();
                    requestBody = new HttpRequestMessage(HttpMethod.Post, "/api/token");
                    requestBody.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("applications/json"));
                    requestBody.Content = new StringContent(serializations);
                    requestBody.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var reponse = await client.SendAsync(requestBody);
                    if (reponse.IsSuccessStatusCode)
                    {
                        var token = await reponse.Content.ReadAsStringAsync();

                        var response = await client.GetAsync("/api/users/getbyemail/" + model.email);
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
                var client = _httpClientFactory.CreateClient("DPTClient");

                var token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                {
                    return View("Login");
                }
                //var menuIds = permission.Select(x => x.menuId).ToList();
                var response = await client.GetAsync("/api/menus");
                var content = await response.Content.ReadAsStringAsync();

                // Deserialize the response content into the list of MenuInfo objects
                menus = System.Text.Json.JsonSerializer.Deserialize<List<MenuInfo>>(content).Where(x => x.isActive).ToList();

                // Get distinct parent IDs from the active menus
                var parentIds = menus.Select(menu => menu.parentslno ?? 0).Distinct().ToList();

                // Filter menus to include only those that are parents or whose parent IDs are in the list of parent IDs
                menus = menus.Where(menu => parentIds.Contains(menu.slno) || menu.parentslno.HasValue && parentIds.Contains(menu.parentslno.Value)).ToList();

                // Separate the parent and child menus for the ViewBag
                ViewBag.ParentMenu = menus.Where(x => x.parentslno == 0).ToList();
                ViewBag.ChildMenu = menus.Where(x => x.parentslno != 0).ToList();

                // Pass the entire menus list to the ViewBag
                ViewBag.Menus = menus.ToList();

                // Return the view
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

        public ActionResult LoadPage(string pageName, int? id = 0, string mode = "", int? pageId = 0)

        {

            try

            {
                var client = _httpClientFactory.CreateClient("DPTClient");

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
        public async Task<JsonResult> GetGridData(int id, string from, string to, string status, string param1)
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
                var response = await client.GetAsync("/api/masters/gridmaster?id=" + id + "&param1=" + param1);
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
