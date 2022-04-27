using GLaDOSAutoCheckin.Services;
using GLaDOSAutoCheckin.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var baseUrl = context.Configuration["HttpOption:BaseUrl"] ?? "https://glados.rocks/api";
        services.AddHttpClient<AuthService>(client =>
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

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
