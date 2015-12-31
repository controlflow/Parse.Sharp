using JetBrains.Annotations;
using NUnit.Framework;

namespace Parse.Sharp.Tests
{
  public abstract class ParserTest
  {
    public static void AssertParse([NotNull] Parser<bool> parser, [NotNull] string input)
    {
      AssertParse(parser, input, expectedValue: true);
    }

    public static void AssertParse<T>([NotNull] Parser<T> parser, [NotNull] string input, T expectedValue)
    {
      Assert.NotNull(parser, "parser != null");
      Assert.NotNull(input, "input != null");

      var parsedValue = parser.Parse(input);
      Assert.AreEqual(parsedValue, expectedValue);
    }
  }
}