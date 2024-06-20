using DPT.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Primitives;

namespace DPT.MVC.Controllers
{
    public class FilesController : Controller
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _httpClientFactory;
        public HttpClient HttpClient = new HttpClient();
        public static string DashboardType = string.Empty;
        string connection = string.Empty;
        private readonly FileExtensionContentTypeProvider _FileExtentionContentProvider;
        public static List<MenuInfo> menus = new List<MenuInfo>();


        public FilesController(ILogger<FilesController> logger, IConfiguration configuration, FileExtensionContentTypeProvider fileExtensionContentProvider,IHttpClientFactory httpClientFactory)
        {
            _FileExtentionContentProvider = fileExtensionContentProvider;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;
            _configuration = configuration;
            HttpClient.BaseAddress = new Uri(_configuration["MySetting:ApiURL"]);
            connection = _configuration.GetConnectionString("DPT.ConnectionString");
        }

        [HttpPost]
        public async Task<IActionResult> SaveFile([FromForm] IFormFile file, string typeId, string transName)
        {
            try
            {
                if (Request.Headers.TryGetValue("ProdId", out Microsoft.Extensions.Primitives.StringValues header))
                {
                    if (StringValues.IsNullOrEmpty(header))
                    {
                        return BadRequest("File doesn't have Id.");
                    }
                    string Id = header[0];

                    if (file == null || file.Length == 0)
                    {
                        return BadRequest("No file was uploaded.");
                    }

                    var fileName = Path.GetFileName(file.FileName);
                    // Get the current date and time as a formatted string
                    string dateTimeString = DateTime.Now.ToString("yyyyMMddHHmmss");

                    // Append the formatted date and time to the original filename

                    //Below Commented because of we need original file name for this column without any timestemp prefix
                    //fileName = $"{dateTimeString}_{fileName}";

                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "PDF_Files");
                    var filePath = Path.Combine(folderPath, fileName);


                    using (var formData = new MultipartFormDataContent())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            var fileContent = new ByteArrayContent(memoryStream.ToArray());
                            formData.Add(fileContent, "file", file.FileName);
                            formData.Add(new StringContent(transName), "TransName");
                            formData.Add(new StringContent(Id), "TransId");
                            formData.Add(new StringContent(fileName), "FileName");
                            formData.Add(new StringContent(filePath), "FolderName");
                            formData.Add(new StringContent(typeId), "AttachmentTypeSlNo");
                            formData.Add(new StringContent((file.Length / 1024).ToString()), "SizeInKB");
                            var response = await HttpClient.PostAsync("api/fileattachment/upload", formData);
                            var content = await response.Content.ReadAsStringAsync();
                            return Ok(System.Text.Json.JsonSerializer.Deserialize<int>(content));
                        }
                    }

                }

                return BadRequest("ProdId header missing.");
            }
            catch (Exception e)
            {
                // Handle exceptions here
                // Get the controller and action names
                // Get the controller and action names
                var controllerName = ControllerContext.ActionDescriptor.ControllerName;
                var actionName = ControllerContext.ActionDescriptor.ActionName;

                // Get the DisplayName from the session
                var displayName = HttpContext.Session.GetString("DisplayName");

                // Log the error along with the controller, action, and DisplayName
                //  _logger.LogError(e, "An error occurred in {ControllerName}/{ActionName} for {DisplayName}.", controllerName, actionName, displayName);
                throw;
            }
        }

        public async Task<ActionResult> DownloadFile(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DPTClient");
                //var httpClient = _httpClientFactory.CreateClient("DhlQatarClient");
                var request = new HttpRequestMessage(HttpMethod.Post, "/api/fileattachments/download/" + id);
                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode)
                {
                    var fileData = await response.Content.ReadAsByteArrayAsync();
                    var contentType = response.Content.Headers.ContentType.ToString();
                    return File(fileData, contentType, "");
                }
                else
                {
                    return StatusCode(500);
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
                //  _logger.LogError(e, "An error occurred in {ControllerName}/{ActionName} for {DisplayName}.", controllerName, actionName, displayName);
                return Json("");
            }
        }

        public async Task<JsonResult> GetFilesPath(int Id, string transName)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DPTClient");
                if (Id == null)
                {
                    return null;
                }
                var transname = transName;
                if (transName == null)
                {
                    transName = "Asset Photo";
                }
                List<FileObject> fileObjects = new List<FileObject>();
                var response = await client.GetAsync("/api/fileattachments/bytrans/" + transname + "/" + Id);
                var content = await response.Content.ReadAsStringAsync();
                var invoicesDetails = System.Text.Json.JsonSerializer.Deserialize<List<FilesInfo>>(content);
                var newlist = invoicesDetails.Select(id => id.folderName).ToList();



                foreach (var folderName in newlist)
                {
                    if (!_FileExtentionContentProvider.TryGetContentType(folderName, out var ContentType))
                    {
                        ContentType = "application/octet-stream";
                    }
                    if (System.IO.File.Exists(folderName))
                    {
                        var bytes = System.IO.File.ReadAllBytes(folderName);
                        string base64String = Convert.ToBase64String(bytes);
                        string originalformat = "data:" + ContentType + ";base64," + base64String;
                        FileObject fileObject = new FileObject
                        {
                            ContentType = ContentType,
                            FileName = Path.GetFileName(folderName),
                            Base64Content = originalformat
                        };
                        fileObjects.Add(fileObject);
                    }
                }
                return Json(invoicesDetails);
            }

            catch (Exception e)
            { // Get the controller and action names
                var controllerName = ControllerContext.ActionDescriptor.ControllerName;
                var actionName = ControllerContext.ActionDescriptor.ActionName;

                // Get the DisplayName from the session
                var displayName = HttpContext.Session.GetString("DisplayName");

                // Log the error along with the controller, action, and DisplayName
                // _logger.LogError(e, "An error occurred in {ControllerName}/{ActionName} for {DisplayName}.", controllerName, actionName, displayName);
                return Json("");
            }
        }

        public async Task<ActionResult> CheckFileExists(string fileName)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DPTClient");
                var response = await client.GetAsync("api/FileAttachment/CheckFileExists?fileName=" + fileName);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    return File(content, response.Content.Headers.ContentType.ToString(), "");
                }
                return NotFound();
            }
            catch (System.Exception ex)
            {
                return Json(ex);
            }
        }



    }
}
