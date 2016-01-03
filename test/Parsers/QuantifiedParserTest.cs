using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  // todo: test partially-matched step parsers

  [TestFixture] public class QuantifiedParserTest : ParserTest
  {
    [Test] public void ManyRanged()
    {
      const string input = "abc123";
      var parser = Parse.LetterOrDigitChar;

      Func<Parser<List<char>>, Parser<string>> f =
        par => par.Select(chars => new string(chars.ToArray()));

      AssertParse(f(parser.Many(count: 6)), input);
      AssertParse(f(parser.Many()), input, input);
      AssertParse(f(parser.AtLeastOnce()), input, input);
      AssertParse(f(parser.Many(min: 0, max: 6)), input, input);
      AssertParse(f(parser.Many(min: 1, max: 6)), input, input);
      AssertParse(f(parser.Many(min: 2, max: 6)), input, input);
      AssertParse(f(parser.Many(min: 3, max: 6)), input, input);
      AssertParse(f(parser.Many(min: 4, max: 6)), input, input);
      AssertParse(f(parser.Many(min: 5, max: 6)), input, input);
      AssertParse(f(parser.Many(min: 6, max: 6)), input, input);

      Func<Parser<List<char>>, Parser<string>> g =
        par => f(par).WithTail(Parse.AnyChar.Many());

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
      var parser = Parse.Char('a').WithTail(Parse.WhitespaceChar.Many()).IgnoreCase();

      AssertParse(parser, "a", 'a');
      AssertParse(parser, "a ", 'a');
      AssertParse(parser, "a  ", 'a');
      AssertParse(parser, "a   ", 'a');
      AssertParse(parser, "A", 'A');
      AssertParse(parser, "A \t        ", 'A');
      AssertParse(parser, "A  ", 'A');
    }

    [Test] public void ManyToString1()
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

    [Test] public void ManyToStringAllocations()
    {
      var stringBuilder = new StringBuilder();
      stringBuilder.Append('a', 100);
      stringBuilder.Append('A', 100);
      var input = stringBuilder.ToString(); // fresh string

      var parser = Parse.LetterChar.ManyToString();
      var output = parser.Parse(input);
      Assert.IsTrue(ReferenceEquals(input, output));

      var parser2 = Parse.LetterChar.Select(char.ToLowerInvariant).ManyToString();
      var output2 = parser2.Parse(input);
      Assert.IsFalse(ReferenceEquals(input, output2));
      Assert.AreEqual(input.ToLowerInvariant(), output2);
    }
  }
}