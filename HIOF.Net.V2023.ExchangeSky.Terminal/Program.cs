namespace HIOF.Net.V2023.ExchangeSky.Terminal
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-------- Valutakonvertering --------");
            Console.WriteLine("Hei! Vell møtt, og velkommen til denne terminalløsningen for valutakovertering fra Norske kroner.");
            Console.WriteLine("Tast inn ønsket beløp: ");
            string? amountInput = Console.ReadLine();
            decimal decimalAmount = decimal.Parse(amountInput);


            Console.WriteLine("Tast inn ønsket valuta å konvertere til: ");
            string? currencyInput = Console.ReadLine();

            // https://localhost:7076/ExchangeRate/Convert?nok=100&toCurrency=CAD
            var url = $"https://localhost:7076/ExchangeRate/Convert?nok={amountInput}&toCurrency={currencyInput}";

            var client = new HttpClient();

            var convertedAmount = Convert.ToDecimal(client.GetStringAsync(url).Result);

            Console.WriteLine($"Konvertert beløp blir: {convertedAmount: 0.00} {currencyInput}");
            Console.ReadLine();
        }
    }
}