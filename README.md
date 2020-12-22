# oidc.demo
1. OIDCServer：认证授权服务器，是基于 IdentityServer4 的Web项目。访问地址： https://server.oidc.test:5000
1. OIDCProtectedResources：受保护的资源，也是一个 Web 项目。访问地址：https://api.oidc.test:5100
1. OIDCClient：客户端，也是一个 Web 项目。访问地址：https://client.oidc.test:5200

> 这个分支演示`OAuth2.0`的`Hybrid`授权模式。
> ***
> `Hybrid`授权模式只接受以下三种`ResponseType`类型：
> 1. code token
> 1. code id_token
> 1. code token id_token

***

# 需要做的配置
1. 在 `hosts` 文件增加
```
127.0.0.1	server.oidc.test
127.0.0.1	api.oidc.test
127.0.0.1	client.oidc.test
```
2. 信任根证书`cert/myun-ca-root.crt`。

3. 如果需要在登出时跳转回应用（这里是 https://client.oidc.test:5200）则需要将将`IdentityServerHost.Quickstart.UI.AccountOptions`类的`AutomaticRedirectAfterSignOut`属性设置为`true`。该文件位于`OIDCServer`项目下的`/Quickstart/Account/AccountOptions.cs`文件中。

***

# 运行说明
1. 启动 OIDCServer 项目。
1. 启动 OIDCProtectedResources 项目。
1. 启动 OIDCClient 项目。
1. 浏览器访问 OIDCClient 项目 https://client.oidc.test:5200/api/identity

***

# 坑（注意事项）
## 跨过的坑
1. `"errorDescription":"code challenge required"`：
在`Client`将`ResponseType`设置`Hybrid`接受的类型后，默认情况下跳转到授权服务器不会带上`URL`参数`code_challenge`和`code_challenge_method`（`Authorization code`模式会带上这两个参数），因此在`Server`上要将`Client`的`RequirePkce`属性设置为`false`。
1. `"errorDescription":"Client not configured to receive access tokens via browser"`：如果`ResponseType`含有`token`则需要将`Server`上的`Client`的`AllowAccessTokensViaBrowser`属性设置为`true`。

## 没跨过的坑



# OIDC 服务发现地址

https://server.oidc.test:5000/.well-known/openid-configuration

```
{
    "issuer": "https://server.oidc.test:5000",
    "jwks_uri": "https://server.oidc.test:5000/.well-known/openid-configuration/jwks",
    "authorization_endpoint": "https://server.oidc.test:5000/connect/authorize",
    "token_endpoint": "https://server.oidc.test:5000/connect/token",
    "userinfo_endpoint": "https://server.oidc.test:5000/connect/userinfo",
    "end_session_endpoint": "https://server.oidc.test:5000/connect/endsession",
    "check_session_iframe": "https://server.oidc.test:5000/connect/checksession",
    "revocation_endpoint": "https://server.oidc.test:5000/connect/revocation",
    "introspection_endpoint": "https://server.oidc.test:5000/connect/introspect",
    "device_authorization_endpoint": "https://server.oidc.test:5000/connect/deviceauthorization",
    "frontchannel_logout_supported": true,
    "frontchannel_logout_session_supported": true,
    "backchannel_logout_supported": true,
    "backchannel_logout_session_supported": true,
    "scopes_supported": [
        "openid",
        "profile",
        "scope-1",
        "scope-2",
        "scope-3",
        "offline_access"
    ],
    "claims_supported": [
        "sub",
        "name",
        "family_name",
        "given_name",
        "middle_name",
        "nickname",
        "preferred_username",
        "profile",
        "picture",
        "website",
        "gender",
        "birthdate",
        "zoneinfo",
        "locale",
        "updated_at"
    ],
    "grant_types_supported": [
        "authorization_code",
        "client_credentials",
        "refresh_token",
        "implicit",
        "password",
        "urn:ietf:params:oauth:grant-type:device_code"
    ],
    "response_types_supported": [
        "code",
        "token",
        "id_token",
        "id_token token",
        "code id_token",
        "code token",
        "code id_token token"
    ],
    "response_modes_supported": [
        "form_post",
        "query",
        "fragment"
    ],
    "token_endpoint_auth_methods_supported": [
        "client_secret_basic",
        "client_secret_post"
    ],
    "id_token_signing_alg_values_supported": [
        "RS256"
    ],
    "subject_types_supported": [
        "public"
    ],
    "code_challenge_methods_supported": [
        "plain",
        "S256"
    ],
    "request_parameter_supported": true
}
```