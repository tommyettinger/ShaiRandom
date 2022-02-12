//using ShaiRandom.TroschuetzCompat;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: Xunit.TestFramework("ShaiRandom.UnitTests.TestFramework", "ShaiRandom.UnitTests")]
namespace ShaiRandom.UnitTests
{
    public class TestFramework : XunitTestFramework
    {
        public TestFramework(IMessageSink messageSink)
            :base(messageSink)
        {
            Serializer.RegisterShaiRandomDefaultTags();
            //SerializerExtensions.RegisterTroschuetzCompatDefaultTags();
        }
    }
}
