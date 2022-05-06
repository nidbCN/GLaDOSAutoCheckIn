using GLaDOSAutoCheckIn.Utils;
using GLaDOSAutoCheckIn.Worker.Services;

namespace GLaDOSAutoCheckIn.Worker;

public class Worker : BackgroundService
{
    private readonly IUserConsoleService _userService;
    private readonly IMailService _mailService;
    private readonly ILogger<Worker> _logger;

    public Worker(IUserConsoleService userService, IMailService mailService, ILogger<Worker> logger)
        => (_userService, _mailService, _logger) = (userService, mailService, logger);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _userService.RequireVerifyAsync();

        for (var i = 0; i < 3; i++)
        {
            _logger.LogInformation("Try get auth from mail, times {times}", i);
            if (_mailService.TryGetAuthMail(out var mail))
            {
                if (!AuthCodeUtil.TryParseFromHtml(mail.HtmlBody, out var code))
                {
                    _logger.LogError("An error occurred while parse code from mail, exit.");
                    return;
                }

                _logger.LogInformation("Successful read code {sCode}", $"****{code[^2..]}");
                await _userService.LoginAsync(code);

                var startTime = DateTime.Now;
                await _userService.CheckIn();

                while (!stoppingToken.IsCancellationRequested)
                {

                    if (DateTime.Now.Day - startTime.Day >= 1)
                    {
                        await _userService.CheckIn();
                    }
                    Thread.Sleep(100);
                }
            }
            else
            {
                _logger.LogWarning("Could not found auth mail, retry in 5s.");
                Thread.Sleep(5000);
            }
        }

        _logger.LogError("Could not found auth mail after 3 times fetch, exit.");
    }
}