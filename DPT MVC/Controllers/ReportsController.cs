using DPT.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
namespace DPT.MVC.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReportsController> _logger;
        public HttpClient HttpClient = new HttpClient();

        public ReportsController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ReportsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            //HttpClient = _httpClientFactory.CreateClient("DhlQatarClient");
            HttpClient.BaseAddress = new Uri(_configuration["MySetting:ApiURL"]);
        }

        public async Task<JsonResult> GetReportByUser(string? userId = "")
        {
            var response = await HttpClient.GetAsync("/api/reports/ByUser/" + (userId == "" ? HttpContext.Session.GetString("UserId") : userId));
            var content = await response.Content.ReadAsStringAsync();
            var res = System.Text.Json.JsonSerializer.Deserialize<object>(content);
            return Json(res);
        }


        [HttpDelete]
        public async Task<JsonResult> DeleteReportPermission(int id)
        {
            var response = await HttpClient.DeleteAsync("/api/excelpermissions?id=" + id);
            var content = await response.Content.ReadAsStringAsync();
            var res = System.Text.Json.JsonSerializer.Deserialize<object>(content);
            return Json(res);
        }

        public async Task<JsonResult> GetReports()
        {
            var response = await HttpClient.GetAsync("/api/reports");
            var content = await response.Content.ReadAsStringAsync();
            var res = System.Text.Json.JsonSerializer.Deserialize<object>(content);
            return Json(res);
        }

        public async Task<JsonResult> GetProcParams(int id)
        {
            var response = await HttpClient.GetAsync("/api/reports/ProcParams/" + id);
            var content = await response.Content.ReadAsStringAsync();
            var res = System.Text.Json.JsonSerializer.Deserialize<object>(content);
            return Json(res);
        }

        public async Task<JsonResult> GetStatusList(string paramName)
        {
            var response = await HttpClient.GetAsync("/api/reports/getstatuslist/" + paramName);
            var content = await response.Content.ReadAsStringAsync();
            var res = System.Text.Json.JsonSerializer.Deserialize<object>(content);
            return Json(res);
        }

        public async Task<JsonResult> Reports(int id)
        {
            try
            {
                var response = await HttpClient.GetAsync("/api/reports/" + id);
                var content = await response.Content.ReadAsStringAsync();
                var report = System.Text.Json.JsonSerializer.Deserialize<object>(content);
                return Json(report);
            }
            catch (Exception e)
            {
                return Json(new Models.Report());
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetReportData(Report model)
        {
            try
            {
                if (model != null)
                {
                    var requestBody = new HttpRequestMessage(HttpMethod.Post, "/api/reports/getreportdata");
                    var serializations = JsonConvert.SerializeObject(model);
                    requestBody.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    requestBody.Content = new StringContent(serializations);
                    requestBody.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await HttpClient.SendAsync(requestBody);
                    var content = await response.Content.ReadAsStringAsync();
                    var suppliers = System.Text.Json.JsonSerializer.Deserialize<object>(content);
                    return Json(suppliers);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(true);
        }

        public async Task<JsonResult> GetReportData1(string model)
        {
            try
            {
                if (model != null)
                {
                    var requestBody = new HttpRequestMessage(HttpMethod.Post, "/api/reports/getreportdata");
                    var serializations = JsonConvert.SerializeObject(model);
                    requestBody.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    requestBody.Content = new StringContent(model);
                    requestBody.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await HttpClient.SendAsync(requestBody);
                    var content = await response.Content.ReadAsStringAsync();
                    var suppliers = System.Text.Json.JsonSerializer.Deserialize<object>(content);
                    return Json(suppliers);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(true);
        }


    }
}
