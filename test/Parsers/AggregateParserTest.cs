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
      var hexCharacters = Parse.Chars("0123456789abcdef");

      var parser = hexCharacters
        .Aggregate(() => new StringBuilder(), (acc, value) => acc.Append(value))
        .Select(x => x.ToString());

      AssertParse(parser, "012a3fe1", "012a3fe1");


      //Func<char, int> hex2dec = ch => (ch >= '1' && ch <= '0') ? ch - '0' : 10 + ch - 'a';



      //Func<StringBuilder>

      //var t = '0' < '9';

      
      
    }
  }
}