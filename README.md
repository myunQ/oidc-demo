# oidc.demo
1. OIDCServer：认证授权服务器，是基于 IdentityServer4 的Web项目。访问地址： http://oidc.test:5000
1. OIDCProtectedResources：受保护的资源，也是一个 Web 项目。访问地址：http://api.test:5100
1. OIDCClient：客户端，也是一个 Web 项目。访问地址：http://client.test:5200

> 这个分支演示`OAuth2.0`的`ResourceOwnerPassword`授权模式。

***

# 需要做的配置
1. 在 `hosts` 文件增加
```
::1	oidc.test
::1	api.test
::1	client.test
```

***

# 运行说明
1. 启动 OIDCServer 项目。
1. 启动 OIDCProtectedResources 项目。
1. 启动 OIDCClient 项目。
1. 浏览器访问 OIDCClient 项目 http://client.test:5200/api/identity?uname=<xiaoming|jing>&passwd=<123456|123123>

***

# OIDC 服务发现地址

http://oidc.test:5000/.well-known/openid-configuration
```
{
    "issuer": "http://oidc.test:5000",
    "authorization_endpoint": "http://oidc.test:5000/connect/authorize",
    "token_endpoint": "http://oidc.test:5000/connect/token",
    "userinfo_endpoint": "http://oidc.test:5000/connect/userinfo",
    "end_session_endpoint": "http://oidc.test:5000/connect/endsession",
    "check_session_iframe": "http://oidc.test:5000/connect/checksession",
    "revocation_endpoint": "http://oidc.test:5000/connect/revocation",
    "introspection_endpoint": "http://oidc.test:5000/connect/introspect",
    "device_authorization_endpoint": "http://oidc.test:5000/connect/deviceauthorization",
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