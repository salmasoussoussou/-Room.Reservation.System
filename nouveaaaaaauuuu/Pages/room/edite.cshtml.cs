using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using nouveaaaaaauuuu.models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;

namespace nouveaaaaaauuuu.Pages.Room
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public RoomViewModel Room { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7165/api/room/{id}");
                if (response.IsSuccessStatusCode)
                {
                    Room = await response.Content.ReadFromJsonAsync<RoomViewModel>();
                    if (Room == null)
                    {
                        return NotFound();
                    }
                }
                else
                {
                    ErrorMessage = $"Erreur lors du chargement de la salle.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Exception lors du chargement de la salle: {ex.Message}";
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7165/api/room/{id}", Room);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Room/Index");
                }
                else
                {
                    ErrorMessage = $"Erreur lors de la mise à jour de la salle.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Exception lors de la mise à jour de la salle: {ex.Message}";
                return Page();
            }
        }
    }

    public class RoomViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nom de la Salle")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Capacité")]
        public int Capacity { get; set; }

        [Required]
        [Display(Name = "Disponibilité")]
        public bool IsAvailable { get; set; }
    }
}
