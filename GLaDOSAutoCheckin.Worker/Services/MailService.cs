using DnsClient;
using GLaDOSAutoCheckIn.Models.Options;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GLaDOSAutoCheckIn.Worker.Services;

public class MailService : IMailService
{
    private const string FromText = "support@glados.network";
    private const string TitleText = "GLaDOS Authentication";

    private readonly AuthOption _option;

    private readonly ImapClient _mailClient = new();

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
            _option.MailHost, _option.MailPort, _option.UseSsl);
        _mailClient.Connect(
            _option.MailHost,
            _option.MailPort,
            _option.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.None);

        _mailClient.Authenticate(_option.MailAccount, _option.Password);
        _mailClient.Inbox.Open(FolderAccess.ReadOnly);
    }

    public bool TryGetAuthMail(DateTime startTime, out MimeMessage? mailObj)
    {
        mailObj = null;

        var mailUidList = _mailClient.Inbox.Search(SearchQuery.DeliveredAfter(startTime));

        var mailUid = new UniqueId();

        for (var i = mailUidList.Count - 1; i >= 0; i--)
        {
            var uid = mailUidList[i];
            var headerList = _mailClient.Inbox.GetHeaders(uid);
            if (headerList["Subject"] != TitleText)
                continue;
            if (headerList["From"] != FromText)
                continue;

            var date = headerList["Date"];

            if (date is null)
                continue;
            if (!DateTime.TryParse(date, out var mailTime))
                continue;
            if (startTime > mailTime)
                continue;

            mailUid = uid;
            break;
        }

        if (!mailUid.IsValid)
            return false;
                
        _logger.LogInformation("Found auth mail, uid {uid}", mailUid);
        mailObj = _mailClient.Inbox.GetMessage(mailUid);

        return true;
    }
}
