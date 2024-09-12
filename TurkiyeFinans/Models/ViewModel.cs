namespace TurkiyeFinans.Models
{
    public class ViewModel
    {
        public MernisServiceParametters _mernisserviceparametters { get; set; }
        public List<Customer> _Customers { get; set; }
        public List<Account> _Accounts { get; set; }
        public List<Currency> _Currency { get; set; }
        public List<Transaction> _Transactions { get; set; }
        public Customer _Customer { get; set; }
        public ViewModel() {
            _mernisserviceparametters = new MernisServiceParametters();  
            _Customers = new List<Customer>();
            _Currency = new List<Currency>();
            _Accounts = new List<Account>();
            _Customer = new Customer();
            _Transactions = new List<Transaction>();
        }
        public string? FormatIban(string? iban)
        {            
            if (iban != null && iban.Length == 26)
            {
                // 4 4 4 4 4 4 2 formatında bölme
                return string.Format("{0} {1} {2} {3} {4} {5} {6}",
                    iban.Substring(0, 4),  // İlk 4 hane
                    iban.Substring(4, 4),  // 2. 4 hane
                    iban.Substring(8, 4),  // 3. 4 hane
                    iban.Substring(12, 4), // 4. 4 hane
                    iban.Substring(16, 4), // 5. 4 hane
                    iban.Substring(20, 4), // 6. 4 hane
                    iban.Substring(24, 2)  // Son 2 hane
                );
            }
            return iban; // Eğer IBAN uzunluğu 26 değilse, olduğu gibi döndür.
        }
    }
}
