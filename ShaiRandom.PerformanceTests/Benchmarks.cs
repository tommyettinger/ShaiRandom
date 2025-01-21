﻿using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ShaiRandom.Generators;
using Troschuetz.Random;
using Troschuetz.Random.Generators;
#if NETCOREAPP3_0_OR_GREATER
using System.Numerics;
#endif

namespace ShaiRandom.PerformanceTests
{
    /// <summary>
    ///The .NET 5 and 6 benchmarks here were done on an older machine.
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
    ///|             Method |     Mean |     Error |    StdDev |   Median |
    ///|------------------- |---------:|----------:|----------:|---------:|
    ///|           Distinct | 2.273 ns | 0.0732 ns | 0.0872 ns | 2.226 ns |
    ///|              Laser | 2.245 ns | 0.0276 ns | 0.0215 ns | 2.239 ns |
    ///|           Tricycle | 2.313 ns | 0.0750 ns | 0.0921 ns | 2.386 ns |
    ///|          FourWheel | 2.579 ns | 0.0628 ns | 0.0588 ns | 2.601 ns |
    ///|           Stranger | 2.416 ns | 0.0726 ns | 0.0679 ns | 2.440 ns |
    ///| Xoshiro256StarStar | 2.669 ns | 0.0704 ns | 0.0659 ns | 2.700 ns |
    ///|           RomuTrio | 2.003 ns | 0.0116 ns | 0.0097 ns | 2.005 ns |
    ///|            Mizuchi | 2.710 ns | 0.0812 ns | 0.0797 ns | 2.741 ns |
    ///|               Trim | 2.558 ns | 0.0766 ns | 0.0941 ns | 2.534 ns |
    ///|        XorShift128 | 2.654 ns | 0.0794 ns | 0.0945 ns | 2.600 ns |
    ///|                ALF | 3.727 ns | 0.0992 ns | 0.1103 ns | 3.689 ns |
    ///|                NR3 | 3.882 ns | 0.0570 ns | 0.0445 ns | 3.890 ns |
    ///|              NR3Q1 | 2.726 ns | 0.0152 ns | 0.0135 ns | 2.726 ns |
    ///|              NR3Q2 | 2.628 ns | 0.0098 ns | 0.0087 ns | 2.629 ns |
    ///|            MT19937 | 4.643 ns | 0.0144 ns | 0.0135 ns | 4.642 ns |
    /// </summary>
    public class RandomUIntComparison
    {
        private IEnhancedRandom _rng = null!;
        private IGenerator _gen = null!;

        [GlobalSetup(Target = nameof(Distinct))]
        public void DistinctSetup() => _rng = new DistinctRandom(1UL);
        [Benchmark]
        public uint Distinct() => _rng.NextUInt();

        [GlobalSetup(Target = nameof(Laser))]
        public void LaserSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public uint Laser() => _rng.NextUInt();

        [GlobalSetup(Target = nameof(Tricycle))]
        public void TricycleSetup() => _rng = new TricycleRandom(1UL);
        [Benchmark]
        public uint Tricycle() => _rng.NextUInt();

        [GlobalSetup(Target = nameof(FourWheel))]
        public void FourWheelSetup() => _rng = new FourWheelRandom(1UL);
        [Benchmark]
        public uint FourWheel() => _rng.NextUInt();

        [GlobalSetup(Target = nameof(Stranger))]
        public void StrangerSetup() => _rng = new StrangerRandom(1UL);
        [Benchmark]
        public uint Stranger() => _rng.NextUInt();

        [GlobalSetup(Target = nameof(Xoshiro256StarStar))]
        public void Xoshiro256StarStarSetup() => _rng = new Xoshiro256StarStarRandom(1UL);
        [Benchmark]
        public uint Xoshiro256StarStar() => _rng.NextUInt();

        [GlobalSetup(Target = nameof(RomuTrio))]
        public void RomuTrioSetup() => _rng = new RomuTrioRandom(1UL);
        [Benchmark]
        public uint RomuTrio() => _rng.NextUInt();

        [GlobalSetup(Target = nameof(Mizuchi))]
        public void MizuchiSetup() => _rng = new MizuchiRandom(1UL);
        [Benchmark]
        public uint Mizuchi() => _rng.NextUInt();

        [GlobalSetup(Target = nameof(Trim))]
        public void TrimSetup() => _rng = new TrimRandom(1UL);
        [Benchmark]
        public uint Trim() => _rng.NextUInt();

        [GlobalSetup(Target = nameof(Whisker))]
        public void WhiskerSetup() => _rng = new WhiskerRandom(1UL);
        [Benchmark]
        public uint Whisker() => _rng.NextUInt();

        [GlobalSetup(Target = nameof(Scruff))]
        public void ScruffSetup() => _rng = new ScruffRandom(1UL);
        [Benchmark]
        public uint Scruff() => _rng.NextUInt();

        [GlobalSetup(Target = nameof(Ace))]
        public void AceSetup() => _rng = new AceRandom(1UL);
        [Benchmark]
        public uint Ace() => _rng.NextUInt();

        [GlobalSetup(Target = nameof(XorShift128))]
        public void XorShift128Setup() => _gen = new XorShift128Generator(1);
        [Benchmark]
        public uint XorShift128() => _gen.NextUInt();

        [GlobalSetup(Target = nameof(ALF))]
        public void ALFSetup() => _gen = new ALFGenerator(1);
        [Benchmark]
        public uint ALF() => _gen.NextUInt();

        [GlobalSetup(Target = nameof(NR3))]
        public void NR3Setup() => _gen = new NR3Generator(1);
        [Benchmark]
        public uint NR3() => _gen.NextUInt();

        [GlobalSetup(Target = nameof(NR3Q1))]
        public void NR3Q1Setup() => _gen = new NR3Q1Generator(1);
        [Benchmark]
        public uint NR3Q1() => _gen.NextUInt();

        [GlobalSetup(Target = nameof(NR3Q2))]
        public void NR3Q2Setup() => _gen = new NR3Q2Generator(1);
        [Benchmark]
        public uint NR3Q2() => _gen.NextUInt();

        [GlobalSetup(Target = nameof(MT19937))]
        public void MT19937Setup() => _gen = new MT19937Generator(1);
        [Benchmark]
        public uint MT19937() => _gen.NextUInt();

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
    ///On .NET 6.0 (using individual GlobalSetup methods):
    ///<code>
    ///|             Method |      Mean |     Error |    StdDev |
    ///|------------------- |----------:|----------:|----------:|
    ///|             Seeded |  9.115 ns | 0.0271 ns | 0.0240 ns |
    ///|           Unseeded |  3.474 ns | 0.0831 ns | 0.0777 ns |
    ///|           Distinct |  2.760 ns | 0.0148 ns | 0.0131 ns |
    ///|              Laser |  2.604 ns | 0.0789 ns | 0.1027 ns |
    ///|           Tricycle |  2.346 ns | 0.0293 ns | 0.0229 ns |
    ///|          FourWheel |  2.988 ns | 0.0867 ns | 0.1032 ns |
    ///|           Stranger |  2.691 ns | 0.0537 ns | 0.0502 ns |
    ///| Xoshiro256StarStar |  3.719 ns | 0.0955 ns | 0.0938 ns |
    ///|           RomuTrio |  2.788 ns | 0.0795 ns | 0.0976 ns |
    ///|            Mizuchi |  2.499 ns | 0.0812 ns | 0.0869 ns |
    ///|               Trim |  2.919 ns | 0.0198 ns | 0.0175 ns |
    ///|        XorShift128 |  3.853 ns | 0.0986 ns | 0.1055 ns |
    ///|                ALF |  7.750 ns | 0.1831 ns | 0.1959 ns |
    ///|                NR3 |  5.532 ns | 0.1383 ns | 0.1593 ns |
    ///|              NR3Q1 |  4.066 ns | 0.1096 ns | 0.2238 ns |
    ///|              NR3Q2 |  4.257 ns | 0.0225 ns | 0.0211 ns |
    ///|            MT19937 | 11.314 ns | 0.0286 ns | 0.0223 ns |
    ///</code>
    /// </summary>
    public class RandomUIntBoundedComparison
    {
        private IEnhancedRandom _rng = null!;
        private IGenerator _gen = null!;

#if NET6_0_OR_GREATER
        private Random _seededRandom = null!;
        private Random _unseededRandom = null!;

        [GlobalSetup(Target = nameof(Seeded))]
        public void SeededSetup() => _seededRandom = new Random(1);
        [Benchmark]
        public int Seeded() => _seededRandom.Next(999);

        [GlobalSetup(Target = nameof(Unseeded))]
        public void UnseededSetup() => _unseededRandom = new Random();
        [Benchmark]
        public int Unseeded() => _unseededRandom.Next(999);
#endif

