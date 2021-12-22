using Xunit;
using System.Linq;
using System;
using QuantumRNG.Store;
using Xunit.Abstractions;

namespace QuantumRNG.Test
{
    public class UnitTest
    {
        private readonly QuantumRNG _qrgn = new(new InMemoryQuantumCache());

        private readonly ITestOutputHelper _output;

        public UnitTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(1, 4)]
        [InlineData(1, 8)]
        [InlineData(1, 10)]
        [InlineData(1, 12)]
        [InlineData(1, 20)]
        [InlineData(1, 100)]
        [InlineData(1, int.MaxValue)]
        public void ReturnRandomDice(int min, int max)
        {
            var response = _qrgn.NextInt(min, max);

            _output.WriteLine($"{response}");
        }

        [Theory]
        [InlineData(-80, 2)]
        [InlineData(-6, 4)]
        [InlineData(0, 8)]
        [InlineData(-9, 10)]
        [InlineData(-7, 12)]
        [InlineData(5, 20)]
        [InlineData(-99, 100)]
        [InlineData(-100, 0)]
        [InlineData(int.MinValue, 0)]
        public void ReturnRandomNegativeBoundInteger(int min, int max)
        {
            var response = _qrgn.NextInt(min, max);

            _output.WriteLine($"{response}");
        }

        [Theory]
        [InlineData(1, 2, 1)]
        [InlineData(1, 4, 3)]
        [InlineData(1, 8, 8)]
        [InlineData(1, 10, 7)]
        [InlineData(1, 12, 10)]
        [InlineData(1, 20, 5)]
        [InlineData(1, 100, 0)]
        public void ReturnRandomDouble(int min, int max, short decimalPlaces)
        {
            var response = _qrgn.NextDouble(min, max, decimalPlaces);

            _output.WriteLine($"{response}");
        }

        [Fact]
        public void ClearCache()
        {
            _qrgn.Cache.ClearCache();
            Assert.Equal(0, _qrgn.Cache.GetItemsInCacheCount());
        }
    }
}