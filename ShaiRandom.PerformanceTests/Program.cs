using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Troschuetz.Random.Generators;
namespace ShaiRandom.PerformanceTests
{
    /// <summary>
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///|        Distinct | 1.5092 ns | 0.0589 ns | 0.1280 ns | 1.5623 ns |
    ///|           Laser | 1.3595 ns | 0.0576 ns | 0.1606 ns | 1.3748 ns |
    ///|        Tricycle | 2.9756 ns | 0.0880 ns | 0.2287 ns | 3.1318 ns |
    ///|       FourWheel | 3.7578 ns | 0.1094 ns | 0.3227 ns | 3.7378 ns |
    ///|        Stranger | 3.4388 ns | 0.0969 ns | 0.2303 ns | 3.5175 ns |
    ///| XoshiroStarStar | 4.6913 ns | 0.1233 ns | 0.3093 ns | 4.8695 ns |
    ///|        RomuTrio | 4.7724 ns | 0.1245 ns | 0.3491 ns | 4.9871 ns |
    ///|         Mizuchi | 1.4457 ns | 0.0562 ns | 0.1324 ns | 1.4843 ns |
    ///|             ALF | 2.3270 ns | 0.0741 ns | 0.1298 ns | 2.3713 ns |
    ///|         MT19937 | 3.6102 ns | 0.0999 ns | 0.2041 ns | 3.7089 ns |
    ///|             NR3 | 1.5843 ns | 0.0577 ns | 0.1229 ns | 1.5747 ns |
    ///|           NR3Q1 | 1.1030 ns | 0.0493 ns | 0.0723 ns | 1.0815 ns |
    ///|           NR3Q2 | 1.0030 ns | 0.0476 ns | 0.0795 ns | 0.9987 ns |
    ///|     XorShift128 | 0.8824 ns | 0.0097 ns | 0.0076 ns | 0.8857 ns |
    /// </summary>
    /// <remarks>
    /// It looks like .NET does virtually no optimizations relating to ILP,
    /// and smaller states are almost always better. I have no clue why RomuTrio,
    /// with 3 states, is slower than any of the 4-state generators, while Tricycle,
    /// also with 3 states, is faster than any of the 4-state generators. Laser
    /// sometimes seems to get optimized much more or less than other times; here,
    /// it's the fastest of the 64-bit generators except for XorShift128 (which is
    /// a medium-low-quality generator in various ways). I probably should stop
    /// even testing NR3Q1 and NR3Q2, because they fail PractRand testing in a
    /// matter of seconds (so does XorShift128, but not on as many tests).
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
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///|        Distinct |  4.105 ns | 0.1214 ns | 0.3580 ns |  4.035 ns |
    ///|           Laser |  3.676 ns | 0.1023 ns | 0.2066 ns |  3.673 ns |
    ///|        Tricycle |  5.013 ns | 0.1299 ns | 0.2878 ns |  4.958 ns |
    ///|       FourWheel |  5.349 ns | 0.1343 ns | 0.2833 ns |  5.371 ns |
    ///|        Stranger |  5.211 ns | 0.1354 ns | 0.2736 ns |  5.204 ns |
    ///| XoshiroStarStar |  6.789 ns | 0.1638 ns | 0.3194 ns |  6.776 ns |
    ///|        RomuTrio |  6.603 ns | 0.1620 ns | 0.1436 ns |  6.573 ns |
    ///|         Mizuchi |  3.319 ns | 0.0963 ns | 0.1900 ns |  3.309 ns |
    ///|             ALF |  7.838 ns | 0.1858 ns | 0.3754 ns |  7.746 ns |
    ///|         MT19937 | 11.694 ns | 0.2585 ns | 0.5222 ns | 11.683 ns |
    ///|             NR3 |  6.512 ns | 0.1584 ns | 0.3126 ns |  6.396 ns |
    ///|           NR3Q1 |  4.158 ns | 0.1099 ns | 0.1576 ns |  4.180 ns |
    ///|           NR3Q2 |  4.255 ns | 0.1132 ns | 0.1922 ns |  4.357 ns |
    ///|     XorShift128 |  3.953 ns | 0.1059 ns | 0.1177 ns |  4.004 ns |
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
    ///|          Method |     Mean |     Error |    StdDev |   Median |
    ///|---------------- |---------:|----------:|----------:|---------:|
    ///|        Distinct | 2.904 ns | 0.0174 ns | 0.0154 ns | 2.904 ns |
    ///|           Laser | 2.994 ns | 0.0887 ns | 0.1772 ns | 3.044 ns |
    ///|        Tricycle | 4.134 ns | 0.1125 ns | 0.2607 ns | 4.227 ns |
    ///|       FourWheel | 4.473 ns | 0.1186 ns | 0.2422 ns | 4.301 ns |
    ///|        Stranger | 4.797 ns | 0.1233 ns | 0.2434 ns | 4.826 ns |
    ///| XoshiroStarStar | 5.723 ns | 0.1446 ns | 0.2494 ns | 5.759 ns |
    ///|        RomuTrio | 6.675 ns | 0.1606 ns | 0.3690 ns | 6.628 ns |
    ///|         Mizuchi | 3.018 ns | 0.0902 ns | 0.1672 ns | 3.053 ns |
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
    /// <summary>
    ///|          Method |     Mean |     Error |    StdDev |   Median |
    ///|---------------- |---------:|----------:|----------:|---------:|
    ///|        Distinct | 2.239 ns | 0.0745 ns | 0.1266 ns | 2.297 ns |
    ///|           Laser | 2.639 ns | 0.0840 ns | 0.2045 ns | 2.610 ns |
    ///|        Tricycle | 3.452 ns | 0.1005 ns | 0.2141 ns | 3.454 ns |
    ///|       FourWheel | 3.891 ns | 0.1026 ns | 0.1627 ns | 3.919 ns |
    ///|        Stranger | 4.001 ns | 0.1112 ns | 0.2169 ns | 4.011 ns |
    ///| XoshiroStarStar | 5.190 ns | 0.1360 ns | 0.3608 ns | 5.223 ns |
    ///|        RomuTrio | 5.364 ns | 0.1386 ns | 0.2862 ns | 5.496 ns |
    ///|         Mizuchi | 2.515 ns | 0.0306 ns | 0.0256 ns | 2.511 ns |
    ///|             ALF | 6.454 ns | 0.0569 ns | 0.0504 ns | 6.445 ns |
    ///|         MT19937 | 9.911 ns | 0.1074 ns | 0.0897 ns | 9.878 ns |
    ///|             NR3 | 3.913 ns | 0.0827 ns | 0.0691 ns | 3.911 ns |
    ///|           NR3Q1 | 1.678 ns | 0.0686 ns | 0.1548 ns | 1.692 ns |
    ///|           NR3Q2 | 2.268 ns | 0.0774 ns | 0.1748 ns | 2.294 ns |
    ///|     XorShift128 | 1.697 ns | 0.0661 ns | 0.1141 ns | 1.719 ns |
    /// </summary>
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

