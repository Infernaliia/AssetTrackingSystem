using AssetTrackingSystem.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AssetTrackingSystem
{
    public class AssetFileManager
    {
        public void SaveAssets(List<Asset> assets)
        {
            string json = JsonSerializer.Serialize(assets, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            try
            {
                File.WriteAllText("assets.json", json);
                Console.WriteLine("Assets saved successfully!");
            }
            catch (IOException)
            {
                Console.WriteLine("Error: Unable to save the assets file.");
            }
        }
        public void ExportAssetsToCsv(List<Asset> assets, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("AssetId,Brand,Model,PurchaseDate,PriceUSD,PriceLocal,Office,CurrencyCode,AssetType");

                    foreach (var asset in assets)
                    {
                        writer.WriteLine($"{asset.AssetId},{EscapeCsvValue(asset.Brand)},{EscapeCsvValue(asset.Model)},{asset.PurchaseDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)},{asset.PriceUSD.ToString("F2", CultureInfo.InvariantCulture)},{asset.PriceLocal.ToString("F2", CultureInfo.InvariantCulture)},{EscapeCsvValue(asset.Office)},{asset.CurrencyCode},{EscapeCsvValue(asset.AssetType)}");
                    }
                }

                Console.WriteLine($"Assets exported to CSV successfully at {filePath}!");
            }
            catch (IOException)
            {
                Console.WriteLine("Error: Unable to write to the CSV file.");
            }
        }
        public void ExportAssetsToExcel(List<Asset> assets, string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Asset Inventory");
                    string[] header =
                    {
                        "Asset Id",
                        "Brand",
                        "Model",
                        "Purchase Date",
                        "Price (USD)",
                        "Price (Local)",
                        "Office",
                        "Currency Code",
                        "Asset Type"
                    };
                    for (int i = 0; i < header.Length; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = header[i];
                    }
                    var headerRange = worksheet.Range(1, 1, 1, header.Length);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Font.FontColor = XLColor.White;
                    headerRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    int currentRow = 2;
                    foreach (var asset in assets)
                    {
                        worksheet.Cell(currentRow, 1).Value = asset.AssetId;
                        worksheet.Cell(currentRow, 2).Value = asset.Brand;
                        worksheet.Cell(currentRow, 3).Value = asset.Model;
                        worksheet.Cell(currentRow, 4).Value = asset.PurchaseDate;
                        worksheet.Cell(currentRow, 5).Value = asset.PriceUSD;
                        worksheet.Cell(currentRow, 6).Value = asset.PriceLocal;
                        worksheet.Cell(currentRow, 7).Value = asset.Office;
                        worksheet.Cell(currentRow, 8).Value = asset.CurrencyCode;
                        worksheet.Cell(currentRow, 9).Value = asset.AssetType;
                        currentRow++;
                    }
                    worksheet.Column(4).Style.DateFormat.Format = "yyyy-MM-dd";
                    worksheet.Column(5).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Column(6).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Columns().AdjustToContents();
                    for (int row = 2; row < currentRow; row++)
                    {
                        if (row % 2 == 0)
                        {
                            worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.AliceBlue;
                        }
                    }
                    worksheet.Range(1, 1, currentRow - 1, header.Length).SetAutoFilter();
                    worksheet.SheetView.FreezeRows(1);
                    workbook.SaveAs(filePath);
                }
                Console.WriteLine($"Assets exported to Excel successfully at {filePath}!");
            }
            catch (IOException)
            {
                Console.WriteLine("Error: Unable to write to the Excel file.");
            }
        }
        private string EscapeCsvValue(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
        public List<Asset> LoadAssets()
        {
            if (!File.Exists("assets.json"))
            {
                return new List<Asset>();
            }

            try
            {
                string json = File.ReadAllText("assets.json");
                List<Asset> loadedAssets = JsonSerializer.Deserialize<List<Asset>>(json)
                                           ?? new List<Asset>();
                if (loadedAssets.Any())
                {
                    Asset.UpdateNextAssetId(loadedAssets.Max(asset => asset.AssetId));
                }
                return loadedAssets;
            }
            catch (JsonException)
            {
                Console.WriteLine("Error: The assets file contains invalid JSON.");
                return new List<Asset>();
            }
            catch (IOException)
            {
                Console.WriteLine("Error: Unable to access the assets file.");
                return new List<Asset>();
            }
        }
        public void DeleteAsset(List<Asset> assets)
        {
            Console.Write("Enter the Asset ID you want to delete: ");
            string input = Console.ReadLine() ?? "";

            if (!int.TryParse(input, out int assetId))
            {
                Console.WriteLine("Invalid Asset ID. Please enter a number.");
                return;
            }

            Asset? assetToDelete = assets.FirstOrDefault(asset => asset.AssetId == assetId);

            if (assetToDelete == null)
            {
                Console.WriteLine("No asset found with that ID.");
                return;
            }

            assets.Remove(assetToDelete);
            Console.WriteLine("Asset deleted successfully!");
            SaveAssets(assets);
        }
    }
}
