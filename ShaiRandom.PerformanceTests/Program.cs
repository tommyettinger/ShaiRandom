using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Troschuetz.Random.Generators;
namespace ShaiRandom.PerformanceTests
{
    /// <summary>
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///;;At this point, the generators produce 64 bits of data.
    ///|        Distinct | 1.0176 ns | 0.0497 ns | 0.0993 ns | 1.0707 ns |
    ///|           Laser | 0.6708 ns | 0.0094 ns | 0.0078 ns | 0.6726 ns |
    ///|        Tricycle | 2.9414 ns | 0.0882 ns | 0.1591 ns | 3.0214 ns |
    ///|       FourWheel | 2.9351 ns | 0.0892 ns | 0.1561 ns | 2.9376 ns |
    ///|        Stranger | 3.1731 ns | 0.0938 ns | 0.1346 ns | 3.1007 ns |
    ///| XoshiroStarStar | 4.5749 ns | 0.1212 ns | 0.2973 ns | 4.6938 ns |
    ///|        RomuTrio | 4.1344 ns | 0.1136 ns | 0.1898 ns | 4.1628 ns |
    ///;;After this point, the generators produce only 32 bits of data.
    ///|             ALF | 2.4696 ns | 0.0316 ns | 0.0280 ns | 2.4690 ns |
    ///|         MT19937 | 3.6200 ns | 0.0212 ns | 0.0188 ns | 3.6140 ns |
    ///|             NR3 | 1.5387 ns | 0.0139 ns | 0.0130 ns | 1.5406 ns |
    ///|           NR3Q1 | 1.0722 ns | 0.0104 ns | 0.0097 ns | 1.0694 ns |
    ///|           NR3Q2 | 1.0225 ns | 0.0186 ns | 0.0165 ns | 1.0261 ns |
    ///|     XorShift128 | 1.0578 ns | 0.0187 ns | 0.0175 ns | 1.0551 ns |
    /// 
    /// </summary>
    /// <remarks>
    /// It looks like .NET does virtually no optimizations relating to ILP,
    /// and smaller states are almost always better. I have no clue why RomuTrio,
    /// with 3 states, is slower than any of the 4-state generators, while Tricycle,
    /// also with 3 states, is faster than any of the 4-state generators. Laser
    /// sometimes seems to get optimized much more or less than other times; here,
    /// it's the fastest by quite a lot.
    /// </remarks>
    public class RandomComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly StrangerRandom _strangerRandom = new StrangerRandom(1UL);

        private readonly Xoshiro256StarStarRandom _xoshiro256StarStarRandom = new Xoshiro256StarStarRandom(1UL);
        private readonly RomuTrioRandom _romuTrioRandom = new RomuTrioRandom(1UL);

        //Troschuetz.Random

        private readonly XorShift128Generator _xorShift128Generator = new XorShift128Generator(1u);
        private readonly ALFGenerator _aLFGenerator = new ALFGenerator(1u);
        private readonly NR3Generator _nR3Generator = new NR3Generator(1u);
        private readonly NR3Q1Generator _nR3Q1Generator = new NR3Q1Generator(1u);
        private readonly NR3Q2Generator _nR3Q2Generator = new NR3Q2Generator(1u);
        private readonly MT19937Generator _mT19937Generator = new MT19937Generator(1u);

        [Benchmark]
        public ulong Distinct() => _distinctRandom.NextUlong();

        [Benchmark]
        public ulong Laser() => _laserRandom.NextUlong();

        [Benchmark]
        public ulong Tricycle() => _tricycleRandom.NextUlong();

        [Benchmark]
        public ulong FourWheel() => _fourWheelRandom.NextUlong();

        [Benchmark]
        public ulong Stranger() => _strangerRandom.NextUlong();

        [Benchmark]
        public ulong XoshiroStarStar() => _xoshiro256StarStarRandom.NextUlong();

        [Benchmark]
        public ulong RomuTrio() => _romuTrioRandom.NextUlong();

        [Benchmark]
        public uint ALF() => _aLFGenerator.NextUIntInclusiveMaxValue();

        [Benchmark]
        public uint MT19937() => _mT19937Generator.NextUIntInclusiveMaxValue();

        [Benchmark]
        public uint NR3() => _nR3Generator.NextUIntInclusiveMaxValue();

        [Benchmark]
        public uint NR3Q1() => _nR3Q1Generator.NextUIntInclusiveMaxValue();

        [Benchmark]
        public uint NR3Q2() => _nR3Q2Generator.NextUIntInclusiveMaxValue();

        [Benchmark]
        public uint XorShift128() => _xorShift128Generator.NextUIntInclusiveMaxValue();
    }
    internal static class Program
    {
        private static void Main(string[] args)
            => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
