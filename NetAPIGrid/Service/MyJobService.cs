using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace NetAPIGrid.Service
{
    public class MyJobService: ControllerBase
    {
        private readonly HttpClient _httpClient;
        private IWebHostEnvironment _hostingEnvironment;
        public MyJobService(HttpClient httpClient, IWebHostEnvironment hostingEnvironment)
        {
            _httpClient = httpClient;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> RunInsertLogsEndPoint()
        {
            //Console.WriteLine("Insert Logs Successfully");
            string line;

            try
            {
                await using (SqlConnection con = new SqlConnection(@"Data Source=WSEC5009GRDSQ01;Initial Catalog=GRID_LVL_SEVEN;Trusted_Connection=True;Encrypt=False"))
                {
                    con.Open();

                    var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/");
                    var completedPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/completed/");

                    var txtFiles = Directory.EnumerateFiles(uploadPath, "*.txt");
                    
                    if (txtFiles.Count() > 0)
                    {
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
                                //file.MoveTo($@"{completedPath}\{file.Name}");
                                file.Delete();
                            }
                        }
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
    }
}
