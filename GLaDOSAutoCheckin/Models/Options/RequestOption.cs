namespace GLaDOSAutoCheckIn.Models.Options;

public class RequestOption
{
    public string BaseUrl { get; set; } = "https://glados.rocks/api";
    public double TimeOut { get; set; } = 1200; // 1.2s
    public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";
    public string? Cookie { get; set; } = null;
}
