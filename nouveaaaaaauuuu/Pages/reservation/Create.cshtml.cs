using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace nouveaaaaaauuuu.Pages.Reservation
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public Reservation NewReservation { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (NewReservation.RoomId <= 0)
            {
                ErrorMessage = "ID de la salle invalide.";
                return Page();
            }

            var roomResponse = await _httpClient.GetAsync($"https://localhost:7165/api/room/{NewReservation.RoomId}");
            if (!roomResponse.IsSuccessStatusCode)
            {
                ErrorMessage = "Salle avec cet ID non trouvée.";
                return Page();
            }

            var room = await roomResponse.Content.ReadFromJsonAsync<RoomViewModel>();
            if (room == null)
            {
                ErrorMessage = "Salle avec cet ID non trouvée.";
                return Page();
            }

            if (!room.IsAvailable)
            {
                ErrorMessage = "Salle non disponible.";
                return Page();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7165/api/reservation", NewReservation);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Erreur lors de la création de la réservation. Détails: {errorContent}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Une erreur s'est produite : {ex.Message}";
            }

            return Page();
        }

        public class Reservation
        {
            public int Id { get; set; }
            public int RoomId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        public class RoomViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Capacity { get; set; }
            public bool IsAvailable { get; set; }
        }
    }
}
