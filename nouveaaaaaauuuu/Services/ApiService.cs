using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace nouveaaaaaauuuu.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(string userName, string password)
        {
            var loginData = new
            {
                UserName = userName,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/account/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                // Save JWT token (e.g., in local storage)
                // Example: localStorage.SetItem("authToken", result.Token);
                return true;
            }

            return false;
        }

        public async Task RegisterAsync(string userName, string password)
        {
            var registerData = new
            {
                UserName = userName,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/account/register", registerData);

            response.EnsureSuccessStatusCode();
        }

        public class LoginResult
        {
            public string Token { get; set; }
            public DateTime Expiration { get; set; }
        }
    }
}
