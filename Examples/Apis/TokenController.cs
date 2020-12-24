using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Examples.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        [HttpGet("[action]")]
        public IActionResult BuildJwtToken()
        {
            var ssKey = new SymmetricSecurityKey(Startup.key256);

            var credentials = new SigningCredentials(ssKey, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: Startup.Issuer,
                audience: Startup.Audience,
                claims: new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, "bob On jwt"),
                    new Claim(ClaimTypes.Email, "bob@example.test")
                },
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(10D),
                signingCredentials: credentials);

            string result = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return Content(result);
        }

        /// <summary>
        /// 验证 /api/token/buildJwtToken 签发的 JWT 是否有效。
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        //[Authorize] //使用 services.AddAuthentication(defaultScheme) 设定的默认认证方案进行验证。
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] //只使用 JwtBearerDefaults.AuthenticationScheme 认证方案进行验证
        //[Authorize(AuthenticationSchemes = "Bearer,Cookies")] //使用 JwtBearerDefaults.AuthenticationScheme 或 CookieAuthenticationDefaults.AuthenticationScheme 认证方案进行验证
        public IActionResult ValidateJwtToekn()
        {
            return Ok($"Hello {User.Identity.Name}. now : {DateTime.UtcNow.Ticks}");
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> BuildCookiesToken()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, "bob On Cookie"));
            identity.AddClaim(new Claim(ClaimTypes.Email, "bob@example.test"));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return Ok("bob on cookie sign in successed.");
        }

        /// <summary>
        /// 验证 /api/token/buildCookiesToken 签发的 cookie 是否有效。
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [Authorize] //使用 services.AddAuthentication(defaultScheme) 设定的默认认证方案进行验证。
        public IActionResult ValidateCookieToekn()
        {
            return Ok($"Hello {User.Identity.Name}. now : {DateTime.UtcNow.Ticks}");
        }
    }
}
