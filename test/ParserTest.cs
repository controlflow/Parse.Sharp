using System;
using System.Collections.Generic;
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

      using (Parse.AssertNoAllocations())
      {
        var parsedValue = parser.Parse(input);
        GC.KeepAlive(parsedValue);
      }
    }

    public static void AssertParse<T>(
      [NotNull] Parser<T> parser, [NotNull] string input, T expectedValue, IEqualityComparer<T> equalityComparer = null)
    {
      Assert.NotNull(parser, "parser != null");
      Assert.NotNull(input, "input != null");

      using (Parse.AssertNoAllocations())
      {
        var parsedValue = parser.Parse(input);

        if (equalityComparer != null)
          Assert.IsTrue(equalityComparer.Equals(parsedValue, expectedValue));
        else
          Assert.AreEqual(parsedValue, expectedValue);
      }
    }

    public static void AssertFailure<T>(
      [NotNull] Parser<T> parser, [NotNull] string input, [NotNull] string expectedMessage, int failureOffset = 0)
    {
      Assert.NotNull(parser, "parser != null");
      Assert.NotNull(input, "input != null");

      using (Parse.AssertNoAllocations())
      try
      {
        var result = parser.Parse(input);

        throw new AssertionException(string.Format("Parsing failure expected, got: {0}", result));
      }
      catch (ParseException parseException)
      {
        Assert.AreEqual(expectedMessage, parseException.Message, "Failure message is wrong");
        Assert.AreEqual(failureOffset, parseException.Offset, "Failure offset is wrong");
      }
    }
  }
}