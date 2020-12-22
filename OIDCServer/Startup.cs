using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OIDCServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer(options =>
            {
                // 在 http 协议下 chrome 浏览器会将 SameSite = none 的 Cookie 丢弃。所以这里必须设置为 Lax 或 Strict
                //options.Authentication.CookieSameSiteMode = SameSiteMode.Lax;
                //options.Authentication.CheckSessionCookieSameSiteMode = SameSiteMode.Lax;
            }
                )
                .AddDeveloperSigningCredential()
                .AddInMemoryClients(OIDCConfig.Clients)
                .AddInMemoryApiResources(OIDCConfig.ApiResource)
                .AddInMemoryApiScopes(OIDCConfig.ApiScopes)
                .AddInMemoryIdentityResources(OIDCConfig.IdentityResources)
                .AddTestUsers(OIDCConfig.Users);

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            // 用户同意授权的页面需要用到。
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
