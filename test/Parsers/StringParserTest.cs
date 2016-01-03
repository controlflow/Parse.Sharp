using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture] public sealed class StringParserTest : ParserTest
  {
    [Test] public void BasicStrings()
    {
      AssertParse(Parse.String("class"), "class", "class");
      AssertParse(Parse.IgnoreCaseString("class"), "ClaSS", "ClaSS");
      AssertParse(Parse.String("class").IgnoreCase(), "ClaSS", "ClaSS");
      AssertParse(Parse.String("class").IgnoreCase(), "claSS", "claSS");
    }

    [Test, SuppressMessage("ReSharper", "ConvertToConstant.Local")]
    public void LessAllocations()
    {
      var text = "class";
      var parser = Parse.String(text).WithWhitespaceAfter();

      var parsedText1 = parser.Parse("class ");
      Assert.IsTrue(ReferenceEquals(text, parsedText1));

      var parsedText2 = parser.IgnoreCase().Parse("class ");
      Assert.IsTrue(ReferenceEquals(text, parsedText2));

      var parsedText3 = parser.IgnoreCase().Parse("cLass ");
      Assert.IsFalse(ReferenceEquals(text, parsedText3));
    }

    [Test] public void ManyToString()
    {
      AssertParse(Parse.LetterChar.ManyToString(), "abcDEF", "abcDEF");
      AssertParse(Parse.CharExcept(char.IsDigit, "not digit").ManyToString(), "abcDEF", "abcDEF");
      AssertParse(Parse.LetterOrDigitChar.ManyToString(), "abCD45", "abCD45");

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
        Parse.LetterOrDigitChar.ManyToString().Named("identifier").NonEmpty(),
        input: "_abAA534\r\nABC",
        expectedMessage: "non-empty identifier expected, got '_abAA534 ABC'");
    }
  }
}