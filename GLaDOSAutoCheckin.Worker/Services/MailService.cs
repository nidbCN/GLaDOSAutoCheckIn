using DnsClient;
using GLaDOSAutoCheckIn.Models.Options;
using MailKit.Net.Pop3;
using MailKit.Security;
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
        _logger.LogDebug("Use config: {host}:{port}, ssl:{ssl}",
            _option.MailHost, _option.MailPort, _option.UseSSL);
        _mailClient.Connect(
            _option.MailHost,
            _option.MailPort,
            _option.UseSSL
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.None
        );

        _mailClient.Authenticate(_option.MailAccount, _option.Password);
    }

    public bool TryGetAuthMail(Predicate<MimeMessage?> match, out MimeMessage mailObj)
    {
        mailObj = new MimeMessage();

        if (_mailClient.Count == 0)
            return false;

        for (var i = _mailClient.Count - 1; i >= 0; i--)
        {
            var mailItem = _mailClient.GetMessage(i);

            if (!match(mailItem))
                continue;

            _logger.LogInformation("Found auth mail, subject {title}", mailItem.Subject);
            mailObj = mailItem;
            return true;
        }

        return false;
    }
}
