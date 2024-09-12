using Microsoft.Data.SqlClient;

namespace TurkiyeFinans.Models
{
    public class TransactionOperations
    {
        private readonly string _connectionString;

        public TransactionOperations(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<int> AddTransaction(decimal accountID,string type,double amount,string currency,string? description=null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string insertQuery = "INSERT INTO [dbo].[Transactions] (AccountID, TransactionType, Amount, Currency, TransactionDate, Description) " +
                            "VALUES (@AccountID, @TransactionType, @Amount, @Currency, @TransactionDate, @Description); SELECT SCOPE_IDENTITY()";
                SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

                // Parametreleri ekliyoruz (SQL Injection'dan korunmak için)
                insertCommand.Parameters.AddWithValue("@AccountID", accountID);
                insertCommand.Parameters.AddWithValue("@TransactionType", type);
                insertCommand.Parameters.AddWithValue("@Amount", amount);
                insertCommand.Parameters.AddWithValue("@Currency", currency);
                insertCommand.Parameters.AddWithValue("@TransactionDate", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"))).ToString("dd/MM/yyyy HH:mm:ss"));
                if (description == null)
                {
                    insertCommand.Parameters.AddWithValue("@Description", DBNull.Value);
                }
                else
                {
                    insertCommand.Parameters.AddWithValue("@Description", description);
                }
                
                // Son kaydin accountID'sini geri dondurur
                int TransactionID = Convert.ToInt32(insertCommand.ExecuteScalar());

                return TransactionID;
            }
        }

        public async Task<List<Transaction>> ListTransactionAsync(decimal accountID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    string checkQuery = "SELECT * FROM [dbo].[Transaction] WHERE AccountID = @AccountID";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);

                    checkCommand.Parameters.AddWithValue("@AccountID", accountID);

                    List<Transaction> result = new List<Transaction>();
                    using (SqlDataReader reader = await checkCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Transaction bilgilerini al
                            Transaction transaction = new Transaction
                            {
                                TransactionId = reader.GetInt32(reader.GetOrdinal("TransactionId")),
                                AccountId = reader.GetDecimal(reader.GetOrdinal("AccountId")),
                                TransactionType = reader.GetString(reader.GetOrdinal("TransactionType")),
                                Amount = reader.GetDouble(reader.GetOrdinal("Amount")),
                                Currency = reader.GetString(reader.GetOrdinal("Currency")),
                                TransactionDate = reader.GetString(reader.GetOrdinal("TransactionDate")),                                
                                Description = !reader.IsDBNull(reader.GetOrdinal("Description"))
                                       ? reader.GetString(reader.GetOrdinal("Description"))
                                       : null
                            };
                            // Listeye ekle
                            result.Add(transaction);
                        }
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata: " + ex);
                    return null;
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }

    }
}
