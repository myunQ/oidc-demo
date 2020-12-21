using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
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
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string accessToken = await HttpContext.GetTokenAsync(IdentityModel.OidcConstants.TokenTypes.AccessToken);
            string idToken = await HttpContext.GetTokenAsync(IdentityModel.OidcConstants.TokenTypes.IdentityToken);
            string refreshToken = await HttpContext.GetTokenAsync(IdentityModel.OidcConstants.TokenTypes.RefreshToken);

            HttpClient client = new HttpClient();
            client.SetBearerToken(accessToken);

            var response = await client.GetAsync("https://api.oidc.test:5100/api/identity");
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
