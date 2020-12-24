# oidc.demo

# 说明
在混合认证模式的情况下，`[Authorize]`只使用默认的认证方案，即`services.AddAuthentication(defaultScheme)`指定的认证方案。
因此需要手动指定支持的一种或多种方案。


# 示例集

## 自定义身份认证的示例。
1. 签发 `JWT` 的示例。

`GET /api/token/buildJwtToken`

2. 验证上一`API`签发的 `JWT`，验证的代码在 Startup.cs 文件里。

`GET /api/token/validateJwtToekn`
> 调用这个API需要在请求头增加 `Authorization:Bearer <要验证的JWT字符串>`

3. 签发 `Cookie` 的示例。

`GET /api/token/buildCookieToken`

4. 验证上一个`API`签发的 `Cookie`，验证的代码在 Startup.cs 文件里。