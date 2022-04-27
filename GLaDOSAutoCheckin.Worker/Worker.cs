using GLaDOSAutoCheckin.Worker.Services;
using GLaDOSAutoCheckin.Utils;

namespace GLaDOSAutoCheckin.Worker;

public class Worker : BackgroundService
{
    private readonly IUserConsoleService _userService;
    private readonly IMailService _mailService;
    private readonly ILogger<Worker> _logger;

    public Worker(IUserConsoleService userService, IMailService mailService, ILogger<Worker> logger)
        => (_userService, _mailService, _logger) = (userService, mailService, logger);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _userService.SendVerifyAsync().Wait();

        Thread.Sleep(3000);

        _mailService.Initlaze();
        _mailService.TryGetAuthMail(out var mail);

        var code = mail?.HtmlBody;

        Console.WriteLine(code);

        if (code is not null)
        {
            if (AuthCodeUtil.TryParseFromHtml(code, out var c))
            {
               await _userService.LoginAsync(c);
                await _userService.GetStatus();
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}