using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture] public sealed class StringLiteralParserTest : ParserTest
  {
    [Test] public void WithoutUnescaping()
    {
      var stringContents = Parse.CharExcept('"').ManyToString();
      var stringLiteral = stringContents.SurroundWith(Parse.Char('"'));

      AssertParse(stringLiteral, "\"abc\"", "abc");
    }



    // todo: string literal with escaping
    // todo: escaping combinator?
    // todo: combinator for building string from char
    // todo: C# verbatim literals
  }
}