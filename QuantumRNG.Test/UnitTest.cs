using Xunit;
using System.Linq;
using System;
using Xunit.Abstractions;

namespace QuantumRNG.Test
{
    public class UnitTest
    {
        private readonly QuantumRNG _qrgn = new();

        private readonly ITestOutputHelper output;

        public UnitTest(ITestOutputHelper output)
        {
            this.output = output;
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

            output.WriteLine($"{response}");
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

            output.WriteLine($"{response}");
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

            output.WriteLine($"{response}");
        }
    }
}