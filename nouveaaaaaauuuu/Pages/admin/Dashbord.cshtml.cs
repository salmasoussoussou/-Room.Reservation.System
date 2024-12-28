using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace nouveaaaaaauuuu.Pages.admin
{
    public class AdminLoginModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public AdminLoginModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [BindProperty]
        public AdminLoginInputModel Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Vérifier les informations d'identification (remplacez cette logique par la vérification réelle)
                if (Input.Username == "admin" && Input.Password == "password") // Remplacez par votre logique d'authentification
                {
                    
                    // Stocker les informations d'identification de l'administrateur dans une session ou un cookie
                    HttpContext.Session.SetString("AdminUsername", Input.Username);
                   
                    // Redirection vers la page de tableau de bord de l'administrateur
                    return RedirectToPage("/Reservations/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nom d'utilisateur ou mot de passe invalide.");
                }
            }

            return Page();
        }
    }

    public class AdminLoginInputModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
