using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace nouveaaaaaauuuu.Pages.room
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public RoomViewModel Room { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Cette m�thode peut �tre laiss�e vide si aucune initialisation n'est n�cessaire.
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Donn�es invalides.";
                return Page();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7165/api/room", Room);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }

                // Affiche les erreurs de r�ponse
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Erreur lors de l'ajout de la salle. D�tails: {errorContent}";
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Erreur lors de l'ajout de la salle: {ex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Exception lors de l'ajout de la salle: {ex.Message}";
            }

            return Page();
        }

        public class RoomViewModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Le nom est requis.")]
            public string Name { get; set; }

            [Range(1, 500, ErrorMessage = "La capacit� doit �tre comprise entre 1 et 500.")]
            public int Capacity { get; set; }

            public bool IsAvailable { get; set; }
        }
    }
}
