namespace TurkiyeFinans.Models
{
    public class TransferOperations
    {
        private readonly string _connectionString;
        public TransferOperations(string connectionString) {
            _connectionString = connectionString;
        }
    }
}
