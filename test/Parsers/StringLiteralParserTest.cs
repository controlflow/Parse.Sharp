using System;
using System.Globalization;
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

    [Test] public void UnicodeEscaping()
    {
      var hexDigit = Parse.AnyCharOf("0123456789abcdef").IgnoreCase();
      var hexDigitOpt = hexDigit.OptionalToNullable();

      var hexNumber =
        from hex1 in hexDigit
        from hex2 in hexDigitOpt
        from hex3 in hexDigitOpt
        from hex4 in hexDigitOpt
        let text = hex1.ToString() + hex2 + hex3 + hex4
        let value = int.Parse(text, NumberStyles.AllowHexSpecifier)
        select Convert.ToChar(value);

      AssertParse(hexNumber, "0", '\x0');
      AssertParse(hexNumber, "12", '\x12');
      AssertParse(hexNumber, "012", '\x12');
      AssertParse(hexNumber, "0012", '\x12');
      AssertParse(hexNumber, "AC12", '\xAC12');

      AssertFailure(hexNumber, "00123", expectedMessage: "end of string expected, got '3'", failureOffset: 4);
    }

    [Test] public void UnicodeEscaping2()
    {
      var hexDigit = Parse.AnyCharOf("0123456789abcdef").IgnoreCase();
      var hexDigitOpt = hexDigit.OptionalToNullable();

      // todo: the same via .Many(min: 1, max: 4)

      //var hexNumber =
      //  from hex1 in hexDigit
      //  from hex2 in hexDigitOpt
      //  from hex3 in hexDigitOpt
      //  from hex4 in hexDigitOpt
      //  let text = hex1.ToString() + hex2 + hex3 + hex4
      //  let value = int.Parse(text, NumberStyles.AllowHexSpecifier)
      //  select Convert.ToChar(value);

      //AssertParse(hexNumber, "0", '\x0');
      //AssertParse(hexNumber, "12", '\x12');
      //AssertParse(hexNumber, "012", '\x12');
      //AssertParse(hexNumber, "0012", '\x12');
      //AssertParse(hexNumber, "AC12", '\xAC12');
      //
      //AssertFailure(hexNumber, "00123", expectedMessage: "end of string expected, got '3'", failureOffset: 4);
    }

    [Test] public void WithEscaping()
    {
      var ordinaryCharacter = Parse.AnyCharExcept('\\', '\x27', '\x5C', '\r', '\n');

      // unicode symbol via hex code
      


      //Parse.Char('x')

      //hexDigit.Aggregate()




      // todo: unicode symbol parser
      // todo: 
      //Parse.Return()

      //hexDigit.Aggregate()


      Parser<char>[] escapingContentChoice =
      {
        Parse.Char('\'').Select('\''),
        Parse.Char('"').Select('"'),
        Parse.Char('\\').Select('\\'),
        Parse.Char('0').Select('\0'),
        Parse.Char('a').Select('\a'),
        Parse.Char('b').Select('\b'),
        Parse.Char('f').Select('\f'),
        Parse.Char('n').Select('\n'),
        Parse.Char('r').Select('\r'),
        Parse.Char('t').Select('\t'),
        Parse.Char('v').Select('\v'),

        // unicode character
        Parse.Char('x')

      };

      //var s = "\x1";


      //var esc1 = Parse.Char('t').Select(x => Parse.Return('\t'));

      //var s = "\21234";



      // \u



      //"\5aaa"

      //Parse.CharExcept('/')
    }


    // todo: string literal with escaping
    // todo: escaping combinator?
    // todo: combinator for building string from char
    // todo: C# verbatim literals
  }
}