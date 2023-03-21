namespace HIOF.Net.V2023.ExchangeSky.WalletService.Data
{
    public class Wallet
    {
        public Guid WalletId { get; set; }
        public string? Description { get; set; }
        public string? Currency { get; set; }

        public virtual ICollection<Transaction> TransactionsFromWallet { get; set; }
        public virtual ICollection<Transaction> TransactionsToWallet { get; set; }

    }
}
