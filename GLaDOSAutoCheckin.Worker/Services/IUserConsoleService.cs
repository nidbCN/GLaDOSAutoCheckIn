namespace GLaDOSAutoCheckin.Worker.Services;

public interface IUserConsoleService
{
    public Task SendVerifyAsync();
    public Task LoginAsync(string code);
    public Task CheckIn();
    public Task GetStatus();
}
