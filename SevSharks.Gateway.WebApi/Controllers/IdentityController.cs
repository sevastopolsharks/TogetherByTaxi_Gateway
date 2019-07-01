using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SevSharks.Gateway.WebApi.Controllers
{
    /// <summary>
    /// Identity
    /// </summary>
    [Route("api/v1/identity")]
    [Produces("application/json")]
    [Authorize]
    public class IdentityController : Controller
    {
        /// <summary>
        /// Get user claims
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            var data = from c in User.Claims select new { c.Type, c.Value };
            return Ok(data);
        }
    }
}