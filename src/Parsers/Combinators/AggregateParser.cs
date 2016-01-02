using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class AggregateParser<T, TAccumulate, TResult> : Parser<TResult>
  {
    [NotNull] private readonly Parser<T> myUnderlyingParser;
    [NotNull] private readonly Func<TAccumulate> mySeedFactory;
    [NotNull] private readonly Func<TAccumulate, T, TAccumulate> myFold;
    [NotNull] private readonly Func<TAccumulate, TResult> myResultSelector;

    public AggregateParser(
      [NotNull] Parser<T> underlyingParser,
      [NotNull] Func<TAccumulate> seedFactory,
      [NotNull] Func<TAccumulate, T, TAccumulate> fold,
      [NotNull] Func<TAccumulate, TResult> resultSelector)
    {
      myUnderlyingParser = underlyingParser;
      mySeedFactory = seedFactory;
      myFold = fold;
      myResultSelector = resultSelector;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var acc = mySeedFactory();

      for (var first = true; ; first = false)
      {
        if (first) acc = mySeedFactory();

        var result = myUnderlyingParser.TryParseValue(input, offset);
        if (!result.IsSuccessful)
        {
          return new ParseResult(value: myResultSelector(acc), nextOffset: offset);
        }

        var nextOffset = result.Offset;
        if (nextOffset == offset)
          throw new ArgumentException("Infinite aggregation detected");

        acc = myFold(acc, result.Value);
        offset = nextOffset;
      }
    }

    protected override Parser<TResult> CreateIgnoreCaseParser()
    {
      var ignoreCaseParser = myUnderlyingParser.IgnoreCase();
      if (ReferenceEquals(myUnderlyingParser, ignoreCaseParser)) return this;

      return new AggregateParser<T, TAccumulate, TResult>(ignoreCaseParser, mySeedFactory, myFold, myResultSelector);
    }
  }
}