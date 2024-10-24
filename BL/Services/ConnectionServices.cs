using DAL.Data;
using OpcUaClient;

namespace BL.Services
{
    public class ConnectionServices
    {
        MyDbContext dbContext = new();

        private async Task SetLiveBitAsync(bool value)
        {
            await OpcUaClientManager.Instance.WriteValueAsync(@"ns=3;s=""systemStateDB"".""i"".""liveBit""", value);
        }

        public async void InvertLiveBitAsync()
        {
            bool liveBitCurrectValue = await OpcUaClientManager.Instance.ReadValueAsync(@"ns=3;s=""systemStateDB"".""i"".""liveBit""", new bool());
            await SetLiveBitAsync(!liveBitCurrectValue);
        }
    }
}
