using HIOF.Net.V2023.ExchangeSky.WalletService.Data;
using HIOF.Net.V2023.ExchangeSky.WalletService.Model.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HIOF.Net.V2023.ExchangeSky.WalletService.Controllers.V1
{
    [ApiController]
    [Route("V1/Wallet")]
    public class V1WalletController : ControllerBase
    {
      
        private readonly ILogger<V1WalletController> _logger;

        public V1WalletController(ILogger<V1WalletController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<V1Result<IEnumerable<V1Wallet>>> Get()
        {
            var dbContext = new WalletDbContext();
            var responseWallets = await dbContext.Wallets
                .Select(wallet => new V1Wallet
                {
                    Id = wallet.WalletId,
                    Description = wallet.Description,
                    Currency = wallet.Currency,
                    Balance = wallet.TransactionsToWallet.Sum(transaction => transaction.Amount * transaction.ExchangeRate) 
                    - wallet.TransactionsFromWallet.Sum(transaction => transaction.Amount)
                })
                .ToListAsync();

            return new V1Result<IEnumerable<V1Wallet>>(responseWallets); 
        }

        [HttpPost]
        public async Task<V1Result<V1Wallet>> CreateWallet(V1PostWallet walletPost)
        {
            var dbContext = new WalletDbContext();

            var wallet = new Wallet() 
            {
                WalletId = Guid.NewGuid(),
                Description = walletPost.Description,
                Currency = walletPost.Currency
            };

            dbContext.Wallets.Add(wallet);
            await dbContext.SaveChangesAsync();

            var result = new V1Result<V1Wallet>(new V1Wallet
            {
                Id = wallet.WalletId,
                Description = walletPost.Description,
                Currency = walletPost.Currency,
                Balance = -1
            });

            return result;
            
        }
    }
}