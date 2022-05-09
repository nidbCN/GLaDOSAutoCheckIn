using MimeKit;

namespace GLaDOSAutoCheckIn.Worker.Services;

public interface IMailService
{
    public void Initialize();

    public bool TryGetAuthMail(DateTime startTime, out MimeMessage? mailObj);
}
