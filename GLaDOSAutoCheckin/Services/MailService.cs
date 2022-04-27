using DnsClient;
using GLaDOSAutoCheckin.Models;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace GLaDOSAutoCheckin.Services
{
    public class MailService
    {
        private readonly AuthOption _option;
        private readonly ILogger<MailService> _logger;

        private readonly ImapClient _mailClient = new();
        private readonly IMailFolder? _mailFolder;

        public MailService(AuthOption option, ILogger<MailService> logger, ILookupClient lookupClient)
        {
            if (option.MailHost is null)
            {
                var mailHost = lookupClient
                    .Query(option.MailAccount[(option.MailAccount.IndexOf('@') + 1)..], QueryType.MX)
                    .Answers.MxRecords().First().Exchange.Value;
                option.MailHost = mailHost;
            }

            _option = option;
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
            if (_mailFolder is null)
                throw new NullReferenceException(nameof(_mailFolder));

            mailObj = _mailFolder.LastOrDefault(
                mail => mail.Subject == "GLaDOS Authentication");

            if (mailObj is null)
                return false;

            return true;
        }

        public bool TryGetAuthMail(Predicate<MimeMessage> match, out MimeMessage? mailObj)
        {
            if (_mailFolder is null)
                throw new NullReferenceException(nameof(_mailFolder));

            mailObj = _mailFolder.LastOrDefault(
                mail => match.Invoke(mail));

            if (mailObj is null)
                return false;

            return true;
        }
    }
}
