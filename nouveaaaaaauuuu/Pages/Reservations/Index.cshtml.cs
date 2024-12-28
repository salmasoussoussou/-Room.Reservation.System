using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace nouveaaaaaauuuu.Pages.Reservations
{

    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Reservations = new List<Reservation>();
        }

        public IList<Reservation> Reservations { get; set; }

        [BindProperty]
        public Reservation NewReservation { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Récupérer toutes les réservations
            var reservationResponse = await _httpClient.GetAsync("https://localhost:7165/api/reservation");
            if (!reservationResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Erreur lors de la récupération des réservations.");
                return Page();
            }

            Reservations = await reservationResponse.Content.ReadFromJsonAsync<IList<Reservation>>();

            // Liste des réservations à supprimer
            var reservationsToDelete = new List<int>();

            // Récupérer les détails des salles pour chaque réservation
            foreach (var reservation in Reservations)
            {
                var roomResponse = await _httpClient.GetAsync($"https://localhost:7165/api/room/{reservation.RoomId}");
                if (roomResponse.IsSuccessStatusCode)
                {
                    var room = await roomResponse.Content.ReadFromJsonAsync<Room>();
                    if (room != null)
                    {
                        reservation.RoomName = room.Name;
                        reservation.RoomCapacity = room.Capacity;
                        reservation.RoomIsAvailable = room.IsAvailable;

                        // Si la salle n'est pas disponible, ajouter la réservation à la liste de suppression
                        if (!room.IsAvailable)
                        {
                            reservationsToDelete.Add(reservation.Id);
                        }
                    }
                }
            }

            // Supprimer les réservations dont les salles ne sont plus disponibles
            foreach (var reservationId in reservationsToDelete)
            {
                var deleteResponse = await _httpClient.DeleteAsync($"https://localhost:7165/api/reservation/{reservationId}");
                if (deleteResponse.IsSuccessStatusCode)
                {
                    // Supprimer la réservation de la liste locale
                    var reservationToRemove = Reservations.FirstOrDefault(r => r.Id == reservationId);
                    if (reservationToRemove != null)
                    {
                        Reservations.Remove(reservationToRemove);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Erreur lors de la suppression de la réservation avec l'ID {reservationId}.");
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"https://localhost:7165/api/reservation/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage();
            }

            ModelState.AddModelError(string.Empty, "Erreur lors de la suppression de la réservation.");
            return Page();
        }

        public class Reservation
        {
            public int Id { get; set; }
            public int RoomId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }

            // Détails de la salle
            public string RoomName { get; set; }
            public int RoomCapacity { get; set; }
            public bool RoomIsAvailable { get; set; }
        }

        public class Room
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Capacity { get; set; }
            public bool IsAvailable { get; set; }
        }
    }
}
