using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using nouveaaaaaauuuu.models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace nouveaaaaaauuuu.Pages.Room
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public List<RoomViewModel> Rooms { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7165/api/room");
                if (response.IsSuccessStatusCode)
                {
                    var allRooms = await response.Content.ReadFromJsonAsync<List<RoomViewModel>>();

                    // Filtrer les salles en fonction du terme de recherche
                    if (!string.IsNullOrEmpty(SearchTerm))
                    {
                        Rooms = allRooms
                            .Where(r => r.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                    }
                    else
                    {
                        Rooms = allRooms;
                    }
                }
                else
                {
                    Rooms = new List<RoomViewModel>();
                    TempData["ErrorMessage"] = "Erreur lors du chargement des salles.";
                }
            }
            catch (Exception ex)
            {
                Rooms = new List<RoomViewModel>();
                TempData["ErrorMessage"] = $"Exception lors du chargement des salles: {ex.Message}";
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

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.DeleteAsync($"https://localhost:7165/api/room/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Salle supprimée avec succès.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Erreur lors de la suppression de la salle.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Exception lors de la suppression de la salle: {ex.Message}";
            }

            return RedirectToPage();
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