        [GlobalSetup(Target = nameof(Distinct))]
        public void DistinctSetup() => _rng = new DistinctRandom(1UL);
        [Benchmark]
        public uint Distinct() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(Laser))]
        public void LaserSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public uint Laser() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(Tricycle))]
        public void TricycleSetup() => _rng = new TricycleRandom(1UL);
        [Benchmark]
        public uint Tricycle() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(FourWheel))]
        public void FourWheelSetup() => _rng = new FourWheelRandom(1UL);
        [Benchmark]
        public uint FourWheel() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(Stranger))]
        public void StrangerSetup() => _rng = new StrangerRandom(1UL);
        [Benchmark]
        public uint Stranger() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(Xoshiro256StarStar))]
        public void Xoshiro256StarStarSetup() => _rng = new Xoshiro256StarStarRandom(1UL);
        [Benchmark]
        public uint Xoshiro256StarStar() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(Xorshift128Plus))]
        public void Xorshift128PlusSetup() => _rng = new Xorshift128PlusRandom(1UL);
        [Benchmark]
        public uint Xorshift128Plus() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(RomuTrio))]
        public void RomuTrioSetup() => _rng = new RomuTrioRandom(1UL);
        [Benchmark]
        public uint RomuTrio() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(Mizuchi))]
        public void MizuchiSetup() => _rng = new MizuchiRandom(1UL);
        [Benchmark]
        public uint Mizuchi() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(Trim))]
        public void TrimSetup() => _rng = new TrimRandom(1UL);
        [Benchmark]
        public uint Trim() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(Whisker))]
        public void WhiskerSetup() => _rng = new WhiskerRandom(1UL);
        [Benchmark]
        public uint Whisker() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(Scruff))]
        public void ScruffSetup() => _rng = new ScruffRandom(1UL);
        [Benchmark]
        public uint Scruff() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(Ace))]
        public void AceSetup() => _rng = new AceRandom(1UL);
        [Benchmark]
        public uint Ace() => _rng.NextUInt(999u);

        [GlobalSetup(Target = nameof(XorShift128))]
        public void XorShift128Setup() => _gen = new XorShift128Generator(1);
        [Benchmark]
        public uint XorShift128() => _gen.NextUInt(999u);

        [GlobalSetup(Target = nameof(ALF))]
        public void ALFSetup() => _gen = new ALFGenerator(1);
        [Benchmark]
        public uint ALF() => _gen.NextUInt(999u);

        [GlobalSetup(Target = nameof(NR3))]
        public void NR3Setup() => _gen = new NR3Generator(1);
        [Benchmark]
        public uint NR3() => _gen.NextUInt(999u);

        [GlobalSetup(Target = nameof(NR3Q1))]
        public void NR3Q1Setup() => _gen = new NR3Q1Generator(1);
        [Benchmark]
        public uint NR3Q1() => _gen.NextUInt(999u);

        [GlobalSetup(Target = nameof(NR3Q2))]
        public void NR3Q2Setup() => _gen = new NR3Q2Generator(1);
        [Benchmark]
        public uint NR3Q2() => _gen.NextUInt(999u);

        [GlobalSetup(Target = nameof(MT19937))]
        public void MT19937Setup() => _gen = new MT19937Generator(1);
        [Benchmark]
        public uint MT19937() => _gen.NextUInt(999u);

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
	/// .NET 6.0 (tested directly with no interface; this allows some TR generators in):
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
    /// .NET 6.0 (via IEnhancedRandom interface, initialized individually):
    ///|             Method |     Mean |     Error |    StdDev |   Median |
    ///|------------------- |---------:|----------:|----------:|---------:|
    ///|           Distinct | 1.270 ns | 0.0297 ns | 0.0248 ns | 1.278 ns |
    ///|              Laser | 1.232 ns | 0.0430 ns | 0.0403 ns | 1.216 ns |
    ///|           Tricycle | 1.250 ns | 0.0459 ns | 0.0429 ns | 1.225 ns |
    ///|          FourWheel | 1.385 ns | 0.0562 ns | 0.0669 ns | 1.334 ns |
    ///|           Stranger | 1.575 ns | 0.0522 ns | 0.0488 ns | 1.608 ns |
    ///| Xoshiro256StarStar | 1.690 ns | 0.0667 ns | 0.0624 ns | 1.712 ns |
    ///|           RomuTrio | 1.267 ns | 0.0532 ns | 0.0546 ns | 1.299 ns |
    ///|            Mizuchi | 1.329 ns | 0.0542 ns | 0.0603 ns | 1.291 ns |
    ///|               Trim | 1.381 ns | 0.0584 ns | 0.1294 ns | 1.347 ns |
    ///Attempting MathNet.Numerics' style for Xoshiro256StarStar... .NET 6.0 still, by interface, individually:
    ///|             Method |     Mean |     Error |    StdDev |   Median |
    ///|------------------- |---------:|----------:|----------:|---------:|
    ///|           Distinct | 1.561 ns | 0.0593 ns | 0.1039 ns | 1.506 ns |
    ///|              Laser | 1.367 ns | 0.0559 ns | 0.1371 ns | 1.411 ns |
    ///|           Tricycle | 1.295 ns | 0.0547 ns | 0.1311 ns | 1.202 ns |
    ///|          FourWheel | 1.378 ns | 0.0567 ns | 0.1391 ns | 1.366 ns |
    ///|           Stranger | 1.295 ns | 0.0565 ns | 0.1265 ns | 1.365 ns |
    ///| Xoshiro256StarStar | 2.879 ns | 0.0875 ns | 0.1938 ns | 3.021 ns |
    ///|           RomuTrio | 1.308 ns | 0.0541 ns | 0.1141 ns | 1.262 ns |
    ///|            Mizuchi | 1.360 ns | 0.0556 ns | 0.1267 ns | 1.311 ns |
    ///|               Trim | 1.375 ns | 0.0559 ns | 0.1238 ns | 1.323 ns |
    ///Same as above, but with the Xoshiro256StarStar changes reverted... .NET 6.0, by interface, individually:
    ///|             Method |     Mean |     Error |    StdDev |   Median |
    ///|------------------- |---------:|----------:|----------:|---------:|
    ///|           Distinct | 1.643 ns | 0.0625 ns | 0.1158 ns | 1.711 ns |
    ///|              Laser | 1.189 ns | 0.0446 ns | 0.0348 ns | 1.201 ns |
    ///|           Tricycle | 1.363 ns | 0.0565 ns | 0.1141 ns | 1.434 ns |
    ///|          FourWheel | 1.413 ns | 0.0568 ns | 0.0994 ns | 1.476 ns |
    ///|           Stranger | 1.280 ns | 0.0491 ns | 0.0383 ns | 1.300 ns |
    ///| Xoshiro256StarStar | 1.661 ns | 0.0618 ns | 0.1370 ns | 1.718 ns |
    ///|           RomuTrio | 1.244 ns | 0.0503 ns | 0.0494 ns | 1.220 ns |
    ///|            Mizuchi | 1.407 ns | 0.0572 ns | 0.0971 ns | 1.471 ns |
    ///|               Trim | 1.323 ns | 0.0558 ns | 0.1249 ns | 1.235 ns |
    /// </summary>
    public class RandomULongComparison
    {
        private IEnhancedRandom _rng = null!;

        [GlobalSetup(Target = nameof(Distinct))]
        public void DistinctSetup() => _rng = new DistinctRandom(1UL);
        [Benchmark]
        public ulong Distinct() => _rng.NextULong();

        [GlobalSetup(Target = nameof(Laser))]
        public void LaserSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public ulong Laser() => _rng.NextULong();

        [GlobalSetup(Target = nameof(Tricycle))]
        public void TricycleSetup() => _rng = new TricycleRandom(1UL);
        [Benchmark]
        public ulong Tricycle() => _rng.NextULong();

        [GlobalSetup(Target = nameof(FourWheel))]
        public void FourWheelSetup() => _rng = new FourWheelRandom(1UL);
        [Benchmark]
        public ulong FourWheel() => _rng.NextULong();

        [GlobalSetup(Target = nameof(Stranger))]
        public void StrangerSetup() => _rng = new StrangerRandom(1UL);
        [Benchmark]
        public ulong Stranger() => _rng.NextULong();

        [GlobalSetup(Target = nameof(Xoshiro256StarStar))]
        public void Xoshiro256StarStarSetup() => _rng = new Xoshiro256StarStarRandom(1UL);
        [Benchmark]
        public ulong Xoshiro256StarStar() => _rng.NextULong();

        [GlobalSetup(Target = nameof(Xorshift128Plus))]
        public void Xorshift128PlusSetup() => _rng = new Xorshift128PlusRandom(1UL);
        [Benchmark]
        public ulong Xorshift128Plus() => _rng.NextULong();

        [GlobalSetup(Target = nameof(RomuTrio))]
        public void RomuTrioSetup() => _rng = new RomuTrioRandom(1UL);
        [Benchmark]
        public ulong RomuTrio() => _rng.NextULong();

        [GlobalSetup(Target = nameof(Mizuchi))]
        public void MizuchiSetup() => _rng = new MizuchiRandom(1UL);
        [Benchmark]
        public ulong Mizuchi() => _rng.NextULong();

        [GlobalSetup(Target = nameof(Trim))]
        public void TrimSetup() => _rng = new TrimRandom(1UL);
        [Benchmark]
        public ulong Trim() => _rng.NextULong();

        [GlobalSetup(Target = nameof(Whisker))]
        public void WhiskerSetup() => _rng = new WhiskerRandom(1UL);
        [Benchmark]
        public ulong Whisker() => _rng.NextULong();

        [GlobalSetup(Target = nameof(Scruff))]
        public void ScruffSetup() => _rng = new ScruffRandom(1UL);
        [Benchmark]
        public ulong Scruff() => _rng.NextULong();

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
    /// On .NET 9.0, newr machine:
    ///BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.3880/23H2/2023Update/SunValley3)
    ///12th Gen Intel Core i7-12800H, 1 CPU, 20 logical and 14 physical cores
    ///.NET SDK 9.0.101
    ///  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
    ///  Job-LBPMMR : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
    ///
    ///Runtime=.NET 9.0  Toolchain=net90
    ///
    ///| Method             | Mean      | Error     | StdDev    |
    ///|------------------- |----------:|----------:|----------:|
    ///| Seeded             | 9.1019 ns | 0.1863 ns | 0.1555 ns |
    ///| Unseeded           | 0.5776 ns | 0.0099 ns | 0.0093 ns |
    ///| Distinct           | 0.3463 ns | 0.0179 ns | 0.0168 ns |
    ///| Laser              | 0.1209 ns | 0.0112 ns | 0.0105 ns |
    ///| Tricycle           | 0.3586 ns | 0.0133 ns | 0.0111 ns |
    ///| FourWheel          | 0.1368 ns | 0.0057 ns | 0.0047 ns |
    ///| Stranger           | 0.3475 ns | 0.0068 ns | 0.0064 ns |
    ///| Xoshiro256StarStar | 0.1385 ns | 0.0075 ns | 0.0070 ns |
    ///| Xorshift128Plus    | 0.1449 ns | 0.0065 ns | 0.0061 ns |
    ///| RomuTrio           | 0.1367 ns | 0.0145 ns | 0.0128 ns |
    ///| Mizuchi            | 0.1336 ns | 0.0096 ns | 0.0090 ns |
    ///| Trim               | 0.1424 ns | 0.0137 ns | 0.0128 ns |
    ///| Whisker            | 0.3322 ns | 0.0123 ns | 0.0115 ns |
    ///| Scruff             | 0.1361 ns | 0.0080 ns | 0.0075 ns |
    ///| Ace                | 0.1352 ns | 0.0082 ns | 0.0076 ns |
    /// </summary>
    public class RandomULongBoundedComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly StrangerRandom _strangerRandom = new StrangerRandom(1UL);
        private readonly Xoshiro256StarStarRandom _xoshiro256StarStarRandom = new Xoshiro256StarStarRandom(1UL);
        private readonly Xorshift128PlusRandom _xorshift128PlusRandom = new Xorshift128PlusRandom(1UL);
        private readonly RomuTrioRandom _romuTrioRandom = new RomuTrioRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);
        private readonly TrimRandom _trimRandom = new TrimRandom(1UL);
        private readonly WhiskerRandom _whiskerRandom = new WhiskerRandom(1UL);
        private readonly ScruffRandom _scruffRandom = new ScruffRandom(1UL);
        private readonly AceRandom _aceRandom = new AceRandom(1UL);

