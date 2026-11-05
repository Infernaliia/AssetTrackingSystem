using AssetTrackingSystem;
using AssetTrackingSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;

CurrencyExchangeService currencyService = new CurrencyExchangeService();

try
{
    decimal sekRate = await currencyService.GetExchangeRateAsync("SEK");
    decimal tryRate = await currencyService.GetExchangeRateAsync("TRY");

    Asset.SetExchangeRates(sekRate, tryRate);

    Console.WriteLine("Currency exchange rates updated successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"Unable to retrieve exchange rates. Using fallback rates. {ex.Message}");
}

List<Asset> assets = new List<Asset>();
AssetFileManager fileManager = new AssetFileManager();
assets = fileManager.LoadAssets();

if (!File.Exists("assets.json"))
{
    Computer computer = new Computer(
    "Dell",
    "XPS 15",
    new DateTime(2023, 9, 15),
    1500.00m,
    "USA",
    "Computer"
    );
    assets.Add(computer);

    Computer secondComputer = new Computer(
        "HP",
        "Spectre x360",
        new DateTime(2024, 3, 15),
        1300.00m,
        "Sweden",
        "Computer"
    );
    assets.Add(secondComputer);

    MobilePhone mobilePhone = new MobilePhone(
        "Apple",
        "iPhone 13",
        new DateTime(2025, 5, 15),
        999.00m,
        "Turkey",
        "Mobile Phone"
    );
    assets.Add(mobilePhone);
    fileManager.SaveAssets(assets);
}

bool running = true;

while (running)
{
    Console.WriteLine("\n===== ASSET TRACKING SYSTEM =====");
    Console.WriteLine("1. Display all assets");
    Console.WriteLine("2. Exit");
    Console.WriteLine("3. Add a new asset");
    Console.WriteLine("4. Save assets");
    Console.WriteLine("5. Delete asset");
    Console.WriteLine("6. Display assets by office");
    Console.WriteLine("7. Export assets to CSV");
    Console.WriteLine("8. Export assets to Excel");
    Console.Write("Choose an option: ");

    string choice = Console.ReadLine() ?? "";

    switch (choice)
    {
        case "1":
            foreach (Asset asset in assets.OrderBy(a => a.AssetType)
                              .ThenBy(a => a.PurchaseDate))
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                string assetStatus = asset.GetAssetStatus();

                switch (assetStatus)
                {
                    case "Red":
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case "Yellow":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                }

                Console.WriteLine($"Asset Status: {assetStatus}");
                Console.ForegroundColor = originalColor;
                Console.WriteLine($"End of Life: {asset.GetEndOfLifeDate():yyyy-MM-dd}");
                Console.WriteLine($"Remaining life: {asset.GetRemainingLife().TotalDays:F2} days");
                Console.WriteLine($"Brand: {asset.Brand}, Model: {asset.Model}, Purchase Date: {asset.PurchaseDate.ToShortDateString()}, Price (USD): {asset.PriceUSD:F2}, Price (Local): {asset.PriceLocal:F2}, Office: {asset.Office}, Asset Type: {asset.AssetType}");
                Console.WriteLine($"Asset Category: {asset.GetAssetCategory()}");
                int age = asset.GetAssetAge();
                string yearText = age == 1 ? "year" : "years";
                Console.WriteLine($"Asset Age: {age} {yearText}");
                Console.WriteLine($"Asset ID: {asset.AssetId}");
                Console.WriteLine($"Local Price: {asset.PriceLocal:F2} {asset.CurrencyCode}");
            }
            break;

        case "4":
            fileManager.SaveAssets(assets);
            break;

        case "5":
            fileManager.DeleteAsset(assets);
            break;

        case "6":
            foreach (Asset asset in assets.OrderBy(a => a.Office)
                                          .ThenBy(a => a.PurchaseDate))
            {
                Console.WriteLine($"Asset ID: {asset.AssetId}");
                Console.WriteLine($"Brand: {asset.Brand}");
                Console.WriteLine($"Model: {asset.Model}");
                Console.WriteLine($"Purchase Date: {asset.PurchaseDate:yyyy-MM-dd}");
                Console.WriteLine($"Office: {asset.Office}");
                Console.WriteLine($"Asset Type: {asset.AssetType}");
                int age = asset.GetAssetAge();
                string yearText = age == 1 ? "year" : "years";
                Console.WriteLine($"Asset Age: {age} {yearText}");
                Console.WriteLine($"Asset Status: {asset.GetAssetStatus()}");
                Console.WriteLine($"End of Life: {asset.GetEndOfLifeDate():yyyy-MM-dd}");
                Console.WriteLine($"Price: {asset.PriceLocal:F2} {asset.CurrencyCode}");
                Console.WriteLine("-----------------------------------");
            }
            break;
        case "7":
            Console.WriteLine("Enter the file path for the CSV export:");
            string csvPath = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(csvPath))
            {
                Console.WriteLine("File path cannot be empty.");
                break;
            }

            fileManager.ExportAssetsToCsv(assets, csvPath);
            break;
        case "8":
            Console.WriteLine("Enter the file path for the Excel export:");
            string excelPath = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(excelPath))
            {
                Console.WriteLine("File path cannot be empty.");
                break;
            }

            fileManager.ExportAssetsToExcel(assets, excelPath);
            break;
        case "3":
            Console.WriteLine("Select asset type:");
            Console.WriteLine("1. Computer");
            Console.WriteLine("2. Mobile Phone");

            string assetChoice = Console.ReadLine() ?? "";
            switch (assetChoice)
            {
                case "1":
                    {
                        string brand = GetValidBrand("computer");
                        string model = GetValidModel("computer");

                        Console.WriteLine($"Brand entered: {brand}");
                        Console.WriteLine($"Model entered: {model}");

                        DateTime purchaseDate;

                        while (true)
                        {
                            Console.WriteLine("Enter the purchase date (YYYY-MM-DD):");
                            string dateInput = Console.ReadLine() ?? "";

                            if (DateTime.TryParseExact(
                                dateInput,
                                "yyyy-MM-dd",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out purchaseDate) &&
                                purchaseDate <= DateTime.Today)
                            {
                                break;
                            }

                            Console.WriteLine("Invalid date. Please enter a valid date.");
                        }

                        Console.WriteLine($"Purchase date entered: {purchaseDate:yyyy-MM-dd}");

                        decimal priceUSD;

                        while (true)
                        {
                            Console.WriteLine("Enter the purchase price in USD:");
                            string priceInput = Console.ReadLine() ?? "";

                            if (decimal.TryParse(priceInput, out priceUSD) && priceUSD >= 0)
                            {
                                break;
                            }

                            Console.WriteLine("Invalid price. Please enter a valid non-negative number.");
                        }

                        Console.WriteLine($"Purchase price entered: {priceUSD:F2} USD");

                        string office;

                        while (true)
                        {
                            Console.WriteLine("Enter the office location (Sweden, USA, Turkey):");
                            string officeInput = Console.ReadLine() ?? "";
                            officeInput = officeInput.Trim();

                            if (officeInput.Equals("Sweden", StringComparison.OrdinalIgnoreCase))
                            {
                                office = "Sweden";
                                break;
                            }
                            else if (officeInput.Equals("USA", StringComparison.OrdinalIgnoreCase))
                            {
                                office = "USA";
                                break;
                            }
                            else if (officeInput.Equals("Turkey", StringComparison.OrdinalIgnoreCase))
                            {
                                office = "Turkey";
                                break;
                            }

                            Console.WriteLine("Invalid office. Please enter Sweden, USA, or Turkey.");
                        }

                        Console.WriteLine($"Office entered: {office}");


                        Computer newComputer = new Computer(
                            brand,
                            model,
                            purchaseDate,
                            priceUSD,
                            office,
                            "Computer"
                        );

                        assets.Add(newComputer);

                        Console.WriteLine("Computer added successfully!");
                        fileManager.SaveAssets(assets);

                        break;
                    }

                case "2":
                    {
                        string brand = GetValidBrand("mobile phone");
                        string model = GetValidModel("mobile phone");

                        Console.WriteLine($"Brand entered: {brand}");
                        Console.WriteLine($"Model entered: {model}");

                        DateTime purchaseDate;

                        while (true)
                        {
                            Console.WriteLine("Enter the purchase date (YYYY-MM-DD):");
                            string dateInput = Console.ReadLine() ?? "";

                            if (DateTime.TryParseExact(
                                dateInput,
                                "yyyy-MM-dd",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out purchaseDate) &&
                                purchaseDate <= DateTime.Today)
                            {
                                break;
                            }

                            Console.WriteLine("Invalid date. Please enter a valid date.");
                        }

                        Console.WriteLine($"Purchase date entered: {purchaseDate:yyyy-MM-dd}");

                        decimal priceUSD;

                        while (true)
                        {
                            Console.WriteLine("Enter the purchase price in USD:");
                            string priceInput = Console.ReadLine() ?? "";

                            if (decimal.TryParse(priceInput, out priceUSD) && priceUSD >= 0)
                            {
                                break;
                            }

                            Console.WriteLine("Invalid price. Please enter a valid non-negative number.");
                        }
                        Console.WriteLine($"Purchase price entered: {priceUSD:F2} USD");

                        string office;

                        while (true)
                        {
                            Console.WriteLine("Enter the office location (Sweden, USA, Turkey):");
                            string officeInput = Console.ReadLine() ?? "";
                            officeInput = officeInput.Trim();

                            if (officeInput.Equals("Sweden", StringComparison.OrdinalIgnoreCase))
                            {
                                office = "Sweden";
                                break;
                            }
                            else if (officeInput.Equals("USA", StringComparison.OrdinalIgnoreCase))
                            {
                                office = "USA";
                                break;
                            }
                            else if (officeInput.Equals("Turkey", StringComparison.OrdinalIgnoreCase))
                            {
                                office = "Turkey";
                                break;
                            }

                            Console.WriteLine("Invalid office. Please enter Sweden, USA, or Turkey.");
                        }

                        Console.WriteLine($"Office entered: {office}");


                        MobilePhone newMobilePhone = new MobilePhone(
                            brand,
                            model,
                            purchaseDate,
                            priceUSD,
                            office,
                            "Mobile Phone"
                        );

                        assets.Add(newMobilePhone);

                        Console.WriteLine("Mobile phone added successfully!");
                        fileManager.SaveAssets(assets);

                        break;
                    }

                default:
                    Console.WriteLine("Invalid asset type.");

                    break;
            }

            break;

        case "2":
            running = false;
            Console.WriteLine("Exiting application...");
            break;

        default:
            Console.WriteLine("Invalid option. Try again.");
            break;
    }
}

string GetValidBrand(string assetType)
{
    while (true)
    {
        Console.WriteLine($"Enter the {assetType} brand:");
        string brand = Console.ReadLine() ?? "";

        if (!string.IsNullOrWhiteSpace(brand))
        {
            return brand;
        }

        Console.WriteLine("Brand cannot be empty. Please try again.");
    }
}
string GetValidModel(string assetType)
{
    while (true)
    {
        Console.WriteLine($"Enter the {assetType} model:");
        string model = Console.ReadLine() ?? "";

        if (!string.IsNullOrWhiteSpace(model))
        {
            return model;
        }

        Console.WriteLine("Model cannot be empty. Please try again.");
    }
}