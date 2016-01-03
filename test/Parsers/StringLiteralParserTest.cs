using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
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
      var hexDigit = Parse.AnyCharOf("0123456789abcdef");

      var hexNumber = hexDigit.ManyToString(min: 1, max: 4)
        .Select(text => Convert.ToChar(int.Parse(text, NumberStyles.AllowHexSpecifier)))
        .IgnoreCase();

      AssertParse(hexNumber, "0", '\x0');
      AssertParse(hexNumber, "12", '\x12');
      AssertParse(hexNumber, "012", '\x12');
      AssertParse(hexNumber, "0012", '\x12');
      AssertParse(hexNumber, "AC12", '\xAC12');

      AssertFailure(hexNumber, "00123", expectedMessage: "end of string expected, got '3'", failureOffset: 4);
    }

    [Test] public void WithEscaping()
    {
      var hexDigit = Parse.AnyCharOf("0123456789abcdef").IgnoreCase();
      var hexNumber = hexDigit.Many(min: 1, max: 4).Select(HexStringToValue);

      var escapedCharacter = Parse.Choice(
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
        Parse.Char('x').InFrontOf(hexNumber),
        Parse.Fail<char>(expectation: "valid escape sequence"));

      var ordinaryCharacter = Parse.AnyCharExcept('\\', '\x27', '\x5C', '\r', '\n');
      var literalPart = ordinaryCharacter.Or(Parse.Char('\\').InFrontOf(escapedCharacter));

      var literalContent = literalPart.Many(min: 0, max: uint.MaxValue)
        // todo: aggregate to string
        .Select(x => new string(x.ToArray()));

      var stringLiteral = literalContent.SurroundWith(Parse.Char('"'));

      AssertParse(stringLiteral, @"""ab\r\nc012""", "ab\r\nc012");
    }

    private static char HexStringToValue([NotNull] List<char> characters)
    {
      var text = new string(characters.ToArray());
      var value = int.Parse(text, NumberStyles.AllowHexSpecifier);

      return Convert.ToChar(value);
    }


    // todo: string literal with escaping
    // todo: escaping combinator?
    // todo: combinator for building string from char
    // todo: C# verbatim literals
  }
}