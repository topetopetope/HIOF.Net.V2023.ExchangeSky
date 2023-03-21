using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HIOF.Net.V2023.ExchangeSky.WalletService.Data
{
    public class WalletDbContext : DbContext
    {
        public DbSet<Wallet> Wallets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=NaaSkalViSjekkeOmDetteFunker;Trusted_Connection=True;TrustServerCertificate=True");

            base.OnConfiguring(optionsBuilder);  
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wallet>(mb => 
                {
                    mb.ToTable("Wallet");

                    mb.Property(wallet => wallet.WalletId);
                    mb.Property(wallet => wallet.Description).HasMaxLength(85);
                    mb.Property(wallet => wallet.Currency).HasMaxLength(3).IsFixedLength();
                    
                    mb.HasKey(wallet => wallet.WalletId);

                });

            modelBuilder.Entity<Transaction>(mb =>
            {
                mb.Property(transaction => transaction.TransactionId);
                mb.Property(transaction => transaction.Date);
                mb.Property(transaction => transaction.Description);
                mb.Property(transaction => transaction.FromWalletId).IsRequired(false);
                mb.Property(transaction => transaction.ToWalletId).IsRequired(false);
                mb.Property(transaction => transaction.Amount);
                mb.Property(transaction => transaction.ExchangeRate);

                mb.HasOne(transaction => transaction.FromWallet)
                .WithMany(wallet => wallet.TransactionsFromWallet)
                .HasForeignKey(transaction => transaction.FromWalletId)
                .OnDelete(DeleteBehavior.NoAction);

                mb.HasOne(transaction => transaction.ToWallet)
                .WithMany(wallet => wallet.TransactionsToWallet)
                .HasForeignKey(transaction => transaction.ToWalletId)
                .OnDelete(DeleteBehavior.NoAction);

                mb.HasKey(transaction => transaction.TransactionId);

            });
        }
    }
}
