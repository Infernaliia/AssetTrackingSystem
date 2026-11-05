using System;

namespace AssetTrackingSystem.Models
{
    public class Computer : Asset
    {
        public Computer(string brand, string model, DateTime purchaseDate, decimal priceUSD, string office, string assetType) : base(brand, model, purchaseDate, priceUSD, office, assetType)
        {
        }

        public override string GetAssetCategory()
        {
            return "Computers";
        }
    }
}
