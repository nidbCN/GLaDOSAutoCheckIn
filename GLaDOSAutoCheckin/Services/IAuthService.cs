namespace GLaDOSAutoCheckin.Services;

public interface IAuthService
{
    public Task SendVerifyAsync();
    public Task LoginAsync(string code);
}
