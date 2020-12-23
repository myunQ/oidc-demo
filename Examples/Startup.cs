using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Examples
{
    public class Startup
    {
        public const string Issuer = "https://token.example.test";
        public const string Audience = "target audience";

        //public static byte[] key256 = new byte[256];
        public static byte[] key256 = new byte[] { 131, 44, 4, 137, 115, 24, 237, 149, 96, 160, 14, 118, 14, 228, 136, 36, 4, 134, 17, 78, 131, 6, 179, 87, 230, 61, 6, 208, 170, 110, 175, 227, 210, 62, 178, 4, 98, 58, 147, 63, 39, 242, 142, 33, 177, 67, 196, 53, 144, 212, 136, 155, 112, 183, 133, 7, 86, 226, 88, 209, 83, 210, 78, 5, 137, 88, 228, 45, 12, 113, 67, 208, 0, 219, 129, 2, 1, 218, 141, 210, 77, 182, 145, 138, 75, 105, 124, 3, 108, 116, 94, 21, 119, 49, 7, 241, 15, 209, 51, 35, 181, 39, 253, 255, 136, 181, 106, 139, 172, 165, 207, 152, 207, 83, 31, 75, 225, 103, 116, 234, 146, 201, 225, 43, 91, 92, 187, 152, 131, 209, 248, 67, 17, 250, 5, 90, 49, 89, 120, 65, 204, 23, 176, 202, 22, 106, 96, 23, 166, 220, 98, 90, 167, 255, 222, 245, 85, 206, 40, 227, 238, 150, 51, 132, 203, 245, 42, 11, 223, 140, 42, 22, 28, 198, 31, 137, 92, 130, 86, 244, 92, 142, 21, 255, 88, 210, 113, 214, 71, 175, 237, 159, 79, 91, 148, 112, 87, 145, 240, 74, 184, 84, 161, 155, 9, 187, 31, 130, 194, 49, 213, 149, 66, 15, 188, 150, 224, 215, 91, 65, 231, 197, 46, 84, 62, 139, 9, 167, 173, 199, 58, 239, 101, 122, 181, 62, 141, 37, 224, 127, 198, 22, 223, 245, 157, 204, 142, 193, 100, 225, 37, 120, 124, 75, 200, 226 };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            /*using RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(key256);*/
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Examples", Version = "v1" });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey = new SymmetricSecurityKey(Startup.key256),
                        // 设置要正确的签发者，并验证 jwt 中包含的签发者是否根这里设置的一致。
                        ValidIssuer = Issuer,
                        // 设置多个签发者，jwt 中只要包含其中一个签发者就可以。
                        //ValidIssuers = new string[] { Issuer },
                        ValidateIssuer = true,
                        // 设置要正确的受众，并验证 jwt 中包含的受众是否根这里设置的一致。
                        //ValidAudience = Audience,
                        // 设置多个受众，jwt 中只要包含其中一个受众就可以。
                        ValidAudiences = new string[] { Audience, "无中生有" },
                        ValidateAudience = true
                    };
                });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Examples v1"));
            }

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
