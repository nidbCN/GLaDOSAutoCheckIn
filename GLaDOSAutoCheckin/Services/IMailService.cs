using MimeKit;

namespace GLaDOSAutoCheckin.Services;

public interface IMailService
{
    public void Initlaze();

    public bool TryGetAuthMail(out MimeMessage? mailObj);

    public bool TryGetAuthMail(Predicate<MimeMessage> match, out MimeMessage? mailObj);
}
