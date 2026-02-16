using System.Text.Json;
using ToptalFinialSolution.Application.Interfaces;

namespace ToptalFinialSolution.Infrastructure.Services;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(double Latitude, double Longitude)?> GeocodeAddressAsync(string address)
    {
        try
        {
            // Using Nominatim (OpenStreetMap) for geocoding
            // In production, consider using Google Maps API, Azure Maps, or other paid services
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "RestaurantReviewAPI/1.0");
            
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<List<NominatimResult>>(content);

            if (results == null || results.Count == 0)
            {
                return null;
            }

            var result = results[0];
            return (double.Parse(result.lat), double.Parse(result.lon));
        }
        catch
        {
            return null;
        }
    }

    private class NominatimResult
    {
        public string lat { get; set; } = string.Empty;
        public string lon { get; set; } = string.Empty;
    }
}
