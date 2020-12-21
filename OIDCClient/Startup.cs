using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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
            // ��֪��������������Ƿ��ܴﵽ���Ƶ�Ч����
            //System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            //System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://server.oidc.test:5000";
                //options.RequireHttpsMetadata = false;
                options.ResponseType = "token id_token";
                //options.ResponseMode = "fragment";
                options.ClientId = "implicit client";
                options.SaveTokens = true;
                options.Scope.Add("scope-1");
                // Э���й涨 implicit ��Ȩģʽ���ṩ refresh token��
                //options.Scope.Add("offline_access");
                //options.GetClaimsFromUserInfoEndpoint = true;
                //options.ClaimActions.MapUniqueJsonKey("myclaim1", "myclaim1");

                // ��Щ������ Microsoft.AspNetCore.Authentication.OpenIdConnect ���������
                options.CallbackPath = "/oidc/signin-oidc";
                // ��Щ������ Microsoft.AspNetCore.Authentication.OpenIdConnect ���������
                options.SignedOutCallbackPath = "/oidc/signout-callback-oidc";

                // �� http Э���� chrome ������Ὣ SameSite = none �� Cookie ���������������������Ϊ Lax �� Strict
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