#if NET6_0_OR_GREATER
        private readonly Random _seededRandom = new Random(1);
        private readonly Random _unseededRandom = new Random();

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
        public ulong Xoshiro256StarStar() => _xoshiro256StarStarRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong Xorshift128Plus() => _xorshift128PlusRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong RomuTrio() => _romuTrioRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong Mizuchi() => _mizuchiRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong Trim() => _trimRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong Whisker() => _whiskerRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong Scruff() => _scruffRandom.NextULong(1UL, 1000UL);

        [Benchmark]
        public ulong Ace() => _aceRandom.NextULong(1UL, 1000UL);
    }
    /// <summary>
    /// On .NET 9.0, newer machine:
    /// <code>
    ///BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.3880/23H2/2023Update/SunValley3)
    ///12th Gen Intel Core i7-12800H, 1 CPU, 20 logical and 14 physical cores
    ///.NET SDK 9.0.101
    ///  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
    ///  Job-BFMEEA : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
    ///
    ///Runtime=.NET 9.0  Toolchain=net90
    ///
    ///| Method             | Mean      | Error     | StdDev    |
    ///|------------------- |----------:|----------:|----------:|
    ///| Seeded             | 8.8398 ns | 0.0448 ns | 0.0397 ns |
    ///| Unseeded           | 0.5654 ns | 0.0159 ns | 0.0149 ns |
    ///| Distinct           | 0.3708 ns | 0.0122 ns | 0.0114 ns |
    ///| Laser              | 0.0000 ns | 0.0000 ns | 0.0000 ns |
    ///| Tricycle           | 0.3560 ns | 0.0056 ns | 0.0052 ns |
    ///| FourWheel          | 0.3582 ns | 0.0068 ns | 0.0064 ns |
    ///| Stranger           | 0.3455 ns | 0.0118 ns | 0.0110 ns |
    ///| Xoshiro256StarStar | 0.3432 ns | 0.0087 ns | 0.0068 ns |
    ///| Xorshift128Plus    | 0.3535 ns | 0.0075 ns | 0.0070 ns |
    ///| RomuTrio           | 0.1393 ns | 0.0092 ns | 0.0087 ns |
    ///| Mizuchi            | 0.3496 ns | 0.0118 ns | 0.0110 ns |
    ///| Trim               | 0.2369 ns | 0.0083 ns | 0.0078 ns |
    ///| Whisker            | 0.4129 ns | 0.0121 ns | 0.0113 ns |
    ///| Scruff             | 0.3437 ns | 0.0069 ns | 0.0065 ns |
    ///| Ace                | 0.3083 ns | 0.0072 ns | 0.0067 ns |
    /// </code>
    /// </summary>
    public class RandomULongBigMulComparison
    {
        private readonly DistinctRandom _distinctRandom = new DistinctRandom(1UL);
        private readonly LaserRandom _laserRandom = new LaserRandom(1UL);
        private readonly TricycleRandom _tricycleRandom = new TricycleRandom(1UL);
        private readonly FourWheelRandom _fourWheelRandom = new FourWheelRandom(1UL);
        private readonly StrangerRandom _strangerRandom = new StrangerRandom(1UL);
        private readonly Xoshiro256StarStarRandom _xoshiro256StarStarRandom = new Xoshiro256StarStarRandom(1UL);
        private readonly Xorshift128PlusRandom _xorshift128PlusRandom = new Xorshift128PlusRandom(1UL);
        private readonly RomuTrioRandom _romuTrioRandom = new RomuTrioRandom(1UL);
        private readonly MizuchiRandom _mizuchiRandom = new MizuchiRandom(1UL);
        private readonly TrimRandom _trimRandom = new TrimRandom(1UL);
        private readonly WhiskerRandom _whiskerRandom = new WhiskerRandom(1UL);
        private readonly ScruffRandom _scruffRandom = new ScruffRandom(1UL);
        private readonly AceRandom _aceRandom = new AceRandom(1UL);

#if NET6_0_OR_GREATER
        private readonly Random _seededRandom = new Random(1);
        private readonly Random _unseededRandom = new Random();

        [Benchmark]
        public long Seeded() => _seededRandom.NextInt64(1L, 1000L);

        [Benchmark]
        public long Unseeded() => _unseededRandom.NextInt64(1L, 1000L);
#endif

        [Benchmark]
        public ulong Distinct() => Math.BigMul(_distinctRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;

        [Benchmark]
        public ulong Laser() => Math.BigMul(_laserRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;

        [Benchmark]
        public ulong Tricycle() => Math.BigMul(_tricycleRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;

        [Benchmark]
        public ulong FourWheel() => Math.BigMul(_fourWheelRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;

        [Benchmark]
        public ulong Stranger() => Math.BigMul(_strangerRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;

        [Benchmark]
        public ulong Xoshiro256StarStar() => Math.BigMul(_xoshiro256StarStarRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;

        [Benchmark]
        public ulong Xorshift128Plus() => Math.BigMul(_xorshift128PlusRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;

        [Benchmark]
        public ulong RomuTrio() => Math.BigMul(_romuTrioRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;

        [Benchmark]
        public ulong Mizuchi() => Math.BigMul(_mizuchiRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;

        [Benchmark]
        public ulong Trim() => Math.BigMul(_trimRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;

        [Benchmark]
        public ulong Whisker() => Math.BigMul(_whiskerRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;

        [Benchmark]
        public ulong Scruff() => Math.BigMul(_scruffRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;

        [Benchmark]
        public ulong Ace() => Math.BigMul(_aceRandom.NextULong(), 1000UL - 1UL, out _) + 1UL;
    }
    /// <summary>
    /// .NET 6.0 (newer benchmark, using IEnhancedRandom and IGenerator):
    ///|             Method |     Mean |     Error |    StdDev |   Median |
	///|------------------- |---------:|----------:|----------:|---------:|
	///|             Seeded | 9.078 ns | 0.0716 ns | 0.0670 ns | 9.098 ns |
	///|           Unseeded | 2.591 ns | 0.0797 ns | 0.0918 ns | 2.635 ns |
	///|           Distinct | 2.647 ns | 0.0541 ns | 0.0452 ns | 2.658 ns |
	///|              Laser | 2.816 ns | 0.0855 ns | 0.1998 ns | 2.838 ns |
	///|           Tricycle | 2.480 ns | 0.0176 ns | 0.0173 ns | 2.476 ns |
	///|          FourWheel | 3.090 ns | 0.0391 ns | 0.0305 ns | 3.093 ns |
	///|           Stranger | 2.823 ns | 0.0861 ns | 0.2237 ns | 2.969 ns |
	///| Xoshiro256StarStar | 2.597 ns | 0.0791 ns | 0.0879 ns | 2.577 ns |
	///|    Xorshift128Plus | 3.281 ns | 0.0932 ns | 0.1608 ns | 3.323 ns |
	///|           RomuTrio | 2.966 ns | 0.0898 ns | 0.1398 ns | 2.968 ns |
	///|            Mizuchi | 2.872 ns | 0.0916 ns | 0.1952 ns | 2.869 ns |
	///|               Trim | 2.663 ns | 0.0856 ns | 0.2358 ns | 2.640 ns |
	///|        XorShift128 | 2.420 ns | 0.0761 ns | 0.1808 ns | 2.508 ns |
	///|                ALF | 6.280 ns | 0.0891 ns | 0.0833 ns | 6.296 ns |
	///|                NR3 | 3.501 ns | 0.0333 ns | 0.0327 ns | 3.499 ns |
	///|              NR3Q1 | 2.307 ns | 0.0751 ns | 0.1741 ns | 2.354 ns |
	///|              NR3Q2 | 2.443 ns | 0.0786 ns | 0.1200 ns | 2.498 ns |
	///|            MT19937 | 8.673 ns | 0.2029 ns | 0.3711 ns | 8.533 ns |
	/// .NET 5.0 (older benchmark with direct generators):
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
	/// .NET 6.0 (older benchmark with direct generators):
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
        private IEnhancedRandom _rng = null!;
        private IGenerator _gen = null!;

#if NET6_0_OR_GREATER
        private Random _seededRandom = null!;
        private Random _unseededRandom = null!;

        [GlobalSetup(Target = nameof(Seeded))]
        public void SeededSetup() => _seededRandom = new Random(1);
        [Benchmark]
        public double Seeded() => _seededRandom.NextDouble();

        [GlobalSetup(Target = nameof(Unseeded))]
        public void UnseededSetup() => _unseededRandom = new Random();
        [Benchmark]
        public double Unseeded() => _unseededRandom.NextDouble();
#endif

        [GlobalSetup(Target = nameof(Distinct))]
        public void DistinctSetup() => _rng = new DistinctRandom(1UL);
        [Benchmark]
        public double Distinct() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(Laser))]
        public void LaserSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public double Laser() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(Tricycle))]
        public void TricycleSetup() => _rng = new TricycleRandom(1UL);
        [Benchmark]
        public double Tricycle() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(FourWheel))]
        public void FourWheelSetup() => _rng = new FourWheelRandom(1UL);
        [Benchmark]
        public double FourWheel() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(Stranger))]
        public void StrangerSetup() => _rng = new StrangerRandom(1UL);
        [Benchmark]
        public double Stranger() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(Xoshiro256StarStar))]
        public void Xoshiro256StarStarSetup() => _rng = new Xoshiro256StarStarRandom(1UL);
        [Benchmark]
        public double Xoshiro256StarStar() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(Xorshift128Plus))]
        public void Xorshift128PlusSetup() => _rng = new Xorshift128PlusRandom(1UL);
        [Benchmark]
        public double Xorshift128Plus() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(RomuTrio))]
        public void RomuTrioSetup() => _rng = new RomuTrioRandom(1UL);
        [Benchmark]
        public double RomuTrio() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(Mizuchi))]
        public void MizuchiSetup() => _rng = new MizuchiRandom(1UL);
        [Benchmark]
        public double Mizuchi() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(Trim))]
        public void TrimSetup() => _rng = new TrimRandom(1UL);
        [Benchmark]
        public double Trim() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(Whisker))]
        public void WhiskerSetup() => _rng = new WhiskerRandom(1UL);
        [Benchmark]
        public double Whisker() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(Scruff))]
        public void ScruffSetup() => _rng = new ScruffRandom(1UL);
        [Benchmark]
        public double Scruff() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(Ace))]
        public void AceSetup() => _rng = new AceRandom(1UL);
        [Benchmark]
        public double Ace() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(XorShift128))]
        public void XorShift128Setup() => _gen = new XorShift128Generator(1);
        [Benchmark]
        public double XorShift128() => _gen.NextDouble();

        [GlobalSetup(Target = nameof(ALF))]
        public void ALFSetup() => _gen = new ALFGenerator(1);
        [Benchmark]
        public double ALF() => _gen.NextDouble();

        [GlobalSetup(Target = nameof(NR3))]
        public void NR3Setup() => _gen = new NR3Generator(1);
        [Benchmark]
        public double NR3() => _gen.NextDouble();

        [GlobalSetup(Target = nameof(NR3Q1))]
        public void NR3Q1Setup() => _gen = new NR3Q1Generator(1);
        [Benchmark]
        public double NR3Q1() => _gen.NextDouble();

        [GlobalSetup(Target = nameof(NR3Q2))]
        public void NR3Q2Setup() => _gen = new NR3Q2Generator(1);
        [Benchmark]
        public double NR3Q2() => _gen.NextDouble();

        [GlobalSetup(Target = nameof(MT19937))]
        public void MT19937Setup() => _gen = new MT19937Generator(1);
        [Benchmark]
        public double MT19937() => _gen.NextDouble();
    }

    /// <summary>
    /// .NET 6.0:
    ///|             Method |     Mean |     Error |    StdDev |   Median |
    ///|------------------- |---------:|----------:|----------:|---------:|
    ///|           Distinct | 4.935 ns | 0.1259 ns | 0.2868 ns | 4.759 ns |
    ///|              Laser | 4.962 ns | 0.1276 ns | 0.2773 ns | 4.914 ns |
    ///|           Tricycle | 5.299 ns | 0.1338 ns | 0.2309 ns | 5.352 ns |
    ///|          FourWheel | 5.089 ns | 0.0272 ns | 0.0254 ns | 5.080 ns |
    ///|           Stranger | 5.257 ns | 0.1353 ns | 0.1852 ns | 5.189 ns |
    ///| Xoshiro256StarStar | 5.108 ns | 0.0568 ns | 0.0531 ns | 5.120 ns |
    ///|           RomuTrio | 5.079 ns | 0.0484 ns | 0.0453 ns | 5.065 ns |
    ///|            Mizuchi | 5.064 ns | 0.0868 ns | 0.0812 ns | 5.039 ns |
    ///|               Trim | 5.005 ns | 0.0810 ns | 0.0758 ns | 4.993 ns |
    /// </summary>
    public class RandomDoubleBoundedComparison
    {
        private IEnhancedRandom _rng = null!;

        [GlobalSetup(Target = nameof(Distinct))]
        public void DistinctSetup() => _rng = new DistinctRandom(1UL);
        [Benchmark]
        public double Distinct() => _rng.NextDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Laser))]
        public void LaserSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public double Laser() => _rng.NextDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Tricycle))]
        public void TricycleSetup() => _rng = new TricycleRandom(1UL);
        [Benchmark]
        public double Tricycle() => _rng.NextDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(FourWheel))]
        public void FourWheelSetup() => _rng = new FourWheelRandom(1UL);
        [Benchmark]
        public double FourWheel() => _rng.NextDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Stranger))]
        public void StrangerSetup() => _rng = new StrangerRandom(1UL);
        [Benchmark]
        public double Stranger() => _rng.NextDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Xoshiro256StarStar))]
        public void Xoshiro256StarStarSetup() => _rng = new Xoshiro256StarStarRandom(1UL);
        [Benchmark]
        public double Xoshiro256StarStar() => _rng.NextDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Xorshift128Plus))]
        public void Xorshift128PlusSetup() => _rng = new Xorshift128PlusRandom(1UL);
        [Benchmark]
        public double Xorshift128Plus() => _rng.NextDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(RomuTrio))]
        public void RomuTrioSetup() => _rng = new RomuTrioRandom(1UL);
        [Benchmark]
        public double RomuTrio() => _rng.NextDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Mizuchi))]
        public void MizuchiSetup() => _rng = new MizuchiRandom(1UL);
        [Benchmark]
        public double Mizuchi() => _rng.NextDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Trim))]
        public void TrimSetup() => _rng = new TrimRandom(1UL);
        [Benchmark]
        public double Trim() => _rng.NextDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Whisker))]
        public void WhiskerSetup() => _rng = new WhiskerRandom(1UL);
        [Benchmark]
        public double Whisker() => _rng.NextDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Scruff))]
        public void ScruffSetup() => _rng = new ScruffRandom(1UL);
        [Benchmark]
        public double Scruff() => _rng.NextDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Ace))]
        public void AceSetup() => _rng = new AceRandom(1UL);
        [Benchmark]
        public double Ace() => _rng.NextDouble(1.1, -0.1);
    }

    /// <summary>
    /// .NET 5.0, via IEnhancedRandom
    ///|             Method |     Mean |     Error |    StdDev |   Median |
    ///|------------------- |---------:|----------:|----------:|---------:|
    ///|           Distinct | 3.592 ns | 0.1013 ns | 0.0947 ns | 3.608 ns |
    ///|              Laser | 3.348 ns | 0.0342 ns | 0.0320 ns | 3.355 ns |
    ///|           Tricycle | 3.353 ns | 0.0958 ns | 0.1728 ns | 3.345 ns |
    ///|          FourWheel | 3.593 ns | 0.0585 ns | 0.0547 ns | 3.619 ns |
    ///|           Stranger | 3.697 ns | 0.0947 ns | 0.1388 ns | 3.651 ns |
    ///| Xoshiro256StarStar | 3.861 ns | 0.1060 ns | 0.1741 ns | 3.865 ns |
    ///|           RomuTrio | 3.511 ns | 0.0996 ns | 0.1942 ns | 3.470 ns |
    ///|            Mizuchi | 3.945 ns | 0.1059 ns | 0.1710 ns | 4.061 ns |
    ///|               Trim | 3.810 ns | 0.1059 ns | 0.1376 ns | 3.842 ns |
    /// .NET 6.0, via IEnhancedRandom
    ///|             Method |     Mean |     Error |    StdDev |   Median |
    ///|------------------- |---------:|----------:|----------:|---------:|
    ///|           Distinct | 3.530 ns | 0.0991 ns | 0.1886 ns | 3.579 ns |
    ///|              Laser | 3.350 ns | 0.0955 ns | 0.1793 ns | 3.378 ns |
    ///|           Tricycle | 3.326 ns | 0.0161 ns | 0.0126 ns | 3.331 ns |
    ///|          FourWheel | 3.120 ns | 0.0827 ns | 0.1133 ns | 3.070 ns |
    ///|           Stranger | 3.473 ns | 0.0986 ns | 0.1992 ns | 3.384 ns |
    ///| Xoshiro256StarStar | 3.473 ns | 0.0982 ns | 0.1844 ns | 3.410 ns |
    ///|           RomuTrio | 3.222 ns | 0.0953 ns | 0.1644 ns | 3.131 ns |
    ///|            Mizuchi | 3.358 ns | 0.0955 ns | 0.1818 ns | 3.395 ns |
    ///|               Trim | 3.044 ns | 0.0381 ns | 0.0374 ns | 3.039 ns |
    ///
    /// The rest are older, and use each generator directly (not via an IEnhancedRandom).
    ///
	/// .NET 5.0, maybe?
    ///|    Method |     Mean |     Error |    StdDev |   Median |
    ///|---------- |---------:|----------:|----------:|---------:|
    ///|  Distinct | 2.524 ns | 0.0783 ns | 0.2118 ns | 2.613 ns |
    ///|     Laser | 2.461 ns | 0.0346 ns | 0.0324 ns | 2.466 ns |
    ///| FourWheel | 3.949 ns | 0.0143 ns | 0.0127 ns | 3.952 ns |
    ///|   Mizuchi | 2.477 ns | 0.0376 ns | 0.0352 ns | 2.476 ns |
    /// .NET 6.0 (current NextExclusiveDouble() method)
    ///|    Method |     Mean |     Error |    StdDev |   Median |
	///|---------- |---------:|----------:|----------:|---------:|
	///|  Distinct | 2.941 ns | 0.0878 ns | 0.1963 ns | 2.888 ns |
	///|     Laser | 3.106 ns | 0.0653 ns | 0.0610 ns | 3.104 ns |
	///|  Tricycle | 2.853 ns | 0.0857 ns | 0.1969 ns | 2.886 ns |
	///| FourWheel | 2.733 ns | 0.0848 ns | 0.1896 ns | 2.606 ns |
	///|   Mizuchi | 2.820 ns | 0.0846 ns | 0.1784 ns | 2.895 ns |
	/// .NET 6.0 (old NextExclusiveDouble() method):
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
        private IEnhancedRandom _rng = null!;

        [GlobalSetup(Target = nameof(Distinct))]
        public void DistinctSetup() => _rng = new DistinctRandom(1UL);
        [Benchmark]
        public double Distinct() => _rng.NextExclusiveDouble();

        [GlobalSetup(Target = nameof(Laser))]
        public void LaserSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public double Laser() => _rng.NextExclusiveDouble();

        [GlobalSetup(Target = nameof(Tricycle))]
        public void TricycleSetup() => _rng = new TricycleRandom(1UL);
        [Benchmark]
        public double Tricycle() => _rng.NextExclusiveDouble();

        [GlobalSetup(Target = nameof(FourWheel))]
        public void FourWheelSetup() => _rng = new FourWheelRandom(1UL);
        [Benchmark]
        public double FourWheel() => _rng.NextExclusiveDouble();

        [GlobalSetup(Target = nameof(Stranger))]
        public void StrangerSetup() => _rng = new StrangerRandom(1UL);
        [Benchmark]
        public double Stranger() => _rng.NextExclusiveDouble();

        [GlobalSetup(Target = nameof(Xoshiro256StarStar))]
        public void Xoshiro256StarStarSetup() => _rng = new Xoshiro256StarStarRandom(1UL);
        [Benchmark]
        public double Xoshiro256StarStar() => _rng.NextExclusiveDouble();

        [GlobalSetup(Target = nameof(Xorshift128Plus))]
        public void Xorshift128PlusSetup() => _rng = new Xorshift128PlusRandom(1UL);
        [Benchmark]
        public double Xorshift128Plus() => _rng.NextExclusiveDouble();

        [GlobalSetup(Target = nameof(RomuTrio))]
        public void RomuTrioSetup() => _rng = new RomuTrioRandom(1UL);
        [Benchmark]
        public double RomuTrio() => _rng.NextExclusiveDouble();

        [GlobalSetup(Target = nameof(Mizuchi))]
        public void MizuchiSetup() => _rng = new MizuchiRandom(1UL);
        [Benchmark]
        public double Mizuchi() => _rng.NextExclusiveDouble();

        [GlobalSetup(Target = nameof(Trim))]
        public void TrimSetup() => _rng = new TrimRandom(1UL);
        [Benchmark]
        public double Trim() => _rng.NextExclusiveDouble();

        [GlobalSetup(Target = nameof(Whisker))]
        public void WhiskerSetup() => _rng = new WhiskerRandom(1UL);
        [Benchmark]
        public double Whisker() => _rng.NextExclusiveDouble();

        [GlobalSetup(Target = nameof(Scruff))]
        public void ScruffSetup() => _rng = new ScruffRandom(1UL);
        [Benchmark]
        public double Scruff() => _rng.NextExclusiveDouble();

        [GlobalSetup(Target = nameof(Ace))]
        public void AceSetup() => _rng = new AceRandom(1UL);
        [Benchmark]
        public double Ace() => _rng.NextExclusiveDouble();
    }

    /// <summary>
    /// Using IEnhancedRandom, .NET 6.0:
    ///|             Method |     Mean |     Error |    StdDev |   Median |
    ///|------------------- |---------:|----------:|----------:|---------:|
    ///|           Distinct | 5.872 ns | 0.1415 ns | 0.2203 ns | 5.756 ns |
    ///|              Laser | 5.780 ns | 0.0779 ns | 0.0928 ns | 5.795 ns |
    ///|           Tricycle | 5.897 ns | 0.1439 ns | 0.2482 ns | 5.818 ns |
    ///|          FourWheel | 6.301 ns | 0.1539 ns | 0.2571 ns | 6.425 ns |
    ///|           Stranger | 5.826 ns | 0.1450 ns | 0.2616 ns | 5.711 ns |
    ///| Xoshiro256StarStar | 5.903 ns | 0.1149 ns | 0.0959 ns | 5.949 ns |
    ///|           RomuTrio | 6.060 ns | 0.1501 ns | 0.2668 ns | 5.932 ns |
    ///|            Mizuchi | 6.036 ns | 0.0432 ns | 0.0361 ns | 6.043 ns |
    ///|               Trim | 6.326 ns | 0.1527 ns | 0.2593 ns | 6.341 ns |
    ///
    /// Old, before using IEnhancedRandom as the type, but with corrected bounds; .NET 6.0:
    /// |    Method |     Mean |     Error |    StdDev |   Median |
    /// |---------- |---------:|----------:|----------:|---------:|
    /// |  Distinct | 5.354 ns | 0.1355 ns | 0.3058 ns | 5.430 ns |
    /// |     Laser | 5.264 ns | 0.1320 ns | 0.2667 ns | 5.080 ns |
    /// |  Tricycle | 4.995 ns | 0.0245 ns | 0.0205 ns | 5.002 ns |
    /// | FourWheel | 6.358 ns | 0.0289 ns | 0.0241 ns | 6.363 ns |
    /// |   Mizuchi | 6.210 ns | 0.0621 ns | 0.0581 ns | 6.209 ns |
    /// </summary>
    public class RandomExclusiveDoubleBoundedComparison
    {
        private IEnhancedRandom _rng = null!;

        [GlobalSetup(Target = nameof(Distinct))]
        public void DistinctSetup() => _rng = new DistinctRandom(1UL);
        [Benchmark]
        public double Distinct() => _rng.NextExclusiveDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Laser))]
        public void LaserSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public double Laser() => _rng.NextExclusiveDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Tricycle))]
        public void TricycleSetup() => _rng = new TricycleRandom(1UL);
        [Benchmark]
        public double Tricycle() => _rng.NextExclusiveDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(FourWheel))]
        public void FourWheelSetup() => _rng = new FourWheelRandom(1UL);
        [Benchmark]
        public double FourWheel() => _rng.NextExclusiveDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Stranger))]
        public void StrangerSetup() => _rng = new StrangerRandom(1UL);
        [Benchmark]
        public double Stranger() => _rng.NextExclusiveDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Xoshiro256StarStar))]
        public void Xoshiro256StarStarSetup() => _rng = new Xoshiro256StarStarRandom(1UL);
        [Benchmark]
        public double Xoshiro256StarStar() => _rng.NextExclusiveDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Xorshift128Plus))]
        public void Xorshift128PlusSetup() => _rng = new Xorshift128PlusRandom(1UL);
        [Benchmark]
        public double Xorshift128Plus() => _rng.NextExclusiveDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(RomuTrio))]
        public void RomuTrioSetup() => _rng = new RomuTrioRandom(1UL);
        [Benchmark]
        public double RomuTrio() => _rng.NextExclusiveDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Mizuchi))]
        public void MizuchiSetup() => _rng = new MizuchiRandom(1UL);
        [Benchmark]
        public double Mizuchi() => _rng.NextExclusiveDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Trim))]
        public void TrimSetup() => _rng = new TrimRandom(1UL);
        [Benchmark]
        public double Trim() => _rng.NextExclusiveDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Whisker))]
        public void WhiskerSetup() => _rng = new WhiskerRandom(1UL);
        [Benchmark]
        public double Whisker() => _rng.NextExclusiveDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Scruff))]
        public void ScruffSetup() => _rng = new ScruffRandom(1UL);
        [Benchmark]
        public double Scruff() => _rng.NextExclusiveDouble(1.1, -0.1);

        [GlobalSetup(Target = nameof(Ace))]
        public void AceSetup() => _rng = new AceRandom(1UL);
        [Benchmark]
        public double Ace() => _rng.NextExclusiveDouble(1.1, -0.1);
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

    /// <summary>
    ///.NET 6.0, using manually-inlined ulong generation and BitConverter:
    ///|      Method |     Mean |     Error |    StdDev |   Median |
    ///|------------ |---------:|----------:|----------:|---------:|
    ///|       Laser | 2.773 ns | 0.0802 ns | 0.0924 ns | 2.817 ns |
    ///|      LaserS | 1.672 ns | 0.0595 ns | 0.0662 ns | 1.664 ns |
    ///|    Tricycle | 2.891 ns | 0.0860 ns | 0.1233 ns | 2.876 ns |
    ///|   TricycleS | 1.573 ns | 0.0479 ns | 0.0449 ns | 1.558 ns |
    ///|    RomuTrio | 2.902 ns | 0.0861 ns | 0.0992 ns | 2.953 ns |
    ///|   RomuTrioS | 1.752 ns | 0.0066 ns | 0.0055 ns | 1.752 ns |
    ///|     Mizuchi | 2.812 ns | 0.0854 ns | 0.1278 ns | 2.727 ns |
    ///|    MizuchiS | 1.759 ns | 0.0659 ns | 0.1118 ns | 1.732 ns |
    ///| XorShift128 | 2.025 ns | 0.0691 ns | 0.0822 ns | 2.023 ns |
    ///|         NR3 | 3.718 ns | 0.1035 ns | 0.1232 ns | 3.730 ns |
    ///|       NR3Q1 | 2.290 ns | 0.0466 ns | 0.0436 ns | 2.283 ns |
    ///|       NR3Q2 | 2.456 ns | 0.0612 ns | 0.0572 ns | 2.471 ns |
    /// .NET 6.0, using an older way:
    ///|      Method |     Mean |     Error |    StdDev |   Median |
    ///|------------ |---------:|----------:|----------:|---------:|
    ///|      Seeded | 8.710 ns | 0.1743 ns | 0.1712 ns | 8.654 ns |
    ///|    Unseeded | 2.690 ns | 0.0825 ns | 0.1723 ns | 2.693 ns |
    ///|       Laser | 2.740 ns | 0.0762 ns | 0.0990 ns | 2.695 ns |
    ///|      LaserS | 2.543 ns | 0.0812 ns | 0.1640 ns | 2.621 ns |
    ///|    Tricycle | 2.816 ns | 0.0845 ns | 0.1628 ns | 2.725 ns |
    ///|   TricycleS | 2.589 ns | 0.0810 ns | 0.1481 ns | 2.570 ns |
    ///|    RomuTrio | 2.722 ns | 0.0837 ns | 0.1633 ns | 2.722 ns |
    ///|   RomuTrioS | 2.669 ns | 0.0814 ns | 0.0905 ns | 2.697 ns |
    ///|     Mizuchi | 2.881 ns | 0.0865 ns | 0.1559 ns | 2.926 ns |
    ///|    MizuchiS | 2.605 ns | 0.0803 ns | 0.1427 ns | 2.662 ns |
    ///| XorShift128 | 1.770 ns | 0.0448 ns | 0.0374 ns | 1.754 ns |
    ///|         NR3 | 3.674 ns | 0.1031 ns | 0.2035 ns | 3.521 ns |
    ///|       NR3Q1 | 2.339 ns | 0.0689 ns | 0.0919 ns | 2.297 ns |
    ///|       NR3Q2 | 2.413 ns | 0.0779 ns | 0.1364 ns | 2.438 ns |
    /// With unsafe code...? Also .NET 6.0.
    ///|      Method |     Mean |     Error |    StdDev |   Median |
    ///|------------ |---------:|----------:|----------:|---------:|
    ///|      Seeded | 8.760 ns | 0.1944 ns | 0.2238 ns | 8.700 ns |
    ///|    Unseeded | 2.635 ns | 0.0810 ns | 0.1521 ns | 2.551 ns |
    ///|       Laser | 2.874 ns | 0.0868 ns | 0.1401 ns | 2.907 ns |
    ///|      LaserS | 2.525 ns | 0.0443 ns | 0.0492 ns | 2.513 ns |
    ///|    Tricycle | 2.895 ns | 0.0859 ns | 0.1023 ns | 2.907 ns |
    ///|   TricycleS | 2.664 ns | 0.0817 ns | 0.1319 ns | 2.649 ns |
    ///|    RomuTrio | 2.560 ns | 0.0804 ns | 0.1530 ns | 2.602 ns |
    ///|   RomuTrioS | 2.627 ns | 0.0808 ns | 0.1687 ns | 2.611 ns |
    ///|     Mizuchi | 2.951 ns | 0.0864 ns | 0.1513 ns | 2.955 ns |
    ///|    MizuchiS | 2.409 ns | 0.0769 ns | 0.0601 ns | 2.403 ns |
    ///| XorShift128 | 1.904 ns | 0.0676 ns | 0.1237 ns | 1.843 ns |
    ///|         NR3 | 3.807 ns | 0.1042 ns | 0.2222 ns | 3.794 ns |
    ///|       NR3Q1 | 2.486 ns | 0.0781 ns | 0.1542 ns | 2.580 ns |
    ///|       NR3Q2 | 2.322 ns | 0.0095 ns | 0.0074 ns | 2.321 ns |
    /// With UnsafeFormDouble() on manually-inlined NextULong() code used by LaserS and TricycleS only, .NET 6.0:
    ///|      Method |     Mean |     Error |    StdDev |   Median |
    ///|------------ |---------:|----------:|----------:|---------:|
    ///|      Seeded | 8.724 ns | 0.1993 ns | 0.2448 ns | 8.768 ns |
    ///|    Unseeded | 2.609 ns | 0.0806 ns | 0.1391 ns | 2.641 ns |
    ///|       Laser | 3.025 ns | 0.0893 ns | 0.1309 ns | 3.064 ns |
    ///|      LaserS | 2.392 ns | 0.0751 ns | 0.1568 ns | 2.275 ns |
    ///|    Tricycle | 2.925 ns | 0.0862 ns | 0.1837 ns | 2.943 ns |
    ///|   TricycleS | 2.484 ns | 0.0786 ns | 0.1838 ns | 2.550 ns |
    ///|    RomuTrio | 2.594 ns | 0.0823 ns | 0.1420 ns | 2.525 ns |
    ///|   RomuTrioS | 2.704 ns | 0.0603 ns | 0.0564 ns | 2.735 ns |
    ///|     Mizuchi | 2.761 ns | 0.0841 ns | 0.1405 ns | 2.788 ns |
    ///|    MizuchiS | 2.650 ns | 0.0764 ns | 0.0751 ns | 2.683 ns |
    ///| XorShift128 | 1.954 ns | 0.0668 ns | 0.1319 ns | 1.989 ns |
    ///|         NR3 | 3.762 ns | 0.1032 ns | 0.1938 ns | 3.794 ns |
    ///|       NR3Q1 | 2.542 ns | 0.0659 ns | 0.0616 ns | 2.499 ns |
    ///|       NR3Q2 | 2.349 ns | 0.0642 ns | 0.0536 ns | 2.331 ns |
    /// </summary>
    /// <remarks>
    /// The tests followed by "S" use NextSparseDouble(); the others use NextDouble() on
    /// either IEnhancedRandom or IGenerator. The speedup for NextSparseDouble() is now
    /// quite significant, in the first block of benchmarks. This uses manually-inlined
    /// NextULong() code fed into BitConverter to get a double.
    /// </remarks>
    public class RandomDoubleTechniqueComparison
    {
        private IEnhancedRandom _rng = null!;
        private IGenerator _gen = null!;

//#if NET6_0_OR_GREATER
//        private System.Random _seededRandom = null!;
//        private System.Random _unseededRandom = null!;

//        [GlobalSetup(Target = nameof(Seeded))]
//        public void SeededSetup() => _seededRandom = new System.Random(1);
//        [Benchmark]
//        public double Seeded() => _seededRandom.NextDouble();

//        [GlobalSetup(Target = nameof(Unseeded))]
//        public void UnseededSetup() => _unseededRandom = new System.Random();
//        [Benchmark]
//        public double Unseeded() => _unseededRandom.NextDouble();
//#endif

        [GlobalSetup(Target = nameof(Laser))]
        public void LaserSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public double Laser() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(LaserS))]
        public void LaserSSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public double LaserS() => _rng.NextSparseDouble();

        [GlobalSetup(Target = nameof(Tricycle))]
        public void TricycleSetup() => _rng = new TricycleRandom(1UL);
        [Benchmark]
        public double Tricycle() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(TricycleS))]
        public void TricycleSSetup() => _rng = new TricycleRandom(1UL);
        [Benchmark]
        public double TricycleS() => _rng.NextSparseDouble();

        [GlobalSetup(Target = nameof(RomuTrio))]
        public void RomuTrioSetup() => _rng = new RomuTrioRandom(1UL);
        [Benchmark]
        public double RomuTrio() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(RomuTrioS))]
        public void RomuTrioSSetup() => _rng = new RomuTrioRandom(1UL);
        [Benchmark]
        public double RomuTrioS() => _rng.NextSparseDouble();

        [GlobalSetup(Target = nameof(Mizuchi))]
        public void MizuchiSetup() => _rng = new MizuchiRandom(1UL);
        [Benchmark]
        public double Mizuchi() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(MizuchiS))]
        public void MizuchiSSetup() => _rng = new MizuchiRandom(1UL);
        [Benchmark]
        public double MizuchiS() => _rng.NextSparseDouble();

        [GlobalSetup(Target = nameof(XorShift128))]
        public void XorShift128Setup() => _gen = new XorShift128Generator(1);
        [Benchmark]
        public double XorShift128() => _gen.NextDouble();

        [GlobalSetup(Target = nameof(NR3))]
        public void NR3Setup() => _gen = new NR3Generator(1);
        [Benchmark]
        public double NR3() => _gen.NextDouble();

        [GlobalSetup(Target = nameof(NR3Q1))]
        public void NR3Q1Setup() => _gen = new NR3Q1Generator(1);
        [Benchmark]
        public double NR3Q1() => _gen.NextDouble();

        [GlobalSetup(Target = nameof(NR3Q2))]
        public void NR3Q2Setup() => _gen = new NR3Q2Generator(1);
        [Benchmark]
        public double NR3Q2() => _gen.NextDouble();
    }
    /// <summary>
    /// Used in attempts to disassemble these methods and see how they work...
    /// Except the disassembler currently just prints the same 24-byte method for each benchmark.
    /// .NET 6.0, using BitConverter for "S" methods:
    ///|           Method |     Mean |     Error |    StdDev |   Median |
    ///|----------------- |---------:|----------:|----------:|---------:|
    ///|            Laser | 2.722 ns | 0.0624 ns | 0.0613 ns | 2.761 ns |
    ///|           LaserS | 1.832 ns | 0.0646 ns | 0.1114 ns | 1.900 ns |
    ///|  Xorshift128Plus | 2.704 ns | 0.0813 ns | 0.1241 ns | 2.628 ns |
    ///| Xorshift128PlusS | 1.807 ns | 0.0631 ns | 0.0925 ns | 1.841 ns |
    ///|      XorShift128 | 2.090 ns | 0.0692 ns | 0.0769 ns | 2.117 ns |
    /// Old .NET 6.0:
    ///|           Method |     Mean |     Error |    StdDev |
    ///|----------------- |---------:|----------:|----------:|
    ///|            Laser | 2.615 ns | 0.0185 ns | 0.0173 ns |
    ///|           LaserS | 2.390 ns | 0.0376 ns | 0.0352 ns |
    ///|  Xorshift128Plus | 2.527 ns | 0.0396 ns | 0.0370 ns |
    ///| Xorshift128PlusS | 1.911 ns | 0.0674 ns | 0.0923 ns |
    ///|      XorShift128 | 2.029 ns | 0.0709 ns | 0.1205 ns |
    /// </summary>
    public class RandomDoubleTechniqueDisassembler
    {
        private IEnhancedRandom _rng = null!;
        private IGenerator _gen = null!;

        [GlobalSetup(Target = nameof(Laser))]
        public void LaserSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public double Laser() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(LaserS))]
        public void LaserSSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public double LaserS() => _rng.NextSparseDouble();

        [GlobalSetup(Target = nameof(Xorshift128Plus))]
        public void Xorshift128PlusSetup() => _rng = new Xorshift128PlusRandom(1UL);
        [Benchmark]
        public double Xorshift128Plus() => _rng.NextDouble();

        [GlobalSetup(Target = nameof(Xorshift128PlusS))]
        public void Xorshift128PlusSSetup() => _rng = new Xorshift128PlusRandom(1UL);
        [Benchmark]
        public double Xorshift128PlusS() => _rng.NextSparseDouble();

        [GlobalSetup(Target = nameof(XorShift128))]
        public void XorShift128Setup() => _gen = new XorShift128Generator(1);
        [Benchmark]
        public double XorShift128() => _gen.NextDouble();
    }
    /// <summary>
    /// .NET 6.0
    ///|              Method |      Mean |     Error |    StdDev |
    ///|-------------------- |----------:|----------:|----------:|
    ///| InlinedUnsafeDouble | 1.3904 ns | 0.0560 ns | 0.1106 ns |
    ///|   InlinedSafeDouble | 0.6483 ns | 0.0084 ns | 0.0066 ns |
    ///|        UnsafeDouble | 1.2840 ns | 0.0553 ns | 0.0969 ns |
    ///|          SafeDouble | 0.9116 ns | 0.0373 ns | 0.0349 ns |
    /// </summary>
    public class DoubleTechniqueComparison
    {
        private ulong StateA = 1UL;
        private ulong StateB = 1UL;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong NextULong()
        {
            var tx = StateA;
            var ty = StateB;
            StateA = ty;
            tx ^= tx << 23;
            tx ^= tx >> 17;
            tx ^= ty ^ (ty >> 26);
            StateB = tx;
            return tx + ty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double UnsafeFormDouble(ulong value)
        {
            value = (value >> 12) | 0x3FF0000000000000UL;
            return *((double*)&value) - 1.0;
        }

        [GlobalSetup(Targets = new[]{nameof(InlinedUnsafeDouble), nameof(InlinedSafeDouble)})]
        public void DoubleSetup()
        {
            StateA = 1UL;
            StateB = 1UL;
        }

        [Benchmark]
        public double InlinedUnsafeDouble()
        {
            var tx = StateA;
            var ty = StateB;
            StateA = ty;
            tx ^= tx << 23;
            tx ^= tx >> 17;
            tx ^= ty ^ (ty >> 26);
            StateB = tx;
            return UnsafeFormDouble(tx + ty);
        }

        [Benchmark]
        public double InlinedSafeDouble()
        {
            var tx = StateA;
            var ty = StateB;
            StateA = ty;
            tx ^= tx << 23;
            tx ^= tx >> 17;
            tx ^= ty ^ (ty >> 26);
            StateB = tx;
            return BitConverter.Int64BitsToDouble((long)((tx + ty) >> 12) | 0x3FF0000000000000L) - 1.0;
        }
        [Benchmark]
        public double UnsafeDouble()
        {
            return UnsafeFormDouble(NextULong());
        }

        [Benchmark]
        public double SafeDouble()
        {
            return BitConverter.Int64BitsToDouble((long)(NextULong() >> 12) | 0x3FF0000000000000L) - 1.0;
        }
    }
    /// <summary>
    /// .NET 5.0 with all "S" methods inlined manually:
    ///|              Method |     Mean |     Error |    StdDev |   Median |
    ///|-------------------- |---------:|----------:|----------:|---------:|
    ///|            Distinct | 2.666 ns | 0.0570 ns | 0.0505 ns | 2.631 ns |
    ///|           DistinctS | 1.893 ns | 0.0569 ns | 0.0720 ns | 1.858 ns |
    ///|           FourWheel | 2.793 ns | 0.0102 ns | 0.0095 ns | 2.792 ns |
    ///|          FourWheelS | 1.743 ns | 0.0566 ns | 0.0716 ns | 1.701 ns |
    ///|               Laser | 2.589 ns | 0.0778 ns | 0.0728 ns | 2.558 ns |
    ///|              LaserS | 1.784 ns | 0.0638 ns | 0.1600 ns | 1.854 ns |
    ///|            Tricycle | 2.673 ns | 0.0888 ns | 0.2228 ns | 2.689 ns |
    ///|           TricycleS | 1.454 ns | 0.0084 ns | 0.0066 ns | 1.454 ns |
    ///|            RomuTrio | 3.248 ns | 0.0931 ns | 0.1859 ns | 3.215 ns |
    ///|           RomuTrioS | 1.951 ns | 0.0656 ns | 0.1022 ns | 2.016 ns |
    ///|             Mizuchi | 2.916 ns | 0.0791 ns | 0.0846 ns | 2.960 ns |
    ///|            MizuchiS | 1.540 ns | 0.0610 ns | 0.1116 ns | 1.517 ns |
    ///|            Stranger | 2.997 ns | 0.0864 ns | 0.0887 ns | 3.023 ns |
    ///|           StrangerS | 2.066 ns | 0.0691 ns | 0.1630 ns | 2.090 ns |
    ///|                Trim | 2.962 ns | 0.0835 ns | 0.0857 ns | 2.978 ns |
    ///|               TrimS | 1.633 ns | 0.0604 ns | 0.1375 ns | 1.625 ns |
    ///|     Xorshift128Plus | 3.208 ns | 0.0913 ns | 0.1966 ns | 3.213 ns |
    ///|    Xorshift128PlusS | 1.774 ns | 0.0620 ns | 0.1320 ns | 1.841 ns |
    ///|  Xoshiro256StarStar | 2.998 ns | 0.0880 ns | 0.2004 ns | 3.002 ns |
    ///| Xoshiro256StarStarS | 2.276 ns | 0.0728 ns | 0.1551 ns | 2.153 ns |
    ///
    /// .NET 6.0 with all "S" methods inlined manually:
    ///|              Method |     Mean |     Error |    StdDev |   Median |
    ///|-------------------- |---------:|----------:|----------:|---------:|
    ///|            Distinct | 3.133 ns | 0.0902 ns | 0.2144 ns | 3.254 ns |
    ///|           DistinctS | 1.655 ns | 0.0610 ns | 0.1438 ns | 1.624 ns |
    ///|           FourWheel | 2.943 ns | 0.0852 ns | 0.1759 ns | 3.051 ns |
    ///|          FourWheelS | 1.828 ns | 0.0634 ns | 0.1405 ns | 1.786 ns |
    ///|               Laser | 2.870 ns | 0.0864 ns | 0.2054 ns | 2.947 ns |
    ///|              LaserS | 1.655 ns | 0.0124 ns | 0.0116 ns | 1.652 ns |
    ///|            Tricycle | 2.861 ns | 0.0276 ns | 0.0258 ns | 2.856 ns |
    ///|           TricycleS | 1.763 ns | 0.0103 ns | 0.0091 ns | 1.762 ns |
    ///|            RomuTrio | 2.944 ns | 0.0216 ns | 0.0202 ns | 2.946 ns |
    ///|           RomuTrioS | 2.161 ns | 0.0249 ns | 0.0233 ns | 2.161 ns |
    ///|             Mizuchi | 2.859 ns | 0.0143 ns | 0.0127 ns | 2.862 ns |
    ///|            MizuchiS | 1.433 ns | 0.0157 ns | 0.0147 ns | 1.432 ns |
    ///|            Stranger | 2.940 ns | 0.0292 ns | 0.0273 ns | 2.947 ns |
    ///|           StrangerS | 2.019 ns | 0.0135 ns | 0.0126 ns | 2.020 ns |
    ///|                Trim | 2.870 ns | 0.0182 ns | 0.0171 ns | 2.873 ns |
    ///|               TrimS | 1.737 ns | 0.0145 ns | 0.0136 ns | 1.739 ns |
    ///|     Xorshift128Plus | 2.918 ns | 0.0220 ns | 0.0206 ns | 2.913 ns |
    ///|    Xorshift128PlusS | 1.623 ns | 0.0125 ns | 0.0117 ns | 1.627 ns |
    ///|  Xoshiro256StarStar | 3.075 ns | 0.0256 ns | 0.0227 ns | 3.068 ns |
    ///| Xoshiro256StarStarS | 2.193 ns | 0.0283 ns | 0.0251 ns | 2.202 ns |
    ///
    /// .NET 6.0 with only some methods inlined manually:
    ///|              Method |     Mean |     Error |    StdDev |   Median |
    ///|-------------------- |---------:|----------:|----------:|---------:|
    ///|            Distinct | 2.909 ns | 0.0830 ns | 0.0956 ns | 2.864 ns |
    ///|           DistinctS | 1.754 ns | 0.0613 ns | 0.1008 ns | 1.725 ns |
    ///|           FourWheel | 2.805 ns | 0.0325 ns | 0.0254 ns | 2.806 ns |
    ///|          FourWheelS | 2.391 ns | 0.0428 ns | 0.0334 ns | 2.382 ns |
    ///|               Laser | 2.879 ns | 0.0853 ns | 0.1685 ns | 2.849 ns |
    ///|              LaserS | 1.677 ns | 0.0603 ns | 0.1087 ns | 1.671 ns |
    ///|            Tricycle | 2.843 ns | 0.0829 ns | 0.1474 ns | 2.829 ns |
    ///|           TricycleS | 2.833 ns | 0.0834 ns | 0.1545 ns | 2.723 ns |
    ///|            RomuTrio | 2.985 ns | 0.0869 ns | 0.1567 ns | 3.085 ns |
    ///|           RomuTrioS | 2.425 ns | 0.0749 ns | 0.1596 ns | 2.337 ns |
    ///|             Mizuchi | 2.752 ns | 0.0826 ns | 0.1510 ns | 2.756 ns |
    ///|            MizuchiS | 1.574 ns | 0.0547 ns | 0.0562 ns | 1.602 ns |
    ///|            Stranger | 3.009 ns | 0.0866 ns | 0.1539 ns | 3.090 ns |
    ///|           StrangerS | 2.674 ns | 0.0799 ns | 0.1577 ns | 2.606 ns |
    ///|                Trim | 2.762 ns | 0.0839 ns | 0.1491 ns | 2.663 ns |
    ///|               TrimS | 2.388 ns | 0.0774 ns | 0.1527 ns | 2.491 ns |
    ///|     Xorshift128Plus | 2.830 ns | 0.0839 ns | 0.1616 ns | 2.786 ns |
    ///|    Xorshift128PlusS | 2.603 ns | 0.0792 ns | 0.2235 ns | 2.609 ns |
    ///|  Xoshiro256StarStar | 3.270 ns | 0.0941 ns | 0.1436 ns | 3.315 ns |
    ///| Xoshiro256StarStarS | 2.698 ns | 0.0809 ns | 0.1187 ns | 2.739 ns |
    /// </summary>
    /// <remarks>
    /// The tests followed by "S" use NextSparseFloat(); the others use NextFloat() on
    /// either IEnhancedRandom.
    /// In the first and second sets of benchmarks, all "S" generators are manually inlined.
    /// In the last set of benchmarks, DistinctS, LaserS, and MizuchiS are the only ones inlined.
    /// </remarks>
    public class RandomFloatTechniqueComparison
    {
        private IEnhancedRandom _rng = null!;

        [GlobalSetup(Target = nameof(Distinct))]
        public void DistinctSetup() => _rng = new DistinctRandom(1UL);
        [Benchmark]
        public float Distinct() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(DistinctS))]
        public void DistinctSSetup() => _rng = new DistinctRandom(1UL);
        [Benchmark]
        public float DistinctS() => _rng.NextSparseFloat();

        [GlobalSetup(Target = nameof(FourWheel))]
        public void FourWheelSetup() => _rng = new FourWheelRandom(1UL);
        [Benchmark]
        public float FourWheel() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(FourWheelS))]
        public void FourWheelSSetup() => _rng = new FourWheelRandom(1UL);
        [Benchmark]
        public float FourWheelS() => _rng.NextSparseFloat();

        [GlobalSetup(Target = nameof(Laser))]
        public void LaserSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public float Laser() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(LaserS))]
        public void LaserSSetup() => _rng = new LaserRandom(1UL);
        [Benchmark]
        public float LaserS() => _rng.NextSparseFloat();

        [GlobalSetup(Target = nameof(Tricycle))]
        public void TricycleSetup() => _rng = new TricycleRandom(1UL);
        [Benchmark]
        public float Tricycle() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(TricycleS))]
        public void TricycleSSetup() => _rng = new TricycleRandom(1UL);
        [Benchmark]
        public float TricycleS() => _rng.NextSparseFloat();

        [GlobalSetup(Target = nameof(RomuTrio))]
        public void RomuTrioSetup() => _rng = new RomuTrioRandom(1UL);
        [Benchmark]
        public float RomuTrio() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(RomuTrioS))]
        public void RomuTrioSSetup() => _rng = new RomuTrioRandom(1UL);
        [Benchmark]
        public float RomuTrioS() => _rng.NextSparseFloat();

        [GlobalSetup(Target = nameof(Mizuchi))]
        public void MizuchiSetup() => _rng = new MizuchiRandom(1UL);
        [Benchmark]
        public float Mizuchi() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(MizuchiS))]
        public void MizuchiSSetup() => _rng = new MizuchiRandom(1UL);
        [Benchmark]
        public float MizuchiS() => _rng.NextSparseFloat();
        [GlobalSetup(Target = nameof(Stranger))]
        public void StrangerSetup() => _rng = new StrangerRandom(1UL);
        [Benchmark]
        public float Stranger() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(StrangerS))]
        public void StrangerSSetup() => _rng = new StrangerRandom(1UL);
        [Benchmark]
        public float StrangerS() => _rng.NextSparseFloat();

        [GlobalSetup(Target = nameof(Trim))]
        public void TrimSetup() => _rng = new TrimRandom(1UL);
        [Benchmark]
        public float Trim() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(TrimS))]
        public void TrimSSetup() => _rng = new TrimRandom(1UL);
        [Benchmark]
        public float TrimS() => _rng.NextSparseFloat();

        [GlobalSetup(Target = nameof(Whisker))]
        public void WhiskerSetup() => _rng = new WhiskerRandom(1UL);
        [Benchmark]
        public float Whisker() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(WhiskerS))]
        public void WhiskerSSetup() => _rng = new WhiskerRandom(1UL);
        [Benchmark]
        public float WhiskerS() => _rng.NextSparseFloat();

        [GlobalSetup(Target = nameof(Scruff))]
        public void ScruffSetup() => _rng = new ScruffRandom(1UL);
        [Benchmark]
        public float Scruff() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(ScruffS))]
        public void ScruffSSetup() => _rng = new ScruffRandom(1UL);
        [Benchmark]
        public float ScruffS() => _rng.NextSparseFloat();

        [GlobalSetup(Target = nameof(Ace))]
        public void AceSetup() => _rng = new AceRandom(1UL);
        [Benchmark]
        public float Ace() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(AceS))]
        public void AceSSetup() => _rng = new AceRandom(1UL);
        [Benchmark]
        public float AceS() => _rng.NextSparseFloat();

        [GlobalSetup(Target = nameof(Xorshift128Plus))]
        public void Xorshift128PlusSetup() => _rng = new Xorshift128PlusRandom(1UL);
        [Benchmark]
        public float Xorshift128Plus() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(Xorshift128PlusS))]
        public void Xorshift128PlusSSetup() => _rng = new Xorshift128PlusRandom(1UL);
        [Benchmark]
        public float Xorshift128PlusS() => _rng.NextSparseFloat();

        [GlobalSetup(Target = nameof(Xoshiro256StarStar))]
        public void Xoshiro256StarStarSetup() => _rng = new Xoshiro256StarStarRandom(1UL);
        [Benchmark]
        public float Xoshiro256StarStar() => _rng.NextFloat();

        [GlobalSetup(Target = nameof(Xoshiro256StarStarS))]
        public void Xoshiro256StarStarSSetup() => _rng = new Xoshiro256StarStarRandom(1UL);
        [Benchmark]
        public float Xoshiro256StarStarS() => _rng.NextSparseFloat();
    }
    /// <summary>
    /// .NET 6.0, old machine:
    ///|             Method |      Mean |     Error |    StdDev |    Median |
    ///|------------------- |----------:|----------:|----------:|----------:|
    ///|           Distinct | 0.4259 ns | 0.0389 ns | 0.0660 ns | 0.4645 ns |
    ///|              Laser | 0.4763 ns | 0.0396 ns | 0.0371 ns | 0.4857 ns |
    ///|           Tricycle | 0.4728 ns | 0.0176 ns | 0.0137 ns | 0.4699 ns |
    ///|          FourWheel | 0.7419 ns | 0.0449 ns | 0.0534 ns | 0.7096 ns |
    ///|           Stranger | 0.8115 ns | 0.0458 ns | 0.0490 ns | 0.8316 ns |
    ///| Xoshiro256StarStar | 1.1587 ns | 0.0169 ns | 0.0132 ns | 1.1602 ns |
    ///|    Xorshift128Plus | 0.4567 ns | 0.0397 ns | 0.0674 ns | 0.4128 ns |
    ///|           RomuTrio | 0.6962 ns | 0.0433 ns | 0.0577 ns | 0.7357 ns |
    ///|            Mizuchi | 0.6047 ns | 0.0424 ns | 0.0536 ns | 0.6202 ns |
    ///|               Trim | 0.7616 ns | 0.0451 ns | 0.0422 ns | 0.7475 ns |
    ///|            Whisker | 0.9124 ns | 0.0492 ns | 0.0460 ns | 0.8892 ns |
    ///|             Scruff | 0.7445 ns | 0.0455 ns | 0.0558 ns | 0.7205 ns |
    ///.NET 6.0, new machine:
    ///BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.3880/23H2/2023Update/SunValley3)
    ///12th Gen Intel Core i7-12800H, 1 CPU, 20 logical and 14 physical cores
    ///.NET SDK 9.0.101
    ///  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
    ///  Job-ZGKEJL : .NET 6.0.36 (6.0.3624.51421), X64 RyuJIT AVX2
    ///
    ///Runtime=.NET 6.0  Toolchain=net60
    ///
    ///| Method             | Mean      | Error     | StdDev    |
    ///|------------------- |----------:|----------:|----------:|
    ///| Distinct           | 0.2973 ns | 0.0064 ns | 0.0059 ns |
    ///| Laser              | 0.1066 ns | 0.0169 ns | 0.0158 ns |
    ///| Tricycle           | 0.0879 ns | 0.0070 ns | 0.0059 ns |
    ///| FourWheel          | 0.0987 ns | 0.0074 ns | 0.0070 ns |
    ///| Stranger           | 0.1241 ns | 0.0100 ns | 0.0093 ns |
    ///| Xoshiro256StarStar | 0.3507 ns | 0.0033 ns | 0.0029 ns |
    ///| Xorshift128Plus    | 0.0920 ns | 0.0036 ns | 0.0032 ns |
    ///| RomuTrio           | 0.0766 ns | 0.0079 ns | 0.0074 ns |
    ///| Mizuchi            | 0.0987 ns | 0.0085 ns | 0.0079 ns |
    ///| Trim               | 0.1029 ns | 0.0084 ns | 0.0078 ns |
    ///| Whisker            | 0.1570 ns | 0.0074 ns | 0.0069 ns |
    ///| Scruff             | 0.0887 ns | 0.0077 ns | 0.0072 ns |
    ///| Ace                | 0.2280 ns | 0.0047 ns | 0.0041 ns |
    ///.NET 9.0:
    ///BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.3880/23H2/2023Update/SunValley3)
    ///12th Gen Intel Core i7-12800H, 1 CPU, 20 logical and 14 physical cores
    ///.NET SDK 9.0.101
    ///  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
    ///  Job-UYLXYQ : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
    ///
    ///Runtime=.NET 9.0  Toolchain=net90
    ///
    ///| Method             | Mean      | Error     | StdDev    |
    ///|------------------- |----------:|----------:|----------:|
    ///| Distinct           | 0.3538 ns | 0.0084 ns | 0.0079 ns |
    ///| Laser              | 0.1390 ns | 0.0087 ns | 0.0082 ns |
    ///| Tricycle           | 0.1431 ns | 0.0079 ns | 0.0070 ns |
    ///| FourWheel          | 0.1383 ns | 0.0062 ns | 0.0058 ns |
    ///| Stranger           | 0.1330 ns | 0.0060 ns | 0.0056 ns |
    ///| Xoshiro256StarStar | 0.2319 ns | 0.0109 ns | 0.0102 ns |
    ///| Xorshift128Plus    | 0.0000 ns | 0.0000 ns | 0.0000 ns |
    ///| RomuTrio           | 0.1319 ns | 0.0134 ns | 0.0125 ns |
    ///| Mizuchi            | 0.1322 ns | 0.0101 ns | 0.0094 ns |
    ///| Trim               | 0.1184 ns | 0.0063 ns | 0.0053 ns |
    ///| Whisker            | 0.1287 ns | 0.0090 ns | 0.0080 ns |
    ///| Scruff             | 0.1345 ns | 0.0104 ns | 0.0097 ns |
    ///| Ace                | 0.1360 ns | 0.0085 ns | 0.0080 ns |
    /// </summary>
    /// <remarks>
    /// OK, now THIS is where the sub-nanosecond times are. The .NET 5 and 6 benchmarks were run on a laptop with a
    /// 10th-gen Intel i7 mobile hexacore processor, like all the older benchmarks here.
    /// The .NET 9 benchmarks were run on a newer processor, on Windows 11 instead of 10, and who knows what else is
    /// different on a much-newer machine. They do appear much faster!
    /// </remarks>
    public class BareULongComparison
    {
        public DistinctRandom _DistinctRandom = null!;
        public LaserRandom _LaserRandom = null!;
        public TricycleRandom _TricycleRandom = null!;
        public FourWheelRandom _FourWheelRandom = null!;
        public StrangerRandom _StrangerRandom = null!;
        public Xoshiro256StarStarRandom _Xoshiro256StarStarRandom = null!;
        public Xorshift128PlusRandom _Xorshift128PlusRandom = null!;
        public RomuTrioRandom _RomuTrioRandom = null!;
        public MizuchiRandom _MizuchiRandom = null!;
        public TrimRandom _TrimRandom = null!;
        public WhiskerRandom _WhiskerRandom = null!;
        public ScruffRandom _ScruffRandom = null!;
        public AceRandom _AceRandom = null!;

        [GlobalSetup(Target = nameof(Distinct))]
        public void DistinctSetup() => _DistinctRandom = new DistinctRandom(1UL);
        [Benchmark]
        public ulong Distinct() => _DistinctRandom.NextULong();

        [GlobalSetup(Target = nameof(Laser))]
        public void LaserSetup() => _LaserRandom = new LaserRandom(1UL);
        [Benchmark]
        public ulong Laser() => _LaserRandom.NextULong();

        [GlobalSetup(Target = nameof(Tricycle))]
        public void TricycleSetup() => _TricycleRandom = new TricycleRandom(1UL);
        [Benchmark]
        public ulong Tricycle() => _TricycleRandom.NextULong();

        [GlobalSetup(Target = nameof(FourWheel))]
        public void FourWheelSetup() => _FourWheelRandom = new FourWheelRandom(1UL);
        [Benchmark]
        public ulong FourWheel() => _FourWheelRandom.NextULong();

        [GlobalSetup(Target = nameof(Stranger))]
        public void StrangerSetup() => _StrangerRandom = new StrangerRandom(1UL);
        [Benchmark]
        public ulong Stranger() => _StrangerRandom.NextULong();

        [GlobalSetup(Target = nameof(Xoshiro256StarStar))]
        public void Xoshiro256StarStarSetup() => _Xoshiro256StarStarRandom = new Xoshiro256StarStarRandom(1UL);
        [Benchmark]
        public ulong Xoshiro256StarStar() => _Xoshiro256StarStarRandom.NextULong();

        [GlobalSetup(Target = nameof(Xorshift128Plus))]
        public void Xorshift128PlusSetup() => _Xorshift128PlusRandom = new Xorshift128PlusRandom(1UL);
        [Benchmark]
        public ulong Xorshift128Plus() => _Xorshift128PlusRandom.NextULong();

        [GlobalSetup(Target = nameof(RomuTrio))]
        public void RomuTrioSetup() => _RomuTrioRandom = new RomuTrioRandom(1UL);
        [Benchmark]
        public ulong RomuTrio() => _RomuTrioRandom.NextULong();

        [GlobalSetup(Target = nameof(Mizuchi))]
        public void MizuchiSetup() => _MizuchiRandom = new MizuchiRandom(1UL);
        [Benchmark]
        public ulong Mizuchi() => _MizuchiRandom.NextULong();

        [GlobalSetup(Target = nameof(Trim))]
        public void TrimSetup() => _TrimRandom = new TrimRandom(1UL);
        [Benchmark]
        public ulong Trim() => _TrimRandom.NextULong();

        [GlobalSetup(Target = nameof(Whisker))]
        public void WhiskerSetup() => _WhiskerRandom = new WhiskerRandom(1UL);
        [Benchmark]
        public ulong Whisker() => _WhiskerRandom.NextULong();

        [GlobalSetup(Target = nameof(Scruff))]
        public void ScruffSetup() => _ScruffRandom = new ScruffRandom(1UL);
        [Benchmark]
        public ulong Scruff() => _ScruffRandom.NextULong();

        [GlobalSetup(Target = nameof(Ace))]
        public void AceSetup() => _AceRandom = new AceRandom(1UL);
        [Benchmark]
        public ulong Ace() => _AceRandom.NextULong();
    }

    internal static class Benchmarks
    {
        private static void Main(string[] args)
            => BenchmarkSwitcher.FromAssembly(typeof(Benchmarks).Assembly).Run(args);
    }
}
