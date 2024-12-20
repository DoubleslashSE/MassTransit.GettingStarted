using System.Threading.Tasks;

namespace GettingStarted.ClaimCheck.Interfaces;

public interface IClaimCheckStore
{
    Task<string> StoreAsync(byte[] messageData);
    Task<byte[]> RetrieveAsync(string reference);
}