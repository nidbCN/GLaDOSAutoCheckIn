using GLaDOSAutoCheckin.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace GLaDOSAutoCheckin.Services;

public class AuthService : IAuthService
{
    private readonly AuthOption _option;
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IOptions<AuthOption> option, HttpClient httpClient, ILogger<AuthService> logger)
    {
        _option = option.Value;
        (_httpClient, _logger) = (httpClient, logger);
    }
    public async Task SendVerifyAsync()
        => await _httpClient.PostAsync("authorization",
            JsonContent.Create(new
            {
                address = _option.MailAccount,
                site = "glados.network"
            }));


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

        Console.WriteLine(JsonSerializer.Serialize(resp.Headers));
    }
}
