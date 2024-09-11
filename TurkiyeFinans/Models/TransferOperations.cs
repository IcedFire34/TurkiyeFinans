namespace TurkiyeFinans.Models
{
    public class TransferOperations
    {
        private readonly string _connectionString;
        public TransferOperations(string connectionString) {
            _connectionString = connectionString;
        }

        //Transfer Gerceklestir
        public async Task<bool> Submit(Transfer transfer)
        {
            //  decimal FromAccountID, decimal ToAccountID, float Amount, string Currency, string TransferDate



            return false;
        }
    }
}
