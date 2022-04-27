using GLaDOSAutoCheckin.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace GLaDOSAutoCheckin.Services
{
    public class AuthService
    {
        private readonly AuthOption _option;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;

        public string Account { get => _option.MailAccount; }
        public bool HasLoggedin { get; private set; }

        public AuthService(AuthOption option, HttpClient httpClient, ILogger<AuthService> logger)
            => (_option, _httpClient, _logger) = (option, httpClient, logger);

        public async Task SendVerifyAsync()
            => await _httpClient.PostAsync("authorization",
                JsonContent.Create(new
                {
                    address = Account,
                    site = "glados.network"
                }));


        public async Task LoginAsync(string code)
            => await _httpClient.PostAsync("login",
                JsonContent.Create(new
                {
                    method = "email",
                    site = "glados.network",
                    email = Account,
                    mailcode = code
                }));

    }
}
