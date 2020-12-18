using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using IdentityModel.Client;

namespace OIDCClient.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        public static string AccessToken { get; set; }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            HttpClient client = new HttpClient();
            client.SetBearerToken(AccessToken);

            var response = await client.GetAsync("http://api.test:5100/api/identity");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }
            else
            {   
                var content = await response.Content.ReadAsStringAsync();

                return Content(content, "text/json");
            }

        }
    }
}
