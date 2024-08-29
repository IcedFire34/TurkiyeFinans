namespace TurkiyeFinans.Models
{
    public class Response
    {
        public MernisServiceParametters _mernisserviceparametters { get; set; }
        public Response()
        {
            _mernisserviceparametters = new MernisServiceParametters();
        }
    }
}
