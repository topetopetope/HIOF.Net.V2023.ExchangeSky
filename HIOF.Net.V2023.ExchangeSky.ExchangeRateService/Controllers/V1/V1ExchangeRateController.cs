using HIOF.Net.V2023.ExchangeSky.ExchangeRateService.Integrations;
using HIOF.Net.V2023.ExchangeSky.ExchangeRateService.Model.V1;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;

namespace HIOF.Net.V2023.ExchangeSky.ExchangeRateService.Controllers.V1
{

    /// <summary>
    /// Handles the ExchangeRate endpoint.
    /// </summary>
    [ApiController]
    [Route("api/1.0/ExchangeRate")]
    public class V1ExchangeRateController : ControllerBase
    {

        private readonly ILogger<V1ExchangeRateController> _logger;

        public V1ExchangeRateController(ILogger<V1ExchangeRateController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Converts the speciefied amount in NOK (Norwegian krone) to the target currency.
        /// </summary>
        /// <param name="nok">The amount to convert in NOK (Norwegian krone)</param>
        /// <param name="toCurrency">The currency to convert to, specified by the three-letter ISO code, 
        /// for example USD (American dollars).</param>
        /// <returns>Returns the NOK amount in the target currency</returns>
        [HttpGet("Convert")]
        public async Task<decimal> Convert(decimal nok, string toCurrency)
        {
            var apiClient = new NorgesBankAPIClient();

            var rate = await apiClient.FindLatestExchangeRate(toCurrency);

            return nok / rate.Rate;
        }

        [HttpGet]
        public async Task<IEnumerable<V1ExchangeRate>> GetExchangeRates(DateTime from)
        {
            var apiClient = new NorgesBankAPIClient();

            var norgesBankRates = await apiClient.FindExchangeRates(from);

            var rates = new List<V1ExchangeRate>();

            foreach (var norgesBankExchangeRate in norgesBankRates)
            {
                rates.Add(new V1ExchangeRate()
                {
                    Date = norgesBankExchangeRate.Date,
                    Rate = norgesBankExchangeRate.Rate,
                    Currency = norgesBankExchangeRate.Currency
                });
                    
            }

            return rates;
        }
    }
}