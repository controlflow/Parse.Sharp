using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture]
  public sealed class CharParserTest : ParserTest
  {
    [Test] public void Char()
    {
      AssertParse(Parse.Char('0'), "0", '0');
      AssertParse(Parse.Char('['), "[", '[');
    }
  }
}