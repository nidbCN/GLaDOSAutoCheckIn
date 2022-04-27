using GLaDOSAutoCheckin.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args).Build();

// Build a config object, using env vars and JSON providers.
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();


var settings = config.GetRequiredSection(nameof(AuthOption)).Get<AuthOption>();

 Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, service) =>
            {
                context.
            });

host.Build().RunAsync();