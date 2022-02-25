namespace ShaiRandom
{
    internal static class ErrorMessages
    {
        public const string EmptyList = "List must not be empty.";
        public const string InfiniteMaxValue = "maxValue cannot be equal to positive infinity.";
        public const string InfiniteMaxValueMinusMinValue = "maxValue minus minValue cannot be equal to infinity.";
        public const string InvalidParams = "Given parameter (or parameters) are not valid.";
        public const string MaxValueIsTooSmall = "Given max value is too small.";
        public const string MinValueGreaterThanMaxValue = "maxValue should be greater than or equal to minValue.";
        public const string NegativeMaxValue = "maxValue must be greater than or equal to zero.";
        public const string NullBuffer = "Buffer must not be undefined.";
        public const string NullDistribution = "Distribution must not be undefined.";
        public const string NullGenerator = "Generator must not be undefined.";
        public const string NullList = "List must not be undefined.";
        public const string NullWeights = "Weights collection must not be undefined.";
        public const string UndefinedMean = "Mean is undefined for given distribution.";
        public const string UndefinedMeanForParams = "Mean is undefined for given distribution under given parameters.";
        public const string UndefinedMedian = "Median is undefined for given distribution.";
        public const string UndefinedMode = "Mode is undefined for given distribution.";
        public const string UndefinedModeForParams = "Mode is undefined for given distribution under given parameters.";
        public const string UndefinedVariance = "Variance is undefined for given distribution.";
        public const string UndefinedVarianceForParams = "Variance is undefined for given distribution under given parameters.";
    }

}
