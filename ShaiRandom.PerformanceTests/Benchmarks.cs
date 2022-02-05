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
    ///|        Distinct | 0.4745 ns | 0.0360 ns | 0.0337 ns | 0.4874 ns |
    ///|           Laser | 0.6127 ns | 0.0404 ns | 0.0719 ns | 0.6031 ns |
    ///|        Tricycle | 0.6029 ns | 0.0398 ns | 0.0707 ns | 0.5853 ns |
    ///|       FourWheel | 0.8354 ns | 0.0443 ns | 0.0800 ns | 0.7986 ns |
    ///|        Stranger | 0.8738 ns | 0.0443 ns | 0.0606 ns | 0.8566 ns |
    ///| XoshiroStarStar | 1.3129 ns | 0.0525 ns | 0.0664 ns | 1.3393 ns |
    ///|        RomuTrio | 0.8190 ns | 0.0429 ns | 0.0629 ns | 0.8447 ns |
    ///|         Mizuchi | 0.6612 ns | 0.0395 ns | 0.0638 ns | 0.7030 ns |
    ///|             ALF | 2.2916 ns | 0.0720 ns | 0.1223 ns | 2.3359 ns |
    ///|         MT19937 | 3.4479 ns | 0.0958 ns | 0.1845 ns | 3.5262 ns |
    ///|             NR3 | 1.5639 ns | 0.0608 ns | 0.1081 ns | 1.5296 ns |
    ///|           NR3Q1 | 0.8714 ns | 0.0108 ns | 0.0096 ns | 0.8699 ns |
    ///|           NR3Q2 | 1.0245 ns | 0.0147 ns | 0.0130 ns | 1.0244 ns |
    ///|     XorShift128 | 0.9665 ns | 0.0405 ns | 0.0379 ns | 0.9772 ns |
    ///.NET 6.0:
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///|        Distinct | 0.5506 ns | 0.0386 ns | 0.0726 ns | 0.5839 ns |
    ///|           Laser | 0.5842 ns | 0.0388 ns | 0.0680 ns | 0.5661 ns |
    ///|        Tricycle | 0.6619 ns | 0.0402 ns | 0.0736 ns | 0.6556 ns |
    ///|       FourWheel | 0.8330 ns | 0.0450 ns | 0.0987 ns | 0.8488 ns |
    ///|        Stranger | 0.8706 ns | 0.0074 ns | 0.0058 ns | 0.8700 ns |
    ///| XoshiroStarStar | 1.3646 ns | 0.0535 ns | 0.0733 ns | 1.3581 ns |
    ///|        RomuTrio | 0.3362 ns | 0.0434 ns | 0.0446 ns | 0.3497 ns |
    ///|         Mizuchi | 0.6428 ns | 0.0415 ns | 0.0568 ns | 0.6458 ns |
    ///|             ALF | 2.3453 ns | 0.0670 ns | 0.0626 ns | 2.3330 ns |
    ///|         MT19937 | 3.3950 ns | 0.0800 ns | 0.0748 ns | 3.4054 ns |
    ///|             NR3 | 1.4914 ns | 0.0563 ns | 0.0526 ns | 1.4864 ns |
    ///|           NR3Q1 | 1.0456 ns | 0.0470 ns | 0.0560 ns | 1.0385 ns |
    ///|           NR3Q2 | 0.9520 ns | 0.0185 ns | 0.0164 ns | 0.9470 ns |
    ///|     XorShift128 | 0.7554 ns | 0.0434 ns | 0.0953 ns | 0.7727 ns |
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
    /// On .NET 5.0 (without GlobalSetup):
    /// <code>
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///|        Distinct |  2.064 ns | 0.0676 ns | 0.0724 ns |  2.107 ns |
    ///|           Laser |  1.894 ns | 0.0299 ns | 0.0234 ns |  1.899 ns |
    ///|        Tricycle |  2.351 ns | 0.0731 ns | 0.0782 ns |  2.376 ns |
    ///|       FourWheel |  2.984 ns | 0.0088 ns | 0.0069 ns |  2.987 ns |
    ///|        Stranger |  2.461 ns | 0.0730 ns | 0.1115 ns |  2.406 ns |
    ///| XoshiroStarStar |  2.752 ns | 0.0821 ns | 0.1416 ns |  2.747 ns |
    ///|        RomuTrio |  2.505 ns | 0.0782 ns | 0.1507 ns |  2.445 ns |
    ///|         Mizuchi |  2.120 ns | 0.0726 ns | 0.1064 ns |  2.108 ns |
    ///|             ALF |  7.158 ns | 0.1447 ns | 0.1353 ns |  7.123 ns |
    ///|         MT19937 | 10.896 ns | 0.0986 ns | 0.0874 ns | 10.877 ns |
    ///|             NR3 |  4.722 ns | 0.0429 ns | 0.0401 ns |  4.717 ns |
    ///|           NR3Q1 |  3.002 ns | 0.0336 ns | 0.0298 ns |  3.004 ns |
    ///|           NR3Q2 |  3.317 ns | 0.0914 ns | 0.0855 ns |  3.291 ns |
    ///|     XorShift128 |  2.944 ns | 0.0504 ns | 0.0472 ns |  2.945 ns |
    ///</code>
    ///On .NET 6.0 (using GlobalSetup):
    ///<code>
    ///|          Method |      Mean |     Error |    StdDev |    Median |
    ///|---------------- |----------:|----------:|----------:|----------:|
    ///|          Seeded | 9.9412 ns | 0.0342 ns | 0.0285 ns | 9.9384 ns |
    ///|        Unseeded | 3.6330 ns | 0.0991 ns | 0.1710 ns | 3.7056 ns |
    ///|        Distinct | 1.0242 ns | 0.0477 ns | 0.0770 ns | 1.0721 ns |
    ///|           Laser | 0.8008 ns | 0.0436 ns | 0.0652 ns | 0.7673 ns |
    ///|        Tricycle | 0.7610 ns | 0.0427 ns | 0.0822 ns | 0.8020 ns |
    ///|       FourWheel | 0.8735 ns | 0.0146 ns | 0.0114 ns | 0.8779 ns |
    ///|        Stranger | 0.9106 ns | 0.0107 ns | 0.0089 ns | 0.9125 ns |
    ///| XoshiroStarStar | 1.4380 ns | 0.0588 ns | 0.1188 ns | 1.4370 ns |
    ///|        RomuTrio | 0.8621 ns | 0.0457 ns | 0.0670 ns | 0.8912 ns |
    ///|         Mizuchi | 0.9261 ns | 0.0458 ns | 0.0791 ns | 0.8731 ns |
    ///|             ALF | 6.7523 ns | 0.1446 ns | 0.1352 ns | 6.7941 ns |
    ///|         MT19937 | 9.5423 ns | 0.2136 ns | 0.3388 ns | 9.6623 ns |
    ///|             NR3 | 4.2629 ns | 0.1033 ns | 0.0966 ns | 4.2384 ns |
    ///|           NR3Q1 | 2.6040 ns | 0.0808 ns | 0.0898 ns | 2.6066 ns |
    ///|           NR3Q2 | 2.8465 ns | 0.0649 ns | 0.0542 ns | 2.8663 ns |
    ///|     XorShift128 | 2.5099 ns | 0.0740 ns | 0.0656 ns | 2.5272 ns |
    ///</code>
    /// </summary>
    public class RandomUIntBoundedComparison
    {
        private DistinctRandom _distinctRandom;
        private LaserRandom _laserRandom;
        private TricycleRandom _tricycleRandom;
        private FourWheelRandom _fourWheelRandom;
        private StrangerRandom _strangerRandom;
        private Xoshiro256StarStarRandom _xoshiro256StarStarRandom;
        private RomuTrioRandom _romuTrioRandom;
        private MizuchiRandom _mizuchiRandom;

        //Troschuetz.Random

        private XorShift128Generator _xorShift128Generator;
        private ALFGenerator _aLFGenerator;
        private NR3Generator _nR3Generator;
        private NR3Q1Generator _nR3Q1Generator;
        private NR3Q2Generator _nR3Q2Generator;
        private MT19937Generator _mT19937Generator;

#if NET6_0_OR_GREATER
        private System.Random _seededRandom;
        private System.Random _unseededRandom;
#endif

        [GlobalSetup]
        public void GlobalSetup()
        {
        _distinctRandom = new DistinctRandom(1UL);
        _laserRandom = new LaserRandom(1UL);
        _tricycleRandom = new TricycleRandom(1UL);
        _fourWheelRandom = new FourWheelRandom(1UL);
        _strangerRandom = new StrangerRandom(1UL);
        _xoshiro256StarStarRandom = new Xoshiro256StarStarRandom(1UL);
        _romuTrioRandom = new RomuTrioRandom(1UL);
        _mizuchiRandom = new MizuchiRandom(1UL);

        //Troschuetz.Random

        _xorShift128Generator = new XorShift128Generator(1u);
        _aLFGenerator = new ALFGenerator(1u);
        _nR3Generator = new NR3Generator(1u);
        _nR3Q1Generator = new NR3Q1Generator(1u);
        _nR3Q2Generator = new NR3Q2Generator(1u);
        _mT19937Generator = new MT19937Generator(1u);

#if NET6_0_OR_GREATER
        _seededRandom = new System.Random(1);
        _unseededRandom = new System.Random();
#endif

    }
#if NET6_0_OR_GREATER
    [Benchmark]
        public int Seeded() => _seededRandom.Next(999);

        [Benchmark]
        public int Unseeded() => _unseededRandom.Next(999);
#endif

        [Benchmark]
        public uint Distinct() => _distinctRandom.NextUInt(999u);

        [Benchmark]
        public uint Laser() => _laserRandom.NextUInt(999u);

        [Benchmark]
        public uint Tricycle() => _tricycleRandom.NextUInt(999u);

        [Benchmark]
        public uint FourWheel() => _fourWheelRandom.NextUInt(999u);

        [Benchmark]
        public uint Stranger() => _strangerRandom.NextUInt(999u);

        [Benchmark]
        public uint XoshiroStarStar() => _xoshiro256StarStarRandom.NextUInt(999u);

        [Benchmark]
        public uint RomuTrio() => _romuTrioRandom.NextUInt(999u);

        [Benchmark]
        public uint Mizuchi() => _mizuchiRandom.NextUInt(999u);

        [Benchmark]
        public uint ALF() => _aLFGenerator.NextUInt(999u);

        [Benchmark]
        public uint MT19937() => _mT19937Generator.NextUInt(999u);

        [Benchmark]
        public uint NR3() => _nR3Generator.NextUInt(999u);

        [Benchmark]
        public uint NR3Q1() => _nR3Q1Generator.NextUInt(999u);

        [Benchmark]
        public uint NR3Q2() => _nR3Q2Generator.NextUInt(999u);

        [Benchmark]
        public uint XorShift128() => _xorShift128Generator.NextUInt(999u);
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
    ///|          Method |     Mean |     Error |    StdDev |
    ///|---------------- |---------:|----------:|----------:|
    ///|        Distinct | 3.040 ns | 0.0838 ns | 0.1030 ns |
    ///|           Laser | 3.714 ns | 0.1008 ns | 0.1445 ns |
    ///|        Tricycle | 2.821 ns | 0.0193 ns | 0.0171 ns |
    ///|       FourWheel | 3.295 ns | 0.0938 ns | 0.1488 ns |
    ///|        Stranger | 3.221 ns | 0.0896 ns | 0.1421 ns |
    ///| XoshiroStarStar | 3.316 ns | 0.0785 ns | 0.0655 ns |
    ///|        RomuTrio | 2.778 ns | 0.0345 ns | 0.0270 ns |
    ///|         Mizuchi | 2.839 ns | 0.0718 ns | 0.0636 ns |
    /// </code>
    /// On .NET 6.0:
    /// <code>
    ///|          Method |       Mean |     Error |    StdDev |     Median |
    ///|---------------- |-----------:|----------:|----------:|-----------:|
    ///|          Seeded | 24.2991 ns | 0.5075 ns | 0.8196 ns | 24.6338 ns |
    ///|        Unseeded |  3.8089 ns | 0.1042 ns | 0.2033 ns |  3.7476 ns |
    ///|        Distinct |  0.8029 ns | 0.0447 ns | 0.0783 ns |  0.7884 ns |
    ///|           Laser |  0.7350 ns | 0.0375 ns | 0.0474 ns |  0.7127 ns |
    ///|        Tricycle |  0.7600 ns | 0.0441 ns | 0.0795 ns |  0.7471 ns |
    ///|       FourWheel |  0.9675 ns | 0.0463 ns | 0.0846 ns |  0.9673 ns |
    ///|        Stranger |  0.9746 ns | 0.0476 ns | 0.0822 ns |  0.9634 ns |
    ///| XoshiroStarStar |  1.5989 ns | 0.0593 ns | 0.1054 ns |  1.6483 ns |
    ///|        RomuTrio |  0.9722 ns | 0.0480 ns | 0.0761 ns |  1.0006 ns |
    ///|         Mizuchi |  1.0065 ns | 0.0498 ns | 0.0648 ns |  0.9937 ns |
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

    /// <summary>
    /// With each implementing class sealed:
    /// |          Method |     Mean |     Error |    StdDev |
    /// |---------------- |---------:|----------:|----------:|
    /// |        Distinct | 1.430 ns | 0.0558 ns | 0.0665 ns |
    /// |           Laser | 1.250 ns | 0.0082 ns | 0.0068 ns |
    /// |       FourWheel | 1.695 ns | 0.0140 ns | 0.0110 ns |
    /// | XoshiroStarStar | 1.981 ns | 0.0662 ns | 0.1453 ns |
    /// |        RomuTrio | 2.039 ns | 0.0682 ns | 0.1809 ns |
    /// |         Mizuchi | 1.609 ns | 0.0604 ns | 0.1163 ns |
    /// With no implementing classes sealed:
    /// |          Method |     Mean |     Error |    StdDev |   Median |
    /// |---------------- |---------:|----------:|----------:|---------:|
    /// |        Distinct | 2.704 ns | 0.0818 ns | 0.1688 ns | 2.711 ns |
    /// |           Laser | 2.727 ns | 0.0063 ns | 0.0059 ns | 2.725 ns |
    /// |       FourWheel | 2.677 ns | 0.0818 ns | 0.1496 ns | 2.704 ns |
    /// | XoshiroStarStar | 2.756 ns | 0.0820 ns | 0.0767 ns | 2.779 ns |
    /// |        RomuTrio | 2.910 ns | 0.0845 ns | 0.1725 ns | 3.001 ns |
    /// |         Mizuchi | 2.770 ns | 0.0139 ns | 0.0123 ns | 2.768 ns |
    /// </summary>
    public class SealedOverheadComparison
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
