namespace GLaDOSAutoCheckIn.Models.ResponseModels;

public class CheckInResponse
{
#nullable disable
    public uint Code { get; set; }
    public string Message { get; set; }
    public IList<object> List { get; set; }
#nullable restore
}
