using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using System.Collections.Generic;
using QuantumRNG.Store;
using QuantumRNG.Utils;

namespace QuantumRNG
{
    public class QuantumRNG
    {
        private readonly HttpClient _httpClient;
        private readonly Random _rng;

        private readonly int _cacheSize;
        public IQuantumCache Cache { get; set; }

        private record QrngResponse(string Type, short Length, short Size, string[] Data, bool Success);
        
        public QuantumRNG(IQuantumCache cache, int cacheSize = 512)
        {
            Cache = cache;
            _httpClient = new HttpClient();
            _rng = new Random();
            _cacheSize = cacheSize;
        }

        private IEnumerable<byte> GetQuantumNumberFromApi(int quantity)
        {
            var response = _httpClient.GetFromJsonAsync<QrngResponse>
                    ($"https://qrng.anu.edu.au/API/jsonI.php?length={quantity}&type=hex16&size=1024")
                .ConfigureAwait(false).GetAwaiter().GetResult();

            return response?.Data.SelectMany(Convert.FromHexString);
        }

        private IEnumerable<byte> GetBytes(int quantity)
        {
            if (Cache.GetItemsInCacheCount() == 0 || quantity > Cache.GetItemsInCacheCount())
                Cache.InsertIntoCache(GetQuantumNumberFromApi(_cacheSize));

            return Cache.GetBytesFromCache(quantity);
        }

        public ulong NextInt(int min, int max)
        {
            var byteQuantity = (int) Math.Max(Math.Log2(MathUtils.ApproximatePowerOf2((ulong)max))/8, 1);
            var bytesToComplete = Enumerable.Repeat((byte) 0, 8 - byteQuantity).ToList();
            var bytesFromCache = bytesToComplete.Concat(GetBytes(byteQuantity).ToList());

            var randomNumber =
                BitConverter.ToUInt64(bytesFromCache.ToArray());
            
            return (randomNumber % (ulong)(max - min + 1)) + (ulong)min;
        }

        public double NextDouble(int min, int max, short decimalPlaces) => NextInt(min, max) + (_rng.Next((int)Math.Pow(10, decimalPlaces - 1), (int)Math.Pow(10, decimalPlaces) - 1) / Math.Pow(10, decimalPlaces));
    }
}