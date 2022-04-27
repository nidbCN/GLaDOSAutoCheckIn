namespace GLaDOSAutoCheckIn.Worker.Services;

public interface IUserConsoleService
{
    public Task RequireVerifyAsync();
    public Task LoginAsync(string code);
    public Task CheckIn();
    public Task GetStatus();
}
