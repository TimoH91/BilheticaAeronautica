using BilheticaAeronautica.Mobile.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;

using System.Text;
using System.Text.Json;



namespace BilheticaAeronautica.Mobile.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = AppConfig.BaseUrl;
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
            AddAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{_baseUrl}api/tickets");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Ticket>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return Enumerable.Empty<Ticket>();
        }


        public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
        {

            var response = await _httpClient.GetAsync($"{_baseUrl}api/flights/GetAllFutureFlights");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Flight>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return Enumerable.Empty<Flight>();
        }

        public async Task<IEnumerable<Flight>> GetFlightsMobileAsync(
            int? originAirportId = null,
            int? destinationAirportId = null,
            DateTime? departureDate = null)

        {
            var queryParams = new List<string>();

            if (originAirportId.HasValue)
                queryParams.Add($"originAirportId={originAirportId.Value}");

            if (destinationAirportId.HasValue)
                queryParams.Add($"destinationAirportId={destinationAirportId.Value}");

            if (departureDate.HasValue)
                queryParams.Add($"date={departureDate.Value:yyyy-MM-dd}");

            //if (returnDate.HasValue)
            //    queryParams.Add($"date={returnDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Count > 0
                ? "?" + string.Join("&", queryParams)
                : string.Empty;

            var response = await _httpClient.GetAsync($"{_baseUrl}api/Flights/flights{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Flight>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return Enumerable.Empty<Flight>();
        }

        public async Task<IEnumerable<Airport>> GetAllAirportsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/Airports/GetAllAirports");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Airport>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return Enumerable.Empty<Airport>();

        }


        public async Task<IEnumerable<Seat>> GetSeatsByFlightAsync(int flightId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/Seats/GetSeats/{flightId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Seat>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return Enumerable.Empty<Seat>();
        }

      

        public async Task<ApiResponse<bool>> RegisterUser(string name, string surname, string email, string password, string confirmPassword)
        {
            try
            {
                var register = new RegisterNewUser()
                {
                    FirstName = name,
                    LastName = surname,
                    Username = email,
                    Password = password,
                    Confirm = confirmPassword
                };

                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Users/Register", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                    };
                }

                var json2 = await response.Content.ReadAsStringAsync();
                var json3 = JsonSerializer.Deserialize<RegisterUserResponse>(json2, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return new ApiResponse<bool>
                {
                    Data = true,
                    RegisterUser = json3
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao registrar o usuário: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
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
                    Preferences.Set("accesstoken", result.AccessToken!);
                    Preferences.Set("userid", result.UserId!);
                    Preferences.Set("username", result.UserName);
                    Preferences.Set("firstname", result.FirstName);
                    Preferences.Set("lastname", result.LastName);


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

        public async Task<ApiResponse<bool>> HoldSeat(int seatId)
        {
            //var response = await _httpClient.PostAsync($"{_baseUrl}api/Seats/{seatId}/hold", null);

            var result = await PostRequest($"api/Seats/{seatId}/hold", null);

            return new ApiResponse<bool> { Data = true};

            //return new ApiResponse<bool> { Data = false };
        }


        //TODO sort the password recovery
        public async Task<ApiResponse<bool>> RecoverPassword(string email)
        {

            RecoverPassword recoverPasswordModel = new RecoverPassword
            {
                Email = email
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
                Email = Preferences.Get("username", ""),
                OldPassword = oldPassword,
                NewPassword = newPassword,
                Confirm = confirmPassword
            };

            var json = JsonSerializer.Serialize(changePasswordModel, _serializerOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await PostRequest("api/Users/ChangePassword", content);

            return new ApiResponse<bool> { Data = true };
        }

        public async Task<ApiResponse<bool>> ChangeUserInfo(string? name, string? lastName)
        {
            ChangeUser changeUser = new ChangeUser
            {
                FirstName = name ?? Preferences.Get("firstname", string.Empty),
                LastName = lastName ?? Preferences.Get("lastname", string.Empty),
            };

            var json = JsonSerializer.Serialize(changeUser, _serializerOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await PutRequest("api/Users/changeuserinfo", content);

            if (result.IsSuccessStatusCode)
            {
                Preferences.Set("firstname", name);
                Preferences.Set("lastname", lastName);
            }

            return new ApiResponse<bool> { Data = true };
        }


        public async Task<ApiResponse<bool>> ConfirmOrder(List<ShoppingBasketTicket> tickets)
        {
            var json = JsonSerializer.Serialize(tickets, _serializerOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await PostRequest("api/Orders/ConfirmOrder", content);

            return new ApiResponse<bool> { Data = true };
        }

        public async Task<ApiResponse<bool>> UploadUserImage(byte[] imageArray)
        {
            try
            {
                AddAuthorizationHeader();

                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(imageArray), "image", "image.jpg");
                var response = await PostRequest("api/Users/uploadphoto", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                      ? "Unauthorized"
                      : $"Error when sending HTTP request: {response.StatusCode}";

                    _logger.LogError($"Error when sending HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when uploading user profile image: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<(ProfileImage? ProfileImage, string? ErrorMessage)> GetUserProfileImage()
        {
            string endpoint = "api/Users/profileimage";
            return await GetAsync<ProfileImage>(endpoint);
        }

        private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Erro de requisição HTTP: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"Erro de desserialização JSON: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro inesperado: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
        }

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<HttpResponseMessage> PutRequest(string uri, HttpContent content)
        {
            var urlAddress = AppConfig.BaseUrl + uri;

            try
            {
                AddAuthorizationHeader();
                var result = await _httpClient.PutAsync(urlAddress, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when sending PUT request for {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent? content)
        {
            var urlAddress = AppConfig.BaseUrl + uri;

            try
            {
                var result = await _httpClient.PostAsync(urlAddress, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when sending POST request to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }


        //public async Task<(List<OrderByUser>?, string? ErrorMessage)> GetTicketsByUser(string userId)
        //{
        //    string endpoint = $"api/Orders/GetOrdersByUser/{usuarioId}";

        //    return await GetAsync<List<OrderByUser>>(endpoint);
        //}
    }
}
