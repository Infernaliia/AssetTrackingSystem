# Asset Tracking System

## Project Overview

The Asset Tracking System is a C# console application designed to manage company assets across multiple international offices.

The application allows users to register computers and mobile phones, track purchase information, calculate asset age and end-of-life dates, convert prices into local currencies, and export inventory reports.

## Features

### Asset Management
- Add computers and mobile phones.
- Store brand, model, purchase date, purchase price and office location.
- Generate unique asset IDs.
- Delete assets using their IDs.
- Validate user input.

### Asset Tracking
- Calculate asset age.
- Calculate asset end-of-life dates based on a three-year lifecycle.
- Display remaining asset life.
- Identify assets approaching end of life using colour-coded status indicators.

### International Currency Support
Supports three office locations:
- Sweden — SEK
- USA — USD
- Turkey — TRY

Exchange rates are retrieved through the Frankfurter API, with fallback exchange rates if the API is unavailable.

### Sorting and Display
- Sort assets by asset type and purchase date.
- Sort assets by office and purchase date.
- Display asset information and inventory details.

### File Management and Export
- Save and load assets using JSON.
- Automatically save newly added and deleted assets.
- Export inventory to CSV.
- Export inventory to Excel (`.xlsx`).
- Format exported prices to two decimal places.

## Technologies Used

- C#
- .NET
- Visual Studio
- Object-Oriented Programming (OOP)
- Inheritance and polymorphism
- LINQ
- JSON serialization (`System.Text.Json`)
- Frankfurter Exchange Rate API
- ClosedXML (Excel export)

## Application Menu

| Option | Function |
|---|---|
| 1 | Display all assets |
| 2 | Exit application |
| 3 | Add a new asset |
| 4 | Save assets |
| 5 | Delete an asset |
| 6 | Display assets by office |
| 7 | Export assets to CSV |
| 8 | Export assets to Excel |

## How to Run

1. Open the project in Visual Studio.
2. Restore the NuGet packages.
3. Build the solution.
4. Run the application.
5. Follow the console menu instructions.

An internet connection is required to retrieve current exchange rates. If the API is unavailable, the application uses fallback rates.

## Data Storage

Asset information is stored locally in `assets.json`.

The application loads previously saved assets when it starts and saves changes to the JSON file.

CSV and Excel reports can be generated through the application menu.

## Learning Objectives

This project demonstrates practical experience with:

- C# classes and objects.
- Abstract classes and inheritance.
- Polymorphism.
- Collections and LINQ.
- DateTime and TimeSpan.
- Input validation and exception handling.
- API integration and asynchronous programming.
- JSON serialization and file management.
- CSV and Excel report generation.

## Project Status

**Completed — Mini Project: Asset Tracking System #1**
