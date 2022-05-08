using MimeKit;

namespace GLaDOSAutoCheckIn.Worker.Services;

public interface IMailService
{
    public void Initialize();

    public bool TryGetAuthMail(Predicate<MimeMessage?> match, out MimeMessage mailObj);
}
