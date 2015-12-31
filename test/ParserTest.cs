using System;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Parse.Sharp.Tests
{
  public abstract class ParserTest
  {
    public static void AssertParse<T>([NotNull] Parser<T> parser, [NotNull] string input)
    {
      Assert.NotNull(parser, "parser != null");
      Assert.NotNull(input, "input != null");

      var parsedValue = parser.Parse(input);
      GC.KeepAlive(parsedValue);
    }

    public static void AssertParse<T>([NotNull] Parser<T> parser, [NotNull] string input, T expectedValue)
    {
      Assert.NotNull(parser, "parser != null");
      Assert.NotNull(input, "input != null");

      var parsedValue = parser.Parse(input);
      Assert.AreEqual(parsedValue, expectedValue);
    }

    public static void AssertFailure<T>(
      [NotNull] Parser<T> parser, [NotNull] string input, [NotNull] string expectedMessage, int failureOffset)
    {
      Assert.NotNull(parser, "parser != null");
      Assert.NotNull(input, "input != null");

      try
      {
        var result = parser.Parse(input);
        GC.KeepAlive(result);
      }
      catch (ParseException parseException)
      {
        Assert.AreEqual(expectedMessage, parseException.Message, "Failure message is wrong");
        Assert.AreEqual(failureOffset, parseException.Offset, "Failure offset is wrong");
        return;
      }

      throw new AssertionException("Parsing failure expected");
    }
  }
}