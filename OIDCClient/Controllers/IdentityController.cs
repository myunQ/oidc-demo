using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


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
            //Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectParameterNames.AccessToken == IdentityModel.OidcConstants.TokenTypes.AccessToken

            string accessToken = await HttpContext.GetTokenAsync(IdentityModel.OidcConstants.TokenTypes.AccessToken);
            string idToken = await HttpContext.GetTokenAsync(IdentityModel.OidcConstants.TokenTypes.IdentityToken);
            string refreshToken = await HttpContext.GetTokenAsync(IdentityModel.OidcConstants.TokenTypes.RefreshToken);

            var authenticateResult = await HttpContext.AuthenticateAsync();

            return new JsonResult(new
            {
                User = User.Claims.GroupBy(o => o.Type, o => o.Value).ToDictionary(o => o.Key, o => o.Count() > 1 ? string.Join(",", o) : o.First()),
                IdToken = idToken,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AuthenticateResult_Properties_Parameters = authenticateResult.Properties.Parameters,
                AuthenticateResult_Properties_Items = authenticateResult.Properties.Items
            });
        }

        [HttpGet("api1")]
        public async Task<IActionResult> GetFromOIDCProtectedResources()
        {
            string accessToken = await HttpContext.GetTokenAsync(IdentityModel.OidcConstants.TokenTypes.AccessToken);

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

        [HttpGet("sign-out")]
        public IActionResult UserSignout()
        {
            /*
             * 这将清除本地cookie，然后重定向到IdentityServer。IdentityServer将清除其cookie，然后为用户提供一个链接以返回到本应用程序。
             * 如果要让 IdentityServer 在登出后自动跳转回本应用程序则需要修改 OIDCServer 项目下的 /Quickstart/Account/AccountOptions.cs 文件，
             * 将 AutomaticRedirectAfterSignOut 属性设置为 true。
             */
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }


        [HttpGet("sign-out-by-reference-token")]
        public async Task<IActionResult> UserSignoutByReferenceToken()
        {
            string accessToken = await HttpContext.GetTokenAsync(IdentityModel.OidcConstants.TokenTypes.AccessToken);
            if (string.IsNullOrEmpty(accessToken))
            {
                return Ok();
            }


            HttpClient client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://server.oidc.test:5000");

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var revocationResponse = await client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = disco.RevocationEndpoint,
                ClientId = "hybrid client",
                ClientSecret = "123",
                Token = accessToken
            });

            if (revocationResponse.IsError)
            {
                throw revocationResponse.Exception;
            }

            await HttpContext.SignOutAsync();

            return Ok();
        }



    }
}
