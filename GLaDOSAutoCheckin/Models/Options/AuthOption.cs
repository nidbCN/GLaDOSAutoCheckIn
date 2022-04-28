namespace GLaDOSAutoCheckIn.Models.Options;

public class AuthOption
{
#nullable disable
    public string MailAccount { get; set; }

    public string Password { get; set; }

    public ushort MailPort { get; set; } = 143;
#nullable restore

    public string? MailHost { get; set; }
    public bool UseSSL { get; set; } = false;
}
