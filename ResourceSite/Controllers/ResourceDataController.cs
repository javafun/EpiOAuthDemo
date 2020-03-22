using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace ResourceSite.Controllers
{
    public class ResourceDataController : ApiController
    {
        [Authorize]
        public IHttpActionResult Get()
        {
            var identity = User.Identity as ClaimsIdentity;
            var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
            return Ok(claims);
        }
    }
}
