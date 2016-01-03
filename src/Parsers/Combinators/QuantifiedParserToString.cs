using System;
using System.Text;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class QuantifiedParserToString<T> : Parser<string>
  {
    [NotNull] private readonly Parser<T> myUnderlyingParser;
    private readonly uint myMin, myMax;

    public QuantifiedParserToString([NotNull] Parser<T> parser, uint min, uint max)
    {
      myUnderlyingParser = parser;
      myMin = min;
      myMax = max;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var builder = new StringBuilder(capacity: (int) myMin);

      uint index = 0;
      while (index < myMax)
      {
        var result = myUnderlyingParser.TryParseValue(input, offset);
        if (result.IsSuccessful)
        {
          builder.Append(result.Value);

          if (offset == result.Offset)
            throw new ArgumentException("Infinite iteration detected");

          offset = result.Offset;
          index++;

          if (index == myMax) return new ParseResult(value: builder.ToString(), nextOffset: offset);
        }
        else
        {
          if (result.Offset == offset && index >= myMin)
            return new ParseResult(value: builder.ToString(), nextOffset: offset);

          return new ParseResult(failPoint: result.FailPoint, atOffset: result.Offset);
        }
      }

      return new ParseResult(builder.ToString(), offset);
    }

    protected internal override ParseAttempt TryParseVoid(string input, int offset)
    {
      throw new ArgumentException(".ManyToString() should not be used in positions where it's value is ignored");
    }

    protected override Parser<string> CreateIgnoreCaseParser()
    {
      var ignoreCaseParser = myUnderlyingParser.IgnoreCase();
      if (ReferenceEquals(myUnderlyingParser, ignoreCaseParser)) return this;

      return new QuantifiedParserToString<T>(ignoreCaseParser, myMin, myMax);
    }
  }
}