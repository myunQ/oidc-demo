# oidc.demo
# 示例集

## 自定义身份认证的示例。
1. 签发 `JWT` 的示例。

`GET /api/token/buildJwtToken`

2. 验证上一`API`签发的 `JWT`，验证的代码在 Startup.cs 文件里。

`GET /api/token/validateJwtToekn`
> 调用这个API需要在请求头增加 `Authorization:Bearer <要验证的JWT字符串>`