    /// <summary>
    ///|    Method |     Mean |     Error |    StdDev |   Median |
    ///|---------- |---------:|----------:|----------:|---------:|
    ///|  Distinct | 2.524 ns | 0.0783 ns | 0.2118 ns | 2.613 ns |
    ///|     Laser | 2.461 ns | 0.0346 ns | 0.0324 ns | 2.466 ns |
    ///| FourWheel | 3.949 ns | 0.0143 ns | 0.0127 ns | 3.952 ns |
    ///|   Mizuchi | 2.477 ns | 0.0376 ns | 0.0352 ns | 2.476 ns |
    /// </summary>
    public class RandomExclusiveDoubleComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        //private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);

        [Benchmark]
        public double Distinct() => _distinctRandom.NextExclusiveDouble();

        [Benchmark]
        public double Laser() => _laserRandom.NextExclusiveDouble();

        //[Benchmark]
        //public double Tricycle() => _tricycleRandom.NextExclusiveDouble();

        [Benchmark]
        public double FourWheel() => _fourWheelRandom.NextExclusiveDouble();

        [Benchmark]
        public double Mizuchi() => _mizuchiRandom.NextExclusiveDouble();
    }

    /// <summary>
    ///|   Method |     Mean |     Error |    StdDev |
    ///|--------- |---------:|----------:|----------:|
    ///| Distinct | 2.566 ns | 0.0786 ns | 0.1152 ns |
    ///|    Laser | 2.224 ns | 0.0755 ns | 0.0982 ns |
    ///|  Mizuchi | 2.515 ns | 0.0792 ns | 0.1526 ns |
    /// </summary>
    public class RandomExclusiveDoubleBitwiseComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        //private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        //private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);

        [Benchmark]
        public double Distinct() => _distinctRandom.NextExclusiveDoubleBitwise();

        [Benchmark]
        public double Laser() => _laserRandom.NextExclusiveDoubleBitwise();

        //[Benchmark]
        //public double Tricycle() => _tricycleRandom.NextExclusiveDoubleBitwise();

        //[Benchmark]
        //public double FourWheel() => _fourWheelRandom.NextExclusiveDoubleBitwise();

        [Benchmark]
        public double Mizuchi() => _mizuchiRandom.NextExclusiveDoubleBitwise();
    }
    /// <summary>
    ///|   Method |     Mean |     Error |    StdDev |   Median |
    ///|--------- |---------:|----------:|----------:|---------:|
    ///| Distinct | 2.736 ns | 0.0845 ns | 0.1959 ns | 2.764 ns |
    ///|    Laser | 2.550 ns | 0.0818 ns | 0.0804 ns | 2.573 ns |
    ///|  Mizuchi | 3.536 ns | 0.0997 ns | 0.2332 ns | 3.667 ns |
    /// </summary>
    public class RandomExclusiveDoublUnsafeComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        //private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        //private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);

        [Benchmark]
        public double Distinct() => _distinctRandom.NextExclusiveDoubleUnsafe();

        [Benchmark]
        public double Laser() => _laserRandom.NextExclusiveDoubleUnsafe();

        //[Benchmark]
        //public double Tricycle() => _tricycleRandom.NextExclusiveDoubleBitwise();

        //[Benchmark]
        //public double FourWheel() => _fourWheelRandom.NextExclusiveDoubleUnsafe();

        [Benchmark]
        public double Mizuchi() => _mizuchiRandom.NextExclusiveDoubleUnsafe();
    }

    internal static class Program
    {
        private static void Main(string[] args)
            => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
