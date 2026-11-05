using System;

namespace AssetTrackingSystem.Models
{
    public class MobilePhone : Asset
    {
        public MobilePhone(string brand, string model, DateTime purchaseDate, decimal priceUSD, string office, string assetType) : base(brand, model, purchaseDate, priceUSD, office, assetType)
        {
        }
        public override string GetAssetCategory()
        {
            return "Mobile Phone";
        }
    }
}
