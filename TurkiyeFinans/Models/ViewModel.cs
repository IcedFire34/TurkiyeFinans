namespace TurkiyeFinans.Models
{
    public class ViewModel
    {
        public MernisServiceParametters _mernisserviceparametters { get; set; }
        public List<Customer> _Customer { get; set; }
        public ViewModel() {
            _mernisserviceparametters = new MernisServiceParametters();            
        }
    }
}
