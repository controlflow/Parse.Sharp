using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture]
  public sealed class ChoiceParserTest : ParserTest
  {
    [Test] public void Choice()
    {
      var parser = Parse.Choice(Parse.LetterChar, Parse.DigitChar);
      AssertParse(parser, "a", 'a');
      AssertParse(parser, "A", 'A');
      AssertParse(parser, "1", '1');

      AssertFailure(parser,
        input: "+",
        expectedMessage: "letter character or digit character expected, got '+'",
        failureOffset: 0);
    }
  }
}