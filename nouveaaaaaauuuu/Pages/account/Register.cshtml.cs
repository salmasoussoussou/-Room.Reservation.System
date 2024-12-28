using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace nouveaaaaaauuuu.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public RegisterModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.Password != Input.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }

            var registerData = new
            {
                UserName = Input.UserName,
                Email = Input.Email,
                Password = Input.Password,
                ConfirmPassword = Input.ConfirmPassword
            };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7165/api/account/register", registerData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Account/Login");
            }
            else
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                if (errorResponse != null && errorResponse.Errors != null)
                {
                    foreach (var error in errorResponse.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                
                return Page();
            }
        }

        public class RegisterInputModel
        {
            [Required(ErrorMessage = "Le nom d'utilisateur est requis.")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "L'email est requis.")]
            [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Le mot de passe est requis.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "La confirmation du mot de passe est requise.")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
            public string ConfirmPassword { get; set; }
        }

        public class ErrorResponse
        {
            public List<string> Errors { get; set; }
        }
    }
}
