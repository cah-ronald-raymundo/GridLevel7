using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetAPIGrid.Models;
using YamlDotNet.Core.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetAPIGrid.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RenewalsController : ControllerBase
    {

        private readonly RenewalsDBContext _db;

        public RenewalsController(RenewalsDBContext db)
        {
            _db = db;

        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<VwUser>>> Users()
        {
            try
            {
                var result = await _db.VwUsers.ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {

              return BadRequest(ex.Message);
            }
    
        }

    }
}
