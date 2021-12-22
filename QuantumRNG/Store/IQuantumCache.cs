using System;
using System.Collections.Generic;

namespace QuantumRNG.Store;

public interface IQuantumCache
{
    public IEnumerable<byte> GetBytesFromCache(int quantity);
    public void InsertIntoCache(IEnumerable<byte> bytes);
    public int GetItemsInCacheCount();
    public void ClearCache();
}