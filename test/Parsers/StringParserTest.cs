using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture]
  public sealed class StringParserTest : ParserTest
  {
    [Test] public void BasicStrings()
    {
      AssertParse(Parse.String("class"), "class", "class");
      AssertParse(Parse.IgnoreCaseString("class"), "ClaSS", "ClaSS");

      AssertParse(
        parser: from kw in Parse.String("class")
                from lb in Parse.Char('{')
                from rb in Parse.Char('}')
                select true,
        input: "class{}");
    }

    [Test] public void ManyToString()
    {
      AssertParse(Parse.LetterChar.ManyToString(), "abcDEF", "abcDEF");
      AssertParse(Parse.Digit.Not("not digit").ManyToString(), "abcDEF", "abcDEF");
      AssertParse(Parse.LetterOrDigitChar.ManyToString("identifier"), "abCD45", "abCD45");
      AssertParse(Parse.LetterOrDigitChar.Not().Not().ManyToString("identifier"), "abCD45", "abCD45");

      AssertFailure(
        Parse.LetterChar.ManyToString(), input: "abc123",
        expectedMessage: "end of string expected, got '123'",
        failureOffset: 3);

      AssertFailure(
        Parse.IgnoreCaseString("aa").ManyToString(),
        input: "aAAaAAaaAaAAaaa",
        expectedMessage: "end of string expected, got 'a'",
        failureOffset: 14);

      AssertFailure(
        Parse.LetterOrDigitChar.Not().Not().ManyToString("identifier").NotEmpty(),
        input: "_abAA534\r\nABC",
        expectedMessage: "non-empty identifier expected, got '_abAA534 ABC'",
        failureOffset: 0);
    }
  }
}