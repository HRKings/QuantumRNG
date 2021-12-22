using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using System.Collections.Generic;
using QuantumRNG.Utils;

namespace QuantumRNG
{
    public class QuantumRNG
    {
        private readonly HttpClient _httpClient;
        private readonly Random _rng;

        private record QrngResponse(string Type, short Length, short Size, string[] Data, bool Success);

        private readonly Queue<byte> _cache;

        private readonly int _cacheSize;

        public QuantumRNG(int cacheSize = 512)
        {
            _httpClient = new HttpClient();
            _rng = new Random();
            _cache = new Queue<byte>();
            _cacheSize = Math.Min(cacheSize, 1024);
        }

        private IEnumerable<byte> GetQuantumNumberFromApi(int quantity)
        {
            var response = _httpClient.GetFromJsonAsync<QrngResponse>
                    ($"https://qrng.anu.edu.au/API/jsonI.php?length={quantity}&type=hex16&size=1024")
                .ConfigureAwait(false).GetAwaiter().GetResult();

            return response?.Data.SelectMany(Convert.FromHexString);
        }

        private IEnumerable<byte> GetBytesFromCache(int quantity)
        {
            if (_cache.Count == 0 || quantity > _cache.Count)
            {
                var request = GetQuantumNumberFromApi(_cacheSize).ToArray();

                foreach (var value in request)
                {
                    _cache.Enqueue(value);
                }
            }

            for (var i = 0; i < quantity; i++)
                yield return _cache.Dequeue();
        }

        public void ClearCache() => _cache.Clear();

        public ulong NextInt(int min, int max)
        {
            IEnumerable<byte> bytesFromCache;
            var byteQuantity = (int) Math.Max(Math.Log2(MathUtils.ApproximatePowerOf2((ulong)max)/8), 1);
            var bytesToComplete = Enumerable.Repeat((byte) 0, 8 - byteQuantity).ToList();
            bytesFromCache = bytesToComplete.Concat(GetBytesFromCache(byteQuantity).ToList());

            var randomNumber =
                BitConverter.ToUInt64(bytesFromCache.ToArray());
            
            return (randomNumber % (ulong)(max - min + 1)) + (ulong)min;
        }

        public double NextDouble(int min, int max, short decimalPlaces) => NextInt(min, max) + (_rng.Next((int)Math.Pow(10, decimalPlaces - 1), (int)Math.Pow(10, decimalPlaces) - 1) / Math.Pow(10, decimalPlaces));
    }
}