﻿using System.Text;
using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture] public sealed class TrivialParsersTest : ParserTest
  {
    [Test] public void ReturnParser()
    {
      var value = new object();
      var parser = Parse.Return(value).WithTail(Parse.AnyChar);
      Assert.IsTrue(ReferenceEquals(parser, parser.IgnoreCase()));

      AssertParse(parser, "a", value);
    }

    [Test] public void FailureParser()
    {
      var parser = Parse.Fail<StringBuilder>("valid escaping");
      Assert.IsTrue(ReferenceEquals(parser, parser.IgnoreCase()));

      AssertFailure(parser, input: "abc", expectedMessage: "valid escaping expected, got 'abc'");
    }

    [Test] public void NotParser()
    {
      var notDigit = Parse.Digit.Not();
      AssertParse(notDigit.WithTail(Parse.AnyChar), "z");

      AssertFailure(notDigit, input: "1", expectedMessage: "not digit expected, got '1'");
      AssertFailure(Parse.Digit.Not("letter"), input: "1", expectedMessage: "letter expected, got '1'");
    }
  }
}