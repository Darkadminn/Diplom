using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MobileApplication
{
    public class ApiService
    {
        private HttpClient _httpClient;
        //private JsonSerializerOptions _jsonOptions;


        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:5112") };

            /*_jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };*/

        }

        public async Task<UserAuthorizationPatient> AuthorizationPacient(string login, string password)
        {
            try
            {
                var request = new LoginRequest
                {
                    login = login,
                    password = password
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/authorization/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<UserAuthorizationPatient>(responseJson)
                        ?? new UserAuthorizationPatient { id = -1, success = false, message = "Ошибка десериализации" };
                }
                else
                {
                    return new UserAuthorizationPatient
                    {
                        id = -1,
                        success = false,
                        message = $"Ошибка HTTP: {response.StatusCode}"

                    };
                }
            }
            catch (HttpRequestException ex)
            {
                return new UserAuthorizationPatient
                {
                    id = -1,
                    success = false,
                    message = $"Нет подключения к серверу: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new UserAuthorizationPatient
                {
                    id = -1,
                    success = false,
                    message = $"Ошибка: {ex.Message}"
                };
            }




        }



    }
}
