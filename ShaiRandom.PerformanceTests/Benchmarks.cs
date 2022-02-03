using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ShaiRandom.Generators;
using Troschuetz.Random.Generators;
#if NETCOREAPP3_0_OR_GREATER
using System.Numerics;
#endif

namespace ShaiRandom.PerformanceTests
{
    /// <summary>
	///.NET 5.0
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
	///.NET 6.0:
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///|        Distinct | 1.3143 ns | 0.0481 ns | 0.0472 ns | 1.3149 ns |
    ///|           Laser | 1.3786 ns | 0.0521 ns | 0.0557 ns | 1.4028 ns |
    ///|        Tricycle | 1.3994 ns | 0.0548 ns | 0.0960 ns | 1.4011 ns |
    ///|       FourWheel | 1.6187 ns | 0.0587 ns | 0.0947 ns | 1.6493 ns |
    ///|        Stranger | 1.5240 ns | 0.0589 ns | 0.1061 ns | 1.4495 ns |
    ///| XoshiroStarStar | 2.0554 ns | 0.0693 ns | 0.1606 ns | 2.0958 ns |
    ///|        RomuTrio | 2.0602 ns | 0.0606 ns | 0.0567 ns | 2.0751 ns |
    ///|         Mizuchi | 1.4402 ns | 0.0550 ns | 0.0919 ns | 1.4613 ns |
    ///|             ALF | 2.4424 ns | 0.0229 ns | 0.0215 ns | 2.4390 ns |
    ///|         MT19937 | 3.5251 ns | 0.0318 ns | 0.0297 ns | 3.5239 ns |
    ///|             NR3 | 1.5136 ns | 0.0156 ns | 0.0146 ns | 1.5088 ns |
    ///|           NR3Q1 | 0.8354 ns | 0.0085 ns | 0.0080 ns | 0.8369 ns |
    ///|           NR3Q2 | 1.0360 ns | 0.0509 ns | 0.1229 ns | 0.9810 ns |
    ///|     XorShift128 | 0.8993 ns | 0.0103 ns | 0.0096 ns | 0.8964 ns |
    /// </summary>
    public class RandomUIntComparison
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
        public uint Distinct() => _distinctRandom.NextUInt();

        [Benchmark]
        public uint Laser() => _laserRandom.NextUInt();

        [Benchmark]
        public uint Tricycle() => _tricycleRandom.NextUInt();

        [Benchmark]
        public uint FourWheel() => _fourWheelRandom.NextUInt();

        [Benchmark]
        public uint Stranger() => _strangerRandom.NextUInt();

        [Benchmark]
        public uint XoshiroStarStar() => _xoshiro256StarStarRandom.NextUInt();

        [Benchmark]
        public uint RomuTrio() => _romuTrioRandom.NextUInt();

        [Benchmark]
        public uint Mizuchi() => _mizuchiRandom.NextUInt();

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
    /// On .NET 5.0:
    /// <code>
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
    ///</code>
    ///On .NET 6.0:
    ///<code>
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///|          Seeded | 10.714 ns | 0.2373 ns | 0.3001 ns | 10.693 ns |
    ///|        Unseeded |  3.811 ns | 0.1048 ns | 0.2117 ns |  3.916 ns |
    ///|        Distinct |  1.630 ns | 0.0615 ns | 0.1229 ns |  1.587 ns |
    ///|           Laser |  1.540 ns | 0.0576 ns | 0.1024 ns |  1.479 ns |
    ///|        Tricycle |  1.452 ns | 0.0580 ns | 0.1322 ns |  1.347 ns |
    ///|       FourWheel |  1.801 ns | 0.0649 ns | 0.1203 ns |  1.846 ns |
    ///|        Stranger |  1.794 ns | 0.0126 ns | 0.0118 ns |  1.792 ns |
    ///| XoshiroStarStar |  2.210 ns | 0.0218 ns | 0.0193 ns |  2.211 ns |
    ///|        RomuTrio |  2.310 ns | 0.0139 ns | 0.0130 ns |  2.309 ns |
    ///|         Mizuchi |  1.077 ns | 0.0672 ns | 0.1194 ns |  1.089 ns |
    ///|             ALF |  6.987 ns | 0.1673 ns | 0.3601 ns |  7.130 ns |
    ///|         MT19937 |  9.910 ns | 0.2284 ns | 0.4060 ns | 10.059 ns |
    ///|             NR3 |  4.329 ns | 0.1148 ns | 0.2318 ns |  4.339 ns |
    ///|           NR3Q1 |  2.803 ns | 0.0847 ns | 0.1671 ns |  2.753 ns |
    ///|           NR3Q2 |  3.218 ns | 0.0905 ns | 0.1537 ns |  3.300 ns |
    ///|     XorShift128 |  2.756 ns | 0.0827 ns | 0.1632 ns |  2.802 ns |
    ///</code>
    /// </summary>
    public class RandomUIntBoundedComparison
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

#if NET6_0
        private readonly System.Random _seededRandom = new System.Random(1);
        private readonly System.Random _unseededRandom = new System.Random();

