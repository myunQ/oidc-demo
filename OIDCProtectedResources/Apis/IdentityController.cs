﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

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
            var authenticateResult = await HttpContext.AuthenticateAsync();

            return new JsonResult(new
            {
                User = from c in User.Claims select new { c.Type, c.Value },
                AuthenticateResult_Properties_Parameters = authenticateResult.Properties.Parameters,
                AuthenticateResult_Properties_Items = authenticateResult.Properties.Items
            });
        }
    }
}
