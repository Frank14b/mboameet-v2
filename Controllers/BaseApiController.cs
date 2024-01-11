using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] //api/routes for users
    [Route("api/v1/admin/[controller]")] //api/routes for admin

    public class BaseApiController : ControllerBase
    {
        public BaseApiController()
        {

            // this._context = context;
        }
    }
}