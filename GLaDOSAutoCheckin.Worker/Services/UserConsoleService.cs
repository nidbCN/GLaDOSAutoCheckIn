using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using GLaDOSAutoCheckIn.Models;
using GLaDOSAutoCheckIn.Models.Options;
using GLaDOSAutoCheckIn.Models.ResponseModels;

namespace GLaDOSAutoCheckIn.Worker.Services;

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
    public async Task RequireVerifyAsync()
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
        _logger.LogInformation("Login success, {msg}", await resp.Content.ReadAsStringAsync());
    }

    public async Task CheckIn()
    {
        var resp = await _httpClient.PostAsync("user/checkin",
            JsonContent.Create(new
            {
                token = "glados_network"
            }));

        resp.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<CheckInResponse>(
            await resp.Content.ReadAsStringAsync());

        if (result is null || result.Code != 0)
        {
            // failed
            _logger.LogError("Error occurred during check-in, message {msg}", result?.Message);
            return;
        }

        _logger.LogInformation("CheckIn success, message {msg}", result.Message);
    }

    public Task GetStatus()
    {
        throw new NotImplementedException();

        //var resp = await _httpClient.GetAsync("user/status");

        //resp.EnsureSuccessStatusCode();

        //_logger.LogInformation("User status: {s}",
        //    resp.Content.ReadAsStringAsync().Result);
    }
}
