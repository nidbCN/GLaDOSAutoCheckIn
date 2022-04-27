using DnsClient;
using GLaDOSAutoCheckin.Models;
using GLaDOSAutoCheckin.Services;
using GLaDOSAutoCheckin.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<AuthOption>(
            context.Configuration.GetSection(nameof(AuthOption))
        );
        services.AddSingleton<ILookupClient>(new LookupClient());

        var baseUrl = context.Configuration["HttpOption:BaseUrl"] ?? "https://glados.rocks/api";

        services.AddHttpClient<IUserConsoleService, UserConsoleService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);

            // Add a user-agent default request header.
            client.DefaultRequestHeaders
                .UserAgent.ParseAdd(context.Configuration["HttpOption:UserAgent"]);
            client.DefaultRequestHeaders.Add("authority", "glados.rocks");

            var cookie = context.Configuration["HttpOption:UserAgent"];
            if (cookie != null)
            {
                client.DefaultRequestHeaders.Add("cookie", cookie);
            }
        }).ConfigurePrimaryHttpMessageHandler(() =>
            new HttpClientHandler()
            {
                UseCookies = true
            });

        services.AddSingleton<IMailService, MailService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
