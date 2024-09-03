using Microsoft.Data.SqlClient;

namespace TurkiyeFinans.Models
{
    public class CustomerOperations
    {
        private readonly string _connectionString;

        public CustomerOperations(string connectionString) {
            _connectionString = connectionString;
        }

        // Müşteri ekler.
        // Parametre olarak MernisServiceParametters tipinde bir müşteri bilgisi alır ve bunun kontrollerini yapıp veri tabanına ekler. 
        public async Task<bool> AddCustomerAsync(MernisServiceParametters customer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    // 1. Müşteriyi IdentificationNumber ile kontrol et
                    string checkQuery = "SELECT COUNT(*) FROM [dbo].[Customer] WHERE IdentificationNumber = @IdentificationNumber";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@IdentificationNumber", customer.TCKimlikNo);

                    int customerCount = (int)await checkCommand.ExecuteScalarAsync();

                    // Eğer müşteri zaten mevcutsa, ekleme yapma
                    if (customerCount > 0)
                    {
                        Console.WriteLine("<<<<< Müşteri zaten mevcut. >>>>>");
                        return false;
                    }

                    // 2. Müşteriyi ekle
                    string insertQuery = "INSERT INTO [dbo].[Customer] (FirstName, LastName, DateOfBirth, Address, PhoneNumber, Email, IdentificationNumber) " +
                        "VALUES (@FirstName, @LastName, @DateOfBirth, @Address, @PhoneNumber, @Email, @IdentificationNumber)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

                    // Parametreleri ekliyoruz (SQL Injection'dan korunmak için)
                    insertCommand.Parameters.AddWithValue("@FirstName", customer.Ad);
                    insertCommand.Parameters.AddWithValue("@LastName", customer.Soyad);
                    insertCommand.Parameters.AddWithValue("@DateOfBirth", customer.DogumTarihi);
                    insertCommand.Parameters.AddWithValue("@Address", customer.Adres);
                    insertCommand.Parameters.AddWithValue("@PhoneNumber", customer.Telefon);
                    insertCommand.Parameters.AddWithValue("@Email", customer.Email);
                    insertCommand.Parameters.AddWithValue("@IdentificationNumber", customer.TCKimlikNo);

                    // ExecuteNonQuery, etkilenen satır sayısını döndürür
                    int result = await insertCommand.ExecuteNonQueryAsync();

                    // Eğer ekleme başarılıysa 1 döner, aksi takdirde 0
                    return result > 0;
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

        // Müşteri siler
        // Parametre olarak MernisServiceParametters tipinde bir müşteri bilgisi alır ve bunun kontrollerini yapıp siler.
        public async Task<bool> DelCustomerAsync(MernisServiceParametters customer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    // 1. Müşteriyi IdentificationNumber ile kontrol et
                    string checkQuery = "SELECT COUNT(*) FROM [dbo].[Customer] WHERE IdentificationNumber = @IdentificationNumber";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@IdentificationNumber", customer.TCKimlikNo);
                    int customerCount = (int)await checkCommand.ExecuteScalarAsync();

                    // Eğer müşteri mevcutsa, silme islemi yap.
                    if (customerCount > 0)
                    {
                        Console.WriteLine("<<<<< Müşteri mevcut. >>>>>");
                        // 2. Müşteriyi sil
                        string deleteQuery = "DELETE [dbo].[Customer] WHERE IdentificationNumber = @IdentificationNumber";
                        SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);

                        // Parametreleri ekliyoruz (SQL Injection'dan korunmak için)
                        deleteCommand.Parameters.AddWithValue("@IdentificationNumber", customer.TCKimlikNo);

                        // ExecuteNonQuery, etkilenen satır sayısını döndürür
                        int result = await deleteCommand.ExecuteNonQueryAsync();

                        // Eğer ekleme başarılıysa 1 döner, aksi takdirde 0
                        return result > 0;
                    }
                    else
                    {
                        Console.WriteLine("<<<<< Müşteri mevcut degil. >>>>>");
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
