using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Office.Server.Auditing;
using Microsoft.SharePoint.Client;
using NetAPIGrid.Models;
using NetAPIGrid.Service;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using static System.Net.WebRequestMethods;
using Hangfire;
using NetAPIGrid.jobs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetAPIGrid.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GridController : ControllerBase
    {

        private IWebHostEnvironment _hostingEnvironment;
        private readonly JwtService _jwtService;
        private readonly AES _aes;

        public GridController(IWebHostEnvironment environment, JwtService jwtService, AES aes)
        {
            _hostingEnvironment = environment;
            _jwtService = jwtService;
            _aes = aes;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UploadFile([FromForm] APIRequestModel requestModel)
        {
            try
            {
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/", requestModel.MyByteData.FileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await requestModel.MyByteData.CopyToAsync(stream);
                    stream.Close();
                }
                return Ok("File uploaded successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error. msg: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> EncryptFile([FromForm] APIRequestModel requestModel)
        {
            try
            {
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "encrypted/", requestModel.MyByteData.FileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    var result = new StringBuilder();
                    using (var reader = new StreamReader(requestModel.MyByteData.OpenReadStream()))
                    {
                        while (reader.Peek() >= 0)
                            result.AppendLine(reader.ReadLine());
                    }

                    var encryptedText = _aes.Encrypt(result.ToString());
                    byte[] data = new UTF8Encoding(true).GetBytes(encryptedText);
                    await stream.WriteAsync(data, 0, data.Length);
                    stream.Close();

                }
                return Ok("File encrypted successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error. msg: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> DecryptFile([FromForm] APIRequestModel requestModel)
        {
            try
            {
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "decrypted/", requestModel.MyByteData.FileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    var result = new StringBuilder();
                    using (var reader = new StreamReader(requestModel.MyByteData.OpenReadStream()))
                    {
                        while (reader.Peek() >= 0)
                            result.AppendLine(reader.ReadLine());
                    }

                    var encryptedText = _aes.Decrypt(result.ToString());
                    byte[] data = new UTF8Encoding(true).GetBytes(encryptedText);
                    await stream.WriteAsync(data, 0, data.Length);
                    stream.Close();

                }
                return Ok("File decrypted successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error. msg: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public  async Task<ActionResult<UserRequest>> GenerateToken()
        {
            var login = new UserRequest { Username = "gridadmin" };
            var result = await _jwtService.Authenticate(login);
            return Ok(result);
        }


        public class APIRequestModel
        {
            public string FilePath { get; set; }
            public IFormFile MyByteData { get; set; }
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> InsertLogs()
        {
            string line;

            try
            {
               await using (SqlConnection con = new SqlConnection(@"Data Source=WSEC5009GRDSQ01;Initial Catalog=GRID_LVL_SEVEN;Trusted_Connection=True;Encrypt=False"))
                {
                    con.Open();

                    var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/");
                    var completedPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/completed/");

                    var txtFiles = Directory.EnumerateFiles(uploadPath, "*.txt");
                    foreach (string currentFile in txtFiles)
                    {
                        using (StreamReader file = new StreamReader(currentFile))
                        {
                            while ((line = file.ReadLine()) != null)
                            {
                                string[] fields = line.Split("|||");

                                SqlCommand cmd = new SqlCommand("INSERT INTO tblActivityLogs(EID, EventType, TimeStamp,Details) VALUES (@eid, @eventtype, @timestamp,@details)", con);
                                cmd.Parameters.AddWithValue("@eid", fields[3].ToString());
                                cmd.Parameters.AddWithValue("@eventtype", fields[1].ToString());
                                cmd.Parameters.AddWithValue("@timestamp", fields[0].ToString());
                                cmd.Parameters.AddWithValue("@details", fields[2].ToString());
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    if (Directory.Exists(uploadPath))
                    {
                        foreach (var file in new DirectoryInfo(uploadPath).GetFiles())
                        {
                            file.MoveTo($@"{completedPath}\{file.Name}");
                        }
                    }

                    con.Close();
                }

                return Ok("Log has been inserted!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error. msg: {ex.Message}");
            }

        }

        //[HttpPost]
        //public ActionResult CreateBackgroundJob()
        //{
        //    //BackgroundJob.Enqueue(() => Console.WriteLine("Background Job Triggered"));
        //    BackgroundJob.Enqueue<TestJob>(x => x.WriteLog("Background  Job Triggered"));
        //    return Ok();
        //}

        //[HttpPost]
        //public ActionResult CreateScheduledJob()
        //{
        //    var scheduleTime = DateTime.UtcNow.AddSeconds(5);
        //    var dateTimeOffset = new DateTimeOffset(scheduleTime);
        //    //BackgroundJob.Schedule(() => Console.WriteLine("Scheduled Job Trigged"), dateTimeOffset);
        //    BackgroundJob.Schedule<TestJob>(x => x.WriteLog("Background  Job Triggered"),dateTimeOffset);
        //    return Ok();
        //}

        //[HttpPost]
        //public ActionResult CreateRecurringJob()
        //{
        //    //RecurringJob.AddOrUpdate("RecurringJob1",() => Console.WriteLine("Recurring Job Triggered"),"* * * * *");
        //    RecurringJob.AddOrUpdate<TestJob>("RecurringJob1", x => x.WriteLog("Recurring Job Triggered"), "* * * * *");
        //    return Ok();
        //}

    }
}
