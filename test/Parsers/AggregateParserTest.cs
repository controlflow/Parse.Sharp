using System.Text;
using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture] public class AggregateParserTest : ParserTest
  {
    [Test] public void SumOfDigits()
    {
      var parser = Parse.Digit.Aggregate(0, (acc, value) => acc + value);

      AssertParse(parser, "", 0);
      AssertParse(parser, "1", 1);
      AssertParse(parser, "123", 6);
    }

    [Test] public void AggregateToString()
    {
      var hexCharacters = Parse.AnyCharOf("0123456789abcdef");

      var parser1 = hexCharacters
        .Aggregate(() => new StringBuilder(), (acc, value) => acc.Append(value))
        .Select(sb => sb.ToString());

      AssertParse(parser1, "012a3fe1", "012a3fe1");

      var parser2 = hexCharacters.AggregateToString().IgnoreCase();
      AssertParse(parser2, "012a3fe1", "012a3fe1");
      AssertParse(parser2, "012A3fBE1", "012A3fBE1");
    }

    [Test] public void AggregateCharactersToString()
    {
      var hexCharacters = Parse.AnyCharOf("0123456789abcdef");

      //hexCharacters.Many(min: 0, max: 10).Aggregate()
    }
  }
}