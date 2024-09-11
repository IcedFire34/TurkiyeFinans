using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
                    string insertIBANQuery = "";
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
                        insertQuery = "INSERT INTO [dbo].[Accounts] (CustomerID, AccountType, Balance, Currency, OpenDate, Status, Iban) " +
                        "VALUES (@CustomerID, @AccountType, @Balance, @Currency, @OpenDate, @Status, @Iban); SELECT SCOPE_IDENTITY()";
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
                        insertCommand.Parameters.AddWithValue("@Iban", DBNull.Value);

                        // Son kaydin accountID'sini geri dondurur
                        decimal accountID = Convert.ToDecimal(insertCommand.ExecuteScalar());

                        //IBAN ekle
                        if(accountType == "Vadesiz")
                        {                            
                            bool resultIBAN = AddIBAN("TR","00010",accountID).Result;                           
                        }
                        

                        // Eğer ekleme başarılıysa 1 döner, aksi takdirde 0
                        return accountID > 0;

                    }
                    else if (accountType == "Vadeli")
                    {
                        // Accounts a ekleniyor //

                        insertQuery = "INSERT INTO [dbo].[Accounts] (CustomerID, AccountType, Balance, Currency, OpenDate, Status, Iban) " +
                        "VALUES (@CustomerID, @AccountType, @Balance, @Currency, @OpenDate, @Status, @Iban);" + "SELECT SCOPE_IDENTITY();";

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
                        insertAccountCommand.Parameters.AddWithValue("@Iban", DBNull.Value);

                        // Son kaydin accountID'sini geri dondurur
                        int accountID = Convert.ToInt32(insertAccountCommand.ExecuteScalar());

                        //IBAN ekle
                        bool resultIBAN = AddIBAN("TR", "00010", accountID).Result;                        

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

                        insertQuery = "INSERT INTO [dbo].[Accounts] (CustomerID, AccountType, Balance, Currency, OpenDate, Status, Iban) " +
                        "VALUES (@CustomerID, @AccountType, @Balance, @Currency, @OpenDate, @Status, @Iban);" + "SELECT SCOPE_IDENTITY();";

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
                        insertAccountCommand.Parameters.AddWithValue("@Iban", DBNull.Value);

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

        // Hesaplari Listeler
        // Parametre olarak almis oldugu customerID'nin hesaplarini geri dondurur.
        public async Task<List<Account>> ListAccountAsync(int customerID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    string checkQuery = "SELECT * FROM [dbo].[Accounts] WHERE CustomerID = @CustomerID";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);

                    checkCommand.Parameters.AddWithValue("@CustomerID", customerID);

                    List<Account> result = new List<Account>();
                    using (SqlDataReader reader = await checkCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Account bilgilerini al
                            Account account = new Account
                            {
                               
                                AccountId = reader.GetDecimal(reader.GetOrdinal("AccountID")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                AccountType = reader.GetString(reader.GetOrdinal("AccountType")),
                                Balance = reader.GetDouble(reader.GetOrdinal("Balance")),
                                Currency = reader.GetString(reader.GetOrdinal("Currency")),
                                OpenDate = reader.GetString(reader.GetOrdinal("OpenDate")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                Iban = !reader.IsDBNull(reader.GetOrdinal("Iban"))
                                       ? reader.GetString(reader.GetOrdinal("Iban"))
                                       : null
                            };                            
                            if (account.Status == "Open")
                            {
                                // Listeye ekle
                                result.Add(account);
                            }
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

        // Hesabi kapatir
        // Parametre olarak aldigi accountID'yi kapatir.
        public async Task<bool> CloseAccountAsync(int accountID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    // SQL sorgusunun doğru yazımı
                    string checkQuery = "UPDATE [dbo].[Accounts] SET Status = @Status WHERE AccountID = @AccountID";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);

                    checkCommand.Parameters.AddWithValue("@Status", "Close");
                    checkCommand.Parameters.AddWithValue("@AccountID", accountID);

                    int affectedRows = await checkCommand.ExecuteNonQueryAsync();

                    // Eğer etkilenen satır sayısı 0'dan büyükse işlem başarılıdır
                    return affectedRows > 0;
                }
                catch (Exception ex)
                {
                    // Hata durumunda hata mesajını yazdır
                    Console.WriteLine("Hata: " + ex.Message);
                    return false;
                }                
            }
        }
    
        // Iban olusturur ve accounta ekler.        
        // Parametre olarak aldigi (ulkeID, bankID, accountID) degerlerine gore Iban olusturur.
        public string GenerateIBAN(string countryID, string bankID, decimal accountID)
        {
            string countryValue = (countryID[0] - 'A' + 10).ToString() + (countryID[1] - 'A' + 10).ToString() + "00" ; // Harflerin sayısal degerini hesapla
            string accountID_16 = accountID.ToString().PadLeft(16,'0'); // AccountId'yi 16 haneye cikariyor
            string rezerveDigit = "0"; // Revize edilmis bit
            decimal mod = (Convert.ToDecimal(bankID + rezerveDigit + accountID_16 + countryValue) % 97); 
            string checkDigits = (98 - (int)mod).ToString("D2"); // Kontrol biti hesaplanıyor
            return countryID + checkDigits + bankID + rezerveDigit + accountID_16; // IBAN geri donduruluyor
        }
        public async Task<bool> AddIBAN(string countryID, string bankID,decimal accountID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string updateIBANQuery = "UPDATE [dbo].[Accounts] SET Iban = @Iban WHERE AccountID = @AccountID";
                    SqlCommand insertIBANCommand = new SqlCommand(updateIBANQuery, connection);

                    insertIBANCommand.Parameters.AddWithValue("@AccountID", accountID);
                    insertIBANCommand.Parameters.AddWithValue("@Iban", GenerateIBAN(countryID, bankID, Convert.ToDecimal(accountID)));

                    int resultIBAN = await insertIBANCommand.ExecuteNonQueryAsync();
                    
                    return resultIBAN > 0;

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
        
        // Para yatirma
        public async Task<bool> Deposit(decimal accountID,double amount)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    // Hesaptaki balance değerini al

                    Account account;
                    string selectAccountQuery = "SELECT * FROM [dbo].[Accounts] WHERE AccountID = @AccountID";

                    SqlCommand selectAccountCommand = new SqlCommand(selectAccountQuery, connection);

                    selectAccountCommand.Parameters.AddWithValue("@AccountID", accountID);

                    using (SqlDataReader reader = await selectAccountCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            account = new Account
                            {
                                AccountId = reader.GetDecimal(reader.GetOrdinal("AccountID")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                AccountType = reader.GetString(reader.GetOrdinal("AccountType")),
                                Balance = reader.GetDouble(reader.GetOrdinal("Balance")),
                                Currency = reader.GetString(reader.GetOrdinal("Currency")),
                                OpenDate = reader.GetString(reader.GetOrdinal("OpenDate")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                Iban = reader.GetString(reader.GetOrdinal("Iban"))
                            };
                        }
                        else
                        {
                            return false;
                        }                            
                    }

                    // Hesaba yatır
                    double newBalance = account.Balance + amount;
                    string updateBalanceQuery = "UPDATE [dbo].[Accounts] SET Balance = @Balance WHERE AccountID = @AccountID";
                    SqlCommand updateBalanceCommand = new SqlCommand(updateBalanceQuery, connection);

                    updateBalanceCommand.Parameters.AddWithValue("@Balance",newBalance);
                    updateBalanceCommand.Parameters.AddWithValue("@AccountID", accountID);

                    int resultBalance = await updateBalanceCommand.ExecuteNonQueryAsync();

                    return resultBalance > 0;
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
        // Para cekme
        public async Task<bool> Withdraw(decimal accountID, double amount)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    // Hesaptaki balance değerini al

                    Account account;
                    string selectAccountQuery = "SELECT * FROM [dbo].[Accounts] WHERE AccountID = @AccountID";

                    SqlCommand selectAccountCommand = new SqlCommand(selectAccountQuery, connection);

                    selectAccountCommand.Parameters.AddWithValue("@AccountID", accountID);

                    using (SqlDataReader reader = await selectAccountCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            account = new Account
                            {
                                AccountId = reader.GetDecimal(reader.GetOrdinal("AccountID")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                AccountType = reader.GetString(reader.GetOrdinal("AccountType")),
                                Balance = reader.GetDouble(reader.GetOrdinal("Balance")),
                                Currency = reader.GetString(reader.GetOrdinal("Currency")),
                                OpenDate = reader.GetString(reader.GetOrdinal("OpenDate")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                Iban = reader.GetString(reader.GetOrdinal("Iban"))
                            };
                        }
                        else
                        {
                            return false;
                        }
                    }

                    // Hesaptan cek
                    if (account.Balance>= amount )
                    {
                        double newBalance = account.Balance - amount;
                        string updateBalanceQuery = "UPDATE [dbo].[Accounts] SET Balance = @Balance WHERE AccountID = @AccountID";
                        SqlCommand updateBalanceCommand = new SqlCommand(updateBalanceQuery, connection);

                        updateBalanceCommand.Parameters.AddWithValue("@Balance", newBalance);
                        updateBalanceCommand.Parameters.AddWithValue("@AccountID", accountID);

                        int resultBalance = await updateBalanceCommand.ExecuteNonQueryAsync();

                        return resultBalance > 0;
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
