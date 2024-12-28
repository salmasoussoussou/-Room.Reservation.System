using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using nouveaaaaaauuuu.models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace nouveaaaaaauuuu.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public LoginModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; }

        public void OnGet()
        {
            // Rien à faire lors de la demande GET
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var loginData = new
            {
                UserName = Input.UserName,
                Password = Input.Password
            };

            // Envoyer les données de connexion à l'API
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7165/api/account/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();

                // Sauvegarder le token JWT dans la session
                HttpContext.Session.SetString("JWToken", result.Token);

                // Rediriger vers la page d'accueil
                return RedirectToPage("/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Échec de la connexion.");
                return Page();
            }
        }

        public class LoginInputModel
        {
            [Required(ErrorMessage = "Le nom d'utilisateur est requis.")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Le mot de passe est requis.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public class LoginResult
        {
            public string Token { get; set; }
            public DateTime Expiration { get; set; }
        }
    }
}
