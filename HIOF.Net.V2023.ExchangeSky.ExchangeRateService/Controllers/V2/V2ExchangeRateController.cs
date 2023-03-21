using HIOF.Net.V2023.ExchangeSky.ExchangeRateService.Integrations;
using HIOF.Net.V2023.ExchangeSky.ExchangeRateService.Model.V2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;

namespace HIOF.Net.V2023.ExchangeSky.ExchangeRateService.Controllers.V2
{

    /// <summary>
    /// Handles the ExchangeRate endpoint.
    /// </summary>
    [ApiController]
    [Route("api/2.0/ExchangeRate")]
    public class V2ExchangeRateController : ControllerBase
    {

        private readonly ILogger<V2ExchangeRateController> _logger;

        public V2ExchangeRateController(ILogger<V2ExchangeRateController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Converts the speciefied amount in a specified currency to a new currency.
        /// </summary>
        /// <param name="fromCurrency">The currency to be converted from, specified by the three-letter ISO code</param>
        /// for example USD (American dollars)
        /// <param name="toCurrency">The currency to convert to, specified by the three-letter ISO code</param>
        /// <param name="amount">The amount of money to convert</param>
        /// <returns>Returns the NOK amount in the target currency</returns>
        [HttpGet("Convert")]
        [ProducesResponseType(typeof(V2Result<decimal>), 200)]
        [ProducesResponseType(typeof(V2Result<decimal>), 400)]
        [ProducesResponseType(500)]

        public async Task<ActionResult<V2Result<decimal>>> Convert(string fromCurrency, string toCurrency, decimal amount)
        {
            var apiClient = new NorgesBankAPIClient();


            //4 case
            // 1. Fra NOK til NOK
            // 2. Fra NOK til annen valuta
            // 3. Fra annen valuta til NOK
            // 4. Fra annen valuta til annen valuta

            // tre stegs prosess
            // 1. Konvertere til nok
            // 2. Konvertere fra nok til målvaluta
            // 3. Returnere resulatat

            decimal inNok;
            decimal inTarget;

            if (fromCurrency == "NOK") 
                inNok = amount;
            else
            {
                var fromRate = await apiClient.FindLatestExchangeRate(fromCurrency);

                if (fromRate == null)
                {
                    var currencyDoesNotExist =
                        new V2Result<decimal>($"Unable to find an exchange rate to the currency '{fromCurrency}'");
                    return BadRequest(currencyDoesNotExist);
                }

                inNok = amount * fromRate.Rate;
            }

            if(toCurrency == "NOK")
                inTarget = inNok;
            else
            {
                var toRate = await apiClient.FindLatestExchangeRate(toCurrency);

                if (toRate == null)
                {
                    var currencyDoesNotExist =
                        new V2Result<decimal>($"Unable to find an exchange rate to the currency '{toCurrency}'");
                    return BadRequest(currencyDoesNotExist);
                }

                inTarget = inNok * toRate.Rate;
            }

            return new V2Result<decimal>( inTarget );
        }

        /// <summary>
        /// Returns all exchage rates from the specified date, onwards
        /// </summary>
        /// <param name="from">The earliest date to retrieve dates from</param>
        /// <returns>All exchage rates returned from the specified date.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(V2Result<IEnumerable<V2ExchangeRate>>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<IEnumerable<V2ExchangeRate>>>> GetExchangeRates(DateTime from)
        {
            var apiClient = new NorgesBankAPIClient();

            var norgesBankRates = await apiClient.FindExchangeRates(from);

            var rates = new List<V2ExchangeRate>();

            foreach (var norgesBankExchangeRate in norgesBankRates)
            {
                rates.Add(new V2ExchangeRate()
                {
                    Date = norgesBankExchangeRate.Date,
                    Rate = norgesBankExchangeRate.Rate,
                    Currency = norgesBankExchangeRate.Currency
                });
                    
            }

            return new V2Result<IEnumerable<V2ExchangeRate>>(rates);
        }
    }
}