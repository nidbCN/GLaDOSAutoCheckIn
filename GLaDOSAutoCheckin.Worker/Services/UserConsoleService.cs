using GLaDOSAutoCheckin.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace GLaDOSAutoCheckin.Worker.Services;

public class UserConsoleService : IUserConsoleService
{
    private readonly AuthOption _option;
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserConsoleService> _logger;

    public UserConsoleService(IOptions<AuthOption> option, HttpClient httpClient, ILogger<UserConsoleService> logger)
    {
        _option = option.Value;
        (_httpClient, _logger) = (httpClient, logger);
    }
    public async Task SendVerifyAsync()
    {
        var resp = await _httpClient.PostAsync("authorization",
              JsonContent.Create(new
              {
                  address = _option.MailAccount,
                  site = "glados.network"
              }));

        resp.EnsureSuccessStatusCode();
    }

    public async Task LoginAsync(string code)
    {
        var resp = await _httpClient.PostAsync("login",
               JsonContent.Create(new
               {
                   method = "email",
                   site = "glados.network",
                   email = _option.MailAccount,
                   mailcode = code
               }));

        resp.EnsureSuccessStatusCode();
    }

    public Task CheckIn()
    {
        throw new NotImplementedException();
    }

    public async Task GetStatus()
    {
        var resp = await _httpClient.GetAsync("user/status");

        resp.EnsureSuccessStatusCode();

        _logger.LogInformation("User status: {s}", JsonSerializer.Serialize(
            resp.Content.ReadAsStringAsync().Result));
    }
}
