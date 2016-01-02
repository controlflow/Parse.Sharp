using System.Collections.Generic;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class QuantifiedParser<T> : Parser<List<T>>
  {
    [NotNull] private readonly Parser<T> myUnderlyingParser;
    private readonly uint myMin, myMax;

    public QuantifiedParser([NotNull] Parser<T> parser, uint min, uint max)
    {
      myUnderlyingParser = parser;
      myMin = min;
      myMax = max;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var list = new List<T>(capacity: (int)myMin);

      uint index = 0;
      while (index < myMax)
      {
        var result = myUnderlyingParser.TryParseValue(input, offset);
        if (result.IsSuccessful)
        {
          list.Add(result.Value);

          offset = result.Offset;
          index ++;

          if (index == myMax) return new ParseResult(value: list, nextOffset: offset);
        }
        else
        {
          if (index >= myMin) return new ParseResult(value: list, nextOffset: offset);

          return new ParseResult(failPoint: result.FailPoint, atOffset: result.Offset);
        }
      }

      return new ParseResult(list, offset);
    }

    protected internal override ParseAttempt TryParseVoid(string input, int offset)
    {
      uint index = 0;
      while (index < myMax)
      {
        var result = myUnderlyingParser.TryParseValue(input, offset);
        if (result.IsSuccessful)
        {
          offset = result.Offset;
          index++;

          if (index == myMax) return new ParseAttempt(nextOffset: offset);
        }
        else
        {
          if (index >= myMin) return new ParseAttempt(nextOffset: offset);

          return new ParseAttempt(failPoint: result.FailPoint, atOffset: result.Offset);
        }
      }

      return new ParseAttempt(nextOffset: offset);
    }

    protected override Parser<List<T>> CreateIgnoreCaseParser()
    {
      var ignoreCasePArser = myUnderlyingParser.IgnoreCase();
      if (ReferenceEquals(myUnderlyingParser, ignoreCasePArser)) return this;

      return new QuantifiedParser<T>(ignoreCasePArser, myMin, myMax);
    }
  }
}