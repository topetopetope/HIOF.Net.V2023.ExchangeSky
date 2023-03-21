using HIOF.Net.V2023.ExchangeSky.ExchangeRateService.Model;
using System.Globalization;

namespace HIOF.Net.V2023.ExchangeSky.ExchangeRateService.Integrations
{
    public class NorgesBankAPIClient
    {
        public async Task<NorgesBankExchangeRate?> FindLatestExchangeRate(string currency)
        {
            var csvFileUrl = "https://data.norges-bank.no/api/data/EXR/B..NOK.SP?lastNObservations=1&format=csv";

            var rates = await RetreiveAndParseRatesAsync(csvFileUrl);

            return rates.Where(rate => rate.Currency == currency).SingleOrDefault();
        }

        public async Task<IEnumerable<NorgesBankExchangeRate>> FindExchangeRates(DateTime from)
        {
            var csvFileUrl = $"https://data.norges-bank.no/api/data/EXR/B..NOK.SP?startPeriod={from:yyyy-MM-dd}&format=csv";

            return await RetreiveAndParseRatesAsync(csvFileUrl);
        }

        private async Task<IEnumerable<NorgesBankExchangeRate>> RetreiveAndParseRatesAsync(string url)
        {
            using var client = new HttpClient();

            // API documentation: https://app.norges-bank.no/query/#/no/
            var csvFile = await client.GetStringAsync(url);

            var csvFileRows = csvFile.Split("\n");
            var csvFileHeaderColumns = csvFileRows[0].Split(";");

            var csvFileRateColumnIndex = Array.IndexOf(csvFileHeaderColumns, "OBS_VALUE");
            var csvFileCurrencyColumnIndex = Array.IndexOf(csvFileHeaderColumns, "BASE_CUR");
            var csvFileTimePeriodColumnIndex = Array.IndexOf(csvFileHeaderColumns, "TIME_PERIOD");
            var csvFileUnitMultiplierColumnIndex = Array.IndexOf(csvFileHeaderColumns, "Unit Multiplier");

            var exchangeRates = new List<NorgesBankExchangeRate>();

            foreach (var row in csvFileRows.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(row))
                    continue;

                var rowValues = row.Split(";");

                var multiplierValue = rowValues[csvFileUnitMultiplierColumnIndex];
                var multiplier = multiplierValue == "Hundreds" ? 100 : 1;

                var rate = Decimal.Parse(rowValues[csvFileRateColumnIndex], CultureInfo.InvariantCulture);

                exchangeRates.Add(new NorgesBankExchangeRate()
                {
                    Date = DateTime.Parse(rowValues[csvFileTimePeriodColumnIndex], CultureInfo.InvariantCulture),
                    Currency = rowValues[csvFileCurrencyColumnIndex],
                    Rate = rate / multiplier
                }); ;
            }

            return exchangeRates;
        }
    }
}
