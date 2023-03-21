using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIOF.Net.V2023.ExchangeSky.WalletService.Data
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public DateTime Date { get; set; }
        public Guid? FromWalletId { get; set; }
        public Guid? ToWalletId { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public decimal ExchangeRate { get; set; }

        public virtual Wallet FromWallet { get; set; }
        public virtual Wallet ToWallet { get; set; }


    }
}