        [Benchmark]
        public int Seeded() => _seededRandom.Next(1, 1000);

        [Benchmark]
        public int Unseeded() => _unseededRandom.Next(1, 1000);
#endif

        [Benchmark]
        public uint Distinct() => _distinctRandom.NextUInt(1u, 1000u);

        [Benchmark]
        public uint Laser() => _laserRandom.NextUInt(1u, 1000u);

        [Benchmark]
        public uint Tricycle() => _tricycleRandom.NextUInt(1u, 1000u);

        [Benchmark]
        public uint FourWheel() => _fourWheelRandom.NextUInt(1u, 1000u);

        [Benchmark]
        public uint Stranger() => _strangerRandom.NextUInt(1u, 1000u);

        [Benchmark]
        public uint XoshiroStarStar() => _xoshiro256StarStarRandom.NextUInt(1u, 1000u);

        [Benchmark]
        public uint RomuTrio() => _romuTrioRandom.NextUInt(1u, 1000u);

        [Benchmark]
        public uint Mizuchi() => _mizuchiRandom.NextUInt(1u, 1000u);

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
	/// .NET 5.0:
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
	/// .NET 6.0:
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///|        Distinct | 0.9392 ns | 0.0469 ns | 0.0502 ns | 0.9213 ns |
    ///|           Laser | 1.0175 ns | 0.0499 ns | 0.0848 ns | 1.0256 ns |
    ///|        Tricycle | 1.0037 ns | 0.0492 ns | 0.0766 ns | 0.9618 ns |
    ///|       FourWheel | 1.0935 ns | 0.0484 ns | 0.0497 ns | 1.0776 ns |
    ///|        Stranger | 1.2431 ns | 0.0540 ns | 0.0945 ns | 1.2302 ns |
    ///| XoshiroStarStar | 1.8845 ns | 0.0671 ns | 0.1528 ns | 1.8944 ns |
    ///|        RomuTrio | 1.9390 ns | 0.0677 ns | 0.1131 ns | 1.9366 ns |
    ///|         Mizuchi | 0.7374 ns | 0.0102 ns | 0.0080 ns | 0.7390 ns |
    ///|             NR3 | 2.5752 ns | 0.0794 ns | 0.1033 ns | 2.5637 ns |
    ///|           NR3Q1 | 1.0214 ns | 0.0472 ns | 0.0505 ns | 1.0208 ns |
    ///|           NR3Q2 | 1.3573 ns | 0.0557 ns | 0.1087 ns | 1.3559 ns |
    ///|     XorShift128 | 0.8180 ns | 0.0468 ns | 0.0878 ns | 0.8324 ns |
    /// </summary>
    public class RandomULongComparison
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
        public ulong Distinct() => _distinctRandom.NextULong();

        [Benchmark]
        public ulong Laser() => _laserRandom.NextULong();

        [Benchmark]
        public ulong Tricycle() => _tricycleRandom.NextULong();

        [Benchmark]
        public ulong FourWheel() => _fourWheelRandom.NextULong();

        [Benchmark]
        public ulong Stranger() => _strangerRandom.NextULong();

        [Benchmark]
        public ulong XoshiroStarStar() => _xoshiro256StarStarRandom.NextULong();

        [Benchmark]
        public ulong RomuTrio() => _romuTrioRandom.NextULong();

