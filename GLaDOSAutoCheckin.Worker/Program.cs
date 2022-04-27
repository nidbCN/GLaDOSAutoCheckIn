using DnsClient;
using GLaDOSAutoCheckIn.Models;
using GLaDOSAutoCheckIn.Worker;
using GLaDOSAutoCheckIn.Worker.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Configure option
        services.Configure<AuthOption>(
            context.Configuration.GetSection(nameof(AuthOption))
        );

        // Add DNS lookup client
        services.AddSingleton<ILookupClient>(new LookupClient());

        var baseUrl = context.Configuration["HttpOption:BaseUrl"] ?? "https://glados.rocks/api";

        // Add user service with configured HttpClient
        services.AddHttpClient<IUserConsoleService, UserConsoleService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);

            client.DefaultRequestHeaders
                .UserAgent.ParseAdd(context.Configuration["HttpOption:UserAgent"]);
            client.DefaultRequestHeaders.Add("authority", "glados.rocks");

            var cookie = context.Configuration["HttpOption:Cookie"];
            if (cookie != null)
            {
                client.DefaultRequestHeaders.Add("cookie", cookie);
            }
        }).ConfigurePrimaryHttpMessageHandler(() =>
            new HttpClientHandler()
            {
                UseCookies = true
            });

        // Add mail service
        services.AddSingleton<IMailService, MailService>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
