using System.Globalization;
using System.Text.Json;
using ToptalFinialSolution.Application.Interfaces;

namespace ToptalFinialSolution.Infrastructure.Services;

public class GeocodingService(HttpClient httpClient) : IGeocodingService
{
    public async Task<(double Latitude, double Longitude)?> GeocodeAddressAsync(string address, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RestaurantReviewAPI/1.0");

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var results = JsonSerializer.Deserialize<List<NominatimResult>>(content);

            if (results is not { Count: > 0 })
            {
                return null;
            }

            var result = results[0];
            return (
                double.Parse(result.Lat, CultureInfo.InvariantCulture),
                double.Parse(result.Lon, CultureInfo.InvariantCulture)
            );
        }
        catch (Exception) when (true)
        {
            return null;
        }
    }

    private record NominatimResult
    {
        [System.Text.Json.Serialization.JsonPropertyName("lat")]
        public required string Lat { get; init; }
        [System.Text.Json.Serialization.JsonPropertyName("lon")]
        public required string Lon { get; init; }
    }
}