        [Benchmark]
        public ulong Mizuchi() => _mizuchiRandom.NextULong();

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
    /// On .NET 5.0:
    /// <code>
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
    /// </code>
    /// On .NET 6.0:
    /// <code>
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///|          Seeded | 24.306 ns | 0.4981 ns | 0.6117 ns | 24.575 ns |
    ///|        Unseeded |  3.857 ns | 0.1056 ns | 0.2109 ns |  3.893 ns |
    ///|        Distinct |  1.476 ns | 0.0594 ns | 0.1056 ns |  1.524 ns |
    ///|           Laser |  1.545 ns | 0.0577 ns | 0.0482 ns |  1.568 ns |
    ///|        Tricycle |  1.436 ns | 0.0563 ns | 0.1044 ns |  1.470 ns |
    ///|       FourWheel |  1.593 ns | 0.0610 ns | 0.0874 ns |  1.626 ns |
    ///|        Stranger |  1.563 ns | 0.0606 ns | 0.0907 ns |  1.611 ns |
    ///| XoshiroStarStar |  2.114 ns | 0.0734 ns | 0.1360 ns |  2.177 ns |
    ///|        RomuTrio |  2.231 ns | 0.0737 ns | 0.1403 ns |  2.282 ns |
    ///|         Mizuchi |  1.666 ns | 0.0622 ns | 0.1229 ns |  1.711 ns |
    /// </code>
    /// </summary>
    public class RandomULongBoundedComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly StrangerRandom _strangerRandom = new StrangerRandom(1UL);
        private readonly Xoshiro256StarStarRandom _xoshiro256StarStarRandom = new Xoshiro256StarStarRandom(1UL);
        private readonly RomuTrioRandom _romuTrioRandom = new RomuTrioRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);

#if NET6_0
        private readonly System.Random _seededRandom = new System.Random(1);
        private readonly System.Random _unseededRandom = new System.Random();

        [Benchmark]
        public long Seeded() => _seededRandom.NextInt64(1L, 1000L);

        [Benchmark]
        public long Unseeded() => _unseededRandom.NextInt64(1L, 1000L);
