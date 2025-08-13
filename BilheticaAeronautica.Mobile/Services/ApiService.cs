using BilheticaAeronautica.Mobile.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://8dlchknh-7244.uks1.devtunnels.ms/";
        private readonly ILogger<ApiService> _logger;
        JsonSerializerOptions _serializerOptions;
        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<IEnumerable<Ticket>> GetTicketsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_baseUrl}api/tickets");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Ticket>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return Enumerable.Empty<Ticket>();
        }

        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
                var login = new Login
                {
                    Username = email,
                    Password = password
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Users/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"HTTP request failed with status: {response.StatusCode}");

                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"HTTP request failed with status: {response.StatusCode}"
                    };
                }
                var jsonResult = await response.Content.ReadAsStringAsync();

                try
                {
                    var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                    if (result == null || string.IsNullOrEmpty(result.AccessToken))
                    {
                        return new ApiResponse<bool>
                        {
                            ErrorMessage = "Failed to get access token from API response."
                        };
                    }
                    Preferences.Set("accessToken", result.AccessToken!);
                    Preferences.Set("userId", result.UserId!);
                    Preferences.Set("userName", result.UserName);

                    
                }
                catch (JsonException ex)
                {
                    Console.WriteLine("Deserialization error: " + ex.Message);
                }

                return new ApiResponse<bool> { Data = true };

            }

            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                return new ApiResponse<bool>
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var urlAddress = _baseUrl + uri;

            try
            {
                var result = await _httpClient.PostAsync(urlAddress, content);
                return result;
            }
            catch (Exception ex)
            {
                // Log o erro ou trate conforme necessário
                _logger.LogError($"Error when sending POST request to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }


        //TODO sort the password recovery
        public async Task<ApiResponse<bool>> RecoverPassword()
        {
            RecoverPassword recoverPasswordModel = new RecoverPassword
            {
                Email = Preferences.Get("userName", "")
            };

            var json = JsonSerializer.Serialize(recoverPasswordModel, _serializerOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await PostRequest("api/Users/RecoverPassword", content);

            return new ApiResponse<bool> { Data = true };
        }


        public async Task<ApiResponse<bool>> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            ChangePassword changePasswordModel = new ChangePassword
            {
                Email = Preferences.Get("userName", ""),
                OldPassword = oldPassword,
                NewPassword = newPassword,
                Confirm = confirmPassword
            };

            var json = JsonSerializer.Serialize(changePasswordModel, _serializerOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await PostRequest("api/Users/ChangePassword", content);

            return new ApiResponse<bool> { Data = true };
        }


        //public async Task<(List<OrderByUser>?, string? ErrorMessage)> GetTicketsByUser(string userId)
        //{
        //    string endpoint = $"api/Orders/GetOrdersByUser/{usuarioId}";

        //    return await GetAsync<List<OrderByUser>>(endpoint);
        //}
    }
}
