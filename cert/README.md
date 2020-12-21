# 证书文件
- myun-ca-root.crt 是自签名CA证书，用于对oidc.test.pfx进行签名，需要安装到“受信任的根证书颁发机构”里。安装后可以删除。

- oidc.test.pfx 是服务器证书文件。需要配置到web服务器。这项目中已经配置到 Kestrel 服务器了。如果要修改路径或文件名，则需要同时修改三个项目的appsettings.json文件。