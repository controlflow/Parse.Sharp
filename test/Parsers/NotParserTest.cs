using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture]
  public class NotParserTest : ParserTest
  {
    [Test] public void NotCombinator()
    {
      AssertParse(Parse.Digit.Not(), "z");
    }
  }
}