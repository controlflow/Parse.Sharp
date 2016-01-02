using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class SelectParser<T> : Parser<T>
  {
    [NotNull] private readonly Parser myUnderlyingParser;
    private readonly T mySelectValue;

    public SelectParser([NotNull] Parser underlyingParser, T selectValue)
    {
      myUnderlyingParser = underlyingParser;
      mySelectValue = selectValue;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var result = myUnderlyingParser.TryParseVoid(input, offset);
      if (result.IsSuccessful)
      {
        return new ParseResult(value: mySelectValue, nextOffset: result.Offset);
      }

      return new ParseResult(failPoint: result.FailPoint, atOffset: result.Offset);
    }

    protected override Parser<T> CreateIgnoreCaseParser()
    {
      var ignoreCaseParser = myUnderlyingParser.IgnoreCase();
      if (ReferenceEquals(myUnderlyingParser, ignoreCaseParser)) return this;

      return new SelectParser<T>(ignoreCaseParser, mySelectValue);
    }
  }

  internal sealed class SelectParser<T, TResult> : Parser<TResult>
  {
    [NotNull] private readonly Parser<T> myUnderlyingParser;
    [NotNull] private readonly Func<T, TResult> mySelector;

    public SelectParser([NotNull] Parser<T> underlyingParser, [NotNull] Func<T, TResult> selector)
    {
      myUnderlyingParser = underlyingParser;
      mySelector = selector;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var result = myUnderlyingParser.TryParseValue(input, offset);
      if (result.IsSuccessful)
      {
        return new ParseResult(value: mySelector(result.Value), nextOffset: result.Offset);
      }

      return new ParseResult(failPoint: result.FailPoint, atOffset: result.Offset);
    }

    protected override Parser<TResult> CreateIgnoreCaseParser()
    {
      var ignoreCaseParser = myUnderlyingParser.IgnoreCase();
      if (ReferenceEquals(myUnderlyingParser, ignoreCaseParser)) return this;

      return new SelectParser<T, TResult>(ignoreCaseParser, mySelector);
    }
  }
}