using System;
using System.Text.Json.Serialization;

namespace AssetTrackingSystem.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(Computer), "Computer")]
    [JsonDerivedType(typeof(MobilePhone), "MobilePhone")]
    public abstract class Asset
    {
        public Asset(string brand, string model, DateTime purchaseDate, decimal priceUSD, string office, string assetType)
        {
            Brand = brand;
            Model = model;
            PurchaseDate = purchaseDate;
            PriceUSD = priceUSD;
            Office = office;
            PriceLocal = ConvertPriceToLocalCurrency();
            AssetType = assetType;
            AssetId = nextAssetId++;
        }

        public string Brand { get; set; }
        public string Model { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int AssetId { get; set; }
        public decimal PriceUSD { get; set; }
        public decimal PriceLocal { get; set; }
        public string Office { get; set; }
        public string CurrencyCode
        {
            get
            {
                return Office switch
                {
                    "Sweden" => "SEK",
                    "USA" => "USD",
                    "Turkey" => "TRY",
                    _ => "Unknown"
                };
            }
        }
        public decimal ConvertPriceToLocalCurrency()
        {
            return Office switch
            {
                "Sweden" => PriceUSD * usdToSekRate,
                "USA" => PriceUSD,
                "Turkey" => PriceUSD * usdToTryRate,
                _ => 0m
            };
        }
        public string AssetType { get; set; }
        public DateTime GetEndOfLifeDate()
        {
            return PurchaseDate.AddYears(3);
        }
        public int GetAssetAge()
        {
            DateTime today = DateTime.Today;

            int age = today.Year - PurchaseDate.Year;

            if (PurchaseDate.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
        public TimeSpan GetRemainingLife()
        {
            DateTime date1 = DateTime.Now;
            DateTime date2 = GetEndOfLifeDate();

            return date2 - date1;
        }
        public string GetAssetStatus()
        {
            DateTime endOfLife = GetEndOfLifeDate();
            DateTime today = DateTime.Today;

            if (endOfLife <= today.AddMonths(3))
            {
                return "Red";
            }
            else if (endOfLife <= today.AddMonths(6))
            {
                return "Yellow";
            }
            else
            {
                return "Green";
            }
        }
        public abstract string GetAssetCategory();
        private static int nextAssetId = 1;
        private static decimal usdToSekRate = 10m;
        private static decimal usdToTryRate = 30m;

        public static void SetExchangeRates(decimal sekRate, decimal tryRate)
        {
            if (sekRate <= 0 || tryRate <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "Exchange rates must be greater than zero.");
            }
            usdToSekRate = sekRate;
            usdToTryRate = tryRate;
        }
        public static void UpdateNextAssetId(int highestId)
        {
            if (nextAssetId <= highestId)
            {
                nextAssetId = highestId + 1;
            }
        }
    }
}
