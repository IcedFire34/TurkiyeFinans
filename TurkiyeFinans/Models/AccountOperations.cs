using Microsoft.Data.SqlClient;

namespace TurkiyeFinans.Models
{
    public class AccountOperations
    {
        private readonly string _connectionString;
        public AccountOperations(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Hesap olusturur
        // Parametre olarak string tipinde TC bilgisini alir hesap acilir ve veri tabanina ekler. 
        public async Task<bool> AddAccountAsync(int customerID,string accountType,string? currency,int? deposit)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    // 1. Account ekle
                    // int CustomerID ; string AccountType ; float Balance ; string Currency ; string OpenDate ; string Status
                    string insertQuery = "";
                    TimeZoneInfo timeZone;
                    DateTime localTime;

                    if (accountType == "Vadesiz" || accountType == "Altın" || accountType == "Döviz")
                    {
                        switch (accountType)
                        {
                            case "Vadesiz" :
                                currency = "TL";
                                break;
                            case "Altın" :
                                currency = "AU";
                                break;
                        }
                        insertQuery = "INSERT INTO [dbo].[Accounts] (CustomerID, AccountType, Balance, Currency, OpenDate, Status) " +
                        "VALUES (@CustomerID, @AccountType, @Balance, @Currency, @OpenDate, @Status)";
                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

                        timeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                        localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                        // Parametreleri ekliyoruz (SQL Injection'dan korunmak için)
                        insertCommand.Parameters.AddWithValue("@CustomerID", customerID);
                        insertCommand.Parameters.AddWithValue("@AccountType", accountType);
                        insertCommand.Parameters.AddWithValue("@Balance", 0.0);                        
                        insertCommand.Parameters.AddWithValue("@Currency", currency);
                        insertCommand.Parameters.AddWithValue("@OpenDate", localTime.ToString("dd/MM/yyyy HH:mm:ss"));
                        insertCommand.Parameters.AddWithValue("@Status", "Open");

                        // ExecuteNonQuery, etkilenen satır sayısını döndürür
                        int result = await insertCommand.ExecuteNonQueryAsync();

                        // Eğer ekleme başarılıysa 1 döner, aksi takdirde 0
                        return result > 0;

                    }
                    else if (accountType == "Vadeli")
                    {
                        // Accounts a ekleniyor //

                        insertQuery = "INSERT INTO [dbo].[Accounts] (CustomerID, AccountType, Balance, Currency, OpenDate, Status) " +
                        "VALUES (@CustomerID, @AccountType, @Balance, @Currency, @OpenDate, @Status);" + "SELECT SCOPE_IDENTITY();";

                        SqlCommand insertAccountCommand = new SqlCommand(insertQuery, connection);

                        timeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                        localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                        // Parametreleri ekliyoruz (SQL Injection'dan korunmak için)
                        insertAccountCommand.Parameters.AddWithValue("@CustomerID", customerID);
                        insertAccountCommand.Parameters.AddWithValue("@AccountType", accountType);
                        insertAccountCommand.Parameters.AddWithValue("@Balance", 0.0);
                        insertAccountCommand.Parameters.AddWithValue("@Currency", "TL");
                        insertAccountCommand.Parameters.AddWithValue("@OpenDate", localTime.ToString("dd/MM/yyyy HH:mm:ss"));
                        insertAccountCommand.Parameters.AddWithValue("@Status", "Open");

                        // Son kaydin accountID'sini geri dondurur
                        int accountID = Convert.ToInt32(insertAccountCommand.ExecuteScalar());
                        
                        // Vadeili tablosuna ekler //
                        
                        string insertVadeliQuery = "INSERT INTO [dbo].[Account_Vadeli] (AccountID, Vade) " +
                        "VALUES (@AccountID, @Vade);";

                        SqlCommand insertVadeliCommand = new SqlCommand(insertVadeliQuery, connection);

                        timeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                        localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                        // Parametreleri ekliyoruz (SQL Injection'dan korunmak için)
                        insertVadeliCommand.Parameters.AddWithValue("@AccountID", accountID);
                        insertVadeliCommand.Parameters.AddWithValue("@Vade", deposit);

                        int resultVadeli = await insertVadeliCommand.ExecuteNonQueryAsync();
                        //int resultAccount = await insertAccountCommand.ExecuteNonQueryAsync(); //Bundan dolayı 2 tane hesap oluşuyordu aynı anda farklı id lerde . sebebi yukarıda accountID yi almak için çalıştırıyor bu yüzden 2 tane oluyor.
                        return resultVadeli >0;
                    }else if (accountType == "Yatırım")
                    {
                        // Accounts a ekleniyor //

                        insertQuery = "INSERT INTO [dbo].[Accounts] (CustomerID, AccountType, Balance, Currency, OpenDate, Status) " +
                        "VALUES (@CustomerID, @AccountType, @Balance, @Currency, @OpenDate, @Status);" + "SELECT SCOPE_IDENTITY();";

                        SqlCommand insertAccountCommand = new SqlCommand(insertQuery, connection);

                        timeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                        localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                        // Parametreleri ekliyoruz (SQL Injection'dan korunmak için)
                        insertAccountCommand.Parameters.AddWithValue("@CustomerID", customerID);
                        insertAccountCommand.Parameters.AddWithValue("@AccountType", accountType);
                        insertAccountCommand.Parameters.AddWithValue("@Balance", 0.0);
                        insertAccountCommand.Parameters.AddWithValue("@Currency", "TL");
                        insertAccountCommand.Parameters.AddWithValue("@OpenDate", localTime.ToString("dd/MM/yyyy HH:mm:ss"));
                        insertAccountCommand.Parameters.AddWithValue("@Status", "Open");

                        // Son kaydin accountID'sini geri dondurur
                        int accountID = Convert.ToInt32(insertAccountCommand.ExecuteScalar());

                        // Yatırım tablosuna ekler //                        
                        string insertYatirimQuery = "INSERT INTO [dbo].[Account_Yatirim] (AccountID) " + "VALUES (@AccountID);";

                        SqlCommand insertYatirimCommand = new SqlCommand(insertYatirimQuery, connection);

                        timeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                        localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                        // Parametreleri ekliyoruz (SQL Injection'dan korunmak için)
                        insertYatirimCommand.Parameters.AddWithValue("@AccountID", accountID);
                        
                        int resultVadeli = await insertYatirimCommand.ExecuteNonQueryAsync();
                        //int resultAccount = await insertAccountCommand.ExecuteNonQueryAsync(); //Bundan dolayı 2 tane hesap oluşuyordu aynı anda farklı id lerde . sebebi yukarıda accountID yi almak için çalıştırıyor bu yüzden 2 tane oluyor.
                        return resultVadeli > 0;
                    }
                    else
                    {
                        return false;
                    }                     
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata: " + ex.Message);
                    return false;
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }

    }
}
