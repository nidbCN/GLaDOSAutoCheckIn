using DnsClient;
using GLaDOSAutoCheckIn.Models.Options;
using GLaDOSAutoCheckIn.Worker;
using GLaDOSAutoCheckIn.Worker.Services;

await Host.CreateDefaultBuilder(args)
   .UseSystemd()
   .UseWindowsService()
   .ConfigureServices((context, services) =>
   {
        // Configure option
        services.Configure<AuthOption>(
           context.Configuration.GetSection(nameof(AuthOption))
       );

        // Add DNS lookup client
        services.AddSingleton<ILookupClient>(new LookupClient());

        // Add user service with configured HttpClient
        services.AddHttpClient<IUserConsoleService, UserConsoleService>(client =>
        {
            var requestOption = context.Configuration
                .GetSection(nameof(RequestOption))
                .Get<RequestOption>();

           client.BaseAddress = new Uri(requestOption.BaseUrl);
           client.Timeout = TimeSpan.FromMilliseconds(requestOption.TimeOut);

           client.DefaultRequestHeaders
               .UserAgent.ParseAdd(requestOption.UserAgent);
           client.DefaultRequestHeaders
               .Add("authority", "glados.rocks");

           if (requestOption.Cookie != null)
           {
               client.DefaultRequestHeaders.Add("cookie", requestOption.Cookie);
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
   .Build().RunAsync();
