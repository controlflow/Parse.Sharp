using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture]
  public sealed class CharacterParserTest : ParserTest
  {
    [Test] public void ConcreteCharacter()
    {
      AssertParse(Parse.Char('0'), "0", '0');
      AssertParse(Parse.Char('['), "[", '[');
      AssertParse(Parse.Dot, ".", '.');
      AssertParse(Parse.Comma, ",", ',');
      AssertParse(Parse.LBrace, "[", '[');
      AssertParse(Parse.RBrace, "]", ']');

      AssertFailure(
        parser: Parse.Char('+'), input: "///",
        expectedMessage: "'+' expected, got '///'", failureOffset: 0);
    }

    [Test] public void AnyCharacter()
    {
      AssertParse(Parse.AnyChar, "x", 'x');

      AssertFailure(
        parser: Parse.AnyChar, input: "xyz",
        expectedMessage: "end of string expected, got 'yz'", failureOffset: 1);
    }

    [Test] public void PredicateCharacter()
    {
      AssertParse(Parse.LetterChar, "x", 'x');
      AssertParse(Parse.LetterOrDigitChar, "x", 'x');
      AssertParse(Parse.LetterOrDigitChar, "0", '0');
      AssertParse(Parse.WhitespaceChar, " ", ' ');
      AssertParse(Parse.WhitespaceChar, "\t", '\t');
      AssertParse(Parse.LowerCaseChar, "a", 'a');
      AssertParse(Parse.UpperCaseChar, "A", 'A');

      AssertFailure(
        parser: Parse.LetterChar, input: "1",
        expectedMessage: "letter character expected, got '1'", failureOffset: 0);
    }
  }
}