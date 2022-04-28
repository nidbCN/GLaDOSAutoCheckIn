# 简介
自动登录及签到 [GLaDOS](https://glados.rocks/)，配合 `systemd` 可实现每日自动签到。

# 开始使用

## 从源代码构建

### 获取源代码

首先确保安装有 [git](https://git-scm.com/)，然后运行

```sh
git clone https://github.com/nidbCN/GLaDOSAutoCheckIn.git
```

### 环境

安装 [.NET6 SDK](https://docs.microsoft.com/en-us/dotnet/core/sdk)

### 构建

```sh
cd .\GLaDOSAutoCheckin\
dotnet publish -c Release
```

其它选项：

1. 不依赖运行时的部署：[self-contained](https://docs.microsoft.com/en-us/dotnet/core/deploying/#publish-self-contained)
2. 发布单文件：[single file app](https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file/overview#publish-a-single-file-app---cli)

参考：[Micorosft Docs](https://docs.microsoft.com/en-us/dotnet/core/deploying)

### 配置

打开编译完成的目录（默认为 `GLaDOSAutoCheckin.Worker\bin\Release\net6.0\publish`）中的 `appsettings.json` （没有则新建）

写入或修改以下内容：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AuthOption": {
    "MailAccount": "mail@example.com",
    "Password": "yourPassword",
    "MailPort": 143,
    "MailHost": "pop3.example.com",
    "UseSSL": false
  },
  "HttpOption": {
    "UserAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.182 Safari/537.36",
    "BaseUrl": "https://glados.rocks/api/",
    "Cookie": null
  }
}
```

注释：
1. `Logging` 节点为 `Microsoft Logger` 的配置，参考 [Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line#configure-logging)
2. `AuthOption` 为收邮件和登录的配置，`MailAccount` 为登录所用的邮箱；`Password` 为邮箱的密码（用于拉取邮件获取验证码）；`MailPort` 为 POP3 的端口，默认为 `143`，如果需要使用 SSL（见下文）请修改为对应端口（协议默认端口为 `995`）；`MailHost` 为邮件服务器，为空则自动解析；`UseSSL` 设置为 `true` 则使用 SSL 登录，需要手动指定端口为 `995`。
3. `HttpOption` 节点为发送请求的配置，`UserAgent` 为用户UA，建议模仿浏览器；`BaseUrl` 是 GLaDOS 的 API 链接；`Cookie` 暂时无法设置。
 
### 使用

配置完成后，运行 GLaDOSAutoCheckIn 则会自动完成验证、登录和签到

#### Systemd daemon



## TODOs

1. 存储cookie

## License

This project use GNU General Public License v3 for license

support free software, free as in freedom.