using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Combinators
{
  internal sealed class SelectParser<T, TResult> : Parser<TResult>
  {
    [NotNull] private readonly Parser<T> myUnderlyingParser;
    [NotNull] private readonly Func<T, TResult> mySelector;

    public SelectParser([NotNull] Parser<T> underlyingParser, [NotNull] Func<T, TResult> selector)
    {
      myUnderlyingParser = underlyingParser;
      mySelector = selector;
    }

    protected internal override ParseResult TryParse(string input, int offset)
    {
      var result = myUnderlyingParser.TryParse(input, offset);
      if (result.IsSuccessful)
      {
        return new ParseResult(value: mySelector(result.Value), nextOffset: result.Offset);
      }

      return new ParseResult(failPoint: result.FailPoint, atOffset: result.Offset);
    }
  }
}