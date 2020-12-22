using IdentityServer4.Models;
using System.Collections.Generic;

namespace OIDCServer
{
    public static class OIDCConfig
    {
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
                    Scopes = { "scope-1" }
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
                    ClientId = "client credentials client",
                    ClientName = "客户端授权模式的客户端",
                    ClientSecrets = { new Secret(clientSecret) },

                    // 允许此客户端使用的授权的模式。
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // 没有用户账户介入，用不着。
                    //AllowOfflineAccess = true,
                    // 
                    AllowedScopes =
                    {
                        "scope-1"
                    },
                    // 是否允许浏览器传入 access token。默认值 false。
                    //AllowAccessTokensViaBrowser = true,
                    // 没有用户账户介入，用不着。
                    //AlwaysIncludeUserClaimsInIdToken = true
                };
            }
        }
    }
}
