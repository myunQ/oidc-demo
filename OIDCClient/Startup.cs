using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace OIDCClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 不知道下面两条语句是否能达到类似的效果。
            //System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = "https://server.oidc.test:5000";
                //options.RequireHttpsMetadata = false;
                options.ResponseType = "code";
                //options.ResponseMode = IdentityModel.OidcConstants.ResponseModes.Query;
                options.ClientId = "code client";
                options.ClientSecret = "123";
                options.SaveTokens = true;
                options.Scope.Add("scope-1");
                // 协议中规定 implicit 授权模式不提供 refresh token。
                options.Scope.Add(IdentityModel.OidcConstants.StandardScopes.OfflineAccess);
                //options.GetClaimsFromUserInfoEndpoint = true;
                //options.ClaimActions.MapUniqueJsonKey("myclaim1", "myclaim1");

                // 这些请求由 Microsoft.AspNetCore.Authentication.OpenIdConnect 组件负责处理。
                options.CallbackPath = "/oidc/signin-oidc";
                // 这些请求由 Microsoft.AspNetCore.Authentication.OpenIdConnect 组件负责处理。
                options.SignedOutCallbackPath = "/oidc/signout-callback-oidc";

                // 在 http 协议下 chrome 浏览器会将 SameSite = none 的 Cookie 丢弃。所以这里必须设置为 Lax 或 Strict
                //options.NonceCookie.SameSite = SameSiteMode.Lax;
                //options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OIDCClient", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OIDCClient v1"));
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
