using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.SharePoint.ApplicationPages;
using Microsoft.SharePoint.Client;
using System;
using System.IO;
using System.Text;

namespace NetAPIGrid.Service
{
    public class MyJobService: ControllerBase
    {
        private readonly HttpClient _httpClient;
        private IWebHostEnvironment _hostingEnvironment;
        private readonly AES _aes;
        public MyJobService(HttpClient httpClient, IWebHostEnvironment hostingEnvironment, AES aes)
        {
            _httpClient = httpClient;
            _hostingEnvironment = hostingEnvironment;
            _aes = aes;
        }
        public async Task<IActionResult> RunInsertLogsEndPoint()
        {
            string line;
            string dayName = DateTime.Now.ToString("dddd");

            await DecryptFile();

            try
            {
                await using (SqlConnection con = new SqlConnection(@"Data Source=10.51.35.174;Initial Catalog=GRID_LVL_SEVEN;User Id=SA-RPA.POWERAPPS;Password=sa.rpa.Powerapps;TrustServerCertificate=True;"))
                {
                    con.Open();

                    var ePath = Path.Combine(_hostingEnvironment.WebRootPath, "encrypted/");
                    var dPath = Path.Combine(_hostingEnvironment.WebRootPath, "decrypted/");
                    var cPath = Path.Combine(_hostingEnvironment.WebRootPath, "completed/");

                    var txtFiles = Directory.EnumerateFiles(dPath, "*.txt");

                    if (txtFiles.Count() > 0)
                    {
                        //Console.WriteLine("Testing");
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

                        if (Directory.Exists(ePath))
                        {
                            foreach (var file in new DirectoryInfo(ePath).GetFiles())
                            {
                                //file.MoveTo($@"{cPath}\{file.Name}");
                                file.CopyTo($@"{cPath}\{file.Name}");
                                file.Delete();
                            }
                        }

                        if (Directory.Exists(dPath))
                        {
                            foreach (var file in new DirectoryInfo(dPath).GetFiles())
                            {
                                file.Delete();
                            }
                        }

                        //if (dayName == "Sunday")
                        //{
                        //    if (Directory.Exists(cPath))
                        //    {
                        //        foreach (var file in new DirectoryInfo(cPath).GetFiles())
                        //        {
                        //            file.Delete();
                        //        }
                        //    }
                        //}

                    }
                    else
                    {
                        Console.WriteLine("No Logs to Insert.");
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

        public async Task<IActionResult> DecryptFile()
        {
            string line;
            try
            {
                var encryptedPath = Path.Combine(_hostingEnvironment.WebRootPath, "encrypted/");
                var decryptedPath = Path.Combine(_hostingEnvironment.WebRootPath, "decrypted/");

                var txtFiles = Directory.EnumerateFiles(encryptedPath, "*.txt");

                if (txtFiles.Count() > 0)
                {
                    foreach (string currentFile in txtFiles)
                    {
                        //var x = currentFile.Replace(encryptedPath," ");

                        using (FileStream stream = new FileStream(decryptedPath + currentFile.Replace(encryptedPath, " "), FileMode.Create))
                        {
                            var result = new StringBuilder();
                            using (var reader = new StreamReader(currentFile))
                            {
                                while (reader.Peek() >= 0)
                                    result.AppendLine(_aes.Decrypt(reader.ReadLine()));
                            }

                            var encryptedText = result.ToString();
                            byte[] data = new UTF8Encoding(true).GetBytes(encryptedText);
                            await stream.WriteAsync(data, 0, data.Length);
                            stream.Close();
                            stream.Dispose();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No File to Decrypt.");
                }

                return Ok("File has been Decrypted!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error. msg: {ex.Message}");
            }
        }

        //public IActionResult DeleteFiles()
        //{
        //    var ePath = Path.Combine(_hostingEnvironment.WebRootPath, "encrypted/");
        //    var dPath = Path.Combine(_hostingEnvironment.WebRootPath, "decrypted/");

        //    try
        //    {
        //        if (Directory.Exists(dPath))
        //        {
        //            foreach (var file in new DirectoryInfo(dPath).GetFiles())
        //            {
        //                file.Delete();
        //            }
        //        }

        //        if (Directory.Exists(ePath))
        //        {
        //            foreach (var file in new DirectoryInfo(ePath).GetFiles())
        //            {
        //                file.Delete();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Write(ex.ToString());
        //    }

        //    return Ok("Files has been removed");
        //}
    }
}
