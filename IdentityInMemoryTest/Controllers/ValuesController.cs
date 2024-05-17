using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityInMemoryTest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("GetData")]
        public List<int> GetData()
        {
            return new List<int> { 1, 2, 3, 4, 5, 6 };
        }
    }
}
