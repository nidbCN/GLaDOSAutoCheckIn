using CommandLine;

namespace GLaDOSAutoCheckin.Models;

public class Option
{
#nullable disable
    [Value(0)]
    public string MailAccount { get; set; }

    [Value(1)]
    public string Password { get; set; }

    [Option('u', "baseurl")]
    public string BaseUrl { get; set; } = "https://glados.rocks/api/";

    [Option('p', "port")]
    public ushort MailPort { get; set; } = 143;
#nullable restore
    
    [Option('h', "host")]
    public string? MailHost { get; set; }
}
