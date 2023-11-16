using NUnit.Framework;

namespace Telia.GraphQL.Tests
{
    public static class AssertUtils
    {
        public static void AreEqualIgnoreLineBreaks(string expected, string actual)
        {
            Assert.AreEqual(
                expected.Replace("\r\n", " ").Replace("\n", " "),
                actual.Replace("\r\n", " ").Replace("\n", " "));
        }
    }
}
