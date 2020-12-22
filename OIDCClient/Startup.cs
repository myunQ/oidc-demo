using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;



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
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            //System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            //services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "identity";
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = "https://server.oidc.test:5000";
                //options.RequireHttpsMetadata = false;
                options.ResponseType = "code token id_token";
                //options.ResponseMode = IdentityModel.OidcConstants.ResponseModes.Query;
                options.ClientId = "hybrid client";
                options.ClientSecret = "123";
                options.SaveTokens = true;
                options.Scope.Add("scope-1");
                options.Scope.Add("roles");
                // 协议中规定 implicit 授权模式不提供 refresh token。
                options.Scope.Add(IdentityModel.OidcConstants.StandardScopes.OfflineAccess);
                //options.GetClaimsFromUserInfoEndpoint = true;

                // 这些请求由 Microsoft.AspNetCore.Authentication.OpenIdConnect 组件负责处理。
                options.CallbackPath = "/oidc/signin-oidc";
                // 这些请求由 Microsoft.AspNetCore.Authentication.OpenIdConnect 组件负责处理。
                options.SignedOutCallbackPath = "/oidc/signout-callback-oidc";

                // 在 http 协议下 chrome 浏览器会将 SameSite = none 的 Cookie 丢弃。所以这里必须设置为 Lax 或 Strict
                //options.NonceCookie.SameSite = SameSiteMode.Lax;
                //options.CorrelationCookie.SameSite = SameSiteMode.Lax;

                // Claim 与 MVC 的用户对象进行映射
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role
                };

                /*
                 * MVC 控制器里代表当前用户的 User 属性就是 System.Security.Claims.ClaimsPrincipal 类型。
                 * options.ClaimActions 里面存在的 Claim 是给 System.Security.Claims.ClaimsPrincipal.Claims 属性赋值时要过滤调用的 Claim 。
                 * options.ClaimActions.Remove 方法是取消从 jwt 中过滤 Claim。
                 * options.ClaimActions.Add 或 options.ClaimActions.DeleteClaim[s] 这类方法是将 Claim 添加到 options.ClaimActions 中。
                 * 
                 * 这个扩展方法在 Microsoft.AspNetCore.Authentication.OAuth.dll 的 Microsoft.AspNetCore.Authentication.ClaimActionCollectionMapExtensions 类里。
                 */

                //options.ClaimActions.Remove("nbf");
                //options.ClaimActions.Remove("exp");
                //options.ClaimActions.DeleteClaim("sid");
                //options.ClaimActions.DeleteClaim("idp");
                //options.ClaimActions.MapUniqueJsonKey("myclaim1", "myclaim1");

                /*
                options.ClaimActions 集合默认包含的 ClaimType 和 ClaimAction 的类型：
                
                nonce：      Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                aud：        Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                azp：        Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                acr：        Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                iss：        Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                iat：        Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                nbf：        Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                exp：        Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                at_hash：    Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                c_hash：     Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                ipaddr：     Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                platf：      Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                ver：        Microsoft.AspNetCore.Authentication.OAuth.Claims.DeleteClaimAction
                sub：        Microsoft.AspNetCore.Authentication.OpenIdConnect.Claims.UniqueJsonKeyClaimAction
                name：       Microsoft.AspNetCore.Authentication.OpenIdConnect.Claims.UniqueJsonKeyClaimAction
                given_name： Microsoft.AspNetCore.Authentication.OpenIdConnect.Claims.UniqueJsonKeyClaimAction
                family_name：Microsoft.AspNetCore.Authentication.OpenIdConnect.Claims.UniqueJsonKeyClaimAction
                profile：    Microsoft.AspNetCore.Authentication.OpenIdConnect.Claims.UniqueJsonKeyClaimAction
                email：      Microsoft.AspNetCore.Authentication.OpenIdConnect.Claims.UniqueJsonKeyClaimAction
                */
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
