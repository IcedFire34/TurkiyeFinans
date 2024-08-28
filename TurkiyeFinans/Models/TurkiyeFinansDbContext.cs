using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TurkiyeFinans.Models;

public partial class TurkiyeFinansDbContext : DbContext
{
    public TurkiyeFinansDbContext()
    {
    }

    public TurkiyeFinansDbContext(DbContextOptions<TurkiyeFinansDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<CurrencyExchange> CurrencyExchanges { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<ExchangeRate> ExchangeRates { get; set; }

    public virtual DbSet<Investment> Investments { get; set; }

    public virtual DbSet<InvestmentTransaction> InvestmentTransactions { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<LoanPayment> LoanPayments { get; set; }

    public virtual DbSet<Security> Securities { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Transfer> Transfers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Turkish_CI_AS");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(e => e.AccountId)
                .ValueGeneratedNever()
                .HasColumnName("AccountID");
            entity.Property(e => e.AccountType).HasMaxLength(20);
            entity.Property(e => e.Currency).HasMaxLength(25);
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.OpenDate).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(6);

            entity.HasOne(d => d.CurrencyNavigation).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.Currency)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Accounts_Currency");

            entity.HasOne(d => d.Customer).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Accounts_Customer");
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.Property(e => e.CardId)
                .ValueGeneratedNever()
                .HasColumnName("CardID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.CardNumber)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.CardType).HasMaxLength(20);
            entity.Property(e => e.Cvv)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("CVV");
            entity.Property(e => e.ExpiryDate)
                .HasMaxLength(4)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(7);

            entity.HasOne(d => d.Account).WithMany(p => p.Cards)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cards_Accounts");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.ToTable("Currency");

            entity.Property(e => e.CurrencyId)
                .HasMaxLength(25)
                .HasColumnName("CurrencyID");
        });

        modelBuilder.Entity<CurrencyExchange>(entity =>
        {
            entity.HasKey(e => e.ExchangeId);

            entity.ToTable("CurrencyExchange");

            entity.Property(e => e.ExchangeId)
                .ValueGeneratedNever()
                .HasColumnName("ExchangeID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.ExchangeDate).HasMaxLength(50);
            entity.Property(e => e.FromCurrency).HasMaxLength(25);
            entity.Property(e => e.ToCurrency).HasMaxLength(25);

            entity.HasOne(d => d.Account).WithMany(p => p.CurrencyExchanges)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CurrencyExchange_Accounts");

            entity.HasOne(d => d.FromCurrencyNavigation).WithMany(p => p.CurrencyExchangeFromCurrencyNavigations)
                .HasForeignKey(d => d.FromCurrency)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CurrencyExchange_Currency");

            entity.HasOne(d => d.ToCurrencyNavigation).WithMany(p => p.CurrencyExchangeToCurrencyNavigations)
                .HasForeignKey(d => d.ToCurrency)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CurrencyExchange_Currency1");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId)
                .ValueGeneratedNever()
                .HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.DateOfBirth).HasMaxLength(8);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(12);
        });

        modelBuilder.Entity<ExchangeRate>(entity =>
        {
            entity.HasKey(e => e.RateId);

            entity.Property(e => e.RateId)
                .ValueGeneratedNever()
                .HasColumnName("RateID");
            entity.Property(e => e.FromCurrency).HasMaxLength(25);
            entity.Property(e => e.LastUpdate).HasMaxLength(50);
            entity.Property(e => e.ToCurrency).HasMaxLength(25);

            entity.HasOne(d => d.FromCurrencyNavigation).WithMany(p => p.ExchangeRateFromCurrencyNavigations)
                .HasForeignKey(d => d.FromCurrency)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExchangeRates_Currency");

            entity.HasOne(d => d.ToCurrencyNavigation).WithMany(p => p.ExchangeRateToCurrencyNavigations)
                .HasForeignKey(d => d.ToCurrency)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExchangeRates_Currency1");
        });

        modelBuilder.Entity<Investment>(entity =>
        {
            entity.Property(e => e.InvestmentId)
                .ValueGeneratedNever()
                .HasColumnName("InvestmentID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.InvestmentType).HasMaxLength(50);
            entity.Property(e => e.LastValueUpdate).HasMaxLength(50);
            entity.Property(e => e.PurchaseDate).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.Investments)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Investments_Accounts");
        });

        modelBuilder.Entity<InvestmentTransaction>(entity =>
        {
            entity.Property(e => e.InvestmentTransactionId)
                .ValueGeneratedNever()
                .HasColumnName("InvestmentTransactionID");
            entity.Property(e => e.InvestmentId).HasColumnName("InvestmentID");
            entity.Property(e => e.SecurityId).HasColumnName("SecurityID");
            entity.Property(e => e.TransactionDate).HasMaxLength(50);
            entity.Property(e => e.TransactionType).HasMaxLength(4);

            entity.HasOne(d => d.Investment).WithMany(p => p.InvestmentTransactions)
                .HasForeignKey(d => d.InvestmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvestmentTransactions_Investments");

            entity.HasOne(d => d.Security).WithMany(p => p.InvestmentTransactions)
                .HasForeignKey(d => d.SecurityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvestmentTransactions_Securities");
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.Property(e => e.LoanId)
                .ValueGeneratedNever()
                .HasColumnName("LoanID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.EndDate).HasMaxLength(50);
            entity.Property(e => e.LoanType).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(9);

            entity.HasOne(d => d.Customer).WithMany(p => p.Loans)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loans_Customer");
        });

        modelBuilder.Entity<LoanPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);

            entity.Property(e => e.PaymentId)
                .ValueGeneratedNever()
                .HasColumnName("PaymentID");
            entity.Property(e => e.LoanId).HasColumnName("LoanID");
            entity.Property(e => e.PaymentDate).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(9);

            entity.HasOne(d => d.Loan).WithMany(p => p.LoanPayments)
                .HasForeignKey(d => d.LoanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoanPayments_Loans");
        });

        modelBuilder.Entity<Security>(entity =>
        {
            entity.Property(e => e.SecurityId)
                .ValueGeneratedNever()
                .HasColumnName("SecurityID");
            entity.Property(e => e.LastPriceUpdate).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Symbol).HasMaxLength(20);
            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(e => e.TransactionId)
                .ValueGeneratedNever()
                .HasColumnName("TransactionID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Currency).HasMaxLength(25);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.TransactionDate).HasMaxLength(50);
            entity.Property(e => e.TransactionType).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Accounts");
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.Property(e => e.TransferId)
                .ValueGeneratedNever()
                .HasColumnName("TransferID");
            entity.Property(e => e.Currency).HasMaxLength(25);
            entity.Property(e => e.FromAccountId).HasColumnName("FromAccountID");
            entity.Property(e => e.Status).HasMaxLength(9);
            entity.Property(e => e.ToAccountId).HasColumnName("ToAccountID");
            entity.Property(e => e.TransferDate).HasMaxLength(50);

            entity.HasOne(d => d.CurrencyNavigation).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.Currency)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transfers_Currency");

            entity.HasOne(d => d.FromAccount).WithMany(p => p.TransferFromAccounts)
                .HasForeignKey(d => d.FromAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transfers_Accounts");

            entity.HasOne(d => d.ToAccount).WithMany(p => p.TransferToAccounts)
                .HasForeignKey(d => d.ToAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transfers_Accounts1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
