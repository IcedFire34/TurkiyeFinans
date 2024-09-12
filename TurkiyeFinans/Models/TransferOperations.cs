using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System;

namespace TurkiyeFinans.Models
{
    public class TransferOperations
    {
        private readonly string _connectionString;
        public TransferOperations(string connectionString) {
            _connectionString = connectionString;
        }

        public async Task<int> AddTransfer(Transfer transfer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string insertQuery = "INSERT INTO [dbo].[Transfers] (FromAccountID, ToAccountID, Amount, Currency, TransferDate, FromBalance) " +
                            "VALUES (@FromAccountID, @ToAccountID, @Amount, @Currency, @TransferDate, @FromBalance); SELECT SCOPE_IDENTITY()";
                SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

                // Parametreleri ekliyoruz (SQL Injection'dan korunmak için)
                insertCommand.Parameters.AddWithValue("@FromAccountID", transfer.FromAccountId);
                insertCommand.Parameters.AddWithValue("@ToAccountID", transfer.ToAccountId);
                insertCommand.Parameters.AddWithValue("@Amount", transfer.Amount);
                insertCommand.Parameters.AddWithValue("@Currency", transfer.Currency);
                insertCommand.Parameters.AddWithValue("@TransferDate", transfer.TransferDate);
                insertCommand.Parameters.AddWithValue("@FromBalance", transfer.FromBalance);
                // Son kaydin accountID'sini geri dondurur
                int TransferID = Convert.ToInt32(insertCommand.ExecuteScalar());

                return TransferID;
            }                
        }

        //Transfer Gerceklestir
        public async Task<bool> Havale(decimal senderAccount, string recipientIBAN, string recipientName, double recipientAmount)
        {
            //  decimal FromAccountID, decimal ToAccountID, float Amount, string Currency, string TransferDate, double FromBalance
            AccountOperations accountOperations = new AccountOperations(_connectionString);
            Transfer transfer = new Transfer
            {
                FromAccountId = senderAccount,
                ToAccountId = accountOperations.GetAccountWithIBAN(recipientIBAN.Replace(" ", "")).Result.AccountId,
                Amount = recipientAmount,
                Currency = "TL",
                TransferDate = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"))).ToString("dd/MM/yyyy HH:mm:ss"),
                FromBalance = accountOperations.GetAccountWithAccountId(senderAccount).Result.Balance,
            };
            if (transfer.Amount <= transfer.FromBalance)
            {
                bool gitti = accountOperations.Deposit(transfer.ToAccountId, transfer.Amount).Result;
                bool cikti = accountOperations.Withdraw(transfer.FromAccountId, transfer.Amount).Result;
                int transferID = await AddTransfer(transfer);
                return (gitti && cikti) && transferID > 0;
            }
            else
            {
                return false;
            }
        }
    }
}
