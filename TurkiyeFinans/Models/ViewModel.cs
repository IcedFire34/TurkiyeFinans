namespace TurkiyeFinans.Models
{
    public class ViewModel
    {
        public MernisServiceParametters _mernisserviceparametters { get; set; }
        public List<Customer> _Customers { get; set; }
        public List<Account> _Accounts { get; set; }
        public List<Currency> _Currency { get; set; }
        public Customer _Customer { get; set; }
        public ViewModel() {
            _mernisserviceparametters = new MernisServiceParametters();  
            _Customers = new List<Customer>();
            _Currency = new List<Currency>();
            _Accounts = new List<Account>();
            _Customer = new Customer();
        }
    }
}
