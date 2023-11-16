using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Telia.LinqToGraphQLToModel.Tests._Abstract;

public class BaseTestClass
{
    protected void IsEqualIgnoreWhitespace(string text, string expected)
    {
        text = ClearWhiteSpaceAndNewLines(text);

        expected = ClearWhiteSpaceAndNewLines(expected);

        try
        {
            Assert.IsTrue(text == expected);
        }
        catch
        {
            Dump.Write(text);
            Dump.Write(expected);

            Assert.IsTrue(text == expected);
        }
    }

    protected string ClearWhiteSpaceAndNewLines(string data)
    {
        var sb = new StringBuilder(data);

        sb.Replace(Environment.NewLine, " ")
            .Replace("\t", " ")
            .Replace("\\r\\n", " ")
            .Replace("\\n", " ")
            .Replace("  ", " ")
            .Replace("  ", " ")
            .Replace("  ", " ")
            .Replace("  ", " ")
            .Replace(": ", ":")
            .Replace("{ ", "{")
            .Replace(" }", "}")
            .Replace(" {", "{")
            .Replace(", ", ",")
            .Replace("? >", "?>");

        return sb.ToString().TrimEnd();
    }
}
