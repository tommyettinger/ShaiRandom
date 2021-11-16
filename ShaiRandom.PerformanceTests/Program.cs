using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Troschuetz.Random.Generators;
namespace ShaiRandom.PerformanceTests
{
    /// <summary>
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///|        Distinct | 1.3301 ns | 0.0525 ns | 0.0562 ns | 1.3315 ns |
    ///|           Laser | 1.3228 ns | 0.0137 ns | 0.0128 ns | 1.3242 ns |
    ///|        Tricycle | 3.2720 ns | 0.0938 ns | 0.1186 ns | 3.3170 ns |
    ///|       FourWheel | 3.2035 ns | 0.0453 ns | 0.0354 ns | 3.2057 ns |
    ///|        Stranger | 3.5039 ns | 0.0417 ns | 0.0370 ns | 3.5178 ns |
    ///| XoshiroStarStar | 5.0989 ns | 0.0163 ns | 0.0152 ns | 5.0961 ns |
    ///|        RomuTrio | 4.5261 ns | 0.1178 ns | 0.2241 ns | 4.4347 ns |
    ///|             ALF | 2.4955 ns | 0.0754 ns | 0.0981 ns | 2.5089 ns |
    ///|         MT19937 | 3.3030 ns | 0.0623 ns | 0.0553 ns | 3.3183 ns |
    ///|             NR3 | 2.1339 ns | 0.0463 ns | 0.0387 ns | 2.1470 ns |
    ///|           NR3Q1 | 0.7869 ns | 0.0272 ns | 0.0255 ns | 0.7935 ns |
    ///|           NR3Q2 | 1.0056 ns | 0.0355 ns | 0.0409 ns | 1.0060 ns |
    ///|     XorShift128 | 0.9189 ns | 0.0222 ns | 0.0218 ns | 0.9231 ns |
    /// </summary>
    /// <remarks>
    /// It looks like .NET does virtually no optimizations relating to ILP,
    /// and smaller states are almost always better. I have no clue why RomuTrio,
    /// with 3 states, is slower than any of the 4-state generators, while Tricycle,
    /// also with 3 states, is faster than any of the 4-state generators. Laser
    /// sometimes seems to get optimized much more or less than other times; here,
    /// it's the fastest of the 64-bit generators except for XorShift128 (which is
    /// a medium-low-quality generator in various ways).
    /// </remarks>
    public class RandomUintComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly StrangerRandom _strangerRandom = new StrangerRandom(1UL);
        private readonly Xoshiro256StarStarRandom _xoshiro256StarStarRandom = new Xoshiro256StarStarRandom(1UL);
        private readonly RomuTrioRandom _romuTrioRandom = new RomuTrioRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);

        //Troschuetz.Random

        private readonly XorShift128Generator _xorShift128Generator = new XorShift128Generator(1u);
        private readonly ALFGenerator _aLFGenerator = new ALFGenerator(1u);
        private readonly NR3Generator _nR3Generator = new NR3Generator(1u);
        private readonly NR3Q1Generator _nR3Q1Generator = new NR3Q1Generator(1u);
        private readonly NR3Q2Generator _nR3Q2Generator = new NR3Q2Generator(1u);
        private readonly MT19937Generator _mT19937Generator = new MT19937Generator(1u);

        [Benchmark]
        public uint Distinct() => _distinctRandom.NextUint();

        [Benchmark]
        public uint Laser() => _laserRandom.NextUint();

        [Benchmark]
        public uint Tricycle() => _tricycleRandom.NextUint();

        [Benchmark]
        public uint FourWheel() => _fourWheelRandom.NextUint();

        [Benchmark]
        public uint Stranger() => _strangerRandom.NextUint();

        [Benchmark]
        public uint XoshiroStarStar() => _xoshiro256StarStarRandom.NextUint();

        [Benchmark]
        public uint RomuTrio() => _romuTrioRandom.NextUint();

        [Benchmark]
        public uint Mizuchi() => _mizuchiRandom.NextUint();

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
    /// <summary>
    ///
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///|        Distinct |  3.781 ns | 0.1009 ns | 0.1686 ns |  3.863 ns |
    ///|           Laser |  3.314 ns | 0.0692 ns | 0.0971 ns |  3.261 ns |
    ///|        Tricycle |  5.236 ns | 0.1315 ns | 0.1843 ns |  5.323 ns |
    ///|       FourWheel |  4.834 ns | 0.0486 ns | 0.0431 ns |  4.843 ns |
    ///|        Stranger |  6.172 ns | 0.1508 ns | 0.2561 ns |  6.328 ns |
    ///| XoshiroStarStar |  7.424 ns | 0.1749 ns | 0.2670 ns |  7.296 ns |
    ///|        RomuTrio |  6.624 ns | 0.1616 ns | 0.3582 ns |  6.745 ns |
    ///|             ALF |  7.655 ns | 0.1762 ns | 0.2412 ns |  7.738 ns |
    ///|         MT19937 | 11.266 ns | 0.2292 ns | 0.2453 ns | 11.361 ns |
    ///|             NR3 |  5.611 ns | 0.1411 ns | 0.1680 ns |  5.685 ns |
    ///|           NR3Q1 |  4.139 ns | 0.1112 ns | 0.1366 ns |  4.101 ns |
    ///|           NR3Q2 |  4.156 ns | 0.0281 ns | 0.0263 ns |  4.155 ns |
    ///|     XorShift128 |  3.900 ns | 0.1080 ns | 0.1060 ns |  3.932 ns |
    /// </summary>
    public class RandomUintBoundedComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly StrangerRandom _strangerRandom = new StrangerRandom(1UL);
        private readonly Xoshiro256StarStarRandom _xoshiro256StarStarRandom = new Xoshiro256StarStarRandom(1UL);
        private readonly RomuTrioRandom _romuTrioRandom = new RomuTrioRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);

        //Troschuetz.Random

        private readonly XorShift128Generator _xorShift128Generator = new XorShift128Generator(1u);
        private readonly ALFGenerator _aLFGenerator = new ALFGenerator(1u);
        private readonly NR3Generator _nR3Generator = new NR3Generator(1u);
        private readonly NR3Q1Generator _nR3Q1Generator = new NR3Q1Generator(1u);
        private readonly NR3Q2Generator _nR3Q2Generator = new NR3Q2Generator(1u);
        private readonly MT19937Generator _mT19937Generator = new MT19937Generator(1u);

        [Benchmark]
        public uint Distinct() => _distinctRandom.NextUint(1u, 1000u);

        [Benchmark]
        public uint Laser() => _laserRandom.NextUint(1u, 1000u);

        [Benchmark]
        public uint Tricycle() => _tricycleRandom.NextUint(1u, 1000u);

        [Benchmark]
        public uint FourWheel() => _fourWheelRandom.NextUint(1u, 1000u);

        [Benchmark]
        public uint Stranger() => _strangerRandom.NextUint(1u, 1000u);

        [Benchmark]
        public uint XoshiroStarStar() => _xoshiro256StarStarRandom.NextUint(1u, 1000u);

        [Benchmark]
        public uint RomuTrio() => _romuTrioRandom.NextUint(1u, 1000u);

        [Benchmark]
        public uint Mizuchi() => _mizuchiRandom.NextUint(1u, 1000u);

        [Benchmark]
        public uint ALF() => _aLFGenerator.NextUInt(1u, 1000u);

        [Benchmark]
        public uint MT19937() => _mT19937Generator.NextUInt(1u, 1000u);

        [Benchmark]
        public uint NR3() => _nR3Generator.NextUInt(1u, 1000u);

        [Benchmark]
        public uint NR3Q1() => _nR3Q1Generator.NextUInt(1u, 1000u);

        [Benchmark]
        public uint NR3Q2() => _nR3Q2Generator.NextUInt(1u, 1000u);

        [Benchmark]
        public uint XorShift128() => _xorShift128Generator.NextUInt(1u, 1000u);
    }
    /// <summary>
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///|        Distinct | 1.0671 ns | 0.0520 ns | 0.1275 ns | 1.1181 ns |
    ///|           Laser | 1.0966 ns | 0.0544 ns | 0.1195 ns | 1.1619 ns |
    ///|        Tricycle | 1.9388 ns | 0.0701 ns | 0.1399 ns | 2.0169 ns |
    ///|       FourWheel | 3.1970 ns | 0.0931 ns | 0.2369 ns | 3.2971 ns |
    ///|        Stranger | 2.8947 ns | 0.0882 ns | 0.1955 ns | 2.9432 ns |
    ///| XoshiroStarStar | 3.6905 ns | 0.1054 ns | 0.2128 ns | 3.7494 ns |
    ///|        RomuTrio | 4.2563 ns | 0.0196 ns | 0.0183 ns | 4.2569 ns |
    ///|         Mizuchi | 0.9028 ns | 0.0492 ns | 0.1079 ns | 0.8849 ns |
    ///|             NR3 | 2.2041 ns | 0.0773 ns | 0.1133 ns | 2.1215 ns |
    ///|           NR3Q1 | 0.8976 ns | 0.0479 ns | 0.1041 ns | 0.9279 ns |
    ///|           NR3Q2 | 1.2278 ns | 0.0562 ns | 0.1210 ns | 1.2776 ns |
    ///|     XorShift128 | 0.7221 ns | 0.0448 ns | 0.0935 ns | 0.7149 ns |
    /// </summary>
    /// <remarks>
    /// LaserRandom sometimes does better than any of the other "high-quality"
    /// generators here, but this time MizuchiRandom took the lead. While XorShift128
    /// is the fastest, it also fails several tests in under a minute of testing with
    /// PractRand, and was also confirmed by its authors to have a severe linear bit
    /// dependency. NR3Q1 is also fast, but is scraping the bottom of the barrel on
    /// statistical quality. Of the Troschuetz.Random generators that can generate
    /// ulong values natively, only NR3 is high-quality (passing PractRand to at least
    /// 64TB), and it is much slower than Mizuchi or Laser (both of which also pass
    /// PractRand to at least 64TB).
    /// </remarks>
    public class RandomUlongComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly StrangerRandom _strangerRandom = new StrangerRandom(1UL);
        private readonly Xoshiro256StarStarRandom _xoshiro256StarStarRandom = new Xoshiro256StarStarRandom(1UL);
        private readonly RomuTrioRandom _romuTrioRandom = new RomuTrioRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);

        //Troschuetz.Random

        private readonly XorShift128Generator _xorShift128Generator = new XorShift128Generator(1u);
        private readonly NR3Generator _nR3Generator = new NR3Generator(1u);
        private readonly NR3Q1Generator _nR3Q1Generator = new NR3Q1Generator(1u);
        private readonly NR3Q2Generator _nR3Q2Generator = new NR3Q2Generator(1u);

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
        public ulong Mizuchi() => _mizuchiRandom.NextUlong();

        [Benchmark]
        public ulong NR3() => _nR3Generator.NextULong();

        [Benchmark]
        public ulong NR3Q1() => _nR3Q1Generator.NextULong();

        [Benchmark]
        public ulong NR3Q2() => _nR3Q2Generator.NextULong();

        [Benchmark]
        public ulong XorShift128() => _xorShift128Generator.NextULong();

    }
    /// <summary>
    ///|          Method |     Mean |     Error |    StdDev |
    ///|---------------- |---------:|----------:|----------:|
    ///|        Distinct | 3.043 ns | 0.0882 ns | 0.1678 ns |
    ///|           Laser | 2.938 ns | 0.0849 ns | 0.1552 ns |
    ///|        Tricycle | 4.437 ns | 0.0844 ns | 0.0705 ns |
    ///|       FourWheel | 4.375 ns | 0.0851 ns | 0.1013 ns |
    ///|        Stranger | 5.169 ns | 0.0808 ns | 0.0993 ns |
    ///| XoshiroStarStar | 6.861 ns | 0.1597 ns | 0.1334 ns |
    ///|        RomuTrio | 6.565 ns | 0.1591 ns | 0.2613 ns |
    /// </summary>
    public class RandomUlongBoundedComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly StrangerRandom _strangerRandom = new StrangerRandom(1UL);
        private readonly Xoshiro256StarStarRandom _xoshiro256StarStarRandom = new Xoshiro256StarStarRandom(1UL);
        private readonly RomuTrioRandom _romuTrioRandom = new RomuTrioRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);

        [Benchmark]
        public ulong Distinct() => _distinctRandom.NextUlong(1UL, 1000UL);

        [Benchmark]
        public ulong Laser() => _laserRandom.NextUlong(1UL, 1000UL);

        [Benchmark]
        public ulong Tricycle() => _tricycleRandom.NextUlong(1UL, 1000UL);

        [Benchmark]
        public ulong FourWheel() => _fourWheelRandom.NextUlong(1UL, 1000UL);

        [Benchmark]
        public ulong Stranger() => _strangerRandom.NextUlong(1UL, 1000UL);

        [Benchmark]
        public ulong XoshiroStarStar() => _xoshiro256StarStarRandom.NextUlong(1UL, 1000UL);

        [Benchmark]
        public ulong RomuTrio() => _romuTrioRandom.NextUlong(1UL, 1000UL);

        [Benchmark]
        public ulong Mizuchi() => _mizuchiRandom.NextUlong(1UL, 1000UL);
    }

    public class RandomDoubleComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly StrangerRandom _strangerRandom = new StrangerRandom(1UL);
        private readonly Xoshiro256StarStarRandom _xoshiro256StarStarRandom = new Xoshiro256StarStarRandom(1UL);
        private readonly RomuTrioRandom _romuTrioRandom = new RomuTrioRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);

        //Troschuetz.Random

        private readonly XorShift128Generator _xorShift128Generator = new XorShift128Generator(1u);
        private readonly ALFGenerator _aLFGenerator = new ALFGenerator(1u);
        private readonly NR3Generator _nR3Generator = new NR3Generator(1u);
        private readonly NR3Q1Generator _nR3Q1Generator = new NR3Q1Generator(1u);
        private readonly NR3Q2Generator _nR3Q2Generator = new NR3Q2Generator(1u);
        private readonly MT19937Generator _mT19937Generator = new MT19937Generator(1u);

        [Benchmark]
        public double Distinct() => _distinctRandom.NextDouble();

        [Benchmark]
        public double Laser() => _laserRandom.NextDouble();

        [Benchmark]
        public double Tricycle() => _tricycleRandom.NextDouble();

        [Benchmark]
        public double FourWheel() => _fourWheelRandom.NextDouble();

        [Benchmark]
        public double Stranger() => _strangerRandom.NextDouble();

        [Benchmark]
        public double XoshiroStarStar() => _xoshiro256StarStarRandom.NextDouble();

        [Benchmark]
        public double RomuTrio() => _romuTrioRandom.NextDouble();

        [Benchmark]
        public double Mizuchi() => _mizuchiRandom.NextDouble();

        [Benchmark]
        public double ALF() => _aLFGenerator.NextDouble();

        [Benchmark]
        public double MT19937() => _mT19937Generator.NextDouble();

        [Benchmark]
        public double NR3() => _nR3Generator.NextDouble();

        [Benchmark]
        public double NR3Q1() => _nR3Q1Generator.NextDouble();

        [Benchmark]
        public double NR3Q2() => _nR3Q2Generator.NextDouble();

        [Benchmark]
        public double XorShift128() => _xorShift128Generator.NextDouble();
    }


    internal static class Program
    {
        private static void Main(string[] args)
            => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
