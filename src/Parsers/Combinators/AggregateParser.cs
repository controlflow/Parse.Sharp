using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class AggregateParser<T, TAccumulate> : Parser<TAccumulate>
  {
    [NotNull] private readonly Parser<T> myUnderlyingParser;
    [NotNull] private readonly Func<TAccumulate> mySeedFactory;
    [NotNull] private readonly Func<TAccumulate, T, TAccumulate> myFold;

    public AggregateParser(
      [NotNull] Parser<T> underlyingParser,
      [NotNull] Func<TAccumulate> seedFactory,
      [NotNull] Func<TAccumulate, T, TAccumulate> fold)
    {
      myUnderlyingParser = underlyingParser;
      mySeedFactory = seedFactory;
      myFold = fold;

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
          return new ParseResult(value: acc, nextOffset: offset);
        }

        var nextOffset = result.Offset;
        if (nextOffset == offset)
          throw new ArgumentException("Infinite aggregation detected");

        acc = myFold(acc, result.Value);
        offset = nextOffset;
      }
    }
  }
}