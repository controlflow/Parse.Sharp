using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture] public class QuantifiedParserTest : ParserTest
  {
    [Test] public void ManyRanged()
    {
      const string input = "abc123";
      var parser = Parse.LetterOrDigitChar;

      Func<Parser<List<char>>, Parser<string>> f =
        par => par.Select(chars => new string(chars.ToArray()));

      AssertParse(f(parser.Many(count: 6)), input);
      AssertParse(f(parser.Many(min: 0, max: 6)), input, input);
      AssertParse(f(parser.Many(min: 1, max: 6)), input, input);
      AssertParse(f(parser.Many(min: 2, max: 6)), input, input);
      AssertParse(f(parser.Many(min: 3, max: 6)), input, input);
      AssertParse(f(parser.Many(min: 4, max: 6)), input, input);
      AssertParse(f(parser.Many(min: 5, max: 6)), input, input);
      AssertParse(f(parser.Many(min: 6, max: 6)), input, input);

      Func<Parser<List<char>>, Parser<string>> g =
        par => f(par).WithTail(Parse.AnyChar.Many(min: 0, max: uint.MaxValue));

      AssertParse(g(parser.Many(min: 0, max: 0)), input, "");
      AssertParse(g(parser.Many(min: 0, max: 1)), input, "a");
      AssertParse(g(parser.Many(min: 0, max: 2)), input, "ab");
      AssertParse(g(parser.Many(min: 0, max: 3)), input, "abc");
      AssertParse(g(parser.Many(min: 0, max: 4)), input, "abc1");
      AssertParse(g(parser.Many(min: 0, max: 5)), input, "abc12");
      AssertParse(g(parser.Many(min: 0, max: 6)), input, "abc123");
    }

    [Test] public void ManyValueIgnored()
    {
      var parser = Parse.Char('a').WithTail(Parse.WhitespaceChar.Many(min: 0, max: uint.MaxValue)).IgnoreCase();

      AssertParse(parser, "a", 'a');
      AssertParse(parser, "a ", 'a');
      AssertParse(parser, "a  ", 'a');
      AssertParse(parser, "a   ", 'a');
      AssertParse(parser, "A", 'A');
      AssertParse(parser, "A \t        ", 'A');
      AssertParse(parser, "A  ", 'A');
    }
  }
}