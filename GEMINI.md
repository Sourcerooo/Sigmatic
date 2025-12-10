# TaxAlpha Project Overview

TaxAlpha is a .NET console application designed to process financial transaction data, primarily for tax-related calculations and portfolio analysis. It ingests raw financial data from various sources (XML, CSV, JSON), processes it year by year using a core "Portfolio Engine," and generates detailed reports.

## Project Structure

The project is organized into several key directories:

*   **`Core`**: Contains the core business logic, interfaces, models (e.g., `Instrument`, `RawTransaction`, `TaxLot`), and the `PortfolioEngine` responsible for processing financial data.
*   **`Infrastructure`**: Houses the implementations for loading data from external sources and providing specific financial data. This includes:
    *   `Loaders`: Such as `IbkrXmlLoader` for parsing Interactive Brokers XML statements.
    *   `Providers`: Such as `CsvPriceProvider`, `CsvInterestProvider`, and `JsonInstrumentProvider` for fetching prices, interest rates, and instrument details from CSV and JSON files.
*   **`input`**: A directory containing sample input data files required by the application, including:
    *   `basiszins.csv`: Contains base interest rate data.
    *   `instruments.json`: Defines instrument details.
    *   `prices.csv`: Provides historical price data for instruments.
    *   `TaxAlpha_Raw_Data_XXXX.xml`: Raw transaction data files, likely from Interactive Brokers, categorized by year.
*   **`Reporting`**: Contains the `ConsoleReporting` class, which handles the output and presentation of processed data and generated reports to the console.
*   **`Program.cs`**: The application's entry point, orchestrating the data loading, processing loop, and report generation.
*   **`TaxAlpha.csproj`**: The project file, defining the .NET project settings, dependencies, and how input files are handled.

## How it Works

The application follows these main steps:

1.  **Setup Infrastructure**: Initializes data providers (`CsvPriceProvider`, `CsvInterestProvider`, `JsonInstrumentProvider`) and a transaction loader (`IbkrXmlLoader`), pointing them to the respective input files.
2.  **Load Data**: Uses the `IbkrXmlLoader` to load raw transactions from XML files found in the `input` directory.
3.  **Setup Engine**: Instantiates the `PortfolioEngine` with the instrument and interest rate providers.
4.  **Processing Loop**: Iterates through the loaded transactions chronologically, year by year.
    *   For each year, it processes individual transactions using `engine.ProcessTransaction`.
    *   Performs year-end closing procedures (e.g., calculating "VAP" - Verlustverrechnungstopf Aktiengewinne und -verluste) with `engine.PerformYearEndClosing`.
    *   Generates and prints an annual report using `ConsoleReporting.PrintReport`.

The primary goal of TaxAlpha appears to be to automate the process of analyzing financial transaction data for tax reporting or personal portfolio management, specifically handling year-end calculations and providing clear console-based reports.

## Building and Running

This is a .NET console application.

**To build the project:**

```bash
dotnet build
```

**To run the project:**

The application expects input files in an `input` directory relative to the executable. Ensure that `basiszins.csv`, `instruments.json`, `prices.csv`, and `TaxAlpha_Raw_Data_XXXX.xml` files are present in the `input` directory.

```bash
dotnet run
```

Or, after building:

```bash
./bin/Debug/net10.0/TaxAlpha.exe
```

## Development Conventions

*   **Language**: C#
*   **Framework**: .NET 10.0
*   **Data Handling**: Uses specific loaders and providers for different data formats (XML for transactions, CSV for prices/interest, JSON for instruments).
*   **Modularity**: The project is structured with clear separation of concerns into `Core`, `Infrastructure`, and `Reporting` layers.

## Tax Calculation Details

### Vorabpauschale and Teilfreistellung on Sales

When an investment fund is sold, the German tax law dictates a specific order for calculating the taxable capital gain, especially when `Vorabpauschale` (advance lump sum) has been applied in previous years and `Teilfreistellung` (partial exemption) is relevant.

The process implemented in TaxAlpha, and as per the Investmentsteuergesetz (InvStG), follows these steps:

1.  **Calculate Gross Profit (Adjusted for `Vorabpauschale`)**:
    First, the actual capital gain (`Veräußerungsgewinn`) is determined. This is done by taking the sale proceeds and subtracting the acquisition costs. Crucially, any **gross `Vorabpauschale`** that was previously assessed (during the holding period) for the sold units must also be subtracted from this amount. This step effectively "reverses" the fictional gains attributed by the `Vorabpauschale` to arrive at a true, gross profit from the sale.

    Mathematically, this is represented as:
    `Adjusted Gross Profit = Sale Proceeds - Acquisition Costs - Gross Vorabpauschale`

2.  **Apply `Teilfreistellung`**:
    Only *after* the `Adjusted Gross Profit` has been calculated (i.e., after deducting the gross `Vorabpauschale`), is the `Teilfreistellung` applied. This partial exemption reduces the amount that is ultimately subject to taxation.

    Mathematically, this is represented as:
    `Taxable Profit = Adjusted Gross Profit * (1 - Teilfreistellung Quote)`

**Why this order is important:**

*   The `Vorabpauschale` is a fictional gain, and while tax is paid on its `teilfreigestellt` portion, the *full gross amount* of the `Vorabpauschale` is what must be deducted from the actual capital gain upon sale. This prevents double-taxation of the fictional gain.
*   Applying `Teilfreistellung` at the end ensures that the exemption is correctly applied to the net capital gain after all adjustments, consistent with the legal framework.

This approach is implemented in the `HandleSell` method within `Core/Engine/PortfolioEngine.cs`.