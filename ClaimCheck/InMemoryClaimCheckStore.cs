using GettingStarted.ClaimCheck.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GettingStarted.ClaimCheck;

public class InMemoryClaimCheckStore : IClaimCheckStore
{
    private readonly Dictionary<string, byte[]> _store = new();
    public async Task<string> StoreAsync(byte[] messageData)
    {
        var key = Guid.NewGuid().ToString("N");
        _store[key] = messageData;
        return await Task.FromResult(key);
    }

    public async Task<byte[]> RetrieveAsync(string reference)
    {
        if (!_store.TryGetValue(reference, out var data))
            throw new InvalidOperationException("Reference not found");
        return await Task.FromResult(data);
    }
}