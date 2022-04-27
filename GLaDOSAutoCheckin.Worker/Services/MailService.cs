using DnsClient;
using GLaDOSAutoCheckin.Models;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GLaDOSAutoCheckin.Worker.Services;

public class MailService : IMailService
{
    private readonly AuthOption _option;

    private readonly ImapClient _mailClient = new();

    private readonly ILogger<MailService> _logger;

    public MailService(IOptions<AuthOption> option, ILookupClient lookupClient, ILogger<MailService> logger)
    {
        var authOption = option.Value;

        

        if (authOption.MailHost is null)
        {
            var mailHost = lookupClient
                .Query(authOption.MailAccount[(authOption.MailAccount.IndexOf('@') + 1)..], QueryType.MX)
                .Answers.MxRecords().First().Exchange.Value;

            authOption.MailHost = mailHost;
        }

        _option = authOption;
        _logger = logger;
    }

    public void Initlaze()
    {
        _mailClient.Connect(
            _option.MailHost,
            _option.MailPort,
            MailKit.Security.SecureSocketOptions.None
        );

        _logger.LogDebug("connected to mail: {mail}", _option.MailAccount);

        _mailClient.Authenticate(_option.MailAccount, _option.Password);
        _mailClient.Inbox.Open(FolderAccess.ReadOnly);

        _logger.LogDebug("open inbox with {num} recent", _mailClient.Inbox.Recent);
    }

    public bool TryGetAuthMail(out MimeMessage? mailObj)
    {
        if (_mailClient.Inbox is null)
            throw new NullReferenceException(nameof(_mailClient.Inbox));

        mailObj = _mailClient.Inbox.LastOrDefault(
            mail => mail.Subject == "GLaDOS Authentication");

        if (mailObj is null)
            return false;

        return true;
    }

    public bool TryGetAuthMail(Predicate<MimeMessage> match, out MimeMessage? mailObj)
    {
        if (_mailClient.Inbox is null)
            throw new NullReferenceException(nameof(_mailClient.Inbox));

        mailObj = _mailClient.Inbox.LastOrDefault(
            mail => match.Invoke(mail));

        if (mailObj is null)
            return false;

        return true;
    }
}
