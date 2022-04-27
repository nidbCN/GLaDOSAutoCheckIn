using MimeKit;

namespace GLaDOSAutoCheckIn.Worker.Services;

public interface IMailService
{
    public void Initialize();

    public bool TryGetAuthMail(out MimeMessage mailObj);
}
