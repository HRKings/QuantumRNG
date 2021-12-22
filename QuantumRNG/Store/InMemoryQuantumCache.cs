using System;
using System.Collections.Generic;

namespace QuantumRNG.Store;

public class InMemoryQuantumCache : IQuantumCache
{
    private readonly Queue<byte> _cache;

    public InMemoryQuantumCache()
    {
        _cache = new Queue<byte>();
    }

    public IEnumerable<byte> GetBytesFromCache(int quantity)
    {
        for (var i = 0; i < quantity; i++)
            yield return _cache.Dequeue();
    }

    public void InsertIntoCache(IEnumerable<byte> bytes)
    {
        foreach (var quantumByte in bytes)
            _cache.Enqueue(quantumByte);
    }

    public int GetItemsInCacheCount() => _cache.Count;

    public void ClearCache() => _cache.Clear();
}