#endif

        [Benchmark]
        public ulong Distinct() => _distinctRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong Laser() => _laserRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong Tricycle() => _tricycleRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong FourWheel() => _fourWheelRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong Stranger() => _strangerRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong XoshiroStarStar() => _xoshiro256StarStarRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong RomuTrio() => _romuTrioRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong Mizuchi() => _mizuchiRandom.NextULong(1UL, 1000UL);
    }
    /// <summary>
	/// .NET 5.0:
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
	/// .NET 6.0:
    ///|          Method |     Mean |     Error |    StdDev |
    ///|---------------- |---------:|----------:|----------:|
    ///|        Distinct | 2.183 ns | 0.0725 ns | 0.1396 ns |
    ///|           Laser | 2.182 ns | 0.0745 ns | 0.1453 ns |
    ///|        Tricycle | 1.973 ns | 0.0711 ns | 0.1560 ns |
    ///|       FourWheel | 2.579 ns | 0.0808 ns | 0.1738 ns |
    ///|        Stranger | 2.279 ns | 0.0766 ns | 0.1301 ns |
    ///| XoshiroStarStar | 2.819 ns | 0.0875 ns | 0.1555 ns |
    ///|        RomuTrio | 2.839 ns | 0.0262 ns | 0.0245 ns |
    ///|         Mizuchi | 2.426 ns | 0.0277 ns | 0.0260 ns |
    ///|             ALF | 6.177 ns | 0.0264 ns | 0.0221 ns |
    ///|         MT19937 | 9.205 ns | 0.0421 ns | 0.0352 ns |
    ///|             NR3 | 3.694 ns | 0.0147 ns | 0.0123 ns |
    ///|           NR3Q1 | 1.770 ns | 0.0321 ns | 0.0300 ns |
    ///|           NR3Q2 | 2.136 ns | 0.0259 ns | 0.0242 ns |
    ///|     XorShift128 | 1.599 ns | 0.0167 ns | 0.0157 ns |
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
	/// .NET 5.0, maybe?
    ///|    Method |     Mean |     Error |    StdDev |   Median |
    ///|---------- |---------:|----------:|----------:|---------:|
    ///|  Distinct | 2.524 ns | 0.0783 ns | 0.2118 ns | 2.613 ns |
    ///|     Laser | 2.461 ns | 0.0346 ns | 0.0324 ns | 2.466 ns |
    ///| FourWheel | 3.949 ns | 0.0143 ns | 0.0127 ns | 3.952 ns |
    ///|   Mizuchi | 2.477 ns | 0.0376 ns | 0.0352 ns | 2.476 ns |
	/// .NET 6.0:
    ///|    Method |     Mean |     Error |    StdDev |   Median |
    ///|---------- |---------:|----------:|----------:|---------:|
    ///|  Distinct | 2.765 ns | 0.0822 ns | 0.1304 ns | 2.771 ns |
    ///|     Laser | 2.438 ns | 0.0761 ns | 0.1333 ns | 2.508 ns |
    ///|  Tricycle | 2.460 ns | 0.0790 ns | 0.1632 ns | 2.496 ns |
    ///| FourWheel | 2.586 ns | 0.0780 ns | 0.0835 ns | 2.607 ns |
    ///|   Mizuchi | 2.359 ns | 0.0757 ns | 0.1645 ns | 2.390 ns |
    /// </summary>
    public class RandomExclusiveDoubleComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);

        [Benchmark]
        public double Distinct() => _distinctRandom.NextExclusiveDouble();

        [Benchmark]
        public double Laser() => _laserRandom.NextExclusiveDouble();

        [Benchmark]
        public double Tricycle() => _tricycleRandom.NextExclusiveDouble();

        [Benchmark]
        public double FourWheel() => _fourWheelRandom.NextExclusiveDouble();

        [Benchmark]
        public double Mizuchi() => _mizuchiRandom.NextExclusiveDouble();
    }

    /// <summary>
    /// .NET 6
    ///|       Method |     Mean |     Error |    StdDev |   Median |
    ///|------------- |---------:|----------:|----------:|---------:|
    ///|      Strange | 2.691 ns | 0.0816 ns | 0.1341 ns | 2.690 ns |
    ///|        Bitsy | 2.124 ns | 0.0662 ns | 0.0680 ns | 2.140 ns |
    ///| NotExclusive | 2.337 ns | 0.0750 ns | 0.1372 ns | 2.409 ns |
    /// .NET 5
    ///|       Method |     Mean |     Error |    StdDev |
    ///|------------- |---------:|----------:|----------:|
    ///|      Strange | 2.591 ns | 0.0767 ns | 0.0717 ns |
    ///|        Bitsy | 2.381 ns | 0.0755 ns | 0.1262 ns |
    ///| NotExclusive | 2.101 ns | 0.0610 ns | 0.0571 ns |
    /// .NET Core 3.1
    ///|       Method |     Mean |     Error |    StdDev |   Median |
    ///|------------- |---------:|----------:|----------:|---------:|
    ///|      Strange | 2.791 ns | 0.0832 ns | 0.1500 ns | 2.862 ns |
    ///|        Bitsy | 2.480 ns | 0.0779 ns | 0.1425 ns | 2.428 ns |
    ///| NotExclusive | 2.569 ns | 0.0786 ns | 0.1438 ns | 2.666 ns |
    /// .NET Core 3.0
    ///|       Method |     Mean |     Error |    StdDev |   Median |
    ///|------------- |---------:|----------:|----------:|---------:|
    ///|      Strange | 2.913 ns | 0.0864 ns | 0.1684 ns | 2.983 ns |
    ///|        Bitsy | 2.499 ns | 0.0595 ns | 0.0497 ns | 2.490 ns |
    ///| NotExclusive | 2.477 ns | 0.0791 ns | 0.1466 ns | 2.380 ns |
    /// .NET Core 3.1, but without using BitOperations
    ///|       Method |     Mean |     Error |    StdDev |
    ///|------------- |---------:|----------:|----------:|
    ///|      Strange | 2.671 ns | 0.0491 ns | 0.0566 ns |
    ///|        Bitsy | 9.064 ns | 0.2100 ns | 0.3787 ns |
    ///| NotExclusive | 2.226 ns | 0.0750 ns | 0.1253 ns |
    /// </summary>
    /// <remarks>
    /// Just like on the JVM, the bitwise method of getting exclusive doubles is not just fast, it's faster than the "normal" way.
    /// It also gets closer to 0.0 (without reaching it) than that "normal" way. I have so far only verified that Bitsy performs this well on .NET 6.
    /// On .NET 5, the "normal" way is a fair bit faster than the Bitsy way. But, on .NET 3.1, Bitsy and the "normal" way are very close, with Bitsy
    /// just barely faster. There's no clearly-better performance on .NET Core 3.0, other than Strange just not being very fast. If the BitOperations
    /// class isn't available, which should only be true for .NET Standard 2.1 at this point, Bitsy gets incredibly slow relative to its normal state.
    /// There is likely quite a bit of imprecision in the measurements at the sub-nanosecond level.
    /// </remarks>
    public class ExclusiveDoubleComparison
    {
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);

        private static double StrangeDouble(MizuchiRandom random)
        {
            long bits = random.NextLong();
            return BitConverter.Int64BitsToDouble((0x7C10000000000000L + (BitConverter.DoubleToInt64Bits(-0x7FFFFFFFFFFFF001L | bits) & -0x0010000000000000L)) | (~bits & 0x000FFFFFFFFFFFFFL));
        }
        private static double BitsyDouble(MizuchiRandom random)
        {
            ulong bits = random.NextULong();
#if NETCOREAPP3_0_OR_GREATER
            return BitConverter.Int64BitsToDouble(1022L - BitOperations.TrailingZeroCount(bits) << 52 | (long)(bits >> 12));
#else
            ulong v = bits;
            long c = 64L;
            v &= 0UL - v;
            if (v != 0UL) c--;
            if ((v & 0x00000000FFFFFFFFUL) != 0UL) c -= 32;
            if ((v & 0x0000FFFF0000FFFFUL) != 0UL) c -= 16;
            if ((v & 0x00FF00FF00FF00FFUL) != 0UL) c -= 8;
            if ((v & 0x0F0F0F0F0F0F0F0FUL) != 0UL) c -= 4;
            if ((v & 0x3333333333333333UL) != 0UL) c -= 2;
            if ((v & 0x5555555555555555UL) != 0UL) c -= 1;
            return BitConverter.Int64BitsToDouble(1022L - c << 52 | (long)(bits >> 12));
#endif
        }
        [Benchmark]
        public double Strange() => StrangeDouble(_mizuchiRandom);
        [Benchmark]
        public double Bitsy() => BitsyDouble(_mizuchiRandom);
        [Benchmark]
        public double NotExclusive() => _mizuchiRandom.NextDouble();

    }

    //    /// <summary>
    //    ///|   Method |     Mean |     Error |    StdDev |
    //    ///|--------- |---------:|----------:|----------:|
    //    ///| Distinct | 2.566 ns | 0.0786 ns | 0.1152 ns |
    //    ///|    Laser | 2.224 ns | 0.0755 ns | 0.0982 ns |
    //    ///|  Mizuchi | 2.515 ns | 0.0792 ns | 0.1526 ns |
    //    /// </summary>
    //    public class RandomExclusiveDoubleBitwiseComparison
    //    {
    //        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
    //        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
    //        //private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
    //        //private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
    //        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);
    //
    //        [Benchmark]
    //        public double Distinct() => _distinctRandom.NextExclusiveDoubleBitwise();
    //
    //        [Benchmark]
    //        public double Laser() => _laserRandom.NextExclusiveDoubleBitwise();
    //
    //        //[Benchmark]
    //        //public double Tricycle() => _tricycleRandom.NextExclusiveDoubleBitwise();
    //
    //        //[Benchmark]
    //        //public double FourWheel() => _fourWheelRandom.NextExclusiveDoubleBitwise();
    //
    //        [Benchmark]
    //        public double Mizuchi() => _mizuchiRandom.NextExclusiveDoubleBitwise();
    //    }
    //    /// <summary>
    //    ///|   Method |     Mean |     Error |    StdDev |   Median |
    //    ///|--------- |---------:|----------:|----------:|---------:|
    //    ///| Distinct | 2.736 ns | 0.0845 ns | 0.1959 ns | 2.764 ns |
    //    ///|    Laser | 2.550 ns | 0.0818 ns | 0.0804 ns | 2.573 ns |
    //    ///|  Mizuchi | 3.536 ns | 0.0997 ns | 0.2332 ns | 3.667 ns |
    //    /// </summary>
    //    public class RandomExclusiveDoublUnsafeComparison
    //    {
    //        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
    //        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
    //        //private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
    //        //private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
    //        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);
    //
    //        [Benchmark]
    //        public double Distinct() => _distinctRandom.NextExclusiveDoubleUnsafe();
    //
    //        [Benchmark]
    //        public double Laser() => _laserRandom.NextExclusiveDoubleUnsafe();
    //
    //        //[Benchmark]
    //        //public double Tricycle() => _tricycleRandom.NextExclusiveDoubleBitwise();
    //
    //        //[Benchmark]
    //        //public double FourWheel() => _fourWheelRandom.NextExclusiveDoubleUnsafe();
    //
    //        [Benchmark]
    //        public double Mizuchi() => _mizuchiRandom.NextExclusiveDoubleUnsafe();
    //    }

    /// <summary>
    /// With NextUInt() virtual:
    /// |          Method |     Mean |     Error |    StdDev |   Median |
    /// |---------------- |---------:|----------:|----------:|---------:|
    /// |        Distinct | 2.686 ns | 0.0664 ns | 0.0621 ns | 2.714 ns |
    /// |           Laser | 2.584 ns | 0.0788 ns | 0.1379 ns | 2.627 ns |
    /// |       FourWheel | 2.564 ns | 0.0790 ns | 0.1750 ns | 2.500 ns |
    /// | XoshiroStarStar | 2.724 ns | 0.0825 ns | 0.1828 ns | 2.650 ns |
    /// |        RomuTrio | 2.719 ns | 0.0817 ns | 0.1172 ns | 2.764 ns |
    /// |         Mizuchi | 2.644 ns | 0.0824 ns | 0.1548 ns | 2.610 ns |
    ///
    /// With NextUInt() not virtual:
    /// |          Method |     Mean |     Error |    StdDev |   Median |
    /// |---------------- |---------:|----------:|----------:|---------:|
    /// |        Distinct | 2.731 ns | 0.0821 ns | 0.1541 ns | 2.826 ns |
    /// |           Laser | 2.641 ns | 0.0798 ns | 0.1683 ns | 2.733 ns |
    /// |       FourWheel | 2.516 ns | 0.0783 ns | 0.1097 ns | 2.443 ns |
    /// | XoshiroStarStar | 2.746 ns | 0.0817 ns | 0.1650 ns | 2.799 ns |
    /// |        RomuTrio | 2.981 ns | 0.1057 ns | 0.3116 ns | 2.972 ns |
    /// |         Mizuchi | 2.736 ns | 0.0819 ns | 0.1815 ns | 2.775 ns |
    /// </summary>
    public class VirtualOverheadComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        //private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        //private readonly StrangerRandom _strangerRandom = new StrangerRandom(1UL);
        private readonly Xoshiro256StarStarRandom _xoshiro256StarStarRandom = new Xoshiro256StarStarRandom(1UL);
        private readonly RomuTrioRandom _romuTrioRandom = new RomuTrioRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);

        private uint RunUInt(IEnhancedRandom random) => random.NextUInt();

        [Benchmark]
        public uint Distinct() => RunUInt(_distinctRandom);

        [Benchmark]
        public uint Laser() => RunUInt(_laserRandom);

//        [Benchmark]
//        public uint Tricycle() => RunUInt(_tricycleRandom);

        [Benchmark]
        public uint FourWheel() => RunUInt(_fourWheelRandom);

//        [Benchmark]
//        public uint Stranger() => RunUInt(_strangerRandom);

        [Benchmark]
        public uint XoshiroStarStar() => RunUInt(_xoshiro256StarStarRandom);

        [Benchmark]
        public uint RomuTrio() => RunUInt(_romuTrioRandom);

        [Benchmark]
        public uint Mizuchi() => RunUInt(_mizuchiRandom);
    }


    internal static class Benchmarks
    {
        private static void Main(string[] args)
            => BenchmarkSwitcher.FromAssembly(typeof(Benchmarks).Assembly).Run(args);
    }
}
