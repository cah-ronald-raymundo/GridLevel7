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
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "encrypted/", requestModel.MyByteData.FileName);

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

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<ActionResult> DecryptFile2()
        //{
        //    string line;
        //    try
        //    {
        //        var encryptedPath = Path.Combine(_hostingEnvironment.WebRootPath, "encrypted/");
        //        var decryptedPath = Path.Combine(_hostingEnvironment.WebRootPath, "decrypted/");

        //        var txtFiles = Directory.EnumerateFiles(encryptedPath, "*.txt");

        //        if (txtFiles.Count() > 0)
        //        {
        //            foreach (string currentFile in txtFiles)
        //            {
        //                //var x = currentFile.Replace(encryptedPath," ");

        //                using (FileStream stream = new FileStream(decryptedPath + currentFile.Replace(encryptedPath, " "), FileMode.Create))
        //                {
        //                    var result = new StringBuilder();
        //                    using (var reader = new StreamReader(currentFile))
        //                    {
        //                        while (reader.Peek() >= 0)
        //                            result.AppendLine(_aes.Decrypt(reader.ReadLine()));
        //                    }

        //                    var encryptedText = result.ToString();
        //                    byte[] data = new UTF8Encoding(true).GetBytes(encryptedText);
        //                    await stream.WriteAsync(data, 0, data.Length);
        //                    stream.Close();

        //                }
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("No File to Decrypt.");
        //        }

        //        return Ok("File has been Decrypted!");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error. msg: {ex.Message}");
        //    }
        //}

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
    }
}
