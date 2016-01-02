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

      AssertFailure(Parse.Char('+'), input: "///", expectedMessage: "'+' expected, got '///'");
    }

    [Test] public void AnyCharacter()
    {
      AssertParse(Parse.AnyChar, "x", 'x');

      AssertFailure(Parse.AnyChar, input: "xyz",
        expectedMessage: "end of string expected, got 'yz'", failureOffset: 1);
    }

    [Test] public void CharacterExcept()
    {
      AssertParse(Parse.CharExcept('.'), "a", 'a');

      AssertFailure(Parse.CharExcept('.'), input: ".", expectedMessage: "not '.' expected, got '.'");
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

      AssertFailure(Parse.LetterChar, input: "1", expectedMessage: "letter character expected, got '1'");
    }

    [Test] public void PredicateCharacterExcept()
    {
      var parser = Parse.CharExcept(
        predicate: ch => (ch == '\'' || ch == '"'),
        description: "not quote");

      AssertParse(parser, "a", 'a');
      AssertParse(parser, "1", '1');

      AssertFailure(parser, input: "''\"", expectedMessage: "not quote expected, got '''\"'");
    }

    [Test] public void CharactersSet1()
    {
      var parser = Parse.Chars('1', '2', '3');

      AssertParse(parser, "1", '1');
      AssertParse(parser, "2", '2');

      AssertFailure(parser, "a", expectedMessage: "'1'|'2'|'3' expected, got 'a'");
    }

    [Test] public void CharactersSet2()
    {
      var parser = Parse.CharsExcept('1', '2', '3');

      AssertParse(parser, "a", 'a');
      AssertParse(parser, "b", 'b');

      AssertFailure(parser, "1", expectedMessage: "not '1'|'2'|'3' expected, got '1'");
    }

    [Test] public void CharactersSet3()
    {
      AssertParse(Parse.Chars("abc"), "a", 'a');
      AssertParse(Parse.Chars("abc"), "c", 'c');
      AssertParse(Parse.CharsExcept("abc"), "1", '1');
      AssertParse(Parse.CharsExcept("abc"), "2", '2');

      var parser = Parse.Chars("digit", '1', '2', '3', '4', '5', '6', '7', '8', '9', '0');
      AssertParse(parser, "1", '1');

      AssertFailure(parser, input: "a", expectedMessage: "digit expected, got 'a'");
    }

    // todo: characters set
  }
}