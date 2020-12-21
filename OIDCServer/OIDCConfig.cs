using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityModel;

namespace OIDCServer
{
    public static class OIDCConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources
        {
            get
            {
                yield return new IdentityResources.OpenId();
                yield return new IdentityResources.Profile();
                //yield return new IdentityResources.Phone();
                //yield return new IdentityResources.Email();
                //yield return new IdentityResources.Address();
            }
        }

        public static IEnumerable<ApiScope> ApiScopes
        {
            get
            {
                yield return new ApiScope("scope-1");
                yield return new ApiScope("scope-2");
                yield return new ApiScope("scope-3");
            }
        }

        public static IEnumerable<ApiResource> ApiResource
        {
            get
            {
                yield return new ApiResource("api 1")
                {
                    Scopes = {"scope-1"}
                };
            }
        }

        public static IEnumerable<Client> Clients
        {
            get
            {
                string clientSecret = "123".Sha256();

                yield return new Client
                {
                    ClientId = "code client",
                    ClientName = "授权码授权模式的客户端",
                    ClientSecrets = { new Secret(clientSecret) },

                    RequirePkce = true,
                    //RequireClientSecret = false,
                    // 是否需要进入用户显示同意授权页。
                    RequireConsent = true,
                    // 登录成功回调处理地址，处理回调返回的数据
                    RedirectUris = 
                    {
                        // 这些请求由 Microsoft.AspNetCore.Authentication.OpenIdConnect 组件负责处理。
                        //"https://client.oidc.test:5200/signin-oidc"
                        "https://client.oidc.test:5200/oidc/signin-oidc"
                    },
                    // where to redirect to after logout
                    PostLogoutRedirectUris = 
                    {
                        // 这些请求由 Microsoft.AspNetCore.Authentication.OpenIdConnect 组件负责处理。
                        //"https://client.oidc.test:5200/signout-callback-oidc"
                        "https://client.oidc.test:5200/oidc/signout-callback-oidc"
                    },

                    // 允许此客户端使用的授权的模式。
                    AllowedGrantTypes = GrantTypes.Code,
                    // 需要获取 refresh token。
                    AllowOfflineAccess = true,
                    // 
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "scope-1"
                    },
                    // 是否允许浏览器接收 access token。默认值 false。在 Implicit 授权模式下必须设置为 true。
                    //AllowAccessTokensViaBrowser = true,
                    // 在 ResourceOwnerPassword 授权模式下用不着。
                    //AlwaysIncludeUserClaimsInIdToken = true
                };
            }
        }

        public static List<TestUser> Users
        {
            get
            {
                return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = $"U{DateTime.UtcNow.Ticks}.1",
                        Username = "xiaoming",
                        Password = "123456"
                    },

                    new TestUser
                    {
                        SubjectId = $"U{DateTime.UtcNow.Ticks}.2",
                        Username = "jing",
                        Password = "123123",
                        Claims = {
                            
                            //new Claim(ClaimTypes.Name, "静", ClaimValueTypes.String),
                            new Claim(JwtClaimTypes.Name, "静", ClaimValueTypes.String),
                            //new Claim(ClaimTypes.Gender, "女"),
                            new Claim(JwtClaimTypes.Gender, "女"),
                            new Claim(JwtClaimTypes.NickName, "我想静静", ClaimValueTypes.String)
                        }
                    }
                };
            }
        }
    }
}
