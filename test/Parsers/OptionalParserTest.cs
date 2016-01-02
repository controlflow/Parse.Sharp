using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture] public sealed class OptionalParserTest : ParserTest
  {
    [Test] public void Optional()
    {
      var maybeDot = Parse.Dot.Optional2();

      AssertParse(maybeDot, "", expectedValue: null);
      AssertParse(maybeDot, ".", expectedValue: '.');

      var parser = Parse.AnyCharOf("ab").Optional2()
        .WithTail(Parse.AnyChar.Optional2())
        .IgnoreCase();

      AssertParse(parser, "a", expectedValue: 'a');
      AssertParse(parser, "A", expectedValue: 'A');
      AssertParse(parser, "b", expectedValue: 'b');
      AssertParse(parser, "c", expectedValue: null);
    }
  }
}