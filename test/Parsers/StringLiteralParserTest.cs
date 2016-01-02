using System.CodeDom;
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

    [Test] public void WithEscaping()
    {
      // // ordinary character
        //Parse.CharExcept('\\'),

      var ordinaryCharacter = Parse.CharsExcept('\\', '\x27', '\x5C', '\r', '\n');

      var hexDigit = Parse.Chars("0123456789abcdefABCDEF");

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