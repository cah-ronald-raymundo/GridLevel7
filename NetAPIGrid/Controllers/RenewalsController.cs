using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetAPIGrid.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RenewalsController : ControllerBase
    {

        [HttpGet]
        public IEnumerable<string> GetData()
        {
            return new string[] { "value1", "value2" };
        }

    }
}
