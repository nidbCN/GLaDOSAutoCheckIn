using MailKit;
using MailKit.Net.Imap;

namespace Gaein.GLaDOSAutoCheckin;

class Program
{
    static void Main(string[] args)
    {
        string address = null;
        var account = "name@xxx.com";
        var password = "pwd";
        var port = 143;

        if (address is null)
            address = account[account.IndexOf('@')..];

        var startTime = DateTime.Now;

        using var client = new ImapClient();
        client.Connect(address, port, MailKit.Security.SecureSocketOptions.None);

        client.Authenticate(account, password);

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

            if (TryGetAuthCode(authMail.HtmlBody, out code))
            {
                Console.WriteLine($"Auth code is {code}");
            }

        } while (code is null);

        client.Disconnect(true);
    }

    static bool TryGetAuthCode(string html, out string code)
    {
        code = string.Empty;

        var startIndex = html.IndexOf("<b>");
        if (startIndex == -1) 
            return false;

        var half = html[(startIndex + 3)..];

        var endIndex = half.IndexOf("</b>");
        if (endIndex == -1)
            return false;

        code = half[..endIndex];
        return true;
    }
}