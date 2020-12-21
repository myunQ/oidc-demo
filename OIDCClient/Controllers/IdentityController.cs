using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using IdentityModel.Client;

namespace OIDCClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            HttpClient client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("https://server.oidc.test:5000");

            /* 用于 http 协议
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "http://server.oidc.test:5000",
                Policy = new DiscoveryPolicy
                {
                    RequireHttps = false
                }
            });
            */

            if (disco.IsError)
            {
                return StatusCode(501, disco.Error);
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client credentials client",
                ClientSecret = "123"
            });

            if (tokenResponse.IsError)
            {
                return StatusCode(501, tokenResponse.Error);
            }

            Console.WriteLine(tokenResponse.Json);

            client.SetBearerToken(tokenResponse.AccessToken);

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
