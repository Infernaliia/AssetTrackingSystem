using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AssetTrackingSystem
{
    public class CurrencyExchangeService
    {
        private readonly HttpClient httpClient = new HttpClient();

        public async Task<decimal> GetExchangeRateAsync(string targetCurrency)
        {
            if (targetCurrency != "SEK" &&
                targetCurrency != "TRY" &&
                targetCurrency != "USD")
            {
                throw new ArgumentException("Unsupported currency.", nameof(targetCurrency));
            }

            if (targetCurrency == "USD")
            {
                return 1m;
            }

            try
            {
                string url = $"https://api.frankfurter.dev/v1/latest?base=USD&symbols={Uri.EscapeDataString(targetCurrency)}";

                using HttpResponseMessage response = await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                using JsonDocument document = JsonDocument.Parse(json);

                JsonElement rates = document.RootElement.GetProperty("rates");

                decimal exchangeRate = rates.GetProperty(targetCurrency).GetDecimal();

                return exchangeRate;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Currency API error: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Currency API returned invalid JSON: {ex.Message}");
                throw;
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Currency rate was missing: {ex.Message}");
                throw;
            }
        }
    }
}

