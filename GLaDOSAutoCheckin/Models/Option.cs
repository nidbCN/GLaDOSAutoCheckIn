namespace GLaDOSAutoCheckin.Models;

public class Option
{
#nullable disable
    public string MailAccount { get; set; }
    public string Password { get; set; }
    public string BaseUrl { get; set; } = "https://glados.rocks/api/";
    public ushort MailPort { get; set; } = 143;
#nullable restore
    public string? MailHost { get; set; }
}
