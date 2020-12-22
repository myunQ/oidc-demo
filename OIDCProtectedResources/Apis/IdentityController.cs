using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace OIDCProtectedResources.Apis
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //System.Security.Claims.Claim       //表示证件上的某个属性和值
            //System.Security.Claims.ClaimsIdentity  //表示证件
            //System.Security.Claims.ClaimsPrincipal //表示人
            //IEnumerable<System.Security.Claims.ClaimsIdentity> User.Identities //一个人有多个证件
            //System.Security.Principal.IIdentity User.Identity //一个证件


            var authenticateResult = await HttpContext.AuthenticateAsync();


            //AuthenticateResult

            //包含用户身份信息以及其他身份验证状态。
            //AuthenticationTicket


            /*
            if (authenticateResult.Ticket.Properties == authenticateResult.Properties)
            {
                // 获取或设置是否在多个请求之间保留身份验证会话。
                if (authenticateResult.Properties.IsPersistent)
                {

                }
            }
            */

            return new JsonResult(new
            {
                User = User.Claims.GroupBy(o => o.Type, o => o.Value).ToDictionary(o => o.Key, o => o.Count() > 1 ? string.Join(",", o) : o.First()),
                AuthenticateResult_Properties_Parameters = authenticateResult.Properties.Parameters,
                AuthenticateResult_Properties_Items = authenticateResult.Properties.Items
            });
        }
    }
}
