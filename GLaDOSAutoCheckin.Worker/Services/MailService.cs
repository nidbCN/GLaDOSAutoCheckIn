using DnsClient;
using GLaDOSAutoCheckIn.Models;
using MailKit.Net.Pop3;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GLaDOSAutoCheckIn.Worker.Services;

public class MailService : IMailService
{
    private readonly AuthOption _option;

    private readonly Pop3Client _mailClient = new();

    private readonly ILogger<MailService> _logger;

    public MailService(IOptions<AuthOption> option, ILookupClient lookupClient, ILogger<MailService> logger)
    {
        _logger = logger;

        var authOption = option.Value;

        if (authOption.MailHost is null)
        {
            _logger.LogInformation("Could not get mail host, trying resolve {account}", authOption.MailAccount);
            var mailHost = lookupClient
                .Query(authOption.MailAccount[(authOption.MailAccount.IndexOf('@') + 1)..], QueryType.MX)
                .Answers.MxRecords().First().Exchange.Value;
            _logger.LogInformation("Find mx record {rec} for this account.", mailHost);
            authOption.MailHost = mailHost;
        }

        _option = authOption;
    }

    public void Initialize()
    {
        _logger.LogInformation("Connecting to mail host.");
        _mailClient.Connect(
            _option.MailHost,
            _option.MailPort,
            MailKit.Security.SecureSocketOptions.None
        );

        _mailClient.Authenticate(_option.MailAccount, _option.Password);
    }

    public bool TryGetAuthMail(out MimeMessage mailObj)
    {
        mailObj = new();

        if (_mailClient.Count == 0)
            return false;

        for (var i = _mailClient.Count - 1; i >= 0; i--)
        {
            var mailItem = _mailClient.GetMessage(i);

            if (mailItem?.Subject != "GLaDOS Authentication") continue;

            _logger.LogInformation("Found auth mail, subject {title}", mailItem.Subject);
            mailObj = mailItem;
            return true;
        }

        return false;
    }
}
