using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using System.Collections.Generic;

namespace QuantumRNG
{
    public class QuantumRNG
    {
        private readonly HttpClient _httpClient;
        private readonly Random _rng;

        private record QrngResponse(string Type, short Length, short Size, string[] Data, bool Success);

        private readonly Queue<long> _cache;

        private readonly int _cacheSize;

        public QuantumRNG(int cacheSize = 512)
        {
            _httpClient = new HttpClient();
            _rng = new Random();
            _cache = new Queue<long>();
            _cacheSize = Math.Min(cacheSize, 1024);
        }

        private long[] GetQuantumNumberFromAPI(int quantity)
        {
            var response = _httpClient.GetFromJsonAsync<QrngResponse>($"https://qrng.anu.edu.au/API/jsonI.php?length={quantity}&type=hex16&size=7").GetAwaiter().GetResult();

            return response.Data.Select(hex => Convert.ToInt64(hex, 16)).ToArray();
        }

        private long GetFromCache()
        {
            long[] request;

            if (_cache.Count == 0)
            {
                request = GetQuantumNumberFromAPI(_cacheSize);

                foreach (var value in request)
                {
                    _cache.Enqueue(value);
                }
            }

            return _cache.Dequeue();
        }

        public void ClearCache() => _cache.Clear();

        public int NextInt(int min, int max) => (int)(GetFromCache() % (max - min + 1)) + min;

        public double NextDouble(int min, int max, short decimalPlaces) => NextInt(min, max) + (_rng.Next((int)Math.Pow(10, decimalPlaces - 1), (int)Math.Pow(10, decimalPlaces) - 1) / Math.Pow(10, decimalPlaces));
    }
}