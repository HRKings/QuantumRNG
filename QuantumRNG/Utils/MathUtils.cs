namespace QuantumRNG.Utils;

public class MathUtils
{
    public static ulong ApproximatePowerOf2(ulong input)
    {
        input = input - 1;;
        input |= input >> 1;
        input |= input >> 2;
        input |= input >> 4;
        input |= input >> 8;
        input |= input >> 16;

        return input + 1;
    }
}