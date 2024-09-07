using Microsoft.Data.SqlClient;

namespace TurkiyeFinans.Models
{
    public class CurrencyOperations
    {
        private readonly string _connectionString;

        public CurrencyOperations(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Tüm Currencyleri listeler.
        public async Task<List<Currency>> GetAllCurrencyAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // Currency'leri listeler.
                string checkQuery = "SELECT * FROM [dbo].[Currency]";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);

                List<Currency> result = [];
                try
                {
                    using (SqlDataReader reader = await checkCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Currency bilgilerini al
                            Currency currency = new Currency
                            {                                
                                CurrencyId = reader.GetString(reader.GetOrdinal("CurrencyID")),
                                Value = reader.GetDouble(reader.GetOrdinal("Value"))
                            };

                            // Listeye ekle
                            result.Add(currency);
                        }
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata: " + ex.Message);
                    return result;
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }
    

    }
}
