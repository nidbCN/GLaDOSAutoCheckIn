using DnsClient;
using GLaDOSAutoCheckin.Models;
using MailKit;
using MailKit.Net.Imap;
using GLaDOSAutoCheckin.Utils;

namespace GLaDOSAutoCheckin.Services
{
    public class MailService
    {
        private readonly ILookupClient _lookupClient;
        private readonly AuthOption _option;

        public MailService(ILookupClient lookupClient, AuthOption option)
        {
            _lookupClient = lookupClient;
            _option = option;

            if (_option.MailHost is null)
            {
                var mailHost = _lookupClient
                    .Query(_option.MailAccount[(_option.MailAccount.IndexOf('@') + 1)..], QueryType.MX)
                    .Answers.MxRecords().First().Exchange.Value;
                _option.MailHost = mailHost;
            }
        }

        public string GetAuthCode()
        {
            var startTime = DateTime.Now;

            using var client = new ImapClient();
            client.Connect(_option.MailHost, _option.MailPort, MailKit.Security.SecureSocketOptions.None);

            client.Authenticate(_option.MailAccount, _option.Password);

            // The Inbox folder is always available on all IMAP servers...
            var inbox = client.Inbox;
            string? code = null;

            do
            {
                inbox.Open(FolderAccess.ReadWrite);

                Console.WriteLine("Total messages: {0}", inbox.Count);
                Console.WriteLine("Recent messages: {0}", inbox.Recent);

                var authMail = inbox.Last(mail => mail.Subject == "GLaDOS Authentication");

                if (authMail is null)
                {
                    Console.WriteLine("Mail not fount, retry...");
                    break;
                }

                // if (authMail.Date.DateTime < startTime) break;

                if (AuthCodeUtil.TryGetAuthCodeFromHtml(authMail.HtmlBody, out code))
                {
                    Console.WriteLine($"Auth code is {code}");
                }

            } while (code is null);

            client.Disconnect(true);

            return code;
        }
    }
}
