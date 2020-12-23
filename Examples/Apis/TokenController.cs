using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
                    new Claim(ClaimTypes.Name, "bob"),
                    new Claim(ClaimTypes.Email, "bob@example.test")
                },
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(10D),
                signingCredentials: credentials);

            string result = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return Content(result);
        }

        /// <summary>
        /// 验证 /api/token/buildJwtToken 签发的 JWT 是否有效。
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize]
        public IActionResult ValidateJwtToekn()
        {
            return Ok($"Hello {User.Identity.Name}. now : {DateTime.UtcNow.Ticks}");
        }
    }
}
