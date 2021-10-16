using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ShaiRandom.PerformanceTests
{
    /// <summary>
    ///|          Method |     Mean |     Error |    StdDev |   Median |
    ///|---------------- |---------:|----------:|----------:|---------:|
    ///|        Distinct | 1.062 ns | 0.0499 ns | 0.0792 ns | 1.102 ns |
    ///|           Laser | 1.640 ns | 0.0078 ns | 0.0065 ns | 1.637 ns |
    ///|        Tricycle | 2.821 ns | 0.0868 ns | 0.1673 ns | 2.883 ns |
    ///|       FourWheel | 3.227 ns | 0.0925 ns | 0.1668 ns | 3.315 ns |
    ///|        Stranger | 3.417 ns | 0.0965 ns | 0.1185 ns | 3.408 ns |
    ///| XoshiroStarStar | 4.715 ns | 0.0949 ns | 0.0888 ns | 4.742 ns |
    ///|        RomuTrio | 4.739 ns | 0.1245 ns | 0.2147 ns | 4.774 ns |
    /// </summary>
    /// <remarks>
    /// It looks like .NET does virtually no optimizations relating to ILP,
    /// and smaller states are almost always better. I have no clue why RomuTrio,
    /// with 3 states, is slower than any of the 4-state generators, while Tricycle,
    /// also with 3 states, is faster than any of the 4-state generators.
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
    }
    internal static class Program
    {
        private static void Main(string[] args)
            => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
