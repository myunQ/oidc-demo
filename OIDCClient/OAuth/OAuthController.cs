using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OIDCClient.OAuth
{
    [Route("oauth")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        [HttpPost("signin-oidc")]
        public async Task<IActionResult> Signin(
            [FromForm]string id_token,
            [FromForm]string access_token,
            [FromForm] string token_type,
            [FromForm]string scope,
            [FromForm]int expires_in,
            [FromForm] string state,
            [FromForm] string session_state)
        {
            Controllers.IdentityController.AccessToken = access_token;

            HttpClient client = new HttpClient();
            client.SetBearerToken(access_token);

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

        [HttpGet("signout-callback-oidc")]
        public IActionResult SignoutCallbackOIDC()
        {
            return Ok();
        }

    }
}
