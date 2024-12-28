using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace nouveaaaaaauuuu.Pages.Reservation
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public ReservationDto Reservation { get; set; } // Utiliser un DTO pour éviter les conflits

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
                var response = await _httpClient.GetAsync($"https://localhost:7165/api/reservation/{id}");
                if (response.IsSuccessStatusCode)
                {
                    Reservation = await response.Content.ReadFromJsonAsync<ReservationDto>();
                    if (Reservation == null)
                    {
                        return NotFound();
                    }
                }
                else
                {
                    ErrorMessage = "Erreur lors du chargement de la réservation.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Une erreur s'est produite : {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7165/api/reservation/{Reservation.Id}", Reservation);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Erreur lors de la modification de la réservation. Détails: {errorContent}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Une erreur s'est produite : {ex.Message}";
            }

            return Page();
        }

        // Utilise un DTO pour éviter les conflits de noms
        public class ReservationDto
        {
            public int Id { get; set; }
            public int RoomId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }
    }
}
