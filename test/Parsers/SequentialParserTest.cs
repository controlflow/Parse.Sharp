using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture] public class SequentialParserTest : ParserTest
  {
    [Test] public void After()
    {
      AssertParse(Parse.Digit.WithWhitespaceAfter(), "1   ", 1);
      AssertParse(Parse.Digit.WithTail(Parse.WhitespaceChar.Many()), "1   ", 1);
    }

    [Test] public void SurroundWith1()
    {
      var stringContents = Parse.CharExcept('"').ManyToString();
      var stringLiteral = stringContents.SurroundWith(Parse.Char('"'));

      AssertParse(stringLiteral, "\"\"", "");
      AssertParse(stringLiteral, "\"abc\"", "abc");
      AssertParse(stringLiteral, "\"\\taa\"", @"\taa");
    }

    [Test] public void SurroundWith2()
    {
      var stringContents = Parse.AnyCharExcept('"', '\'').ManyToString();
      var stringLiteral = Parse.Choice(
        stringContents.SurroundWith(Parse.Char('"'), Parse.Char('\'')),
        stringContents.SurroundWith(Parse.Char('\''), Parse.Char('"')));

      AssertParse(stringLiteral, "\"abc'", "abc");
      AssertParse(stringLiteral, "'abc\"", "abc");
    }
  }
}